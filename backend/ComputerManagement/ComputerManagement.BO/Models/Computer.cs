using ComputerManagement.Common.Enums;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.Models
{
    [Table("computer")]
    [Index(nameof(MacAddress), IsUnique = true)]
    public class Computer : BaseModel
    {
        /// <summary>
        /// địa chỉ mac
        /// </summary>
        public string MacAddress {  get; set; }

        /// <summary>
        /// số máy
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// vị trí hàng
        /// </summary>
        public int Row { get; set; }

        /// <summary>
        /// vị trí dãy
        /// </summary>
        public int Col { get; set; }

        /// <summary>
        /// trạng thái máy tính
        /// </summary>
        public ComputerState ComputerState { get; set; }

        /// <summary>
        /// tình trạng máy
        /// </summary>
        public string ListErrorId { get; set; } = "";
        public string OS { get; set; } = "";
        public string CPU { get; set; } = "";
        public string RAM { get; set; } = "";
        public string HardDriver { get; set; } = "";
        public string HardDriverUsed { get; set; } = "";

        [ForeignKey("ComputerRoom")]
        public Guid ComputerRoomId { get; set; }
        public virtual ComputerRoom ComputerRoom { get; set; }
        public virtual ICollection<ComputerSoftware> ComputerSoftwares { get; set; } = new List<ComputerSoftware>();
    }
}
