using System;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using WorkerService4.Get_Infomaiton;
using WorkerService4.Get_Information;
using WorkerService4.Model;

namespace WorkerService4.Service
{
    public class MACAddressWorker : BackgroundService
    {
        private readonly ILogger<MACAddressWorker> _logger;
        private readonly HttpClient _httpClient;
        private readonly ApiConfig _apiConfig;

        public MACAddressWorker(ILogger<MACAddressWorker> logger, HttpClient httpClient, IOptions<ApiConfig> apiConfig)
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

                    if (macAddress != null)
                    {
                        var (computerId, computerName, computerRoomId) = await GetComputer.GetMachineDataFromServer(_httpClient, macAddress,_apiConfig, stoppingToken);

                        if (computerId != null)
                        {
                            bool updateStateSuccess = await UpdateStateByMacAddress(macAddress, 1, stoppingToken);

                            if (updateStateSuccess)
                            {
                                bool updateConfigSuccess = await UpdateComputerConfig(computerId, stoppingToken);

                                if (updateConfigSuccess)
                                {
                                    _logger.LogInformation("State and computer config updated successfully.");
                                }
                                else
                                {
                                    _logger.LogWarning("Failed to update computer config.");
                                }
                            }
                            else
                            {
                                _logger.LogWarning("Failed to update state by MAC address.");
                            }
                        }
                        else
                        {
                            _logger.LogWarning("Failed to retrieve valid computer ID.");
                        }
                    }
                    else
                    {
                        _logger.LogWarning("Failed to retrieve valid MAC address.");
                    }
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError($"Error in ExecuteAsync: {ex.Message}");
                }

                int interval = await CHECK_COMPUTER_STATE.GetCheckComputerStateInterval(_httpClient,_apiConfig, stoppingToken) ?? 10000;
                await Task.Delay(TimeSpan.FromMilliseconds(interval), stoppingToken);
            }
        }

        private async Task<bool> UpdateStateByMacAddress(string macAddress, int state, CancellationToken stoppingToken)
        {
            try
            {
                string url = $"{_apiConfig.BaseUrl}{_apiConfig.ApiEndpoints.UpdateStateByMacAddress}{macAddress}";
                var jsonData = JsonSerializer.Serialize(new { state });
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PutAsync(url, content, stoppingToken);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var responseJson = JsonDocument.Parse(responseBody);
                bool success = responseJson.RootElement.GetProperty("success").GetBoolean();

                return success;
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating state by MAC address: {ex.Message}");
                return false;
            }
        }

        private async Task<bool> UpdateComputerConfig(string computerId, CancellationToken stoppingToken)
        {
            try
            {
                string os = ComputerConfig.GetOperatingSystem();
                string cpu = ComputerConfig.GetCPUInfo();
                string ram = ComputerConfig.GetRAMInfo();
                string hardDriver = ComputerConfig.GetHardDriveInfo();
                string hardDriverUsed = ComputerConfig.GetHardDriveUsedInfo();

                string url = $"{_apiConfig.BaseUrl}{_apiConfig.ApiEndpoints.UpdateComputerConfig}{computerId}";
                var jsonData = JsonSerializer.Serialize(new
                {
                    os,
                    cpu,
                    ram,
                    hardDriver,
                    hardDriverUsed
                });
                var content = new StringContent(jsonData, Encoding.UTF8, "application/json");
                HttpResponseMessage response = await _httpClient.PutAsync(url, content, stoppingToken);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var responseJson = JsonDocument.Parse(responseBody);
                bool success = responseJson.RootElement.GetProperty("success").GetBoolean();

                return success;
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error updating computer config: {ex.Message}");
                return false;
            }
        }
    }
}
