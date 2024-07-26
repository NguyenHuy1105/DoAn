using ComputerManagement.BO.DTO;
using ComputerManagement.BO.Models;
using ComputerManagement.Common.Enums;
using ComputerManagement.Common.Exceptions;
using ComputerManagement.Service.Interface;
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
    public class ComputerSoftwareService(IServiceProvider serviceProvider, IComputerSoftwareRepo computerSoftwareRepo) : BaseService<ComputerSoftwareDto, ComputerSoftware>(serviceProvider, computerSoftwareRepo), IComputerSoftwareService
    {
        private readonly IComputerSoftwareRepo _computerSoftwareRepo = computerSoftwareRepo;
        private readonly IComputerRepo _computerRepo = serviceProvider.GetService(typeof(IComputerRepo)) as IComputerRepo;

        public async Task<List<ComputerSoftwareDto>> GetListByComputerIdAsync(Guid computerId, PagingParam pagingParam)
        {
            var rs = await _computerSoftwareRepo.GetListByComputerIdAsync(computerId, pagingParam.KeySearch, pagingParam.FieldSort, pagingParam.SortAsc);
          
            return _mapper.Map<List<ComputerSoftwareDto>>(rs);
        }

        public async Task<bool> UpsertAsync(ComputerSoftwareDto csDto, string flag)
        {
            var rs = false;
            var computerSoftwareExist = await _computerSoftwareRepo.GetQueryable().Where(cs => cs.ComputerId == csDto.ComputerId && cs.SoftwareId == csDto.SoftwareId).FirstOrDefaultAsync();
            if(computerSoftwareExist != null)
            {
                computerSoftwareExist.IsDowloadFile = flag == "dowload" ? csDto.IsDowloadFile : computerSoftwareExist.IsDowloadFile;
                computerSoftwareExist.IsInstalled = flag == "install" ? csDto.IsInstalled : computerSoftwareExist.IsInstalled;
                // update
                await this.BeforeUpdateAsync(computerSoftwareExist);
                rs = await _computerSoftwareRepo.UpdateAsync(computerSoftwareExist);
            }else
            {
                // insert  
                var computerSoftware = new ComputerSoftware
                {
                    ComputerId = csDto.ComputerId,
                    SoftwareId = csDto.SoftwareId,
                    IsDowloadFile = flag == "dowload" ? csDto.IsDowloadFile : false,
                    IsInstalled = flag == "install" ? csDto.IsInstalled : false,
                };
                await this.BeforeAddAsync(computerSoftware);
                rs = await _computerSoftwareRepo.AddAsync(computerSoftware);
            }
            return rs;
        }
    }
}
