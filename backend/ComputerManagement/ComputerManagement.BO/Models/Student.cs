using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.Models
{
    [Table("student")]
    [Index(nameof(StudentCode), IsUnique = true)]
    public class Student : BaseModel
    {
        public string StudentCode { get; set; }
        public string StudentName { get; set; }
    }
}
