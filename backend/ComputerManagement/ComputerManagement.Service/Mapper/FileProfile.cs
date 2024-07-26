using AutoMapper;
using ComputerManagement.BO.DTO.File;
using ComputerManagement.BO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Mapper
{
    public class FileProfile : Profile
    {
        public FileProfile()
        {
            CreateMap<FileDto, FileModel>().ReverseMap();
        }
    }
}
