using AutoMapper;
using ComputerManagement.BO.DTO.ConfigOption;
using ComputerManagement.BO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Mapper
{
    public class ConfigOptionProfile : Profile
    {
        public ConfigOptionProfile()
        {
            CreateMap<ConfigOptionDto, ConfigOption>().ReverseMap();
        }
    }
}
