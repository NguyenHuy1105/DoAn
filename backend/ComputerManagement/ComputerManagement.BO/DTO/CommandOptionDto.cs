using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.DTO
{
    public class CommandOptionDto : BaseDto
    {
        /// <summary>
        /// id của đối tượng (máy tính/ phòng máy/ ...)
        /// </summary>
        public Guid SourceId { get; set; }

        /// <summary>
        /// id map đến (phần mềm)
        /// </summary>
        public Guid DesId { get; set; }

        /// <summary>
        /// cờ của hành động
        /// </summary>
        [Required]
        public string CommandKey { get; set; }

        /// <summary>
        /// thực thi hành động không
        /// </summary>
        public bool CommandValue { get; set; }
    }
}
