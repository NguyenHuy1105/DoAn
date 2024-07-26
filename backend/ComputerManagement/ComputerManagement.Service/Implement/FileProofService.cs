using ComputerManagement.BO.DTO;
using ComputerManagement.BO.DTO.File;
using ComputerManagement.BO.DTO.FileProof;
using ComputerManagement.BO.Models;
using ComputerManagement.Common.Configs;
using ComputerManagement.Common.Constants;
using ComputerManagement.Common.Enums;
using ComputerManagement.Common.Exceptions;
using ComputerManagement.Service.Interface;
using ComputerManagerment.Repos.Implement;
using ComputerManagerment.Repos.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Implement
{
    public class FileProofService(IServiceProvider serviceProvider, IFileProofRepo fileProofRepo, IOptionsMonitor<FileConfig> fileConfig, BaseUrlConfig baseUrlConfig) : BaseService<FileProoftDto, FileProof>(serviceProvider, fileProofRepo), IFileProofService
    {
        private readonly IFileProofRepo _fileProofRepo = fileProofRepo;
        private readonly FileConfig _fileConfig = fileConfig.CurrentValue;
        private readonly BaseUrlConfig _baseUrlConfig = baseUrlConfig;

        public async Task<List<FileProoftDto>> GetListByMonitorSessionIdAsync(Guid monitorSessionId)
        {
            var entities = await _fileProofRepo.GetListByMonitorSessionIdAsync(monitorSessionId);
            var dtos =  _mapper.Map<List<FileProoftDto>>(entities);
            return dtos;
        }

        public async Task<Guid> UploadFileAsync(FileProofFormData formData)
        {
            // validate
            if (formData.FileData == null || formData.FileData.Length == 0)
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Code = ServiceResponseCode.InvalidFile
                };
            }

            var fileName = formData.MonitorSessionId.ToString() + "_" + formData.StudentId + "_" + DateTime.Now.ToString("yyyyMMdd_HHmmss") +  Path.GetExtension(formData.FileData.FileName);
            var fileProof = new FileProof
            {
                FileName = fileName,
                ContentType = formData.FileData.ContentType,
                Size = formData.FileData.Length,
                MonitorSessionId = formData.MonitorSessionId,
                StudentId = formData.StudentId,
                ComputerId = formData.ComputerId,
                ComputerName = formData.ComputerName
            };

            var newGuid = await this.BeforeAddAsync(fileProof);
            var rs = true;
            try
            {
                rs =  await _fileProofRepo.AddAsync(fileProof);

                if(rs)
                {
                    var filePath = await this.StoreFileAsync(formData.FileData, _fileConfig.StoreFileProof, fileName);
                    var enities = await _fileProofRepo.GetQueryable().Include(f => f.Student).Where(f => f.Id == newGuid).FirstOrDefaultAsync();
                    var dto = _mapper.Map<FileProoftDto>(fileProof);
                    // Tạo Task đẩy vào socket
                    await this.CreateAndRunTaskAsync(async () =>
                    {

                        var messageSocket = new MessageSocket
                        {
                            Message = dto,
                            ActionType = SocketKey.ADD_FILE_PROOF,
                        };

                        await _monitorSessionHub.SendMessage(messageSocket);
                    });
                }
            }catch (Exception ex) {
                rs = false;
                // logger
            }
            
            return rs ? newGuid : Guid.Empty;
        }

        public async Task<(byte[], string?)> GetFileByFileName(string fileName)
        {
            var fileSource = await _fileProofRepo.GetQueryable().Where(x => x.FileName == fileName).FirstOrDefaultAsync();
            var folderPath = Path.Combine(_env.ContentRootPath, _fileConfig.StoreFileProof);
            var filePath = Path.Combine(folderPath, fileName);
            if (string.IsNullOrEmpty(fileName) || !File.Exists(filePath))
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Code = ServiceResponseCode.NotFoundFile
                };
            }
            var bytes = await File.ReadAllBytesAsync(filePath);
            var contentType = !string.IsNullOrEmpty(fileSource?.ContentType) ? fileSource.ContentType : "application/octet-stream";
            return (bytes, contentType);
        }

    }
}
