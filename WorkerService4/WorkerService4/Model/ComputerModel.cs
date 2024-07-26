using System;
using System.Collections.Generic;

namespace WorkerService4.Get_Infomaiton
{
    public class GetComputerResponse
    {
        public bool Success { get; set; }
        public int Code { get; set; }
        public string Message { get; set; }
        public ComputerData Data { get; set; }
    }
    public class ComputerData
    {
        public string MacAddress { get; set; }
        public string Name { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public string OS { get; set; }
        public string CPU { get; set; }
        public string RAM { get; set; }
        public string HardDriver { get; set; }
        public string HardDriverUsed { get; set; }
        public string ComputerRoomId { get; set; }
        public ComputerRoom ComputerRoom { get; set; }
        public List<object> ComputerSoftwares { get; set; }
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

    public class ComputerRoom
    {
        public string Name { get; set; }
        public int Row { get; set; }
        public int Col { get; set; }
        public int MaxCapacity { get; set; }
        public int CurrentCapacity { get; set; }
        public int CurrentDownloadSoftware { get; set; }
        public int CurrentInstalledSoftware { get; set; }
        public int CurrentActiveSoftware { get; set; }
        public string Id { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
    }

}
