using ComputerManagement.BO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagerment.Repos.Interface
{
    public interface IFileProofRepo : IBaseRepo<FileProof>
    {
        /// <summary>
        /// lấy thông tin file bằng chứng theo id phiên giám sát
        /// </summary>
        /// <param name="monitorSesisonId"></param>
        /// <returns></returns>
        Task<List<FileProof>> GetListByMonitorSessionIdAsync(Guid monitorSesisonId);
    }
}
