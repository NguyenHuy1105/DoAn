using ComputerManagement.BO.Models;
using ComputerManagement.Common.Enums;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.DTO
{
    public class ComputerDto:BaseDto
    {
        [Required]
        public string MacAddress { get; set; }
        [Required]
        public string Name { get; set; }
        /// <summary>
        /// vị trí hàng
        /// </summary>
        [Required]
        public int Row { get; set; }

        /// <summary>
        /// vị trí dãy
        /// </summary>
        [Required]
        public int Col { get; set; }

        /// <summary>
        /// trạng thái máy tính
        /// </summary>
        public ComputerState? ComputerState { get; set; } = null;
        /// <summary>
        /// tình trạng máy
        /// </summary>
        public List<ComputerErrorId> ListErrorId { get; set; } = new List<ComputerErrorId>();

        public string? OS { get; set; }
        public string? CPU { get; set; }
        public string? RAM { get; set; }
        public string? HardDriver { get; set; }
        public string? HardDriverUsed { get; set; }
        [Required]
        public Guid ComputerRoomId { get; set; }
        public ComputerRoomDto? ComputerRoom { get; set; } = null;
        public List<ComputerSoftwareDto>? ComputerSoftwares { get; set; } = null;
    }
}
