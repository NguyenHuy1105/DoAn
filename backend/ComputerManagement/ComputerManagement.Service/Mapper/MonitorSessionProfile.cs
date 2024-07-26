using AutoMapper;
using ComputerManagement.BO.DTO.MonitorSession;
using ComputerManagement.BO.Models;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Mapper
{
    public class MonitorSessionProfile : Profile
    {
        public MonitorSessionProfile()
        {
            CreateMap<MonitorSessionDto, MonitorSession>()
                .ForMember(dest => dest.Domains, opt => opt.MapFrom(src => JsonConvert.SerializeObject(src.Domains)));
            CreateMap<MonitorSession, MonitorSessionDto>()
                .ForMember(dest => dest.Domains, opt => opt.MapFrom(src => JsonConvert.DeserializeObject(src.Domains)));
        }
    }
}
