using ComputerManagement.BO.DTO;
using ComputerManagement.BO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Interface
{
    public interface IComputerHistoryService: IBaseService<ComputerHistoryDto, ComputerHistory>
    {

        /// <summary>
        /// lấy tất cả lịch sử theo phiên giám sát
        /// </summary>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        Task<List<ComputerHistoryDto>> GetAllByMonitorSessionId(Guid sessionId);
    }
}
