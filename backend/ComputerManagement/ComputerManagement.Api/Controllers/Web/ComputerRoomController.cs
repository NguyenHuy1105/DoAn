using ComputerManagement.BO.DTO;
using ComputerManagement.BO.Models;
using ComputerManagement.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComputerManagement.Controllers.Web
{
    
    public class ComputerRoomController(IComputerRoomService computerRoomService) : BaseController<ComputerRoomDto, ComputerRoom>(computerRoomService)
    {
        private readonly IComputerRoomService _computerRoomService = computerRoomService;

        [HttpPost("GetListFilterBySoftware/{softwareId}")]
        public virtual async Task<IActionResult> GetList([FromRoute] Guid softwareId,[FromBody] PagingParam pagingParam)
        {
            var rs = new ServiceResponse();
            var (computerRoomDtos, totalCount) = await _computerRoomService.GetListBySoftwareIdAsync(softwareId,pagingParam);
            rs.Data = new
            {
                List = computerRoomDtos,
                Total = totalCount
            };
            return Ok(rs);
        }
    }
}
