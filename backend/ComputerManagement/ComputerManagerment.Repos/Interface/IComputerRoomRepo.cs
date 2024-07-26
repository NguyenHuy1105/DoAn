using ComputerManagement.BO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagerment.Repos.Interface
{
    public interface IComputerRoomRepo : IBaseRepo<ComputerRoom>
    {
        /// <summary>
        /// lấy danh sách phòng máy có hiển thị thông tin tải,cài đăt, hoạt động của phần mềm
        /// </summary>
        /// <param name="softwareId"></param>
        /// <param name="keySearch"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="fieldSort"></param>
        /// <param name="sortAsc"></param>
        /// <returns></returns>
        Task<(List<ComputerRoom>, int)> GetListBySoftwareIdAsync(Guid softwareId,string keySearch, int pageNumber, int pageSize, string fieldSort, bool sortAsc);
    }
}
