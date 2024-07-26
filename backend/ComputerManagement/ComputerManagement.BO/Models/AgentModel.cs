using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.Models
{
    [Table("agent")]
    public class AgentModel : BaseModel
    {
        /// <summary>
        /// phiên bản agent
        /// </summary>
        public string Version { get; set; }

        /// <summary>
        /// có cập nhật không
        /// </summary>
        public bool IsUpdate { get; set; }

        public string FileName { get; set; }
        public string ContentType { get; set; }
        public double Size { get; set; }
    }
}
