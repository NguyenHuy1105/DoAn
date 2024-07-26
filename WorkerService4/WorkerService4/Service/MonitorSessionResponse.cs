using System;

namespace WorkerService4.Service
{
    public class MonitorSessionResponse
    {
        public bool Success { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public object Data { get; set; } 
    }
}
