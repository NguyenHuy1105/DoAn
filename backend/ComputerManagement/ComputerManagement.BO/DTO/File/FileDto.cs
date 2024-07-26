using ComputerManagement.BO.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.DTO.File
{
    public class FileDto : BaseDto
    {
        public string FileName { get; set; }
        public string Version { get; set; }
        public string ContentType { get; set; }
        public double Size { get; set; }
        public Guid SoftwareId { get; set; }
        public SoftwareDto? Software { get; set; }
    }
}
