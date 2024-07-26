using ComputerManagement.BO.DTO;
using ComputerManagement.BO.Models;
using ComputerManagement.Controllers.Web;
using ComputerManagement.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComputerManagement.Api.Controllers.Web
{

    public partial class ComputerHistoryController(IComputerHistoryService computerHistoryService): BaseController<ComputerHistoryDto,ComputerHistory>(computerHistoryService)
    {
        private readonly IComputerHistoryService _computerHistoryService = computerHistoryService;

        [HttpPost("GetAllByMonitorSessionId/{monitorSessionId}")]
        public virtual async Task<IActionResult> GetListByComputerId([FromRoute] Guid monitorSessionId)
        {
            var rs = new ServiceResponse();
            var dtos = await _computerHistoryService.GetAllByMonitorSessionId(monitorSessionId);
            rs.Data = dtos;
            return Ok(rs);

        }
    }
}
