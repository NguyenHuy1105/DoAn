using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WorkerService4.Get_Information;
using WorkerService4.Model;
using Interop.UIAutomationClient;
using WorkerService4.Get_Infomaiton;

namespace WorkerService4.Service
{
    public class HistoryService : BackgroundService
    {
        private readonly ILogger<HistoryService> _logger;
        private readonly HttpClient _httpClient;
        private readonly ApiConfig _apiConfig;

        private bool _isHasSession;
        private int _level;
        private int _levelFromChromeProcess;
        private string _computerId;
        private string _computerName;
        private string _computerRoomId;
        private string _monitorSessionId;
        private JsonDocument _sessionData;
        private bool _winFormsAppStarted = false;

        private bool _captureScreenInProgress = false;
        private bool _historyInProgress = false;
        private Timer _captureScreenTimer;
        private Timer _historyTimer;
        private readonly int _captureIntervalInSeconds = 20;
        private readonly int _historyIntervalInSeconds = 20;
        private string _studentId;
        private string _studentCode;
        private bool _wasSessionActive;


        public HistoryService(ILogger<HistoryService> logger, HttpClient httpClient, IOptions<ApiConfig> apiConfig)
        {
            _logger = logger;
            _httpClient = httpClient;
            _apiConfig = apiConfig.Value;

            _isHasSession = false;
            _wasSessionActive = false;
            _level = 1;
            _levelFromChromeProcess = 1;
           _computerId = null;
            _computerRoomId = null;
            _monitorSessionId = null;
            _sessionData = null;
            _computerName = null;
            _studentId = null;
            _studentCode = null;
        }

        public override async Task StartAsync(CancellationToken cancellationToken)
        {
            _captureScreenTimer = new Timer(CaptureScreenCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(_captureIntervalInSeconds));
            _historyTimer = new Timer(HistoryCallback, null, TimeSpan.Zero, TimeSpan.FromSeconds(_historyIntervalInSeconds));

            await base.StartAsync(cancellationToken);
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    // Lấy thông tin sinh viên
                    var (studentId, studentCode) = await GetStudent.GetStudentInfoAsync(_apiConfig, stoppingToken);
                    _studentId = studentId;
                    _studentCode = studentCode;

                    // Lấy địa chỉ MAC
                    string macAddress = GetMac.GetMACAddress();

                    var (computerId, computerName, computerRoomId) = await GetComputer.GetMachineDataFromServer(_httpClient, macAddress, _apiConfig, stoppingToken);
                    _computerId = computerId;
                    _computerName = computerName;
                    _computerRoomId = computerRoomId;


                    if (!string.IsNullOrEmpty(_computerRoomId))
                    {
                        // Kiểm tra session tồn tại
                        _monitorSessionId = await GetMonitorSessionId(_computerRoomId, stoppingToken);
                        await CheckSessionExistence(_computerRoomId, stoppingToken);

                        if (_isHasSession && !_winFormsAppStarted)
                        {
                            StartWinFormsApplication();
                            _winFormsAppStarted = true;
                        }
                        else if (!_isHasSession)
                        {
                            _winFormsAppStarted = false;
                        }

                        _wasSessionActive = _isHasSession;

                    }
                    else
                    {
                        _isHasSession = false;
                        _level = 1;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error in main execution loop");
                }

                int interval = await CHECK_MONITOR_SESSION.GetCheckTimeMonitorSession(_httpClient, _apiConfig, stoppingToken) ?? 10000;
                await Task.Delay(TimeSpan.FromMilliseconds(interval), stoppingToken);
            }
        }

        public void StartWinFormsApplication()
        {
            try
            {
                // Đường dẫn tĩnh trực tiếp đến ứng dụng WinForms
                string winFormsAppPath = @"C:\Software\Agent\WinFormsApp2\WinFormsApp2\bin\Release\net6.0-windows\publish\WinFormsApp2.exe";

                // Kiểm tra xem file có tồn tại hay không
                if (!File.Exists(winFormsAppPath))
                {
                    _logger.LogError($"WinForms application not found: {winFormsAppPath}");
                    return;
                }

                // Khởi động ứng dụng WinForms
                ProcessStartInfo startInfo = new ProcessStartInfo
                {
                    FileName = winFormsAppPath,
                    // Thiết lập thư mục làm việc nếu cần thiết
                    WorkingDirectory = Path.GetDirectoryName(winFormsAppPath),
                    UseShellExecute = true
                };

                _logger.LogInformation($"Starting WinForms application: {winFormsAppPath}");
                Process.Start(startInfo);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while starting WinForms application");
            }
        }

        private async Task<string> GetMonitorSessionId(string computerRoomId, CancellationToken stoppingToken)
        {
            try
            {
                string url = $"{_apiConfig.BaseUrl}{_apiConfig.ApiEndpoints.MonitorSessionEndpoint}{computerRoomId}";
                HttpResponseMessage response = await _httpClient.GetAsync(url, stoppingToken);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var monitorSessionResponse = JsonDocument.Parse(responseBody);
                bool success = monitorSessionResponse.RootElement.GetProperty("success").GetBoolean();

                if (success)
                {
                    var data = monitorSessionResponse.RootElement.GetProperty("data");

                    if (data.ValueKind == JsonValueKind.Object)
                    {
                        // Process when 'data' is an object
                        _sessionData = monitorSessionResponse; // Store the session data for later use
                        return data.GetProperty("id").GetString();
                    }
                    else if (data.ValueKind == JsonValueKind.Null)
                    {
                        // Process when 'data' is null
                        _sessionData = null; // Ensure session data is cleared if 'data' is null
                        return null;
                    }
                    else
                    {
                        throw new InvalidOperationException("Unexpected JSON structure for 'data' element: 'data' is not an object");
                    }
                }
                else
                {
                    throw new InvalidOperationException("Request was not successful. Error message: " + monitorSessionResponse.RootElement.GetProperty("message").GetString());
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "Error while fetching monitor session information");
                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetMonitorSessionId");
                return null;
            }
        }



        private async Task CheckSessionExistence(string computerRoomId, CancellationToken stoppingToken)
        {
            try
            {
                string url = $"{_apiConfig.BaseUrl}{_apiConfig.ApiEndpoints.MonitorSessionEndpoint}{computerRoomId}";
                HttpResponseMessage response = await _httpClient.GetAsync(url, stoppingToken);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Received response: {responseBody}"); // Ghi log toàn bộ phản hồi

                var monitorSessionResponse = JsonDocument.Parse(responseBody);
                bool success = monitorSessionResponse.RootElement.GetProperty("success").GetBoolean();

                if (success)
                {
                    var data = monitorSessionResponse.RootElement.GetProperty("data");

                    if (data.ValueKind == JsonValueKind.Object)
                    {
                        _isHasSession = true;
                        //await ProcessChromeWindows();
                    }
                    else
                    {
                        _isHasSession = false;
                        _logger.LogInformation("No active session found for computer room ID: {0}", computerRoomId);
                        // Xử lý lỗi ở đây, ví dụ như đặt các giá trị mặc định
                    }
                }
                else
                {
                    _isHasSession = false;
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogInformation("No active session found for computer room ID: {0}", computerRoomId);
                _isHasSession = false;
                // Xử lý lỗi kết nối HTTP ở đây
            }
            catch (Exception ex)
            {
                _logger.LogInformation("No active session found for computer room ID: {0}", computerRoomId);
                _isHasSession = false;
                // Xử lý các ngoại lệ khác ở đây
            }
        }





        private void CaptureScreenCallback(object state)
        {
            if (_isHasSession && _levelFromChromeProcess == 2)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        _captureScreenInProgress = true;
                        await CaptureScreen();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in CaptureScreenCallback");
                    }
                    finally
                    {
                        _captureScreenInProgress = false;
                    }
                });
            }
        }

        private void HistoryCallback(object state)
        {
            if (_isHasSession && _winFormsAppStarted)
            {
                Task.Run(async () =>
                {
                    try
                    {
                        _historyInProgress = true;
                        await ProcessChromeWindows();
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Error in HistoryCallback");
                    }
                    finally
                    {
                        _historyInProgress = false;
                    }
                });
            }
        }

        private async Task CaptureScreen()
        {
            try
            {
                using (Bitmap bitmap = new Bitmap(
                    System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width,
                    System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height,
                    PixelFormat.Format32bppArgb))
                {
                    using (Graphics g = Graphics.FromImage(bitmap))
                    {
                        g.CopyFromScreen(
                            System.Windows.Forms.Screen.PrimaryScreen.Bounds.X,
                            System.Windows.Forms.Screen.PrimaryScreen.Bounds.Y,
                            0, 0,
                            bitmap.Size,
                            CopyPixelOperation.SourceCopy);
                    }

                    using (MemoryStream ms = new MemoryStream())
                    {
                        bitmap.Save(ms, ImageFormat.Png);
                        ms.Position = 0;

                        var content = new MultipartFormDataContent();
                        content.Add(new StringContent(_monitorSessionId), "MonitorSessionId");
                        content.Add(new StringContent(_studentId), "StudentId");
                        content.Add(new StringContent(_computerId), "ComputerId");
                        content.Add(new StreamContent(ms), "FileData", "screenshot.png");
                        content.Add(new StringContent(_computerName), "ComputerName");

                        HttpResponseMessage response = await _httpClient.PostAsync($"{_apiConfig.BaseUrl}{_apiConfig.ApiEndpoints.uploadfile}", content);

                        if (response.IsSuccessStatusCode)
                        {
                            _logger.LogInformation("Screenshot uploaded successfully.");
                        }
                        else
                        {
                            _logger.LogError($"Failed to upload screenshot, StatusCode: {response.StatusCode}");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while capturing screen and uploading");
            }
        }

        private async Task ProcessChromeWindows()
        {
            try
            {
                Process[] chromeProcesses = Process.GetProcessesByName("chrome");

                foreach (Process chromeProcess in chromeProcesses)
                {
                    if (chromeProcess.MainWindowHandle == IntPtr.Zero)
                        continue;

                    string chromeWindowTitle = chromeProcess.MainWindowTitle;

                    // Kiểm tra tiêu đề của cửa sổ để chỉ lấy thông tin từ Google Chrome
                    if (!chromeWindowTitle.Contains("Google Chrome", StringComparison.OrdinalIgnoreCase))
                        continue;

                    var automation = new CUIAutomation();
                    IUIAutomationElement browserElement = automation.ElementFromHandle(chromeProcess.MainWindowHandle);

                    IUIAutomationCondition condition = automation.CreatePropertyCondition(UIA_PropertyIds.UIA_ControlTypePropertyId, UIA_ControlTypeIds.UIA_EditControlTypeId);
                    IUIAutomationTreeWalker walker = automation.CreateTreeWalker(condition);

                    IUIAutomationElement entry = walker.GetFirstChildElement(browserElement);

                    while (entry != null)
                    {
                        try
                        {
                            string url = entry.GetCurrentPropertyValue(UIA_PropertyIds.UIA_ValueValuePropertyId) as string;

                            if (!string.IsNullOrEmpty(url))
                            {
                                // Kiểm tra xem URL có khớp với bất kỳ tên miền nào trong _sessionData không
                                if (_sessionData != null)
                                {
                                    var data = _sessionData.RootElement.GetProperty("data");

                                    if (data.ValueKind == JsonValueKind.Null)
                                    {
                                        // Nếu data trong _sessionData là null, gán level = 1
                                        _level = 1;
                                    }
                                    else
                                    {
                                        var domains = data.GetProperty("domains");
                                        if (domains.GetArrayLength() == 0)
                                        {
                                            // Nếu domains là mảng rỗng, gán level = 1
                                            _level = 1;
                                        }
                                        else
                                        {
                                            bool urlMatchesDomain = false;

                                            foreach (var domainElement in domains.EnumerateArray())
                                            {
                                                string domain = domainElement.GetString();
                                                if (url.Contains(domain, StringComparison.OrdinalIgnoreCase))
                                                {
                                                    urlMatchesDomain = true;
                                                    break;
                                                }
                                            }

                                            // Nếu URL khớp với bất kỳ tên miền nào trong domains của _sessionData, _level sẽ là 1, nếu không thì _level sẽ là 2
                                            _level = urlMatchesDomain ? 1 : 2;
                                        }
                                    }    
                                }
                                else
                                {
                                    // Nếu _sessionData là null, gán level = 1
                                    _level = 2;
                                }
                               _levelFromChromeProcess = _level;

                                // Gửi URL với các tham số bổ sung đến máy chủ
                                await PostUrlToServer(url, _computerId, _computerRoomId, _monitorSessionId, _level);
                                Console.WriteLine(url);
                            }

                            entry = walker.GetNextSiblingElement(entry); // Lấy phần tử UI tiếp theo trong cùng cấu trúc cây
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Lỗi trong khi xử lý các phần tử cửa sổ Chrome");
                            break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong ProcessChromeWindows");
            }
        }


        private async Task PostUrlToServer(string url, string computerId, string computerRoomId, string monitorSessionId, int level)
        {
            try
            {
                string message = $"Máy {_computerName}:{_studentCode}:{url}";

                var postData = new
                {
                    message = message,
                    logTime = DateTime.UtcNow,
                    level = level,
                    computerId = _computerId,
                    computerRoomId = _computerRoomId,
                    monitorSessionId = _monitorSessionId
                };

                var json = JsonSerializer.Serialize(postData);

                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await _httpClient.PostAsync($"{_apiConfig.BaseUrl}{_apiConfig.ApiEndpoints.ComputerHistoryEndpoint}", content);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation($"URL posted successfully: {url}");
                }
                else
                {
                    _logger.LogError($"Failed to post URL: {url}, StatusCode: {response.StatusCode}");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error while posting URL: {url}");
            }
        }

        public override async Task StopAsync(CancellationToken cancellationToken)
        {
            _captureScreenTimer?.Change(Timeout.Infinite, 0);

            await base.StopAsync(cancellationToken);
        }

        public override void Dispose()
        {
            _captureScreenTimer?.Dispose();

            base.Dispose();
        }
    }
}