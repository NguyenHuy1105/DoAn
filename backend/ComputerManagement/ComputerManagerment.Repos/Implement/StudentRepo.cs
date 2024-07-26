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
    public class StudentRepo(AppDbContext dbContext) : BaseRepo<Student>(dbContext), IStudentRepo
    {
        public override async Task<(List<Student>, int)> GetListAsync(string keySearch, int pageNumber, int pageSize, string fieldSort, bool sortAsc, Dictionary<string, string>? Filters = null)
        {
            var query = _dbSet.AsQueryable();
            var entities = new List<Student>();

            if (!string.IsNullOrEmpty(keySearch))
            {
                query = query.Where(e => e.StudentCode.Contains(keySearch) || e.StudentName.Contains(keySearch));
            }

            switch (fieldSort?.ToLower())
            {

                case "studentcode":
                    query = sortAsc ? query.OrderBy(e => e.StudentCode) : query.OrderByDescending(e => e.StudentCode);
                    break;
                case "studentname":
                    query = sortAsc ? query.OrderBy(e => e.StudentName) : query.OrderByDescending(e => e.StudentName);
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

    }
}
