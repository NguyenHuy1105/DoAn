using ComputerManagement.BO.DTO.Student;
using ComputerManagement.BO.Models;
using ComputerManagement.Controllers.Web;
using ComputerManagement.Service.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComputerManagement.Api.Controllers.Web
{
    public class StudentController(IStudentService studentService) : BaseController<StudentDto,Student>(studentService)
    {
    }
}
