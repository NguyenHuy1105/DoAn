using ComputerManagement.BO.DTO;
using ComputerManagement.BO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Interface
{
    public interface IComputerSoftwareService : IBaseService<ComputerSoftwareDto, ComputerSoftware>
    {
        /// <summary>
        /// thêm mới/ cập nhật trạng thái tải/ cài đặt phần mềm về agent
        /// </summary>
        /// <param name="computerSoftwareDto"></param>
        /// <param name="flag"></param> cờ capaj nhật tải/ cài đặt
        /// <returns></returns>
        Task<bool> UpsertAsync(ComputerSoftwareDto computerSoftwareDto, string flag);

        /// <summary>
        /// lấy danh sách phần mềm theo id máy tính
        /// </summary>
        /// <param name="computerId"></param>
        /// <param name="pagingParam"></param>
        /// <returns></returns>
        Task<List<ComputerSoftwareDto>> GetListByComputerIdAsync(Guid computerId, PagingParam pagingParam);
    }
}
