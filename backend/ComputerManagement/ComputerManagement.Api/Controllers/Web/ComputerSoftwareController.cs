using ComputerManagement.BO.DTO;
using ComputerManagement.BO.Models;
using ComputerManagement.Controllers.Web;
using ComputerManagement.Service.Implement;
using ComputerManagement.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComputerManagement.Api.Controllers.Web
{
    public class ComputerSoftwareController(IComputerSoftwareService computerSoftwareService) : BaseController<ComputerSoftwareDto, ComputerSoftware>(computerSoftwareService)
    {
        private readonly IComputerSoftwareService _computerSoftwareService = computerSoftwareService;

        [HttpPost("GetListByComputerId/{computerId}")]
        public virtual async Task<IActionResult> GetListByComputerId([FromRoute] Guid computerId, [FromBody] PagingParam pagingParam)
        {
            var rs = new ServiceResponse();
            rs.Data = await _computerSoftwareService.GetListByComputerIdAsync(computerId, pagingParam);

            return Ok(rs);
        }
    }
}
