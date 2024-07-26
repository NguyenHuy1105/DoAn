using ComputerManagement.BO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagerment.Repos.Interface
{
    public interface IComputerRepo : IBaseRepo<Computer>
    {
        /// <summary>
        /// lấy danh sách máy tính theo id phòng máy
        /// </summary>
        /// <param name="computerId"></param>
        /// <param name="keySearch"></param>
        /// <param name="fieldSort"></param>
        /// <param name="sortAsc"></param>
        /// <returns></returns>
        Task<List<Computer>> GetListComputerByComputerRoomIdAsync(Guid computerRoomId, string keySearch, string fieldSort, bool sortAsc);

        /// <summary>
        /// lấy danh sách máy tính theo phần mềm
        /// </summary>
        /// <param name="softwareId"></param>
        /// <param name="keySearch"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="fieldSort"></param>
        /// <param name="sortAsc"></param>
        /// <returns></returns>
        Task<(List<Computer>, int)> GetListBySoftwareIdAsync(Guid softwareId, string keySearch, int pageNumber, int pageSize, string fieldSort, bool sortAsc);

        /// <summary>
        /// lấy số lượng máy đang bật và tổng số máy
        /// </summary>
        /// <param name="checkTime"></param>
        /// <returns></returns>
        Task<List<int>> GetComputerOnlineChart(long checkTime);

        /// <summary>
        /// thống kê số lượng máy tính theo tình trạng
        /// </summary>
        /// <returns></returns>
        Task<List<int>> GetComputerByListErrorChart();
    }
}
