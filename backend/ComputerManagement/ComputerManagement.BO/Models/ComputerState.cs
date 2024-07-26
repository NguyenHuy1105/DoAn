using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.Models
{
    [Table("computer_state")]
    public class ComputerState
    {
        [Key]
        [ForeignKey("Computer")]
        public Guid ComputerId { get; set; }
        /// <summary>
        /// trạng thái
        /// </summary>
        public bool State { get; set; }

        /// <summary>
        /// Cập nhật lần cuối
        /// </summary>
        public DateTime LastUpdate { get; set; }

    }
}
