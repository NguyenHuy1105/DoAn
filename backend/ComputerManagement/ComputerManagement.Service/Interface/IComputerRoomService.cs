using ComputerManagement.BO.DTO;
using ComputerManagement.BO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Interface
{
    public interface IComputerRoomService : IBaseService<ComputerRoomDto, ComputerRoom>
    {
        /// <summary>
        /// lấy danh sách phòng máy có hiển thị thông tin tải,cài đăt, hoạt động của phần mềm
        /// </summary>
        /// <param name="softwareId"></param>
        /// <param name="pagingParam"></param>
        /// <returns></returns>
        Task<(List<ComputerRoomDto>, int)> GetListBySoftwareIdAsync(Guid softwareId, PagingParam pagingParam);
    }
}
