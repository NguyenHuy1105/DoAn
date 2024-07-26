using ComputerManagement.Common.Configs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.Lib.Interface
{
    public interface IEmailService
    {

        EmailConfig GetEmailConfig();
        /// <summary>
        /// gửi email
        /// </summary>
        /// <param name="receiver"></param>
        /// <param name="subject"></param>
        /// <param name="body"></param>
        /// <returns></returns>
        Task SenEmailAsync(string receiver, string subject, string body);
    }
}
