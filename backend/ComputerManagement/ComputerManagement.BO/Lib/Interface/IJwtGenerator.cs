using ComputerManagement.BO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.Lib.Interface
{
    public interface IJwtGenerator
    {
        /// <summary>
        /// generate token
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        string GenerateJwt(User user);
    }
}
