namespace WorkerService4.Models
{
    public class MonitorSessionResponse
    {
        public bool Success { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public MonitorSessionData Data { get; set; }
    }

    public class MonitorSessionData
    {
        public string ComputerRoomId { get; set; }
        public string ComputerRoom { get; set; }
        public int MonitorType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string Domain { get; set; } 
        public string OwnerId { get; set; }
        public string User { get; set; }
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
