using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Common.Configs
{
    public class EmailConfig
    {
        public string FromEmail { get; set; }
        public string FromPassword { get; set; }
        public string SmtpClient {  get; set; }
    }
}
