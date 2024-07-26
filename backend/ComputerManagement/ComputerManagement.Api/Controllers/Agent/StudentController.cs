using ComputerManagement.API.Controllers.Agent;
using ComputerManagement.BO.DTO;
using ComputerManagement.BO.DTO.Student;
using ComputerManagement.BO.Models;
using ComputerManagement.Service.Implement;
using ComputerManagement.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComputerManagement.Api.Controllers.Agent
{
    public class StudentController(IStudentService studentService ) : BaseController<StudentDto, Student>(studentService)
    {
        private readonly IStudentService _studentService = studentService;

        [HttpGet("GetByStudentCode/{studentCode}")]
        public async Task<IActionResult> GetByStudentCode([FromRoute] string studentCode)
        {
            var rs = new ServiceResponse();
            rs.Data = await _studentService.GetByStudentCodeAsync(studentCode);
            return Ok(rs);

        }
    }
}
