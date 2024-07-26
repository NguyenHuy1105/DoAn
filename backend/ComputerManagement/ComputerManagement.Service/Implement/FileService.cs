using ComputerManagement.BO.DTO.File;
using ComputerManagement.BO.Models;
using ComputerManagement.Common.Configs;
using ComputerManagement.Common.Enums;
using ComputerManagement.Common.Exceptions;
using ComputerManagement.Service.Interface;
using ComputerManagerment.Repos.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Implement
{
    public class FileService(IServiceProvider serviceProvider, IFileRepo fileRepo, IOptionsMonitor<FileConfig> fileConfig) : BaseService<FileDto, FileModel>(serviceProvider, fileRepo), IFileService
    {
        private readonly IFileRepo _fileRepo = fileRepo;
        private readonly ISoftwareRepo _softwareRepo = serviceProvider.GetService(typeof(ISoftwareRepo)) as ISoftwareRepo;
        private readonly FileConfig _fileConfig = fileConfig.CurrentValue;
        

        public async Task<Guid> UploadFileAsync(FileSource fileSource)
        {
            // validate
            if (fileSource.FilePath == null || fileSource.FilePath.Length == 0)
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Code = ServiceResponseCode.InvalidFile
                };
            }

            _ = await _softwareRepo.GetAsync(fileSource.SoftwareId) ?? throw new BaseException
            {
                StatusCode = HttpStatusCode.NotFound,
                Code = ServiceResponseCode.NotFoundSoftwareModel
            };

            var fileExist = await _fileRepo.GetQueryable().Where(f => f.SoftwareId == fileSource.SoftwareId && f.Version == fileSource.Version).FirstOrDefaultAsync();
            if (fileExist != null)
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Code = ServiceResponseCode.ConflicFileVersion
                };
            }
            var fileName = fileSource.SoftwareId.ToString() + "_" + fileSource.Version + Path.GetExtension(fileSource.FilePath.FileName);
            var filePath = await this.StoreFileAsync(fileSource.FilePath, _fileConfig.StoreFile, fileName);

            var fileModel = new FileModel
            {
                FileName = fileName,
                ContentType = fileSource.FilePath.ContentType,
                Size = fileSource.FilePath.Length,
                Version = fileSource.Version,
                SoftwareId = fileSource.SoftwareId,
            };

            var newGuid = await this.BeforeAddAsync(fileModel);

            await _fileRepo.AddAsync(fileModel);
            return newGuid;

        }
        //public async Task<string> StoreFileAsync(IFormFile file, string directoryPath, string fileName)
        //{
        //    var filePath = Path.Combine(directoryPath, fileName);
        //    using (var stream = new FileStream(filePath, FileMode.Create))
        //    {
        //        await file.CopyToAsync(stream);
        //    }
        //    return filePath;
        //}

        public async Task<List<FileDto>> GetListFileBySoftwareId(Guid softwareId)
        {
            var fileModels = await _fileRepo.GetListFileBySoftwareId(softwareId);
            var fileDtos = _mapper.Map<List<FileDto>>(fileModels);
            return fileDtos;
        }

        public override async Task<bool> DeleteAsync(Guid id)
        {
            // check exist 
            var fileExist = await _fileRepo.GetAsync(id);
            this.CheckNullModel(fileExist);

            // xóa file lưu trong ổ cứng
            var folderPath = Path.Combine(_env.ContentRootPath, _fileConfig.StoreFile);
            var filePath = Path.Combine(folderPath, fileExist.FileName);
            if (File.Exists(filePath))
            {
                File.Delete(filePath);
            }
            return await _fileRepo.DeleteAsync(fileExist);
        }

        public async Task<(byte[], string?)> GetFileByFileName(string fileName)
        {
            var fileSource = await _fileRepo.GetQueryable().Where(x => x.FileName == fileName).FirstOrDefaultAsync();
            var folderPath = Path.Combine(_env.ContentRootPath, _fileConfig.StoreFile);
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

        public async Task<string> CheckUpdateAsync(string fileName)
        {
            var rs = "";
            var (softwareId, version, contentType) = this.ParseFileName(fileName);
            // check cờ trong db 
            var fileNameLatestBySoftwareId = await _fileRepo.GetQueryable().Where(f => f.SoftwareId == softwareId).OrderByDescending(f => f.FileName).Select(f => f.FileName).FirstOrDefaultAsync();
            if(!string.IsNullOrEmpty(fileNameLatestBySoftwareId) && fileNameLatestBySoftwareId != fileName)
            {
                rs = fileNameLatestBySoftwareId;
            }
            return rs;
        }


        /// <summary>
        /// tách filename từ input
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private (Guid, string, string) ParseFileName(string fileName)
        {
            // Kiểm tra xem tên file có null hoặc rỗng không
            if (string.IsNullOrEmpty(fileName))
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Code = ServiceResponseCode.InValidFileName
                };
            }

            // Tìm vị trí của dấu gạch dưới cuối cùng
            int lastUnderscoreIndex = fileName.LastIndexOf('_');

            // Kiểm tra xem có dấu gạch dưới không
            if (lastUnderscoreIndex == -1)
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Code = ServiceResponseCode.InValidFileName
                };
            }

            // Tìm vị trí của dấu chấm trong tên file
            int dotIndex = fileName.LastIndexOf('.');

            // Kiểm tra xem có dấu chấm không
            if (dotIndex == -1)
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Code = ServiceResponseCode.InValidFileName
                };
            }

            // Tách GUID từ đầu đến dấu gạch dưới cuối cùng
            string guidString = fileName.Substring(0, lastUnderscoreIndex);
            Guid guid;
            if (!Guid.TryParse(guidString, out guid))
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Code = ServiceResponseCode.InValidFileName
                };
            }

            // Tách phiên bản từ dấu chấm đến dấu gạch dưới
            string version = fileName.Substring(lastUnderscoreIndex + 1, dotIndex - lastUnderscoreIndex - 1);

            // Lấy đuôi file từ vị trí của dấu chấm đến cuối tên file
            string extension = fileName.Substring(dotIndex + 1);

            return (guid, version, extension);
        }

        public async Task<string> GetFileVersionLatestBySoftwareIdAsync(Guid softwareId)
        {
            return await _fileRepo.GetQueryable().Where(f => f.SoftwareId == softwareId).OrderByDescending(f => f.FileName).Select(f => f.FileName).FirstOrDefaultAsync();
        }
    }
}
