using ComputerManagement.BO.DTO;
using ComputerManagement.BO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagerment.Repos.Interface
{
    public interface IMonitorSessionRepo : IBaseRepo<MonitorSession>
    {
        /// <summary>
        /// lấy danh sách phiên theo phòng máy
        /// </summary>
        /// <param name="computerRoomId"></param>
        /// <param name="KeySearch"></param>
        /// <param name="PageNumber"></param>
        /// <param name="PageSize"></param>
        /// <param name="FieldSort"></param>
        /// <param name="SortAsc"></param>
        /// <param name="filtes"></param>
        /// <returns></returns>
        Task<(List<MonitorSession>, int)> GetListComputerByComputerRoomIdAsync(Guid computerRoomId, string keySearch, int pageNumber, int pageSize, string fieldSort, bool sortAsc, Dictionary<string, string>? filters = null);
    }
}
