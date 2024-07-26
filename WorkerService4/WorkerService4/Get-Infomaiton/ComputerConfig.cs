using System;
using System.Management;

namespace WorkerService4.Get_Infomaiton
{
    public static class ComputerConfig
    {
        public static string GetOperatingSystem()
        {
            return System.Runtime.InteropServices.RuntimeInformation.OSDescription;
        }

        public static string GetCPUInfo()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("select * from Win32_Processor"))
                {
                    foreach (var obj in searcher.Get())
                    {
                        return obj["Name"]?.ToString() ?? "Unknown";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting CPU info: {ex.Message}");
            }
            return "Unknown";
        }

        public static string GetRAMInfo()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("select * from Win32_ComputerSystem"))
                {
                    foreach (var obj in searcher.Get())
                    {
                        return $"{Math.Round(Convert.ToDouble(obj["TotalPhysicalMemory"]) / (1024 * 1024 * 1024), 2)} GB";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting RAM info: {ex.Message}");
            }
            return "Unknown";
        }
        public static string GetHardDriveUsedInfo()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("select * from Win32_LogicalDisk where DriveType=3"))
                {
                    foreach (var obj in searcher.Get())
                    {
                        return $"{Math.Round(Convert.ToDouble(obj["Size"]) / (1024 * 1024 * 1024), 2)} GB";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting HardDriveUsed info: {ex.Message}");
            }
            return "Unknown";
        }

        public static string GetHardDriveInfo()
        {
            try
            {
                using (var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_DiskDrive"))
                {
                    foreach (var obj in searcher.Get())
                    {
                        return $"{Math.Round(Convert.ToDouble(obj["Size"]) / (1024 * 1024 * 1024), 2)} GB";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting HardDrive info: {ex.Message}");
            }
            return "Unknown";
        }

    }
}
