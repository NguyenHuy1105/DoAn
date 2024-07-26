using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Common.Enums
{
    /// <summary>
    /// trạng thái người dùng
    /// </summary>
    public enum UserState
    {
        /// <summary>
        /// chờ duyệt
        /// </summary>
        Pending = 0,

        /// <summary>
        /// hoạt động
        /// </summary>
        Active = 1,

        /// <summary>
        /// thu hồi
        /// </summary>
        Revoked = 2,
    }
}
