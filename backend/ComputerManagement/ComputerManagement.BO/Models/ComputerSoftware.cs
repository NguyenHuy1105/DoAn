
using ComputerManagement.BO.DTO;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.Models
{
    [Table("computer_software")]
    public class ComputerSoftware : BaseModel
    {
        [ForeignKey("Computer")]
        public Guid ComputerId { get; set;}
        public Computer Computer { get; set;}

        [ForeignKey("SoftwareModel")]
        public Guid SoftwareId { get; set;}
        public SoftwareModel? Software { get; set; }

        /// <summary>
        /// đã tải file cài đặt chưa
        /// </summary>
        public bool IsDowloadFile { get; set; }
        /// <summary>
        /// phần mềm đã được cài đặt chưa
        /// </summary>
        public bool IsInstalled { get; set;}
    }
}
