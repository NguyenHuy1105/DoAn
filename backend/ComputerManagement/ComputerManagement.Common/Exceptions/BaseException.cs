using ComputerManagement.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Common.Exceptions
{
    public class BaseException : Exception
    {
        public HttpStatusCode StatusCode { get; set; } = HttpStatusCode.InternalServerError;
        public ServiceResponseCode Code { get; set; } = ServiceResponseCode.Exception;
        public string Message { get; set; }
        public object Data { get; set; }
    }
}
