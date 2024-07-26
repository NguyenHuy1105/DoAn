using ComputerManagement.BO.DTO.MonitorSession;
using ComputerManagement.BO.DTO.Student;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.DTO.FileProof
{
    public class FileProofFormData
    {
        [Required]
        public Guid MonitorSessionId { get; set; }

        [Required]
        public Guid StudentId { get; set; }

        [Required]
        public Guid ComputerId { get; set; }
        [Required]
        public string ComputerName { get; set; }

        [Required]
        public IFormFile FileData { get; set; }
    }
}
