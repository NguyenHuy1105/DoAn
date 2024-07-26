using AutoMapper;
using ComputerManagement.BO.DTO.FileProof;
using ComputerManagement.BO.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Mapper
{
    public class FileProofProfile : Profile
    {
        public FileProofProfile()
        {
            CreateMap<FileProoftDto, FileProof>().ReverseMap();
        }
    }
}
