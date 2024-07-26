namespace WorkerService4.Models
{
    public class SoftwareModel
    {
        public bool Success { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public SoftwareData Data { get; set; }
    }

    public class SoftwareData
    {
        public string Name { get; set; }
        public string Process { get; set; }
        public string InstallationFileFolder { get; set; }
        public string SoftwareFolder { get; set; }
        public bool IsUpdate { get; set; }
        public bool IsInstall { get; set; }
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }
}
