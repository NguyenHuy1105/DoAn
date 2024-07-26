using ComputerManagement.API.Controllers.Agent;
using ComputerManagement.BO.DTO;
using ComputerManagement.BO.Models;
using ComputerManagement.Service.Implement;
using ComputerManagement.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComputerManagement.Api.Controllers.Agent
{
    public class CommandOptionController(ICommandOptionService commandOptionService) : BaseController<CommandOptionDto, CommandOption>(commandOptionService)
    {
        private readonly ICommandOptionService _commandOptionService = commandOptionService;

        [HttpGet("GetListCommandOptionByComputerId/{computerId}")]
        public async Task<IActionResult> GetListCommandOptionByComputerId([FromRoute] Guid computerId)
        {
            var rs = new ServiceResponse();
            rs.Data = await _commandOptionService.GetListCommandOptionByComputerIdAsync(computerId);
            return Ok(rs);

        }

        [HttpPost("Upsert")]
        public async Task<IActionResult> UpsertCoomand([FromBody] CommandParam commandParam)
        {
            var rs = new ServiceResponse();
            await _commandOptionService.UpsertAsync(commandParam);

            return Ok(rs);
        }

        [HttpGet("GetListCommandOptionByComputerIdAndCommandOptionKey/{computerId}/{commandOpitonKey}")]
        public async Task<IActionResult> GetListCommandOptionByComputerIdAndCommandOptionKey([FromRoute] Guid computerId, [FromRoute] string commandOpitonKey)
        {
            var rs = new ServiceResponse();
            rs.Data = await _commandOptionService.GetListCommandOptionByComputerIdAndCommandOptionKeyAsync(computerId, commandOpitonKey);
            return Ok(rs);

        }
    }
}
