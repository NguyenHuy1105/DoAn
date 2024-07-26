using ComputerManagement.BO.DTO;
using ComputerManagement.Common.Enums;
using ComputerManagement.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ComputerManagement.Controllers.Web
{
    [Route("api-web/[controller]")]
    [ApiController]
    [Authorize]
    public abstract partial class BaseController<TDto, TModel> : ControllerBase
    {
        protected IBaseService<TDto, TModel> _baseService;

        public BaseController(IBaseService<TDto, TModel> baseService)
        {
            _baseService = baseService;
        }

        [HttpGet("detail/{id}")]
        public virtual async Task<IActionResult> GetById([FromRoute] Guid id)
        {
            var rs = new ServiceResponse();
            rs.Data = await _baseService.GetAsync(id);
            return Ok(rs);
        }

        [HttpPost("GetList")]
        public virtual async Task<IActionResult> GetList([FromBody] PagingParam pagingParam)
        {
            var rs = new ServiceResponse();
            var (dtos, totalCount) = await _baseService.GetListAsync(pagingParam);
            rs.Data = new
            {
                List = dtos,
                Total = totalCount
            };
            return Ok(rs);

        }

        [HttpPost("")]
        public virtual async Task<IActionResult> Add([FromBody] TDto dto)
        {
            await _baseService.CheckPermission(new List<UserRole> { UserRole.Admin });
            var rs = new ServiceResponse();
            rs.Data = await _baseService.AddAsync(dto);
            return Ok(rs);
        }

        [HttpPut("update/{id}")]
        public virtual async Task<IActionResult> Update([FromBody] TDto dto, [FromRoute] Guid id)
        {
            await _baseService.CheckPermission(new List<UserRole> { UserRole.Admin });
            var rs = new ServiceResponse();
            rs.Data = await _baseService.UpdateAsync(dto,id);
            return Ok(rs);
        }

        [HttpDelete("delete/{id}")]
        public virtual async Task<IActionResult> Delete( [FromRoute] Guid id)
        {
            await _baseService.CheckPermission(new List<UserRole> { UserRole.Admin });
            var rs = new ServiceResponse();
            rs.Data = await _baseService.DeleteAsync(id);
            return Ok(rs);
        }

        [HttpPost("deletes")]
        public virtual async Task<IActionResult> DeleteRange([FromBody] List<Guid> ids)
        {
            await _baseService.CheckPermission(new List<UserRole> { UserRole.Admin });
            var rs = new ServiceResponse();
            rs.Data = await _baseService.DeleteRangeAsync(ids);
            return Ok(rs);
        }
    }
}
