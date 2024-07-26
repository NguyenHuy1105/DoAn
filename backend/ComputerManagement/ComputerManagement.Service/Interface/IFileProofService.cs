using ComputerManagement.BO.DTO;
using ComputerManagement.BO.DTO.FileProof;
using ComputerManagement.BO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Interface
{
    public interface IFileProofService : IBaseService<FileProoftDto,FileProof>
    {
        /// <summary>
        /// upload file bằng chứng
        /// </summary>
        /// <param name="formData"></param>
        /// <returns></returns>
        Task<Guid> UploadFileAsync(FileProofFormData formData);

        /// <summary>
        /// lấy danh sách thông tin file bằng chứng theo phiên
        /// </summary>
        /// <param name="monitorSessionId"></param>
        /// <returns></returns>
        Task<List<FileProoftDto>> GetListByMonitorSessionIdAsync(Guid monitorSessionId);

        /// <summary>
        /// tải file theo filename
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        Task<(byte[], string?)> GetFileByFileName(string fileName);
    }
}
