using ComputerManagement.BO.DTO;
using ComputerManagement.Common.Enums;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Interface
{
    public interface IBaseService<TDto, TModel>
    {
        /// <summary>
        /// lấy bản ghi theo id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<TDto> GetAsync(Guid id);
        /// <summary>
        /// thêm đối tượng
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task<Guid> AddAsync(TDto dto);

        /// <summary>
        /// cập nhật bản ghi
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Guid> UpdateAsync(TDto dto, Guid id);

        /// <summary>
        /// xóa bản ghi
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<bool> DeleteAsync(Guid id);

        /// <summary>
        /// xử lý đối tượng trước khi thêm
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task<Guid> BeforeAddAsync(TModel model);

        /// <summary>
        /// sau khi thêm thành công
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task AfterAddAsync(TModel model);

        /// <summary>
        /// trước khi cập nhật
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task BeforeUpdateAsync(TModel model);

        /// <summary>
        /// sau khi cập nhật thành công
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task AfterUpdateAsync(TModel model);

        /// <summary>
        /// validate trước khi thên
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task ValidateBeforeAddAsync(TModel model);

        /// <summary>
        /// validate trước khi cập nhật
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task ValidateBeforeUpdateAsync(TModel model);

        /// <summary>
        /// validate trước khi xóa
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        Task ValidateBeforeDeleteAsync(TModel model);

        /// <summary>
        /// mapping đối tượng trước khi xóa
        /// </summary>
        /// <param name="dto"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        Task ValidateBeforeMapUpdateAsync(TDto dto, TModel model);

        /// <summary>
        /// lấy danh sách bản ghi
        /// </summary>
        /// <param name="pagingParam"></param>
        /// <returns></returns>
        Task<(List<TDto>, int)> GetListAsync(PagingParam pagingParam);

        /// <summary>
        /// validate trước khi xóa nhiều
        /// </summary>
        /// <param name="models"></param>
        /// <returns></returns>
        Task ValidateBeforeDeleteRangeAsync(List<TModel> models);

        /// <summary>
        /// xóa nhiều bản ghi
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        Task<bool> DeleteRangeAsync(List<Guid> ids);

        /// <summary>
        /// xử lý dữ liệu thêm trước khi map
        /// </summary>
        /// <param name="dto"></param>
        /// <returns></returns>
        Task HandleDataBeforeMapAddAsync(TDto dto);

        /// <summary>
        /// check permission thực hiện chức năng
        /// </summary>
        /// <param name="permissionKeys"></param>
        /// <returns></returns>
        Task CheckPermission(List<UserRole> permissionKeys);

        /// <summary>
        /// upload file
        /// </summary>
        /// <param name="file"></param>
        /// <param name="directoryPath"></param>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task<string> StoreFileAsync(IFormFile file, string directoryPath, string fileName);
    }
}
