using ComputerManagement.BO.DTO;
using ComputerManagement.BO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Interface
{
    public interface ICommandOptionService : IBaseService<CommandOptionDto, CommandOption>
    {
        /// <summary>
        /// upsert lệnh vào db qua queue
        /// </summary>
        /// <param name="commandParam"></param>
        /// <returns></returns>
        Task UpsertAsync(CommandParam commandParam);

        /// <summary>
        /// lấy danh sách thiết lập lệnh theo id máy tính
        /// </summary>
        /// <param name="computerId"></param>
        /// <returns></returns>
        Task<List<CommandOptionDto>> GetListCommandOptionByComputerIdAsync(Guid computerId);

        /// <summary>
        /// lấy danh sách thiết lập lệnh theo id máy tính vaf key option
        /// </summary>
        /// <param name="computerId"></param>
        /// <param name="commandOptionKey"></param>
        /// <returns></returns>
        Task<List<CommandOptionDto>> GetListCommandOptionByComputerIdAndCommandOptionKeyAsync(Guid computerId, string commandOptionKey);

        
    }
}
