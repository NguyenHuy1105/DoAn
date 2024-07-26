using ComputerManagement.BO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagerment.Repos.Interface
{
    public interface IComputerSoftwareRepo:IBaseRepo<ComputerSoftware>
    {
        /// <summary>
        /// lấy danh sách phần mềm theo id máy
        /// </summary>
        /// <param name="computerId"></param>
        /// <param name="keySearch"></param>
        /// <param name="fieldSort"></param>
        /// <param name="sortAsc"></param>
        /// <returns></returns>
        Task<List<ComputerSoftware>> GetListByComputerIdAsync(Guid computerId, string keySearch, string fieldSort, bool sortAsc);

    }
}
