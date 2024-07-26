using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Common.Configs
{
    public class RabbitMQConfig
    {
        public string HostName { get; set; }
        public int Port { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public string VirtualHost { get; set; }
        public string Exchange { get; set; }
        public string RoutingKey { get; set; }
        public bool Durable { get; set; }
        public bool Exclusive { get; set; }
        public bool AutoDelete { get; set; }
        public bool AutoAck { get; set; }
        public UInt32 PerfetchSize { get; set; }
        public UInt16 PerfetchCount { get; set; }
        public bool Global { get; set; }
        public int BatchSize { get; set; } = 20;
    }
}
