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
    public class ComputerHistoryRepo(AppDbContext dbContext) : BaseRepo<ComputerHistory>(dbContext), IComputerHistoryRepo
    {
        public override async Task<List<ComputerHistory>> GetListByIdsAsync(List<Guid> ids)
        {
            return await _dbSet.Where(cr => ids.Contains(cr.Id)).ToListAsync();
        }
        public async Task<(List<ComputerHistory>, int)> GetListByComputerIdAsync(int pageNumber, int pageSize, Guid computerId)
        {
            var query = _dbSet.AsQueryable();
            var computerHistories = new List<ComputerHistory>();


            query = query.Where(ch => ch.ComputerId == computerId).OrderByDescending(e => e.LogTime);
            var totalCount = await query.CountAsync();
            if (pageNumber > 0 && pageSize > 0)
            {
                computerHistories = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            }
            else
            {
                computerHistories = await query.ToListAsync();
            }


            return (computerHistories, totalCount);
        }
    }
}
