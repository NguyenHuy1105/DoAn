using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.Lib.Interface
{
    public interface IPasswordHasher
    {
        /// <summary>
        /// hash password
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        string Hash(string password);

        /// <summary>
        /// verify password
        /// </summary>
        /// <param name="passwordHash"></param>
        /// <param name="inputPassword"></param>
        /// <returns></returns>
        bool Verify(string passwordHash, string inputPassword);
    }
}
