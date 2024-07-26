using ComputerManagement.BO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagerment.Repos.Interface
{
    public interface IComputerHistoryRepo : IBaseRepo<ComputerHistory>
    {
        /// <summary>
        /// lấy lịch sử theo computerId
        /// </summary>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="computerId"></param>
        /// <returns></returns>
        Task<(List<ComputerHistory>, int)> GetListByComputerIdAsync(int pageNumber, int pageSize, Guid computerId);
    }
}
