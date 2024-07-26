using ComputerManagement.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.Models
{
    [Table("user")]
    public class User:BaseModel
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string Email { get; set; }
        public string Fullname { get; set; }
        public UserRole RoleID { get; set; }
        public string? RoleIDText { get; set; }
        public UserState State { get; set; }
        public ICollection<ScheduleBookRoom> ScheduleBookRooms { get; } = new List<ScheduleBookRoom>();
    }
}
