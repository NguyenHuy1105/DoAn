using ComputerManagement.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.DTO
{
    public class ServiceResponse
    {
        public bool Success { get; set; } = true;
        public ServiceResponseCode Code { get; set; } = ServiceResponseCode.Success;
        public string Message { get; set; } = "Oke";
        public object? Data { get; set; }
    }
}
