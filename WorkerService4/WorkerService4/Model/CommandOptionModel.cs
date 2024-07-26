namespace WorkerService4.Models
{
    public class CommandOptionModel
    {
        public bool Success { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public CommandOptionData[] Data { get; set; }
    }

    public class CommandOptionData
    {
        public Guid SourceId { get; set; }
        public Guid DesId { get; set; }
        public string CommandKey { get; set; }
        public bool CommandValue { get; set; }
        public Guid Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
