using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.DTO.Users
{
    public class UserResetPassword
    {
        public Guid UserId {  get; set; }
        public string NewPassword { get; set; }
    }
}
