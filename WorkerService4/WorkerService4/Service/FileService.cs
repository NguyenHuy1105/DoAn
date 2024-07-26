using System;
using System.IO;
using System.Net.Http;
using System.Net.Mail;
using System.Text;
using System.Text.RegularExpressions;
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
    public class FileService : BackgroundService
    {
        private readonly ILogger<FileService> _logger;
        private readonly HttpClient _httpClient;
        private readonly ApiConfig _apiConfig;

        public FileService(ILogger<FileService> logger, HttpClient httpClient, IOptions<ApiConfig> apiConfig)
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
                    var (computerId, computerName, computerRoomId) = await GetComputer.GetMachineDataFromServer(_httpClient, macAddress, _apiConfig, stoppingToken);
                    if (computerId != null)
                    {
                        var commandOptions = await GetCommandOptionsAsync(computerId, stoppingToken);
                        if (commandOptions != null && commandOptions.Length > 0)
                        {
                            foreach (var option in commandOptions)
                            {
                                if (option.CommandValue)
                                {
                                    var softwareDetails = await GetSoftwareDetailsAsync(option.DesId, stoppingToken);
                                    if (softwareDetails != null)
                                    {
                                        string fileName = FindMatchingFile(softwareDetails.Id, softwareDetails.InstallationFileFolder, _logger);
                                        if (!string.IsNullOrEmpty(fileName))
                                        {
                                            await CheckUpdateAsync(fileName, stoppingToken);
                                        }
                                        else
                                        {
                                            _logger.LogInformation($"Không tìm thấy tệp tin phù hợp với '{softwareDetails.Name}' trong thư mục cài đặt.");
                                            string latestFileName = await GetLatestFileNameAsync(option.DesId, stoppingToken);
                                            if (!string.IsNullOrEmpty(latestFileName))
                                            {
                                                await DownloadFileAsync(latestFileName, stoppingToken);
                                                await UpsertDowloadSoftware(computerId, option.DesId, true, stoppingToken);
                                            }
                                            else
                                            {
                                                _logger.LogInformation($"Không tìm thấy tệp tin mới nhất cho desId {option.DesId}.");
                                                await UpsertDowloadSoftware(computerId, option.DesId, false, stoppingToken);
                                            }
                                        }
                                    }
                                    else
                                    {
                                        _logger.LogInformation($"Không thể lấy chi tiết phần mềm cho desId {option.DesId}.");
                                        await UpsertDowloadSoftware(computerId, option.DesId, false, stoppingToken);
                                    }
                                }
                                else
                                {
                                    _logger.LogInformation("Không cho phép tải xuống dựa trên các tùy chọn lệnh.");
                                    await UpsertDowloadSoftware(computerId, option.DesId, false, stoppingToken);
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

                int interval = await CHECK_DOWLOAD_SOFTWARE.GetCheckTimeDownloadSoftware(_httpClient, _apiConfig, stoppingToken) ?? 10000;
                await Task.Delay(TimeSpan.FromMilliseconds(interval), stoppingToken);
            }
        }


        private async Task UpsertDowloadSoftware(string computerId, Guid desId, bool isDownloadFile, CancellationToken cancellationToken)
        {
            try
            {
                string url = $"{_apiConfig.BaseUrl}{_apiConfig.ApiEndpoints.UpsertDowloadSoftware}";
                _logger.LogInformation($"Gửi yêu cầu upsert đến URL: {url}");

                var requestModel = new
                {
                    computerId = computerId.ToString(),
                    softwareId = desId.ToString(),
                    isDowloadFile = isDownloadFile
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

        private async Task CheckUpdateAsync(string fileName, CancellationToken cancellationToken)
        {
            try
            {
                string url = $"{_apiConfig.BaseUrl}{_apiConfig.ApiEndpoints.CheckUpdateFile}{fileName}";
                _logger.LogInformation($"Gửi yêu cầu kiểm tra cập nhật đến URL: {url}");

                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();

                // Phân tích JSON để lấy giá trị của trường "data"
                var json = JsonConvert.DeserializeObject<JObject>(responseBody);
                string dataValue = json["data"]?.ToString();

                if (!string.IsNullOrEmpty(dataValue))
                {
                    _logger.LogInformation($"Dữ liệu từ API kiểm tra cập nhật: {dataValue}");
                    await DownloadFileAsync(dataValue, cancellationToken);
                }
                else
                {
                    _logger.LogInformation("Không có dữ liệu cập nhật mới từ API.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi kiểm tra cập nhật cho tệp tin '{fileName}'.");
            }
        }

       
        private async Task<CommandOptionData[]> GetCommandOptionsAsync(string computerId, CancellationToken cancellationToken)
        {
            try
            {
                string url = $"{_apiConfig.BaseUrl}{_apiConfig.ApiEndpoints.GetListCommandOptionByComputerIdAndCommandOptionKey}{computerId}/CHECK_DOWLOAD_SOFTWARE";
                _logger.LogInformation($"Gửi yêu cầu đến URL: {url}");

                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();

                // Deserialize JSON thành CommandOptionModel
                var commandOptionModel = JsonConvert.DeserializeObject<CommandOptionModel>(content);

                // Kiểm tra Success và Data
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
            string url = $"{_apiConfig.BaseUrl}{_apiConfig.ApiEndpoints.GetFileById}{desId}";
            _logger.LogInformation($"Gửi yêu cầu đến URL: {url}");

            HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
            response.EnsureSuccessStatusCode();

            var content = await response.Content.ReadAsStringAsync();
            var softwareModel = JsonConvert.DeserializeObject<SoftwareModel>(content);

            if (softwareModel != null && softwareModel.Success && softwareModel.Data != null)
            {
                var softwareData = softwareModel.Data;

                return softwareData;
            }
            else
            {
                _logger.LogWarning($"Không lấy được thông tin phần mềm từ API với desId {desId}. Response: {content}");
                return null;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Lỗi khi lấy chi tiết phần mềm cho desId {desId}.");
            return null;
        }
    }



        private string FindMatchingFile(string softwareName, string installationFolderPath, ILogger logger)
        {
            try
            {
                if (Directory.Exists(installationFolderPath))
                {
                    var files = Directory.GetFiles(installationFolderPath, "*", SearchOption.AllDirectories);
                    string matchingFile = null;
                    Version maxVersion = new Version("0.0.0.0");

                    // Regex để khớp phần phiên bản trong tên tệp
                    Regex versionRegex = new Regex(@"_(\d+\.\d+\.\d+)$", RegexOptions.IgnoreCase);

                    foreach (var file in files)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(file);
                        if (fileName.StartsWith(softwareName, StringComparison.OrdinalIgnoreCase))
                        {
                            // Kiểm tra phần phiên bản
                            Match match = versionRegex.Match(fileName);
                            if (match.Success)
                            {
                                string versionPart = match.Groups[1].Value;
                                if (Version.TryParse(versionPart, out Version version))
                                {
                                    if (version > maxVersion)
                                    {
                                        maxVersion = version;
                                        matchingFile = Path.GetFileName(file); // Lấy tên tệp mà không có đường dẫn
                                                                               // Ghi log khi tìm thấy tệp phù hợp
                                        logger.LogInformation($"Đã tìm thấy tệp phù hợp cho phần mềm '{softwareName}': {matchingFile}, phiên bản: {maxVersion}");
                                    }
                                }
                            }
                        }
                    }

                    if (matchingFile == null)
                    {
                        // Ghi log nếu không tìm thấy tệp phù hợp
                        logger.LogWarning($"Không tìm thấy tệp phù hợp cho phần mềm '{softwareName}' trong thư mục cài đặt '{installationFolderPath}'.");
                    }

                    return matchingFile;
                }
                else
                {
                    // Ghi log nếu thư mục cài đặt không tồn tại
                    logger.LogWarning($"Thư mục cài đặt '{installationFolderPath}' không tồn tại.");
                    return null;
                }
            }
            catch (Exception ex)
            {
                // Ghi log nếu có lỗi xảy ra trong quá trình tìm kiếm tệp tin
                logger.LogError(ex, $"Lỗi khi kiểm tra tệp tin trong thư mục cài đặt '{installationFolderPath}'.");
                return null;
            }
        }


        private async Task<string> GetLatestFileNameAsync(Guid desId, CancellationToken cancellationToken)
        {
            try
            {
                string url = $"{_apiConfig.BaseUrl}{_apiConfig.ApiEndpoints.GetFileNameLatestBySoftwareId}{desId}";
                _logger.LogInformation($"Gửi yêu cầu đến URL: {url}");

                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();

                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<FileResponseModel>(content);

                return result?.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi lấy tên file mới nhất cho desId {desId}.");
                return null;
            }
        }

        private async Task DownloadFileAsync(string fileName, CancellationToken cancellationToken)
        {
            try
            {
                string url = $"{_apiConfig.BaseUrl}{_apiConfig.ApiEndpoints.GetFileByFileName}{fileName}";

                _logger.LogInformation($"Gửi yêu cầu tải xuống đến URL: {url}");

                HttpResponseMessage response = await _httpClient.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();

                var contentBytes = await response.Content.ReadAsByteArrayAsync();
                string savePath = Path.Combine("C:\\Software\\Downloads", fileName); // Thay đổi đường dẫn lưu file tùy vào yêu cầu của bạn

                await File.WriteAllBytesAsync(savePath, contentBytes, cancellationToken);

                _logger.LogInformation($"Tệp tin đã được tải xuống và lưu tại: {savePath}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Lỗi khi tải xuống tệp tin '{fileName}'.");
            }
        }
        //private async Task<int?> GetCheckTimeDowloadFile(CancellationToken stoppingToken)
        //{
        //    try
        //    {
        //        return await CHECK_DOWLOAD_SOFTWARE.GetCheckTimeDownloadSoftware(_httpClient, _apiConfig, stoppingToken);
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError(ex, "Error while fetching monitor session interval");
        //        return null;
        //    }
        //}
    }
}
