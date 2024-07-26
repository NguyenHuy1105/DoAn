namespace WorkerService4.Model
{
    public class ApiConfig
    {
        public string BaseUrl { get; set; }
        public ApiEndpoints ApiEndpoints { get; set; }
    }

    public class ApiEndpoints
    {
        public string UpdateStateByMacAddress { get; set; }
        public string UpdateComputerConfig { get; set; }
        public string CheckDownloadSoftware { get; set; }
        public string MonitorSessionEndpoint { get; set; }
        public string ComputerHistoryEndpoint { get; set; }
        public string GetCurrentSessionEndpoint { get; set; }
        public string ComputerScreenCaptureEndpoint { get; set; }
        public string CheckInstallSoftware { get; set; }
        public string GetListCommandOptionByComputerId { get; set; }

        public string GetComputerByMacAddress { get; set; }
        public string CheckMonitorSession { get; set; }
        public string CheckComputerState { get; set; }
        public string UpsertDowloadSoftware { get; set;}
        public  string CheckUpdateFile {  get; set; }
        public string GetListCommandOptionByComputerIdAndCommandOptionKey { get; set;}
        public string GetFileById { get; set; }
        public string GetByStudentCode { get; set; }
        public string GetFileNameLatestBySoftwareId { get;set; }
        public string GetFileByFileName { get; set; }

        public string uploadfile {  get; set; }
    }
}
