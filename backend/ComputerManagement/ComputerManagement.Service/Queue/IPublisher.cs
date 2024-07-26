using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Queue
{
    public interface IPublisher
    {
        /// <summary>
        /// gửi dữ liệu lên queue
        /// </summary>
        /// <param name="queueName"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        bool EnQueue(string queueName, string message);
    }
}
