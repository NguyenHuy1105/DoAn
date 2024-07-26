using ComputerManagement.BO.DTO.MonitorSession;
using ComputerManagement.BO.DTO.Student;
using ComputerManagement.BO.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.DTO.FileProof
{
    public class FileProoftDto : BaseDto
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string FilePath { get; set; }
        public double Size { get; set; }
        public Guid MonitorSessionId { get; set; }
        public Guid StudentId { get; set; }
        public StudentDto? Student { get; set; }
        public Guid ComputerId { get; set; }
        public string ComputerName { get; set; }

    }
}
