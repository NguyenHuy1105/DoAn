using ComputerManagement.BO.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.DTO
{
    public class ComputerSoftwareDto : BaseDto
    {
        [Required]
        public Guid ComputerId { get; set; }
        public ComputerDto? Computer { get; set; }
        [Required]
        public Guid SoftwareId { get; set; }
        public SoftwareDto? Software { get; set; }


        /// <summary>
        /// đã tải file cài đặt chưa
        /// </summary>
        public bool IsDowloadFile { get; set; }
        /// <summary>
        /// phần mềm đã được cài đặt chưa
        /// </summary>
        public bool IsInstalled { get; set; }
    }
}
