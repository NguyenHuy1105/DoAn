using ComputerManagement.BO.DTO;
using ComputerManagement.BO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Interface
{
    public interface IComputerService : IBaseService<ComputerDto, Computer>
    {
        /// <summary>
        /// lấy ra thông tin máy tính theo địa chỉ mac address
        /// </summary>
        /// <param name="macAddress"></param>
        /// <returns></returns>
        Task<ComputerDto> GetComputerByMacAddress(string macAddress);

        /// <summary>
        /// cập nhật state của máy tính theo địa chỉ mac
        /// </summary>
        /// <param name="macAddress"></param>
        /// <returns></returns>
        Task<bool> UpdateStateByMacAddressAsync(string macAddress);

        /// <summary>
        /// Lấy danh sách computer theo computerRoomId
        /// </summary>
        /// <param name="computerRoomId"></param>
        /// <returns></returns>
        Task<List<ComputerDto>> GetListComputerByComputerRoomIdAsync(Guid computerRoomId, PagingParam pagingParam);

        /// <summary>
        /// cập nhật cấu hình cho máy tính
        /// </summary>
        /// <param name="computerConfig"></param>
        /// <param name="computerId"></param>
        /// <returns></returns>
        Task<bool> UpdateComputerConfigAsync(ComputerConfig computerConfig, Guid computerId);

        /// <summary>
        /// lấy danh sách máy có hiển thị thông tin tải,cài đăt, hoạt động của phần mềm
        /// </summary>
        /// <param name="softwareId"></param>
        /// <param name="pagingParam"></param>
        /// <returns></returns
        Task<(List<ComputerDto>, int)> GetListBySoftwareIdAsync(Guid softwareId, PagingParam pagingParam);

        /// <summary>
        /// lấy số lượng máy đang bật và tổng số máy
        /// </summary>
        /// <param name="checkTime"></param>
        /// <returns></returns>
        Task<List<int>> GetComputerOnlineChart(long checkTime);

        /// <summary>
        /// thống kê số lượng máy tính theo tình trạng
        /// </summary>
        /// <returns></returns>
        Task<List<int>> GetComputerByListErrorChart();
    }
}
