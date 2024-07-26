using AutoMapper;
using ComputerManagement.BO.DTO;
using ComputerManagement.BO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Mapper
{
    public class ComputerRoomMapper : Profile
    {
        public ComputerRoomMapper()
        {
            CreateMap<ComputerRoomDto, ComputerRoom>()
                .ForMember(dest => dest.Id, opt => opt.Ignore());
            CreateMap<ComputerRoom, ComputerRoomDto>();
        }
    }
}
