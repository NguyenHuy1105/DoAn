using ComputerManagement.BO.Models;
using ComputerManagement.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.DTO.MonitorSession
{
    public class MonitorSessionDto : BaseDto
    {
        public Guid ComputerRoomId { get; set; }
        public ComputerRoomDto? ComputerRoom { get; set; }
        public MonitorType MonitorType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        /// <summary>
        /// danh sách domain cho phép
        /// </summary>
        public List<string> Domains { get; set; } = new List<string>();

        public Guid? OwnerId { get; set; }
        public UserDto? User { get; set; }
    }
}
