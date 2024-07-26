using ComputerManagement.BO.Lib.Implement;
using ComputerManagement.BO.Lib.Interface;
using ComputerManagement.BO.Models;
using ComputerManagement.Common.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Data.Seed
{
    public class SeederData
    {
        private readonly AppDbContext _appContextData;

        public SeederData(AppDbContext appContextData)
        {
            _appContextData = appContextData;
        }
        public void Seed()
        {
            if (!_appContextData.Users.Any())
            {
                var passwordHasher = new PasswordHasher();
                // Định nghĩa các đối tượng người dùng (User)
                var users = new List<User>
                    {
                        new User { Id = Guid.NewGuid(), Username = "admin",Email = "admin@gmail.com",Fullname="admin", Password = passwordHasher.Hash("123456"), RoleID = UserRole.Admin },
                        new User { Id = Guid.NewGuid(), Username = "teacher",Email = "teacher@gmail.com",Fullname="teacher", Password = passwordHasher.Hash("1"),RoleID = UserRole.Teacher  },
                        // Thêm các đối tượng người dùng khác vào danh sách
                    };
                _appContextData.Users.AddRange(users);
            }
            _appContextData.SaveChanges();
        }
    }
}
