using ComputerManagement.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.DTO.Users
{
    public class UserUpdateByAdmin
    {
        public UserRole RoleID { get; set; }
        public UserState State { get; set; }
    }
}
