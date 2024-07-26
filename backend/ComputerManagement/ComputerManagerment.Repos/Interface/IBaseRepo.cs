using ComputerManagement.BO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagerment.Repos.Interface
{
    public interface IBaseRepo<T> 
    {
        /// <summary>
        /// linq query
        /// </summary>
        /// <returns></returns>
        IQueryable<T> GetQueryable();

        /// <summary>
        /// lấy danh sách paging, filter, sort
        /// </summary>
        /// <param name="keySearch"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="fieldSort"></param>
        /// <param name="sortAsc"></param>
        /// <returns></returns>
        Task<(List<T>, int)> GetListAsync(string keySearch, int pageNumber, int pageSize, string fieldSort, bool sortAsc, Dictionary<string, string>? filters = null);

        /// <summary>
        /// lấy bản ghi theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<T?> GetAsync(Guid id);

        /// <summary>
        /// thêm mới bản ghi
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<bool> AddAsync(T item);

        /// <summary>
        /// cập nhật bản ghi
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<bool> UpdateAsync(T item);

        /// <summary>
        /// xóa 1 bản ghi
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(T entity);

        /// <summary>
        /// lưu xuống db
        /// </summary>
        /// <returns></returns>
        Task<int> SaveChangeAsync();

        /// <summary>
        /// lấy danh sách bản ghi theo id
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<List<T>> GetListByIdsAsync(List<Guid> ids);

        /// <summary>
        /// xóa nhiều
        /// </summary>
        /// <param name="entities"></param>
        /// <returns></returns>
        Task<bool> DeleteRangeAsync(List<T> entities);

        /// <summary>
        /// lấy thông tin user theo id
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        Task<User> GetUserByIdAsync(Guid userId);
    }
}
