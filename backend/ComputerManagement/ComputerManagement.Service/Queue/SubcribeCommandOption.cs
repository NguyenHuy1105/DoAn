using ComputerManagement.BO.DTO;
using ComputerManagement.BO.Models;
using ComputerManagement.Common.Constants;
using ComputerManagement.Data;
using ComputerManagerment.Repos.Implement;
using ComputerManagerment.Repos.Interface;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using Newtonsoft.Json;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Queue
{
    public class SubcribeCommandOption : SubcriberBase
    {
        private readonly AppDbContext _dbContext;
        public SubcribeCommandOption(AppDbContext dbContext)
        {
            _queueName = QueueKey.COMMAND_OPTION;
            _dbContext = dbContext;
        }

        public override void DeQueue(object model, BasicDeliverEventArgs ea)
        {
            
            var newTask = new Task(async () =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var message = Encoding.UTF8.GetString(body);
                    // Deserialize JSON string to object
                    var obj = JsonConvert.DeserializeObject<MessageQueue>(message);
                    var commandOptions = JsonConvert.DeserializeObject<List<CommandOption>>(obj.Message);
                    switch (obj.ActionType)
                    {
                        case QueueKey.UPSERT_COMMAND_OPTON_DOWLOAD_INSTALL_SOFTWARE:
                            this.UpserttCommandOption(commandOptions);
                            break;
                    }
                }
                catch (Exception ex)
                {
                    // log error
                }
                finally
                {
                    _channel.BasicAck(ea.DeliveryTag, false);
                }

            });
            newTask.Start();

            //newTask.ContinueWith((completedTask) =>
            //{
            //    _channel.BasicAck(ea.DeliveryTag, false);
            //});
        }

        private async Task UpserttCommandOption(List<CommandOption> commandOptions)
        {
            var commandOptionRepo = new CommandOptionRepo(_dbContext);
            foreach (var commandOption in commandOptions)
            {
                var commandExist = await commandOptionRepo.GetQueryable().Where(cm => cm.SourceId == commandOption.SourceId && cm.DesId == commandOption.DesId && cm.CommandKey == commandOption.CommandKey).FirstOrDefaultAsync();
                if (commandExist != null)
                {
                    // update
                    commandExist.CommandValue = commandOption.CommandValue;
                    commandExist.UpdatedAt = DateTime.Now;
                    commandOption.UpdatedBy = QueueKey.UPSERT_COMMAND_OPTON_DOWLOAD_INSTALL_SOFTWARE;
                    await commandOptionRepo.UpdateAsync(commandExist);
                }
                else
                {
                    commandOption.Id = Guid.NewGuid();
                    commandOption.CreatedAt = DateTime.Now;
                    commandOption.UpdatedAt = DateTime.Now;
                    commandOption.CreatedBy = QueueKey.UPSERT_COMMAND_OPTON_DOWLOAD_INSTALL_SOFTWARE;
                    commandOption.UpdatedBy = QueueKey.UPSERT_COMMAND_OPTON_DOWLOAD_INSTALL_SOFTWARE;

                    await commandOptionRepo.AddAsync(commandOption);
                }
            }
            
        }
    }
}
