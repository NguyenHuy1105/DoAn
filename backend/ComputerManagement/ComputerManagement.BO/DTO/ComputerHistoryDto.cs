using ComputerManagement.BO.Models;
using ComputerManagement.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.DTO
{
    public class ComputerHistoryDto : BaseDto
    {
        [Required]
        public string Message { get; set; }

        [Required]
        public DateTime LogTime { get; set; }

        [Required]
        public ComputerLevelLog Level { get; set; }

        [Required]
        public Guid ComputerId { get; set; }

        [Required]
        public Guid ComputerRoomId { get; set; }

        [Required]
        public Guid MonitorSessionId { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime UpdatedAt { get; set; }
    }
}
