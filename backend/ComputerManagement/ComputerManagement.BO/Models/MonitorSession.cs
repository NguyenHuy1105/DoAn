using ComputerManagement.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.Models
{
    [Table("monitor_session")]
    public class MonitorSession : BaseModel
    {
        [ForeignKey("ComputerRoom")]
        public Guid ComputerRoomId { get; set; }
        public ComputerRoom ComputerRoom { get; set; }
        public MonitorType MonitorType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        /// <summary>
        /// danh sách domain cho phép
        /// </summary>
        public string Domains { get; set; } = "";

        [ForeignKey("User")]
        public Guid OwnerId { get; set; }
        public User User { get; set; }

    }
}
