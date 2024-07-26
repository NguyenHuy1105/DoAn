using ComputerManagement.API.Controllers.Agent;
using ComputerManagement.BO.DTO;
using ComputerManagement.BO.Models;
using ComputerManagement.Service.Implement;
using ComputerManagement.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComputerManagement.Api.Controllers.Agent
{

    public class ComputerSoftwareController(IComputerSoftwareService computerSoftwareService) : BaseController<ComputerSoftwareDto, ComputerSoftware>(computerSoftwareService)
    {
        private readonly IComputerSoftwareService _computerSoftwareService = computerSoftwareService;


        [HttpPost("UpsertDowloadSoftware")]
        public async Task<IActionResult> UpsertDowloadSoftware([FromBody] ComputerSoftwareDto computerSoftwareDto)
        {
            var rs = new ServiceResponse();
            rs.Data = await _computerSoftwareService.UpsertAsync(computerSoftwareDto, "dowload");
            return Ok(rs);
        }

        [HttpPost("UpsertInstallSoftware")]
        public async Task<IActionResult> UpsertInstallSoftware([FromBody] ComputerSoftwareDto computerSoftwareDto)
        {
            var rs = new ServiceResponse();
            rs.Data = await _computerSoftwareService.UpsertAsync(computerSoftwareDto, "install");
            return Ok(rs);
        }
    }
}
