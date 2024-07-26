using AutoMapper;
using ComputerManagement.BO.DTO;
using ComputerManagement.BO.Models;
using ComputerManagement.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Mapper
{
    public class ComputerMapper : Profile
    {
        public ComputerMapper()
        {
            CreateMap<ComputerDto, Computer>()
                .ForMember(dest => dest.OS, opt => opt.Ignore())
                .ForMember(dest => dest.CPU, opt => opt.Ignore())
                .ForMember(dest => dest.RAM, opt => opt.Ignore())
                .ForMember(dest => dest.HardDriver, opt => opt.Ignore())
                .ForMember(dest => dest.HardDriverUsed, opt => opt.Ignore())
                .ForMember(dest => dest.ComputerRoom, opt => opt.Ignore())
                .ForMember(dest => dest.ListErrorId, opt => opt.MapFrom(src => string.Join(";", src.ListErrorId.Select(e=>e.ToString()))));
            CreateMap<Computer, ComputerDto>()
               .ForMember(dest => dest.ListErrorId, opt => opt.MapFrom(src => src.ListErrorId.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(e => (ComputerErrorId)Enum.Parse(typeof(ComputerErrorId),e)).ToList()));
        }
    }
}
