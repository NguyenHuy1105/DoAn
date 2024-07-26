using ComputerManagement.BO.DTO;
using ComputerManagement.BO.DTO.ConfigOption;
using ComputerManagement.BO.Models;
using ComputerManagement.Controllers.Web;
using ComputerManagement.Service.Implement;
using ComputerManagement.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComputerManagement.Api.Controllers.Web
{
    public class ConfigOptionController(IConfigOptionService configOptionService) : BaseController<ConfigOptionDto, ConfigOption>(configOptionService)
    {
        private IConfigOptionService _configOptionService = configOptionService;
        [HttpGet("GetByOptionName/{optionName}")]
        public async Task<IActionResult> GetByOptionName([FromRoute] string optionName)
        {
            var rs = new ServiceResponse();
            rs.Data = await _configOptionService.GetByOptionNameAsync(optionName);
            return Ok(rs);

        }
    }
}
