using ComputerManagement.BO.Models;
using ComputerManagement.Common.Enums;
using ComputerManagement.Data;
using ComputerManagerment.Repos.Interface;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagerment.Repos.Implement
{
    public class MonitorSessionRepo(AppDbContext dbContext) : BaseRepo<MonitorSession>(dbContext), IMonitorSessionRepo
    {
        public override async Task<List<MonitorSession>> GetListByIdsAsync(List<Guid> ids)
        {
            return await _dbSet.Where(cr => ids.Contains(cr.Id)).ToListAsync();
        }
        public override async Task<MonitorSession> GetAsync(Guid id)
        {
            return await _dbSet.Include(c => c.ComputerRoom).Include(c => c.User).FirstOrDefaultAsync(c => c.Id == id);
        }
        public override async Task<(List<MonitorSession>, int)> GetListAsync(string keySearch, int pageNumber, int pageSize, string fieldSort, bool sortAsc, Dictionary<string, string>? filters = null)
        {
            var query = _dbSet.AsQueryable();
            var entities = new List<MonitorSession>();
            query = query.Include(c => c.ComputerRoom).Include(c => c.User);
            if (!string.IsNullOrEmpty(keySearch))
            {
                query = query.Where(e => e.ComputerRoom != null && e.ComputerRoom.Name != null && e.ComputerRoom.Name.Contains(keySearch));
            }
            if (filters != null)
            {
                foreach (var keyValuePair in filters)
                {
                    string key = keyValuePair.Key;
                    string value = keyValuePair.Value;
                    if (key == "monitorType" && !string.IsNullOrEmpty(value) && value != "null")
                    {
                        try
                        {
                            List<MonitorType> listMonitorType = JsonConvert.DeserializeObject<List<MonitorType>>(value);
                            if (listMonitorType != null)
                            {
                                query = query.Where(ms => listMonitorType.Contains(ms.MonitorType));
                            }

                        }
                        catch (Exception ex)
                        {
                            // logger
                        };
                    }
                    if (key == "state" && !string.IsNullOrEmpty(value) && value != "null")
                    {
                        try
                        {
                            List<int> states = JsonConvert.DeserializeObject<List<int>>(value);
                            if (states != null && states.Count > 0)
                            {
                                var state = states[0];
                                var now = DateTime.Now;
                                switch (state)
                                {
                                    case 1:
                                        query = query.Where(ms => ms.StartDate < now && now < ms.EndDate);
                                        break;
                                    case 2:
                                        query = query.Where(ms => now < ms.StartDate);
                                        break;
                                    case 3:
                                        query = query.Where(ms => now > ms.EndDate);
                                        break;
                                }

                            }

                        }
                        catch (Exception ex)
                        {
                            // logger
                        };
                    }
                }
            }
            switch (fieldSort?.ToLower())
            {
                case "computerroomname":
                    query = sortAsc ? query.OrderBy(e => e.ComputerRoom.Name) : query.OrderByDescending(e => e.ComputerRoom.Name);
                    break;
                case "startdate":
                    query = sortAsc ? query.OrderBy(e => e.StartDate) : query.OrderByDescending(e => e.StartDate);
                    break;
                case "enddate":
                    query = sortAsc ? query.OrderBy(e => e.StartDate) : query.OrderByDescending(e => e.StartDate);
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

        public async Task<(List<MonitorSession>, int)> GetListComputerByComputerRoomIdAsync(Guid computerRoomId, string keySearch, int pageNumber, int pageSize, string fieldSort, bool sortAsc, Dictionary<string, string>? filters = null)
        {
            var query = _dbSet.AsQueryable().Where(m => m.ComputerRoomId == computerRoomId);
            var entities = new List<MonitorSession>();

            //if (!string.IsNullOrEmpty(keySearch))
            //{
            //    query = query.Where(e => e.Name.Contains(keySearch));
            //}
            if (filters != null)
            {
                foreach (var keyValuePair in filters)
                {
                    string key = keyValuePair.Key;
                    string value = keyValuePair.Value;
                    if (key == "monitorType" && !string.IsNullOrEmpty(value) && value != "null")
                    {
                        try
                        {
                            List<MonitorType> listMonitorType = JsonConvert.DeserializeObject<List<MonitorType>>(value);
                            if (listMonitorType != null)
                            {
                                query = query.Where(ms => listMonitorType.Contains(ms.MonitorType));
                            }

                        }
                        catch (Exception ex)
                        {
                            // logger
                        };
                    }
                    if (key == "state" && !string.IsNullOrEmpty(value) && value != "null")
                    {
                        try
                        {
                            List<int> states = JsonConvert.DeserializeObject<List<int>>(value);
                            if (states != null && states.Count > 0)
                            {
                                var state = states[0];
                                var now = DateTime.Now;
                                switch (state)
                                {
                                    case 1:
                                        query = query.Where(ms => ms.StartDate < now && now < ms.EndDate);
                                        break;
                                    case 2:
                                        query = query.Where(ms => now < ms.StartDate);
                                        break;
                                    case 3:
                                        query = query.Where(ms => now > ms.EndDate);
                                        break;
                                }

                            }

                        }
                        catch (Exception ex)
                        {
                            // logger
                        };
                    }
                }
            }
            switch (fieldSort?.ToLower())
            {

                case "startdate":
                    query = sortAsc ? query.OrderBy(e => e.StartDate) : query.OrderByDescending(e => e.StartDate);
                    break;
                case "enddate":
                    query = sortAsc ? query.OrderBy(e => e.StartDate) : query.OrderByDescending(e => e.StartDate);
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
                .Include(c => c.ComputerRoom)
                .Include(c => c.User)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            }
            else
            {
                entities = await query.Include(c => c.ComputerRoom).Include(c => c.User).ToListAsync();
            }


            return (entities, totalCount);
        }
    }
}
