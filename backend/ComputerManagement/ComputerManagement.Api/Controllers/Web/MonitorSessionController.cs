using ComputerManagement.BO.DTO;
using ComputerManagement.BO.DTO.MonitorSession;
using ComputerManagement.BO.Models;
using ComputerManagement.Common.Enums;
using ComputerManagement.Controllers.Web;
using ComputerManagement.Service.Implement;
using ComputerManagement.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComputerManagement.Api.Controllers.Web
{
    public class MonitorSessionController(IMonitorSessionService monitorSessionService) : BaseController<MonitorSessionDto, MonitorSession>(monitorSessionService)
    {
        private readonly IMonitorSessionService _monitorSessionService = monitorSessionService;

        [HttpPost("GetListByComputerRoomId/{computerRoomId}")]
        public virtual async Task<IActionResult> GetListByComputerRoomId([FromRoute] Guid
    computerRoomId, [FromBody] PagingParam pagingParam)
        {
            var rs = new ServiceResponse();
            var (dtos, totalCount) = await _monitorSessionService.GetListComputerByComputerRoomIdAsync(computerRoomId, pagingParam);
            rs.Data = new
            {
                List = dtos,
                Total = totalCount
            };
            return Ok(rs);

        }
        public override async Task<IActionResult> Add([FromBody] MonitorSessionDto dto)
        {
            var rs = new ServiceResponse();
            rs.Data = await _monitorSessionService.AddAsync(dto);
            return Ok(rs);
        }

    }
}
