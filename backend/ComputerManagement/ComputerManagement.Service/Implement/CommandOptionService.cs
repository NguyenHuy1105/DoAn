using ComputerManagement.BO.DTO;
using ComputerManagement.BO.Models;
using ComputerManagement.Common.Configs;
using ComputerManagement.Common.Constants;
using ComputerManagement.Common.Enums;
using ComputerManagement.Service.Interface;
using ComputerManagement.Service.Queue;
using ComputerManagerment.Repos.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Implement
{
    public class CommandOptionService(IServiceProvider serviceProvider, ICommandOptionRepo commandOptionRepo) : BaseService<CommandOptionDto, CommandOption>(serviceProvider, commandOptionRepo), ICommandOptionService
    {
        private readonly ICommandOptionRepo _commandOptionRepo = commandOptionRepo;
        private readonly IComputerRepo _computerRepo = serviceProvider.GetService(typeof(IComputerRepo)) as IComputerRepo;

        public async Task<List<CommandOptionDto>> GetListCommandOptionByComputerIdAndCommandOptionKeyAsync(Guid computerId, string commandOptionKey)
        {
            var rs = await _commandOptionRepo.GetQueryable().Where(cm => cm.SourceId == computerId && cm.CommandKey == CommandOptionKey.CHECK_DOWLOAD_SOFTWARE).ToListAsync();
            return _mapper.Map<List<CommandOptionDto>>(rs);
        }

        public async Task<List<CommandOptionDto>> GetListCommandOptionByComputerIdAsync(Guid computerId)
        {
            var rs = await _commandOptionRepo.GetQueryable().Where(cm => cm.SourceId == computerId).ToListAsync();
            return _mapper.Map<List<CommandOptionDto>>(rs);
        }

        public async Task UpsertAsync(CommandParam commandParam)
        {
            switch(commandParam.CommandKey)
            {
                case CommandOptionKey.CHECK_INSTALL_SOFTWARE:
                case CommandOptionKey.CHECK_DOWLOAD_SOFTWARE:
                    await UpsertDowloadInstallSoftwareAsync(commandParam);
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// đẩy dữ liệu command option lên queue để upsert cờ dowload phần mềm
        /// </summary>
        /// <param name="commandParam"></param>
        /// <returns></returns>
        private async Task UpsertDowloadInstallSoftwareAsync(CommandParam commandParam)
        {
            var listComputerId = new List<Guid>();

            // lấy danh sách id máy tính theo key mapping
            switch(commandParam.KeyMapping)
            {
                case ConstantKey.ALL:
                    listComputerId = await _computerRepo.GetQueryable().Select(c => c.Id).ToListAsync();
                    break;
                case ConstantKey.COMPUTER_ROOM:
                    listComputerId = await _computerRepo.GetQueryable().Where(c => commandParam.SourceIds.Contains(c.ComputerRoomId)).Select(c => c.Id).ToListAsync();
                    break;
                case ConstantKey.COMPUTER:
                    listComputerId.AddRange(commandParam.SourceIds);
                    break;
            }
            await this.CreateAndRunTaskAsync(async() =>
            {
                // Lấy RabbitMQConfig trực tiếp từ _serviceProvider
                var listCommand = new List<CommandOption>();
                var rbConfig = _serviceProvider.GetRequiredService<IOptions<RabbitMQConfig>>().Value;
                if(rbConfig != null)
                {
                    foreach (var item in listComputerId)
                    {
                        var commandOption = new CommandOption
                        {
                            SourceId = item,
                            DesId = commandParam.DesId,
                            CommandKey = commandParam.CommandKey,
                            CommandValue = commandParam.CommandValue
                        };
                        listCommand.Add(commandOption);
                        //var messageQueue = new MessageQueue
                        //{
                        //    Message = JsonConvert.SerializeObject(commandOption),
                        //    ActionType = QueueKey.UPSERT_COMMAND_OPTON_DOWLOAD_SOFTWARE
                        //};
                        //_ = QueueFactory.EnQueue(rbConfig, QueueKey.COMMAND_OPTION, JsonConvert.SerializeObject(messageQueue));
                    }

                    for (int i = 0; i < listCommand.Count; i += rbConfig.BatchSize)
                    {
                        var batch = listCommand.GetRange(i, Math.Min(rbConfig.BatchSize, listCommand.Count - i));
                        var messageQueue = new MessageQueue
                        {
                            Message = JsonConvert.SerializeObject(batch),
                            ActionType = QueueKey.UPSERT_COMMAND_OPTON_DOWLOAD_INSTALL_SOFTWARE
                        };
                        _ = QueueFactory.EnQueue(rbConfig, QueueKey.COMMAND_OPTION, JsonConvert.SerializeObject(messageQueue));
                    }
                }
            });
        }
    }
}
