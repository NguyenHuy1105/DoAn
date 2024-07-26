using ComputerManagement.BO.DTO;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Hubs
{
    public class MonitorSessionHub(ShareDb shareDb) : Hub
    {
        private readonly ShareDb _shared = shareDb;

        public async Task Connect(string key)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, key);
            _shared.connections[Context.ConnectionId] = key;
            await Clients.Group(key).SendAsync("ReceviceMessageConnect", "Connect establed");
        }

        public async Task SendMessage(MessageSocket msg)
        {
            if (_shared.connections.TryGetValue(Context.ConnectionId, out string key)) {
                await Clients.Group(key).SendAsync("ReceviceMessage", msg);
            }
        }
    }
}
