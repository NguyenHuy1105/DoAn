using ComputerManagement.API.Controllers.Agent;
using ComputerManagement.BO.DTO;
using ComputerManagement.BO.Models;
using ComputerManagement.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComputerManagement.Api.Controllers.Agent
{

    [Route("api-agent/[controller]")]
    [ApiController]
    public partial class ComputerHistoryController(IComputerHistoryService computerHistoryService): BaseController<ComputerHistoryDto, ComputerHistory>(computerHistoryService)
    {
        private readonly IComputerHistoryService _computerHistoryService = computerHistoryService;

        [HttpPost("")]
        public async Task<IActionResult> Add([FromBody] ComputerHistoryDto computerHistoryDto)
        {
            var rs = new ServiceResponse();
            rs.Data = await _computerHistoryService.AddAsync(computerHistoryDto);
            return Ok(rs);
        }
    }
}
