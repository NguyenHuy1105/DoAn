using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.DTO.ConfigOption
{
    public class ConfigOptionDto : BaseDto
    {
        /// <summary>
        /// key config
        /// </summary>
        [Required]
        public string OptionName { get; set; }

        /// <summary>
        /// value config
        /// </summary>
        [Required]
        public string OptionValue { get; set; }

        /// <summary>
        /// mô tả
        /// </summary>
        public string? Desc { get; set; }

        /// <summary>
        /// là system thì không thể sửa key (sẽ được dùng chung có web và agent)
        /// </summary>
        public bool IsSystem { get; set; }
        /// <summary>
        /// key dành cho agent
        /// </summary>
        public bool IsAgent { get; set; }
    }
}
