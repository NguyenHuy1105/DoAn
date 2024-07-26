using ComputerManagement.BO.DTO.Student;
using ComputerManagement.BO.Models;
using ComputerManagement.Common.Enums;
using ComputerManagement.Common.Exceptions;
using ComputerManagement.Service.Interface;
using ComputerManagerment.Repos.Implement;
using ComputerManagerment.Repos.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Implement
{
    public class StudentService(IServiceProvider serviceProvider, IStudentRepo studentRepo) : BaseService<StudentDto, Student>(serviceProvider, studentRepo), IStudentService
    {
        private readonly IStudentRepo _studentRepo = studentRepo;

        public async Task<StudentDto> GetByStudentCodeAsync(string studentCode)
        {
            var rs = await _studentRepo.GetQueryable().Where(s => s.StudentCode == studentCode).FirstOrDefaultAsync();
            return _mapper.Map<StudentDto>(rs);
        }

        public override async Task ValidateBeforeAddAsync(Student student)
        {
            await base.ValidateBeforeAddAsync(student);


            // check conflic mac address
            var studentByStudentCode = await _studentRepo.GetQueryable().Where(c => c.StudentCode == student.StudentCode).FirstOrDefaultAsync();
            if(studentByStudentCode != null)
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Code = ServiceResponseCode.ConflicStudentCode
                };
            }
        }


        public override async Task ValidateBeforeUpdateAsync(Student student)
        {
            // kiểm tra trùng mssv
            var softwareByName = await _studentRepo.GetQueryable().Where(s => s.StudentCode == student.StudentCode && s.Id != student.Id).FirstOrDefaultAsync();
            if (softwareByName != null)
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Code = ServiceResponseCode.ConflicStudentCode
                };
            }
        }

    }
}
