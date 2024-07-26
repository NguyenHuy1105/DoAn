using AutoMapper;
using ComputerManagement.BO.DTO;
using ComputerManagement.BO.Models;
using ComputerManagement.Common.Constants;
using ComputerManagement.Common.Enums;
using ComputerManagement.Common.Exceptions;
using ComputerManagement.Service.Hubs;
using ComputerManagement.Service.Interface;
using ComputerManagerment.Repos.Implement;
using ComputerManagerment.Repos.Interface;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Implement
{
    public class ComputerHistoryService(IServiceProvider serviceProvider, IComputerHistoryRepo computerHistoryRepo) : BaseService<ComputerHistoryDto, ComputerHistory>(serviceProvider, computerHistoryRepo), IComputerHistoryService
    {
        private readonly IComputerHistoryRepo _computerHistoryRepo = computerHistoryRepo;
        private readonly IComputerRepo _computerRepo = serviceProvider.GetService(typeof(IComputerRepo)) as IComputerRepo;

        public override async Task<Guid> AddAsync(ComputerHistoryDto computerHistoryDto)
        {
            computerHistoryDto.Id = Guid.NewGuid();
            computerHistoryDto.CreatedAt = DateTime.Now;
            computerHistoryDto.UpdatedAt = DateTime.Now;

            var computerHistory = _mapper.Map<ComputerHistory>(computerHistoryDto);


            var isSuccess = await _computerHistoryRepo.AddAsync(computerHistory);
            if (isSuccess)
            {
                // Tạo Task đẩy vào socket
                await this.CreateAndRunTaskAsync(async () =>
                {

                    var messageSocket = new MessageSocket
                    {
                        Message = computerHistoryDto,
                        ActionType = SocketKey.ADD_HISTORY,
                    };

                    await _monitorSessionHub.SendMessage(messageSocket);
                });
            }
            return computerHistoryDto.Id;
        }

        public async Task<List<ComputerHistoryDto>> GetAllByMonitorSessionId(Guid sessionId)
        {
            List<ComputerHistory> computerHistories = await _computerHistoryRepo.GetQueryable().Where(ch => ch.MonitorSessionId == sessionId).OrderByDescending(ch => ch.UpdatedAt).OrderByDescending(ch => ch.LogTime).ToListAsync();
            var dtos = _mapper.Map<List<ComputerHistoryDto>>(computerHistories);

            return dtos;
        }
    }
}
