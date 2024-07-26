using System;
using System.IO;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using WorkerService4.Get_Infomaiton;
using WorkerService4.Model;
using WorkerService4.Models;

namespace WorkerService4.Service
{
    public class InstallFileService : BackgroundService
    {
        private readonly ILogger<InstallFileService> _logger;
        private readonly HttpClient _httpClient;
        private readonly ApiConfig _apiConfig;

        public InstallFileService(ILogger<InstallFileService> logger, HttpClient httpClient, IOptions<ApiConfig> apiConfig)
        {
            _logger = logger;
            _httpClient = httpClient;
            _apiConfig = apiConfig.Value;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    string macAddress = GetMac.GetMACAddress();
                    var (computerId, computerName, computerRoomId) = await GetComputer.GetMachineDataFromServer(_httpClient, macAddress,_apiConfig, stoppingToken);
                    if (computerId !=null)
                    {
                        var commandOptions = await GetCommandOptionsAsync(computerId, stoppingToken);
                        if (commandOptions != null && commandOptions.Length > 0)
                        {
                            foreach (var option in commandOptions)
                            {
                                var softwareDetails = await GetSoftwareDetailsAsync(option.DesId, stoppingToken);
                                if (softwareDetails != null)
                                {
                                    bool isInstalled = CheckInstallationStatus(softwareDetails.Name, softwareDetails.SoftwareFolder);
                                    await UpsertInstallSoftware(computerId, option.DesId, isInstalled, stoppingToken);
                                }
                                else
                                {
                                    _logger.LogInformation($"Không thể lấy chi tiết phần mềm cho desId {option.DesId}.");
                                }
                            }
                        }
                        else
                        {
                            _logger.LogInformation("Không tìm thấy tùy chọn lệnh hợp lệ cho máy tính.");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi trong quá trình thực thi dịch vụ.");
                }

                int interval = await CHECK_INSTALL_SOFTWARE.GetCheckTimeInstallSoftware(_httpClient,_apiConfig, stoppingToken) ?? 10000;
                await Task.Delay(TimeSpan.FromMilliseconds(interval), stoppingToken);
            }
        }

        //private async Task<Guid> GetComputerIdAsync(CancellationToken cancellationToken)
        //{
        //    try
        //    {
        //        string macAddress = GetMac.GetMACAddress(); // Lấy địa chỉ MAC của máy tính
        //        _logger.LogInformation($"MAC Address: {macAddress}");

        //        string machineId = await GetComputer.GetMachineIdFromServer(_httpClient, macAddress, cancellationToken);
        //        if (machineId != null && Guid.TryParse(machineId, out Guid computerId))
        //        {
        //            return computerId;
        //        }
        //        else
        //        {
        //            _logger.LogWarning("Không thể lấy ID máy tính hợp lệ.");
        //            return Guid.Empty;
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Lỗi khi lấy ID máy tính.");
        //        return Guid.Empty;
        //    }
        //}

        private async Task<CommandOptionData[]> GetCommandOptionsAsync(string computerId, CancellationToken cancellationToken)
        {
            try
            {
                string url = $"{_apiConfig.BaseUrl}{_apiConfig.ApiEndpoints.GetListCommandOptionByComputerIdAndCommandOptionKey}{computerId}/CHECK_INSTALL_SOFTWARE";
                _logger.LogInformation($"Gửi yêu cầu đến URL: {url}");

                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var commandOptionModel = JsonConvert.DeserializeObject<CommandOptionModel>(responseBody);

                if (commandOptionModel.Success && commandOptionModel.Data != null && commandOptionModel.Data.Length > 0)
                {
                    return commandOptionModel.Data;
                }
                else
                {
                    _logger.LogInformation("Không có dữ liệu tùy chọn lệnh hợp lệ.");
                    return Array.Empty<CommandOptionData>();
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy các tùy chọn lệnh.");
                return null;
            }
        }

        private async Task<SoftwareData> GetSoftwareDetailsAsync(Guid desId, CancellationToken cancellationToken)
        {
            try
            {
                string url = $"http://203.171.20.94:8102/api-agent/Software/GetById/{desId}";
                _logger.LogInformation($"Gửi yêu cầu đến URL: {url}");

                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var softwareData = JsonConvert.DeserializeObject<SoftwareData>(responseBody);

                return softwareData;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi lấy chi tiết phần mềm cho desId {desId}.");
                return null;
            }
        }

        private bool CheckInstallationStatus(string softwareName, string installationFolderPath)
        {
            try
            {
                if (Directory.Exists(installationFolderPath))
                {
                    string[] directories = Directory.GetDirectories(installationFolderPath);
                    foreach (string directory in directories)
                    {
                        string directoryName = new DirectoryInfo(directory).Name;
                        if (directoryName.Equals(softwareName, StringComparison.OrdinalIgnoreCase))
                        {
                            return true;
                        }
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi kiểm tra thư mục cài đặt '{installationFolderPath}'.");
                return false;
            }
        }

        private async Task UpsertInstallSoftware(string computerId, Guid desId, bool isInstalled, CancellationToken cancellationToken)
        {
            try
            {
                string url = "http://203.171.20.94:8102/api-agent/ComputerSoftware/UpsertInstallSoftware";
                _logger.LogInformation($"Gửi yêu cầu upsert đến URL: {url}");

                var requestModel = new
                {
                    computerId = computerId.ToString(),
                    softwareId = desId.ToString(),
                    isInstalled = isInstalled
                };

                string jsonContent = JsonConvert.SerializeObject(requestModel);
                var content = new StringContent(jsonContent, System.Text.Encoding.UTF8, "application/json");

                HttpResponseMessage response = await _httpClient.PostAsync(url, content, cancellationToken);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                _logger.LogInformation($"Đã upsert thành công: {responseBody}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi upsert command option.");
            }
        }
    }
}
