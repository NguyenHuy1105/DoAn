using ComputerManagement.BO.DTO;
using ComputerManagement.BO.Models;
using ComputerManagement.Common.Enums;
using ComputerManagement.Common.Exceptions;
using ComputerManagement.Service.Interface;
using ComputerManagerment.Repos.Implement;
using ComputerManagerment.Repos.Interface;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Implement
{
    public class ComputerRoomService : BaseService<ComputerRoomDto, ComputerRoom>, IComputerRoomService
    {
        private readonly IComputerRoomRepo _computerRoomRepo;
        private readonly IComputerRepo _computerRepo;
        public ComputerRoomService(IServiceProvider serviceProvider, IComputerRoomRepo computerRoomRepo) : base(serviceProvider, computerRoomRepo)
        {
            _computerRoomRepo = computerRoomRepo;
        }

        public async Task<(List<ComputerRoomDto>, int)> GetListBySoftwareIdAsync(Guid softwareId, PagingParam pagingParam)
        {
            var (entities, totalCount) = await _computerRoomRepo.GetListBySoftwareIdAsync(softwareId, pagingParam.KeySearch, pagingParam.PageNumber, pagingParam.PageSize, pagingParam.FieldSort, pagingParam.SortAsc);
            var dtos = _mapper.Map<List<ComputerRoomDto>>(entities);
            return (dtos, totalCount);
        }

        public override async Task ValidateBeforeAddAsync(ComputerRoom computerRoom)
        {
            await base.ValidateBeforeAddAsync(computerRoom);
            var computerRoomByName = await _computerRoomRepo.GetQueryable().Where(c => c.Name == computerRoom.Name).FirstOrDefaultAsync();
            if(computerRoomByName != null)
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Code = ServiceResponseCode.ConflicComputerRoomName
                };
            }
        }

        public override async Task ValidateBeforeMapUpdateAsync(ComputerRoomDto dto, ComputerRoom model)
        {
            if(dto.Row < model.Row || dto.Col < model.Col)
            {
                // check xem có bị miss máy không
                var computer = await _computerRepo.GetQueryable().Where(c => c.ComputerRoomId == model.Id && (c.Row > dto.Row || c.Col > dto.Col)).FirstOrDefaultAsync();
                if(computer != null)
                {
                    throw new BaseException
                    {
                        StatusCode = HttpStatusCode.Conflict,
                        Code = ServiceResponseCode.ConflicRowColComputerRooom
                    };
                }
               
            }
        }

        public override async Task ValidateBeforeUpdateAsync(ComputerRoom computerRoom)
        {
            // kiểm tra trùng tên phòng
            var computerRoomByName = await _computerRoomRepo.GetQueryable().Where(s => s.Name == computerRoom.Name && s.Id != computerRoom.Id).FirstOrDefaultAsync();
            if (computerRoomByName != null)
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Code = ServiceResponseCode.ConflicComputerRoomName
                };
            }
        }
    }
}
