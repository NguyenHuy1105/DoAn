using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using WorkerService4.Model;

namespace WorkerService4.Get_Infomaiton
{
    public static class CommandOptionService
    {
        /// <summary>
        /// Checks if download is allowed and retrieves the associated download ID.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance used for making HTTP requests.</param>
        /// <param name="computerId">The unique identifier of the computer.</param>
        /// <param name="apiConfig">The API configuration containing the base URL and endpoint paths.</param>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the operation.</param>
        /// <param name="logger">An ILogger instance for logging purposes.</param>
        /// <returns>A tuple containing a boolean indicating if download is allowed and the associated download ID.</returns>
        public static async Task<(bool downloadAllowed, Guid desId)> CheckDownloadAllowedAsync(HttpClient httpClient, Guid computerId, ApiConfig apiConfig, CancellationToken cancellationToken, ILogger logger)
        {
            try
            {
                // Construct the URL from the base URL and the endpoint path
                string url = $"{apiConfig.BaseUrl}{apiConfig.ApiEndpoints.GetListCommandOptionByComputerId}/{computerId}";

                logger.LogInformation($"Sending request to URL: {url}");

                HttpResponseMessage response = await httpClient.GetAsync(url, cancellationToken);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var responseJson = JsonDocument.Parse(responseBody);
                bool success = responseJson.RootElement.GetProperty("success").GetBoolean();

                if (success)
                {
                    var data = responseJson.RootElement.GetProperty("data");
                    foreach (var option in data.EnumerateArray())
                    {
                        string commandKey = option.GetProperty("commandKey").GetString();
                        bool commandValue = option.GetProperty("commandValue").GetBoolean();
                        Guid desId = Guid.Parse(option.GetProperty("desId").GetString());

                        if (commandKey == "CHECK_DOWLOAD_SOFTWARE")
                        {
                            return (commandValue, desId);
                        }
                    }

                    logger.LogInformation("No valid download command option found.");
                    return (false, Guid.Empty);
                }
                else
                {
                    logger.LogWarning("Request was not successful: {message}", responseJson.RootElement.GetProperty("message").GetString());
                    return (false, Guid.Empty);
                }
            }
            catch (TaskCanceledException)
            {
                logger.LogInformation("Request was canceled because the service is stopping.");
                throw;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while checking download options.");
                return (false, Guid.Empty);
            }
        }
    }
}
