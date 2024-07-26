using ComputerManagement.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.Models
{
    [Table("command_option")]
    public class CommandOption : BaseModel
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
        public string CommandKey { get; set; }

        /// <summary>
        /// thực thi hành động không
        /// </summary>
        public bool CommandValue { get; set; }
    }
}
