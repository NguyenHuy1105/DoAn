using ComputerManagement.BO.DTO;
using ComputerManagement.BO.DTO.File;
using ComputerManagement.BO.Models;
using ComputerManagement.Controllers.Web;
using ComputerManagement.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ComputerManagement.Api.Controllers.Web
{
    public class FileController(IFileService fileService) : BaseController<FileDto, FileModel>(fileService)
    {
        private readonly IFileService _fileService = fileService;

        [HttpPost("")]
        public override async Task<IActionResult> Add([FromBody] FileDto fileDto)
        {
            throw new NotImplementedException();
        }

        [HttpPut("update/{id}")]
        public override async Task<IActionResult> Update([FromBody] FileDto dto, [FromRoute] Guid id)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// upload file
        /// </summary>
        /// <param name="fileSource"></param>
        /// <returns></returns>
        [HttpPost("upload-file")]
        public async Task<IActionResult> UpdateFile([FromForm] FileSource fileSource)
        {
            // do something
            var rs = new ServiceResponse();
            rs.Data = await _fileService.UploadFileAsync(fileSource);
            return Ok(rs);
        }

        /// <summary>
        /// upload file
        /// </summary>
        /// <param name="fileSource"></param>
        /// <returns></returns>
        [HttpGet("GetListFileBySoftwareId/{softwareId}")]
        public async Task<IActionResult> UpdateFile([FromRoute] Guid softwareId)
        {
            // do something
            var rs = new ServiceResponse();
            rs.Data = await _fileService.GetListFileBySoftwareId(softwareId);
            return Ok(rs);
        }

        [HttpGet("get-file/{filename}")]
        public async Task<IActionResult> GetFile([FromRoute] [Required] string filename)
        {
            // do something
            
            var (bytes, contentType) = await _fileService.GetFileByFileName(filename);

            //return new FileContentResult(bytes, contentType)
            //{
            //    FileDownloadName = filename
            //};
            return File(bytes, contentType);
        }
    }
}
