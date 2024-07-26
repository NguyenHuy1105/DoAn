using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Hubs
{
    public class ShareDb
    {
        private readonly ConcurrentDictionary<string, string> _connections = new ConcurrentDictionary<string, string>();
        public ConcurrentDictionary<string, string> connections => _connections;
    }
}
