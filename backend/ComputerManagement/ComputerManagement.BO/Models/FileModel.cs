using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.Models
{
    [Table("file_source")]
    public class FileModel : BaseModel
    {
        public string FileName { get; set; }
        public string Version { get; set; }
        public string ContentType { get; set; }
        public double Size { get; set; }

        [ForeignKey("SoftwareModel")]
        public Guid SoftwareId { get; set; }
        public SoftwareModel Software { get; set; }
    }
}
