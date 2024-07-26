using ComputerManagement.BO.DTO;
using ComputerManagement.BO.DTO.FileProof;
using ComputerManagement.BO.Models;
using ComputerManagement.Controllers.Web;
using ComputerManagement.Service.Implement;
using ComputerManagement.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ComputerManagement.Api.Controllers.Web
{
    public class FileProofController(IFileProofService fileProofService) : BaseController<FileProoftDto, FileProof>(fileProofService)
    {
        private readonly IFileProofService _fileProofService = fileProofService;

        /// <summary>
        /// upload file
        /// </summary>
        /// <param name="fileSource"></param>
        /// <returns></returns>
        [HttpGet("GetListByMonitorSessionId/{monitorSessionId}")]
        public async Task<IActionResult> UpdateFile([FromRoute] Guid monitorSessionId)
        {
            // do something
            var rs = new ServiceResponse();
            rs.Data = await _fileProofService.GetListByMonitorSessionIdAsync(monitorSessionId);
            return Ok(rs);
        }

        [HttpGet("get-file/{filename}")]
        public async Task<IActionResult> GetFile([FromRoute][Required] string filename)
        {
            // do something

            var (bytes, contentType) = await _fileProofService.GetFileByFileName(filename);

            return File(bytes, contentType);
        }
    }
}
