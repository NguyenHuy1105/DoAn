using ComputerManagement.BO.DTO.Student;
using ComputerManagement.BO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Interface
{
    public interface IStudentService : IBaseService<StudentDto, Student>
    {
        /// <summary>
        /// lấy ra sinh viên theo mssv
        /// </summary>
        /// <param name="studentCode"></param>
        /// <returns></returns>
        Task<StudentDto> GetByStudentCodeAsync(string studentCode);
    }
}
