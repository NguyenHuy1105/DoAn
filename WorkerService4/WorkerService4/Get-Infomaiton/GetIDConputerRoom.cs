using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace WorkerService4.Get_Infomaiton
{
    public class GetIDComputerRoom
    {
        public static async Task<string> GetMachineIdComputerRoom(HttpClient httpClient, string macAddress, CancellationToken stoppingToken)
        {
            try
            {
                string url = $"http://203.171.20.94:8102/api-agent/Computer/GetByMacAddress/{macAddress}";
                HttpResponseMessage response = await httpClient.GetAsync(url, stoppingToken);
                response.EnsureSuccessStatusCode();

                string responseBody = await response.Content.ReadAsStringAsync();
                var responseJson = JsonDocument.Parse(responseBody);
                bool success = responseJson.RootElement.GetProperty("success").GetBoolean();

                if (success)
                {
                    var data = responseJson.RootElement.GetProperty("data");
                    string computerRoomId = data.GetProperty("computerRoomId").GetString(); 
                    return computerRoomId;
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
            catch (Exception ex)
            {
                // Xử lý các ngoại lệ khác
                Console.WriteLine($"An error occurred: {ex.Message}");
                return null;
            }
        }
    }
}
