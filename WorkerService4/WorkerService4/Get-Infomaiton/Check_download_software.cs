using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WorkerService4.Model;

namespace WorkerService4.Get_Infomaiton
{
    public static class CHECK_DOWLOAD_SOFTWARE
    {
        /// <summary>
        /// Retrieves the check time for downloading software.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance used for making HTTP requests.</param>
        /// <param name="apiConfig">The API configuration containing the base URL and endpoint paths.</param>
        /// <param name="stoppingToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>An integer representing the check time interval, or null if an error occurs or the data is not found.</returns>
        public static async Task<int?> GetCheckTimeDownloadSoftware(HttpClient httpClient, ApiConfig apiConfig, CancellationToken stoppingToken)
        {
            try
            {
                // Construct the URL from the base URL and the CheckDownloadSoftware endpoint
                string url = $"{apiConfig.BaseUrl}{apiConfig.ApiEndpoints.CheckDownloadSoftware}";

                // Make an asynchronous GET request to the specified URL
                HttpResponseMessage response = await httpClient.GetAsync(url, stoppingToken);
                // Ensure the HTTP response indicates success
                response.EnsureSuccessStatusCode();

                // Read the response body as a string
                string responseBody = await response.Content.ReadAsStringAsync();
                // Parse the response body as JSON
                var responseJson = JsonDocument.Parse(responseBody);

                // Extract the "data" property from the JSON response
                if (responseJson.RootElement.TryGetProperty("data", out var dataElement) && dataElement.TryGetProperty("optionValue", out var optionValueElement))
                {
                    // Try to parse the "optionValue" property as an integer
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
                // Propagate the task cancellation exception
                throw;
            }
            catch (Exception ex)
            {
                // Log the exception and return null
                Console.WriteLine($"Error: {ex.Message}");
                return null;
            }
        }
    }
}
