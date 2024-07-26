using ComputerManagement.BO.DTO;
using ComputerManagement.BO.DTO.MonitorSession;
using ComputerManagement.BO.Models;
using ComputerManagerment.Repos.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Interface
{
    public interface IMonitorSessionService : IBaseService<MonitorSessionDto, MonitorSession>
    {
        /// <summary>
        /// lấy danh sách phiên theo dõi theo id phòng máy
        /// </summary>
        /// <param name="computerRoomId"></param>
        /// <param name="pagingParam"></param>
        /// <returns></returns>
        Task<(List<MonitorSessionDto>, int)> GetListComputerByComputerRoomIdAsync(Guid computerRoomId, PagingParam pagingParam);

        /// <summary>
        /// lấy thông tin phiên làm việc đang hoạt động theo id phòng máy
        /// </summary>
        /// <param name="computerRoomId"></param>
        /// <returns></returns>
        Task<MonitorSessionDto> GetCurrentByComputerRoomIdAsync(Guid computerRoomId);
    }
}
