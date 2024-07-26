using ComputerManagement.BO.DTO;
using ComputerManagement.BO.Models;
using ComputerManagement.Common.Configs;
using ComputerManagement.Common.Enums;
using ComputerManagement.Common.Exceptions;
using ComputerManagement.Service.Interface;
using ComputerManagerment.Repos.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using static Microsoft.Extensions.Logging.EventSource.LoggingEventSource;

namespace ComputerManagement.Service.Implement
{
    public class SoftwareService(IServiceProvider serviceProvider, ISoftwareRepo softwareRepo, IOptionsMonitor<FileConfig> fileConfig) : BaseService<SoftwareDto, SoftwareModel>(serviceProvider, softwareRepo), ISoftwareService
    {
        private readonly ISoftwareRepo _softwareRepo = softwareRepo;
        private readonly FileConfig _fileConfig = fileConfig.CurrentValue;

        public override async Task<bool> DeleteAsync(Guid id)
        {
            var rs  = await base.DeleteAsync(id);

            // xóa file trong ổ cứng
            if(rs)
            {
                var filePaths = Directory.GetFiles(_fileConfig.StoreFile).ToList();
                foreach(string filePath in filePaths)
                {
                    string fileName = Path.GetFileName(filePath);
                    if (fileName.Contains(id.ToString()))
                    {
                        File.Delete(filePath);
                    }
                }
            }
            return rs;
        }

        public async Task<List<SoftwareDto>> GetListByListIdsAsync(List<Guid> ids)
        {
            var rs = await _softwareRepo.GetQueryable().Where(s => ids.Contains(s.Id)).ToListAsync();
            return _mapper.Map<List<SoftwareDto>>(rs);
        }

        public override async Task ValidateBeforeAddAsync(SoftwareModel software)
        {
            // kiểm tra trùng tên phần mềm
            var softwareByName = await _softwareRepo.GetQueryable().Where(s => s.Name == software.Name).FirstOrDefaultAsync();
            if (softwareByName != null)
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Code = ServiceResponseCode.ConflicSoftwareName
                };
            }
            // kiểm tra trùng tên tiến trình
            var softwareByProcess = await _softwareRepo.GetQueryable().Where(s => s.Process == software.Process).FirstOrDefaultAsync();
            if (softwareByName != null)
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Code = ServiceResponseCode.ConflicSoftwareProcess
                };
            }
        }

        public override async Task ValidateBeforeUpdateAsync(SoftwareModel software)
        {
            // kiểm tra trùng tên phần mềm
            var softwareByName = await _softwareRepo.GetQueryable().Where(s => s.Name == software.Name && s.Id != software.Id).FirstOrDefaultAsync();
            if (softwareByName != null)
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Code = ServiceResponseCode.ConflicSoftwareName
                };
            }
            // kiểm tra trùng tên tiến trình
            var softwareByProcess = await _softwareRepo.GetQueryable().Where(s => s.Process == software.Process && s.Id != software.Id).FirstOrDefaultAsync();
            if (softwareByName != null)
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Code = ServiceResponseCode.ConflicSoftwareProcess
                };
            }
        }
    }
}
