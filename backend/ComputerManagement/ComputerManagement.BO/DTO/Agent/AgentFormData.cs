using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.DTO.Agent
{
    public class AgentFormData
    {
        public Guid Id { get; set; }

        /// <summary>
        /// version
        /// </summary>
        [Required]
        public string Version { get; set; }
        /// <summary>
        /// version
        /// </summary>
        public bool IsUpdate { get; set; }

        /// <summary>
        /// file
        /// </summary>
        [Required]
        public IFormFile FilePath { get; set; }
    }
}
