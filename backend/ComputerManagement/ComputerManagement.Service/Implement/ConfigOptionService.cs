using ComputerManagement.BO.DTO.ConfigOption;
using ComputerManagement.BO.Models;
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
    public class ConfigOptionService(IServiceProvider serviceProvider, IConfigOptionRepo configOptionRepo) : BaseService<ConfigOptionDto, ConfigOption>(serviceProvider, configOptionRepo), IConfigOptionService
    {
        private readonly IConfigOptionRepo _configOptionRepo = configOptionRepo;

        public async Task<ConfigOptionDto> GetByOptionNameAsync(string optionName)
        {
            var configOption = await _configOptionRepo.GetQueryable().Where(co => co.OptionName == optionName).FirstOrDefaultAsync();
            return _mapper.Map<ConfigOptionDto>(configOption);
        }

        public async Task<List<ConfigOptionDto>> GetOptionForAgentAsync()
        {
            var rs = await _configOptionRepo.GetQueryable().Where(co => co.IsAgent).ToListAsync();
            return _mapper.Map<List<ConfigOptionDto>>(rs);
        }

        public override async Task ValidateBeforeAddAsync(ConfigOption configOption)
        {
            await base.ValidateBeforeAddAsync(configOption);
            // check trùng key
            var coExistByOptionName = await _configOptionRepo.GetQueryable().Where(co => co.OptionName == configOption.OptionName).FirstOrDefaultAsync();
            if(coExistByOptionName != null)
            {
                throw new BaseException
                {
                    StatusCode = System.Net.HttpStatusCode.Conflict,
                    Code = Common.Enums.ServiceResponseCode.ConflicOptionName
                };
            }
        }

        public override async Task ValidateBeforeMapUpdateAsync(ConfigOptionDto dto, ConfigOption model)
        {
            // check la isSystem khi không được sửa optonName
            if(model.IsSystem && dto.OptionName != model.OptionName)
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Code = Common.Enums.ServiceResponseCode.CantEditOptionNameSystem
                };
            }
            // check conflic option name 
            var modelByName = await _configOptionRepo.GetQueryable().Where(co => co.OptionName == dto.OptionName && co.Id != dto.Id).FirstOrDefaultAsync();
            if(modelByName != null)
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Code = Common.Enums.ServiceResponseCode.CantEditOptionNameSystem
                };
            }
        }
    }
}
