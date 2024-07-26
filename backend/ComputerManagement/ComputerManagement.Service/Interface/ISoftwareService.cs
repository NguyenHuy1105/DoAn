using ComputerManagement.BO.DTO;
using ComputerManagement.BO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Interface
{
    public interface ISoftwareService : IBaseService<SoftwareDto, SoftwareModel>
    {
        /// <summary>
        /// lấy danh sách phần mềm theo ids
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<List<SoftwareDto>> GetListByListIdsAsync(List<Guid> ids);
    }
}
