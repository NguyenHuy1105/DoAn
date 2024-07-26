using ComputerManagement.API.Controllers.Agent;
using ComputerManagement.BO.DTO.File;
using ComputerManagement.BO.DTO;
using ComputerManagement.BO.DTO.FileProof;
using ComputerManagement.BO.Models;
using ComputerManagement.Service.Implement;
using ComputerManagement.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComputerManagement.Api.Controllers.Agent
{
    public class FileProofController(IFileProofService fileProofService) : BaseController<FileProoftDto, FileProof>(fileProofService)
    {
        private readonly IFileProofService _fileProofService = fileProofService;

        /// <summary>
        /// upload file
        /// </summary>
        /// <param name="fileSource"></param>
        /// <returns></returns>
        [HttpPost("upload-file")]
        public async Task<IActionResult> UpdateFile([FromForm] FileProofFormData formData)
        {
            // do something
            var rs = new ServiceResponse();
            rs.Data = await _fileProofService.UploadFileAsync(formData);
            return Ok(rs);
        }
    }
}
