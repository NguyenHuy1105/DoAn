using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.DTO.Student
{
    public class StudentDto : BaseDto
    {
        [Required]
        public string StudentCode { get; set; }

        [Required]
        public string StudentName { get; set; }
    }
}
