using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using WorkerService4.Model;

namespace WorkerService4.Get_Information
{
    public class GetStudent
    {
        private static readonly HttpClient client = new HttpClient();
        /// <summary>
        /// Retrieves machine data from the server using the specified 
        /// </summary>
        /// <param name="httpClient">The HttpClient instance used for making HTTP requests.</param>
        /// <param name="macAddress">The MAC address of the machine.</param>
        /// <param name="apiConfig">The API configuration containing the base URL and endpoint paths.</param>
        /// <param name="stoppingToken">A cancellation token that can be used to cancel the operation.</param>
        /// <returns>A tuple containing the machine's ID, name, and room ID.</returns>


        public static async Task<(string Id, string StudentCode)> GetStudentInfoAsync(ApiConfig apiConfig, CancellationToken stoppingToken)
        {
            string filePath = @"C:/Software/mssv.txt";
            string mssv = ReadMSSVFromFile(filePath);

            if (!string.IsNullOrEmpty(mssv) && int.TryParse(mssv, out _))
            {
                try
                {
                    string url = $"{apiConfig.BaseUrl}{apiConfig.ApiEndpoints.GetByStudentCode}{mssv}";
                    HttpResponseMessage response = await client.GetAsync(url, stoppingToken);
                    response.EnsureSuccessStatusCode();

                    string responseBody = await response.Content.ReadAsStringAsync();
                    var responseJson = JsonDocument.Parse(responseBody);
                    bool success = responseJson.RootElement.GetProperty("success").GetBoolean();

                    if (success)
                    {
                        var dataElement = responseJson.RootElement.GetProperty("data");
                        string id = dataElement.GetProperty("id").GetString();
                        string studentCode = dataElement.GetProperty("studentCode").GetString();
                        return (id, studentCode);
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
            else
            {
                throw new Exception("Không tìm thấy MSSV hợp lệ trong tệp mssv.txt.");
            }
        }

        private static string ReadMSSVFromFile(string filePath)
        {
            try
            {
                if (File.Exists(filePath))
                {
                    string content = File.ReadAllText(filePath);
                    return content.Trim();
                }
                else
                {
                    throw new FileNotFoundException("Không tìm thấy tệp mssv.txt.");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Lỗi khi đọc tệp mssv.txt: {ex.Message}");
            }
        }
    }
}