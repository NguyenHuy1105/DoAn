using ComputerManagement.API.Controllers.Agent;
using ComputerManagement.BO.DTO;
using ComputerManagement.BO.DTO.MonitorSession;
using ComputerManagement.BO.Models;
using ComputerManagement.Service.Implement;
using ComputerManagement.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComputerManagement.Api.Controllers.Agent
{
    public class MonitorSessionController(IMonitorSessionService monitorSessionService) : BaseController<MonitorSessionDto, MonitorSession>(monitorSessionService)
    {
        private readonly IMonitorSessionService _monitorSessionService = monitorSessionService;

        [HttpGet("GetCurrentByComputerRoomId/{computerRoomId}")]
        public async Task<IActionResult> GetCurrentByComputerRoomId([FromRoute] Guid computerRoomId)
        {
            var rs = new ServiceResponse();
            rs.Data = await _monitorSessionService.GetCurrentByComputerRoomIdAsync(computerRoomId);
            return Ok(rs);

        }
    }
}
