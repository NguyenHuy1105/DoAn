using System.Text.Json;
using WorkerService4.Model;

namespace WorkerService4.Get_Infomaiton
{
    public class GetComputer
    {
        /// <summary>
        /// Retrieves machine data from the server using the specified MAC address.
        /// </summary>
        /// <param name="httpClient">The HttpClient instance used for making HTTP requests.</param>
        /// <param name="macAddress">The MAC address of the machine.</param>
        /// <param name="apiConfig">The API configuration containing the base URL and endpoint paths.</param>
        /// <param name="stoppingToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A tuple containing the machine's ID, name, and room ID.</returns>
        public static async Task<(string id, string name, string roomID)> GetMachineDataFromServer(HttpClient httpClient, string macAddress, ApiConfig apiConfig, CancellationToken stoppingToken)
        {
            try
            {
                // Construct the URL from the base URL and the endpoint path
                string url = $"{apiConfig.BaseUrl}{apiConfig.ApiEndpoints.GetComputerByMacAddress}{macAddress}";

                HttpResponseMessage response = await httpClient.GetAsync(url, stoppingToken);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var responseJson = JsonDocument.Parse(responseBody);
                bool success = responseJson.RootElement.GetProperty("success").GetBoolean();

                if (success)
                {
                    var dataElement = responseJson.RootElement.GetProperty("data");
                    string id = dataElement.GetProperty("id").GetString();
                    string name = dataElement.GetProperty("name").GetString();
                    string roomID = dataElement.GetProperty("computerRoomId").GetString();
                    return (id, name, roomID);
                }
                else
                {
                    var code = responseJson.RootElement.GetProperty("code").GetInt32();
                    var message = responseJson.RootElement.GetProperty("message").GetString();
                    throw new HttpRequestException($"Yêu cầu không thành công. Mã lỗi: {code}, Thông điệp lỗi: {message}");
                }
            }
            catch (TaskCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                throw new HttpRequestException($"Đã xảy ra lỗi: {ex.Message}");
            }
        }
    }
}
