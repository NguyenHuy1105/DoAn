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
    public class ComputerSoftwareRepo(AppDbContext dbContext) : BaseRepo<ComputerSoftware>(dbContext), IComputerSoftwareRepo
    {
        public async Task<List<ComputerSoftware>> GetListByComputerIdAsync(Guid computerId, string keySearch, string fieldSort, bool sortAsc)
        {
            var query = _dbSet.AsQueryable();
            var entities = new List<ComputerSoftware>();
            query = query.Where(c => c.ComputerId == computerId);


            switch (fieldSort?.ToLower())
            {

                case "updatedat":
                    query = sortAsc ? query.OrderBy(e => e.UpdatedAt) : query.OrderByDescending(e => e.UpdatedAt);
                    break;
                case "createdat":
                    query = sortAsc ? query.OrderBy(e => e.CreatedAt) : query.OrderByDescending(e => e.CreatedAt);
                    break;

                default:
                    query = query.OrderByDescending(e => e.UpdatedAt);
                    break;
            }
            entities = await query.Include(c => c.Computer).Include(c => c.Software).ToListAsync();

            return entities;
        }

        public override async Task<List<ComputerSoftware>> GetListByIdsAsync(List<Guid> ids)
        {
            return await _dbSet.Where(cr => ids.Contains(cr.Id)).ToListAsync();
        }
    }
}
