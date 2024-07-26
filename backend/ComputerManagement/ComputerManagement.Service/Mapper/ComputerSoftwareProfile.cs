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
    public class ComputerSoftwareProfile : Profile
    {
        public ComputerSoftwareProfile()
        {
            CreateMap<ComputerSoftwareDto, ComputerSoftware>().ReverseMap();
        }
    }
}
