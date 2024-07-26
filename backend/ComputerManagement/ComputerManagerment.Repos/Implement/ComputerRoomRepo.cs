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
    public class ComputerRoomRepo : BaseRepo<ComputerRoom> , IComputerRoomRepo
    {
        public ComputerRoomRepo(AppDbContext dbContext) : base(dbContext)
        {
            
        }

        public override async Task<List<ComputerRoom>> GetListByIdsAsync(List<Guid> ids)
        {
            return await _dbSet.Where(cr => ids.Contains(cr.Id)).ToListAsync();
        }
        public override async Task<(List<ComputerRoom>, int)> GetListAsync(string keySearch, int pageNumber, int pageSize, string fieldSort, bool sortAsc, Dictionary<string, string>? Filters = null)
        {
            var query = _dbSet.AsQueryable();
            var entities = new List<ComputerRoom>();

            if (!string.IsNullOrEmpty(keySearch))
            {
                query = query.Where(e => e.Name.Contains(keySearch));
            }

            switch (fieldSort?.ToLower())
            {
                
                case "name":
                    query = sortAsc ? query.OrderBy(e => e.Name) : query.OrderByDescending(e => e.Name);
                    break;
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
            var totalCount = await query.CountAsync();
            if (pageNumber > 0 && pageSize > 0)
            {
                entities = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            }
            else
            {
                entities = await query.ToListAsync();
            }


            return (entities, totalCount);
        }

        public async Task<(List<ComputerRoom>, int)> GetListBySoftwareIdAsync(Guid softwareId, string keySearch, int pageNumber, int pageSize, string fieldSort, bool sortAsc)
        {
            var query = _dbSet.AsQueryable();
            var entities = new List<ComputerRoom>();

            if (!string.IsNullOrEmpty(keySearch))
            {
                query = query.Where(e => e.Name.Contains(keySearch));
            }

            switch (fieldSort?.ToLower())
            {

                case "name":
                    query = sortAsc ? query.OrderBy(e => e.Name) : query.OrderByDescending(e => e.Name);
                    break;
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
            var totalCount = await query.CountAsync();
            if (pageNumber > 0 && pageSize > 0)
            {
                entities = await query
                    .Include(cr =>cr.Computers)
                   .ThenInclude(c => c.ComputerSoftwares)
                    .Select(cr => new ComputerRoom
                    {
                        Id = cr.Id,
                        Name = cr.Name,
                        Row = cr.Row,
                        CurrentCapacity = cr.CurrentCapacity,
                        MaxCapacity = cr.MaxCapacity,
                        CurrentDowloadSoftware = cr.Computers.Where(c=> c.ComputerSoftwares.Any(cs => cs.IsDowloadFile == true && cs.SoftwareId == softwareId)).Count(),
                        CurrentInstalledSoftware = cr.Computers.Where(c => c.ComputerSoftwares.Any(cs => cs.IsInstalled == true && cs.SoftwareId == softwareId)).Count(),

                    })
                    .Skip((pageNumber - 1) * pageSize)
                    .Take(pageSize)
                    .ToListAsync();
            }
            else
            {
                entities = await query.Include(cr => cr.Computers)
                    .ThenInclude(c => c.ComputerSoftwares)
                    .Select(cr => new ComputerRoom
                    {
                        Id = cr.Id,
                        Name = cr.Name,
                        Row = cr.Row,
                        CurrentCapacity = cr.CurrentCapacity,
                        MaxCapacity = cr.MaxCapacity,
                        CurrentDowloadSoftware = cr.Computers.Where(c => c.ComputerSoftwares.Any(cs => cs.IsDowloadFile == true && cs.SoftwareId == softwareId)).Count(),
                        CurrentInstalledSoftware = cr.Computers.Where(c => c.ComputerSoftwares.Any(cs => cs.IsInstalled == true && cs.SoftwareId == softwareId)).Count(),

                    }).ToListAsync();
            }

            return (entities, totalCount);
        }
    }
}
