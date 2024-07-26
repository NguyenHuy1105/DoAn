using ComputerManagement.BO;
using ComputerManagement.BO.Models;
using ComputerManagement.Data;
using ComputerManagerment.Repos.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagerment.Repos.Implement
{
    public class UserRepo : BaseRepo<User>, IUserRepop
    {
        public UserRepo(AppDbContext dbContext) : base(dbContext)
        {
            
        }

        public override async Task<(List<User>, int)> GetListAsync(string keySearch, int pageNumber, int pageSize, string fieldSort, bool sortAsc, Dictionary<string, string>? Filters = null)
        {
            var query = _dbSet.AsQueryable();
            var entities = new List<User>();

            if (!string.IsNullOrEmpty(keySearch))
            {
                query = query.Where(e => e.Username.Contains(keySearch) || e.Email.Contains(keySearch) || e.Fullname.Contains(keySearch));
            }

            switch(fieldSort?.ToLower())
            {
                case "Username":
                    query = sortAsc ?  query.OrderBy(e => e.Username) : query.OrderByDescending(e => e.Username);
                    break;
                case "Email":
                    query = sortAsc ? query.OrderBy(e => e.Email) : query.OrderByDescending(e => e.Email);
                    break;
                case "Fullname":
                    query = sortAsc ? query.OrderBy(e => e.Fullname) : query.OrderByDescending(e => e.Fullname);
                    break;
                case "UpdatedAt":
                    query = sortAsc ? query.OrderBy(e => e.UpdatedAt) : query.OrderByDescending(e => e.UpdatedAt);
                    break;
                case "CreatedAt":
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
