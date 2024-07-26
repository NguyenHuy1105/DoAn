using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.Models
{
    [Table("software")]
    public class SoftwareModel : BaseModel
    {
        /// <summary>
        /// Tên phần mềm
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// tên process
        /// </summary>
        public string Process { get; set; }

        /// <summary>
        /// đường dẫn folder chứa file cài
        /// </summary>
        public string InstallationFileFolder { get; set; }

        /// <summary>
        /// đường dẫn folder chứa phần mềm
        /// </summary>
        public string SoftwareFolder { get; set; }


        /// <summary>
        /// cờ update
        /// </summary>
        public bool IsUpdate { get; set; }

        /// <summary>
        /// cờ cài đặt
        /// </summary>
        public bool IsInstall { get; set; }
    }
}
