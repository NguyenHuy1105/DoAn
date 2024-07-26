using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.Models
{
    [Table("file_proof")]
    public class FileProof : BaseModel
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public double Size { get; set; }
        [ForeignKey("MonitorSession")]
        public Guid MonitorSessionId { get; set; }
        public virtual MonitorSession MonitorSession { get; set; }

        [ForeignKey("Student")]
        public Guid StudentId { get; set; }
        public virtual Student Student { get; set; }
        public Guid ComputerId { get; set; }
        public string ComputerName { get; set; }
    }
}
