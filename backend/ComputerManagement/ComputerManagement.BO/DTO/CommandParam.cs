using ComputerManagement.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.BO.DTO
{
    public class CommandParam
    {
        /// <summary>
        /// danh sách id source
        /// </summary>
        public List<Guid> SourceIds { get; set; }= new List<Guid>();

        /// <summary>
        /// id des mapping
        /// </summary>
        public Guid DesId { get; set; }

        /// <summary>
        /// command key
        /// </summary>
        public string CommandKey { get; set; } = "";

        /// <summary>
        /// giá trị command
        /// </summary>
        public bool CommandValue { get; set; }

        /// <summary>
        /// flag để biết mapping từ đâu (computer/computerRoom/all/...)
        /// </summary>
        public string KeyMapping { get; set; }= "";
    }
}
