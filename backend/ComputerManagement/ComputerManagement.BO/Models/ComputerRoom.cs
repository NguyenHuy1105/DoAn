
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.Models
{
    [Table("computer_room")]
    public class ComputerRoom : BaseModel
    {
        /// <summary>
        /// tên phòng máy
        /// </summary>
        public string Name { get; set; }
        
        /// <summary>
        /// số hàng
        /// </summary>
        public int Row { get;set; }
        
        /// <summary>
        /// số dãy
        /// </summary>
        public int Col { get;set; }

        /// <summary>
        /// số máy tối đa
        /// </summary>
        public int MaxCapacity { get; set; }

        /// <summary>
        /// số máy hiện tại
        /// </summary>
        public int CurrentCapacity { get; set; }
        public virtual ICollection<Computer> Computers { get; set; } = new List<Computer>();

        [NotMapped]
        public int CurrentDowloadSoftware { get; set; }
        [NotMapped]
        public int CurrentInstalledSoftware { get; set; }
        [NotMapped]
        public int CurrentActiveSoftware { get; set; }

    }
}
