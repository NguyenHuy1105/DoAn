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
    public class FileRepo(AppDbContext dbContext) : BaseRepo<FileModel>(dbContext), IFileRepo
    {
        public override async Task<List<FileModel>> GetListByIdsAsync(List<Guid> ids)
        {
            return await _dbSet.Where(cr => ids.Contains(cr.Id)).ToListAsync();
        }
        public override async Task<(List<FileModel>, int)> GetListAsync(string keySearch, int pageNumber, int pageSize, string fieldSort, bool sortAsc, Dictionary<string, string>? Filters = null)
        {
            var query = _dbSet.AsQueryable();
            var entities = new List<FileModel>();

            //if (!string.IsNullOrEmpty(keySearch))
            //{
            //    query = query.Where(e => e.Name.Contains(keySearch));
            //}

            switch (fieldSort?.ToLower())
            {
                case "filename":
                    query = sortAsc ? query.OrderBy(e => e.FileName) : query.OrderByDescending(e => e.FileName);
                    break;
                case "updatedat":
                    query = sortAsc ? query.OrderBy(e => e.UpdatedAt) : query.OrderByDescending(e => e.UpdatedAt);
                    break;
                case "createdat":
                    query = sortAsc ? query.OrderBy(e => e.CreatedAt) : query.OrderByDescending(e => e.CreatedAt);
                    break;
                case "softwarename":
                    query = sortAsc ? query.OrderBy(e => e.Software.Name) : query.OrderByDescending(e => e.Software.Name);
                    break;
                default:
                    query = query.OrderByDescending(e => e.UpdatedAt);
                    break;
            }
            var totalCount = await query.CountAsync();
            if (pageNumber > 0 && pageSize > 0)
            {
                entities = await query
                .Include(f => f.Software)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            }
            else
            {
                entities = await query.Include(f => f.Software).ToListAsync();
            }
            return (entities, totalCount);
        }

        public async Task<List<FileModel>> GetListFileBySoftwareId(Guid softwareId)
        {
            var query = _dbSet.AsQueryable();
            var entities = new List<FileModel>();
                entities = await _dbSet.Include(f=>f.Software).Where(f => f.SoftwareId == softwareId).OrderByDescending(e => e.UpdatedAt).ToListAsync();
            return entities;
        }
    }
}
