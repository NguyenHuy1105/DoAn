using ComputerManagement.BO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagerment.Repos.Interface
{
    public interface IFileRepo : IBaseRepo<FileModel>
    {
        /// <summary>
        /// lấy danh sách file cài theo phần mềm
        /// </summary>
        /// <param name="softwareId"></param>
        /// <returns></returns>
        Task<List<FileModel>> GetListFileBySoftwareId(Guid softwareId);
    }
}
