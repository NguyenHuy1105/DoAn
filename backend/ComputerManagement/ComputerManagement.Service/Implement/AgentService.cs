using ComputerManagement.BO.DTO.Agent;
using ComputerManagement.BO.DTO.File;
using ComputerManagement.BO.Models;
using ComputerManagement.Common.Configs;
using ComputerManagement.Common.Enums;
using ComputerManagement.Common.Exceptions;
using ComputerManagement.Service.Interface;
using ComputerManagerment.Repos.Implement;
using ComputerManagerment.Repos.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace ComputerManagement.Service.Implement
{
    public class AgentService(IServiceProvider serviceProvider, IAgentRepo agentRepo, IOptionsMonitor<FileConfig> fileConfig) : BaseService<AgentDto, AgentModel>(serviceProvider, agentRepo), IAgentService
    {
        private readonly IAgentRepo _agentRepo = agentRepo;
        private readonly FileConfig _fileConfig = fileConfig.CurrentValue;

        public async Task<(byte[], string?)> GetFileAgentAsync()
        {
            var agent = await _agentRepo.GetQueryable().FirstOrDefaultAsync();
            if (agent == null)
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Code = ServiceResponseCode.NotFoundAgent
                };
            };
            var folderPath = Path.Combine(_env.ContentRootPath, _fileConfig.StoreFile);
            var filePath = Path.Combine(folderPath, agent.FileName);
            if (string.IsNullOrEmpty(agent.FileName) || !File.Exists(filePath))
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Code = ServiceResponseCode.NotFoundFile
                };
            }
            var bytes = await File.ReadAllBytesAsync(filePath);
            var contentType = !string.IsNullOrEmpty(agent?.ContentType) ? agent.ContentType : "application/octet-stream";
            return (bytes, contentType);
        }

        public async Task<AgentDto> GetFirstAsync()
        {
            var rs = await _agentRepo.GetQueryable().FirstOrDefaultAsync();
            return _mapper.Map<AgentDto>(rs);
        }

        public async Task<Guid> UpsertAgentAsync(AgentFormData agentFormData)
        {
            // validate
            if (agentFormData.FilePath == null || agentFormData.FilePath.Length == 0)
            {
                throw new BaseException { 
                    StatusCode = HttpStatusCode.BadRequest,
                    Code = ServiceResponseCode.InvalidFile,
                };

            }

            var agentExist = await _agentRepo.GetQueryable().FirstOrDefaultAsync();
            var isAdd = true;
            var isSuccess = false;
            if (agentExist == null)
            {
                agentExist = new AgentModel
                {
                    Version = agentFormData.Version,
                    Size = agentFormData.FilePath.Length,
                    ContentType = agentFormData.FilePath.ContentType,
                    IsUpdate = agentFormData.IsUpdate,
                };
                var newId = await this.BeforeAddAsync(agentExist);
                agentExist.FileName = newId.ToString() + Path.GetExtension(agentFormData.FilePath.FileName);
            }else
            {
                isAdd = false;
                agentExist.Version = agentFormData.Version;
                agentExist.FileName = agentExist.Id.ToString() + Path.GetExtension(agentFormData.FilePath.FileName);
                agentExist.Size = agentFormData.FilePath.Length;
                agentExist.ContentType = agentFormData.FilePath.ContentType;
                agentExist.IsUpdate = agentFormData.IsUpdate;
                await this.BeforeUpdateAsync(agentExist);
            }
            var filePath = await this.StoreFileAsync(agentFormData.FilePath, _fileConfig.StoreFile, agentExist.FileName);
            if(isAdd)
            {
                isSuccess = await _agentRepo.AddAsync(agentExist);
            }else
            {
                isSuccess = await _agentRepo.UpdateAsync(agentExist);
            }

            return agentExist.Id;
        }

        //private async Task<string> StoreFileAsync(IFormFile file, string directoryPath, string fileName)
        //{
        //    var filePath = Path.Combine(directoryPath, fileName);
        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }
        //    return filePath;
        //}

    }
}
