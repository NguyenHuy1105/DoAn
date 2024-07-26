using ComputerManagement.BO.DTO;
using ComputerManagement.BO.Lib.Interface;
using ComputerManagement.BO.Models;
using ComputerManagement.Common.Constants;
using ComputerManagement.Common.Enums;
using ComputerManagement.Common.Exceptions;
using ComputerManagement.Service.Interface;
using ComputerManagerment.Repos.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace ComputerManagement.Service.Implement
{
    public class ComputerService(IServiceProvider serviceProvider, IComputerRepo computerRepo) : BaseService<ComputerDto, Computer>(serviceProvider, computerRepo), IComputerService
    {

        private readonly IComputerRepo _computerRepo = computerRepo;
        private readonly IComputerRoomRepo _computerRoomRepo = serviceProvider.GetService(typeof(IComputerRoomRepo)) as IComputerRoomRepo;
        private readonly IComputerStateRepo _computerStateRepo = serviceProvider.GetService(typeof(IComputerStateRepo)) as IComputerStateRepo;
        private readonly IConfigOptionRepo _configOptionRepo = serviceProvider.GetService(typeof(IConfigOptionRepo)) as IConfigOptionRepo;

        public async Task<ComputerDto> GetComputerByMacAddress(string macAddress)
        {
            var computer = await _computerRepo.GetQueryable().Where(c => c.MacAddress == macAddress).Include(c => c.ComputerRoom).FirstOrDefaultAsync() ?? throw new BaseException
            {
                StatusCode = HttpStatusCode.NotFound,
                Code = ServiceResponseCode.NotFoundComputer,
            };

            return _mapper.Map<ComputerDto>(computer);
        }

        public async Task<bool> UpdateStateByMacAddressAsync(string macAddress)
        {
            var isSuccess = false;
            var computerExist = await _computerRepo.GetQueryable().Where(c => c.MacAddress == macAddress).FirstOrDefaultAsync();
            base.CheckNullModel(computerExist);
            var computerState = await _computerStateRepo.GetQueryable().Where(cs => cs.ComputerId == computerExist.Id).FirstOrDefaultAsync();
            if (computerState != null)
            {
                computerState.State = true;
                computerState.LastUpdate = DateTime.Now;
                isSuccess =  await _computerStateRepo.UpdateAsync(computerState);
            }
            else
            {
                computerState = new ComputerState
                {
                    ComputerId = computerExist.Id,
                    State = true,
                    LastUpdate = DateTime.Now,
                };
                isSuccess = await _computerStateRepo.AddAsync(computerState);
            }
            // có thể bắn emit lên client để cập nhật lại state computer
            if(isSuccess)
            {
                // Tạo Task đẩy vào socket
                await this.CreateAndRunTaskAsync(async () =>
                {

                    var messageSocket = new MessageSocket
                    {
                        Message = computerState,
                        ActionType = SocketKey.UPDATE_STATE_COMPUTER,
                    };

                    await _monitorSessionHub.SendMessage(messageSocket);
                });
            }
            return isSuccess;
        }

        public override async Task<Guid> AddAsync(ComputerDto computerDto)
        {
            var computer = _mapper.Map<Computer>(computerDto);
            var newId = await this.BeforeAddAsync(computer);
            var isSaveSuccess = false;
            // validate before add
            await this.ValidateBeforeAddAsync(computer);

            var computerRoom = await _computerRoomRepo.GetAsync(computer.ComputerRoomId)
                                            ?? throw new BaseException
                                            {
                                                StatusCode = HttpStatusCode.NotFound,
                                                Code = ServiceResponseCode.NotFoundComputerRoom
                                            };
            if (computer.Col <= 0 || computer.Col > computerRoom.Col || computer.Row <= 0 || computer.Row > computerRoom.Row)
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Code = ServiceResponseCode.InValidRowColComputer
                };
            }
            if (computerRoom.CurrentCapacity + 1 > computerRoom.MaxCapacity)
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Code = ServiceResponseCode.MaxCapacityComputerRoom
                };
            }

            computerRoom.CurrentCapacity += 1;


            try
            {
                await _uow.BeginTransactionAsync();
                isSaveSuccess = await _computerRepo.AddAsync(computer);
                await _computerRoomRepo.UpdateAsync(computerRoom);
                await _uow.CommitAsync();
                if (isSaveSuccess)
                {
                    await base.AfterSaveAsync(computer);
                }
            }
            catch
            {
                await _uow.RollbackAsync();
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Code = ServiceResponseCode.Error
                };
            }


            return newId;

        }

        public override async Task ValidateBeforeAddAsync(Computer computer)
        {
            await base.ValidateBeforeAddAsync(computer);
            // check conflic mac address
            var computerByMac = await _computerRepo.GetQueryable().Where(c => c.MacAddress == computer.MacAddress).FirstOrDefaultAsync();
            if (computerByMac != null)
            {
                throw new BaseException { StatusCode = HttpStatusCode.Conflict, Code = ServiceResponseCode.ConflicMacAddress };
            }
            var computerByNameAndComputerRoomID = await _computerRepo.GetQueryable().Where(c => c.Name == computer.Name && c.ComputerRoomId == computer.ComputerRoomId).FirstOrDefaultAsync();
            if (computerByNameAndComputerRoomID != null)
            {
                throw new BaseException { StatusCode = HttpStatusCode.Conflict, Code = ServiceResponseCode.ConflicNameComputer };
            }
            // check postion computer
            var computerByRowCol = await _computerRepo.GetQueryable().Where(c => c.Row == computer.Row && c.Col == computer.Col && c.ComputerRoomId == computer .ComputerRoomId).FirstOrDefaultAsync();
            if (computerByRowCol != null)
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Code = ServiceResponseCode.ConflicRowColComputer
                };
            }

        }

        public override async Task<(List<ComputerDto>, int)> GetListAsync(PagingParam pagingParam)
        {
            var (dtos, totalCount) = await base.GetListAsync(pagingParam);
            return (dtos, totalCount);
        }
        public async Task<List<ComputerDto>> GetListComputerByComputerRoomIdAsync(Guid computerRoomId, PagingParam pagingParam)
        {
            var computers = await _computerRepo.GetListComputerByComputerRoomIdAsync(computerRoomId, pagingParam.KeySearch, pagingParam.FieldSort, pagingParam.SortAsc);
            return _mapper.Map<List<ComputerDto>>(computers);
        }

        public override async Task<Guid> BeforeAddAsync(Computer computer)
        {
            var newGuid = await base.BeforeAddAsync(computer);
            return newGuid;
        }

        public async Task<bool> UpdateComputerConfigAsync(ComputerConfig computerConfig, Guid computerId)
        {
            var computerExist = await _computerRepo.GetAsync(computerId) ?? throw new BaseException
            {
                StatusCode = HttpStatusCode.NotFound,
                Code = ServiceResponseCode.NotFoundComputer,
            };

            computerExist.OS = computerConfig.OS;
            computerExist.CPU = computerConfig.CPU;
            computerExist.RAM = computerConfig.RAM;
            computerExist.HardDriver = computerConfig.HardDriver;
            computerExist.HardDriverUsed = computerConfig.HardDriverUsed;

            var rs = await _computerRepo.UpdateAsync(computerExist);
            return rs;
        }

        public async Task<(List<ComputerDto>, int)> GetListBySoftwareIdAsync(Guid softwareId, PagingParam pagingParam)
        {
            var (entities, totalCount) = await _computerRepo.GetListBySoftwareIdAsync(softwareId, pagingParam.KeySearch, pagingParam.PageNumber, pagingParam.PageSize, pagingParam.FieldSort, pagingParam.SortAsc);
            var dtos = _mapper.Map<List<ComputerDto>>(entities);
            return (dtos, totalCount);
        }

        public override async Task<bool> DeleteAsync(Guid id)
        {
            var entityExist = await _computerRepo.GetAsync(id);
            this.CheckNullModel(entityExist);
            var computerRoomId = entityExist.ComputerRoomId;
            var rs = await _computerRepo.DeleteAsync(entityExist);

            if (rs)
            {
                // do after delete
                await this.CreateAndRunTaskAsync(async () =>
                {
                    // cập nhật lại currentCapacity cho computer room
                    var computerRoom = await _computerRoomRepo.GetQueryable().Where(cr => cr.Id == computerRoomId).FirstOrDefaultAsync();
                    if(computerRoom != null)
                    {
                        computerRoom.CurrentCapacity  = computerRoom.CurrentCapacity > 0 ? computerRoom.CurrentCapacity - 1 : 0;
                    }
                    _ = await _computerRoomRepo.UpdateAsync(computerRoom);
                });

            }

            return rs;
        }

        public override async Task ValidateBeforeUpdateAsync(Computer computer)
        {
            var computerByMac = await _computerRepo.GetQueryable().Where(c => c.MacAddress == computer.MacAddress && c.Id != computer.Id).FirstOrDefaultAsync();
            if (computerByMac != null)
            {
                throw new BaseException { StatusCode = HttpStatusCode.Conflict, Code = ServiceResponseCode.ConflicMacAddress };
            }
            var computerByNameAndComputerRoomID = await _computerRepo.GetQueryable().Where(c => c.Name == computer.Name && c.ComputerRoomId == computer.ComputerRoomId && c.Id != computer.Id).FirstOrDefaultAsync();
            if (computerByNameAndComputerRoomID != null)
            {
                throw new BaseException { StatusCode = HttpStatusCode.Conflict, Code = ServiceResponseCode.ConflicNameComputer };
            }
            // check postion computer
            var computerByRowCol = await _computerRepo.GetQueryable().Where(c => c.Row == computer.Row && c.Col == computer.Col && c.ComputerRoomId == computer.ComputerRoomId && c.Id != computer.Id).FirstOrDefaultAsync();
            if (computerByRowCol != null)
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Code = ServiceResponseCode.ConflicRowColComputer
                };
            }
        }

        public async Task<List<int>> GetComputerOnlineChart(long checkTime)
        {
            return await _computerRepo.GetComputerOnlineChart(checkTime);
        }

        public async Task<List<int>> GetComputerByListErrorChart()
        {
            return await _computerRepo.GetComputerByListErrorChart();
        }
    }
}
