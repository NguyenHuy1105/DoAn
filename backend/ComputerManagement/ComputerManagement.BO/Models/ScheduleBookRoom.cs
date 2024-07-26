using ComputerManagement.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.Models
{
    [Table("schedule_book_room")]
    public class ScheduleBookRoom : BaseModel
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ScheduleBookRoomState State { get; set; }

        [ForeignKey("ComputerRoom")]
        public Guid ComputerRoomId { get; set; }
        public ComputerRoom ComputerRoom { get; set; } = null;

        [ForeignKey("User")]
        public Guid UserId { get; set; }
        public User User { get; set; } = null;
    }
}
