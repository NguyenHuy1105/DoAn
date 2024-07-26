using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WorkerService4.Model;

namespace WorkerService4.Get_Infomaiton
{
    public static class CHECK_MONITOR_SESSION
    {
        /// <summary>
        /// Retrieves the check time for monitoring sessions.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance used for making HTTP requests.</param>
        /// <param name="apiConfig">The API configuration containing the base URL and endpoint paths.</param>
        /// <param name="stoppingToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>An integer representing the check time interval, or null if an error occurs or the data is not found.</returns>
        public static async Task<int?> GetCheckTimeMonitorSession(HttpClient httpClient, ApiConfig apiConfig, CancellationToken stoppingToken)
        {
            try
            {
                // Construct the URL from the base URL and the CheckMonitorSession endpoint
                string url = $"{apiConfig.BaseUrl}{apiConfig.ApiEndpoints.CheckMonitorSession}";

                HttpResponseMessage response = await httpClient.GetAsync(url, stoppingToken);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var responseJson = JsonDocument.Parse(responseBody);

                if (responseJson.RootElement.TryGetProperty("data", out var dataElement) && dataElement.TryGetProperty("optionValue", out var optionValueElement))
                {
                    if (int.TryParse(optionValueElement.GetString(), out int checkComputerStateInterval))
                    {
                        return checkComputerStateInterval;
                    }
                    else
                    {
                        return null;
                    }
                }
                else
                {
                    return null;
                }
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch
            {
                return null;
            }
        }
    }
}
