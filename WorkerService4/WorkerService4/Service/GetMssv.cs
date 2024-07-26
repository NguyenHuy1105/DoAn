using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR.Client;

namespace WorkerService4.Service
{
    public class GetMssv
    {
        private readonly ILogger<GetMssv> _logger;
        private readonly HubConnection _hubConnection;
        private string _receivedMSSV;

        public GetMssv(ILogger<GetMssv> logger, HubConnection hubConnection)
        {
            _logger = logger;
            _hubConnection = hubConnection;
            InitializeSignalR();
        }

        private void InitializeSignalR()
        {
            _hubConnection.On<string>("ReceiveMessage", (mssv) =>
            {
                _logger.LogInformation($"Received MSSV: {mssv}");
                _receivedMSSV = mssv; // Lưu MSSV nhận được vào biến _receivedMSSV
            });

            _hubConnection.StartAsync().ContinueWith(task =>
            {
                if (task.IsFaulted)
                {
                    _logger.LogError($"Error starting SignalR connection: {task.Exception.GetBaseException()}");
                }
                else
                {
                    _logger.LogInformation("SignalR connection established.");
                }
            });
        }

        public Task<string> GetReceivedMSSV(CancellationToken stoppingToken)
        {
            // Trả về giá trị MSSV đã nhận từ SignalR
            return Task.FromResult(_receivedMSSV);
        }

        public async Task StartAsync(CancellationToken stoppingToken)
        {
            await _hubConnection.StartAsync();
        }

        public async Task StopAsync(CancellationToken stoppingToken)
        {
            await _hubConnection.StopAsync();
        }
    }
}
