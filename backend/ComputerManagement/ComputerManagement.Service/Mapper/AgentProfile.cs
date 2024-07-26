using AutoMapper;
using ComputerManagement.BO.DTO.Agent;
using ComputerManagement.BO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Mapper
{
    public class AgentProfile : Profile
    {
        public AgentProfile()
        {
            CreateMap<AgentDto, AgentModel>()
                    .ForMember(dest => dest.FileName, opt => opt.Ignore())
                    .ForMember(dest => dest.ContentType, opt => opt.Ignore())
                    .ForMember(dest => dest.Size, opt => opt.Ignore());
            CreateMap<AgentModel, AgentDto>();
        }
    }
}
