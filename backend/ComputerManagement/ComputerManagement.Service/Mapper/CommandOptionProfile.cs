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
    public class CommandOptionProfile : Profile
    {
        public CommandOptionProfile()
        {
            CreateMap<CommandOptionDto, CommandOption>().ReverseMap();
        }
    }
}
