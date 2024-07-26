using ComputerManagement.BO.DTO;
using ComputerManagement.BO.Models;
using ComputerManagement.Controllers.Web;
using ComputerManagement.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComputerManagement.Api.Controllers.Web
{
    public class CommandOptionController(ICommandOptionService commandOptionService) : BaseController<CommandOptionDto, CommandOption>(commandOptionService)
    {
        private readonly ICommandOptionService _commandOptionService = commandOptionService;

        [HttpPost("Upsert")]
        public virtual async Task<IActionResult> UpsertCommand([FromBody] CommandParam commandParam)
        {
            var rs = new ServiceResponse();

            await _commandOptionService.UpsertAsync(commandParam);
            rs.Data = true;
            return Ok(rs);
        }
    }
}