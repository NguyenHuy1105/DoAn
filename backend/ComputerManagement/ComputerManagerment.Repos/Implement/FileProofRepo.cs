using ComputerManagement.BO.Models;
using ComputerManagement.Data;
using ComputerManagerment.Repos.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagerment.Repos.Implement
{
    public class FileProofRepo(AppDbContext dbContext) : BaseRepo<FileProof>(dbContext), IFileProofRepo
    {
        public async Task<List<FileProof>> GetListByMonitorSessionIdAsync(Guid monitorSesisonId)
        {
            var query = _dbSet.AsQueryable();
            var entities = new List<FileProof>();
            query = query.Include(f => f.Student).Where(c => c.MonitorSessionId == monitorSesisonId).OrderByDescending(e => e.UpdatedAt);
            entities = await query.ToListAsync();

            return entities;
        }
    }
}
