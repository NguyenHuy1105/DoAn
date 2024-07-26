using ComputerManagement.API.Controllers.Agent;
using ComputerManagement.BO.DTO;
using ComputerManagement.BO.DTO.Agent;
using ComputerManagement.BO.Models;
using ComputerManagement.Service.Implement;
using ComputerManagement.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ComputerManagement.Api.Controllers.Agent
{
    public class AgentController(IAgentService agentService) : BaseController<AgentDto, AgentModel>(agentService)
    {
        private readonly IAgentService _agentService = agentService;
        [HttpGet("GetFirst")]
        public async Task<IActionResult> GetFirst()
        {
            var rs = new ServiceResponse();
            rs.Data = await _agentService.GetFirstAsync();
            return Ok(rs);
        }

        [HttpGet("GetFile")]
        public async Task<IActionResult> GetFile()
        {
            // do something

            var (bytes, contentType) = await _agentService.GetFileAgentAsync();

            return File(bytes, contentType);
        }

    }
}
