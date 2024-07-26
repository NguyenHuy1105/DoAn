using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Common.Enums
{
    public enum ServiceResponseCode
    {
        /// <summary>
        /// thành công
        /// </summary>
        Success = 0,
        Warning = 1,

        UsernameTaken = 2,
        EmailTaken = 3,
        Error = 99,
        Exception = 999,

        /// <summary>
        /// sai tài khoản hoặc mật khẩu
        /// </summary>
        WrongLogin= 4,

        /// <summary>
        /// tên tài khoản đã tồn tại
        /// </summary>
        UsernameConflict = 6,

        /// <summary>
        /// email đã được sử dụng
        /// </summary>
        EmailConflict = 7,

        /// <summary>
        /// không tim thấy người dùng
        /// </summary>
        NotFoundUser = 8,

        /// <summary>
        /// không tìm thấy máy tính
        /// </summary>
        NotFoundComputer = 9,

        /// <summary>
        /// tên phòng máy trùng
        /// </summary>
        ConflicComputerRoomName = 10,

        /// <summary>
        /// không tim thấy phòng máy
        /// </summary>
        NotFoundComputerRoom=11,

        /// <summary>
        /// tài khoản đang chờ duyệt
        /// </summary>
        UserPending=12,

        /// <summary>
        /// tài khoản đã bị thu hồi
        /// </summary>
        UserRevoked=13,

        /// <summary>
        /// không được phân quyền truy cập
        /// </summary>
        Forbidden = 14,

        /// <summary>
        /// trùng địa chỉ mac
        /// </summary>
        ConflicMacAddress = 15,

        /// <summary>
        /// đạt giới hạn máy trên phòng
        /// </summary>
        MaxCapacityComputerRoom=16,

        /// <summary>
        /// trùng tên máy
        /// </summary>
        ConflicNameComputer = 17,

        /// <summary>
        /// lỗi khi sửa lại hàng/ dãy của phòng máy mà có máy ngoài phạm vị 
        /// </summary>
        ConflicRowColComputerRooom = 18,

        /// <summary>
        /// trùng vị trí máy
        /// </summary>
        ConflicRowColComputer = 19,

        /// <summary>
        /// không tìm thấy phần mềm
        /// </summary>
        NotFoundSoftwareModel = 20,

        /// <summary>
        /// trùng version file cài đặt
        /// </summary>
        ConflicFileVersion = 21,

        /// <summary>
        /// không tìm thấy file cài đặt
        /// </summary>
        NotFoundFile = 22,

        /// <summary>
        /// filename không hợp lệ
        /// </summary>
        InValidFileName = 23,

        /// <summary>
        /// trùng phiên giám sát
        /// </summary>
        ConflicMonitorSessionTime = 24,

        /// <summary>
        /// trùng tên phần mềm
        /// </summary>
        ConflicSoftwareName=25,

        /// <summary>
        ///  trùng tên process
        /// </summary>
        ConflicSoftwareProcess=26,

        /// <summary>
        /// ví trí máy tính vượt phạm vi phòng máy
        /// </summary>
        InValidRowColComputer=27,

        /// <summary>
        /// trùng config option name
        /// </summary>
        ConflicOptionName=28,

        /// <summary>
        /// không thể sửa option name hệ thông
        /// </summary>
        CantEditOptionNameSystem=29,

        /// <summary>
        /// file không hợp lệ
        /// </summary>
        InvalidFile=30,

        NotFoundAgent=31,

        /// <summary>
        /// trùng mssv
        /// </summary>
        ConflicStudentCode=32,

    }
}
