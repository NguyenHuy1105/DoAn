using AutoMapper;
using ComputerManagement.BO.DTO;
using ComputerManagement.BO.Lib.Interface;
using ComputerManagement.BO.Models;
using ComputerManagement.Common.Configs;
using ComputerManagement.Common.Enums;
using ComputerManagement.Common.Exceptions;
using ComputerManagement.Service.Hubs;
using ComputerManagement.Service.Interface;
using ComputerManagerment.Repos.Interface;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Implement
{
    public abstract class BaseService<TDto, TModel> : IBaseService<TDto, TModel> where TDto : BaseDto where TModel : BaseModel 
    {

        #region Fields
        protected readonly IServiceProvider _serviceProvider;
        protected readonly IBaseRepo<TModel> _baseRepo;
        protected readonly IMapper _mapper;
        protected readonly IUnitOfWork _uow;
        protected readonly ContextData _contextData;
        protected readonly IEmailService _emailService;
        protected readonly MonitorSessionHub _monitorSessionHub;
        protected readonly IWebHostEnvironment _env;
        #endregion

        public BaseService(IServiceProvider serviceProvider, IBaseRepo<TModel> baseRepo)
        {
            _serviceProvider = serviceProvider;
            _baseRepo = baseRepo;
            _mapper = serviceProvider.GetService(typeof(IMapper)) as IMapper;
            _uow = serviceProvider.GetService(typeof(IUnitOfWork)) as IUnitOfWork;
            _contextData = serviceProvider.GetService(typeof(ContextData)) as ContextData;
            _emailService = serviceProvider.GetService(typeof(IEmailService)) as IEmailService;

            _monitorSessionHub = serviceProvider.GetService(typeof(MonitorSessionHub)) as MonitorSessionHub;
            _env = serviceProvider.GetRequiredService<IWebHostEnvironment>();

        }

        public virtual async Task<Guid> AddAsync(TDto dto)
        {
            await this.HandleDataBeforeMapAddAsync(dto);
            TModel? model = _mapper.Map<TModel>(dto);
            var newId = await BeforeAddAsync(model);
            await this.ValidateBeforeAddAsync(model);
            var rs = await _baseRepo.AddAsync(model);
            if (rs)
            {
                await AfterAddAsync(model);
                return newId;
            }
            else
            {
                throw new BaseException();
            }

        }

        public virtual async Task<Guid> BeforeAddAsync(TModel model)
        {
            var newId = Guid.NewGuid();
            model.Id = newId;
            model.CreatedBy = _contextData.Fullname;
            model.CreatedAt = DateTime.Now;
            model.UpdatedBy = _contextData.Fullname;
            model.UpdatedAt = DateTime.Now;

            return newId;

        }
        public virtual async Task BeforeUpdateAsync(TModel model)
        {
            model.UpdatedBy = _contextData.Fullname;
            model.UpdatedAt = DateTime.Now;
        }

        public virtual async Task<bool> DeleteAsync(Guid id)
        {
            var entityExist = await _baseRepo.GetAsync(id);
            this.CheckNullModel(entityExist);
            var rs = await _baseRepo.DeleteAsync(entityExist);

            if (rs)
            {
                // do after delete
            }

            return rs;
        }

        public virtual async Task<TDto> GetAsync(Guid id)
        {
            var model = await _baseRepo.GetAsync(id);
            this.CheckNullModel(model);
            return _mapper.Map<TDto>(model);
        }
        public virtual async Task<(List<TDto>, int)> GetListAsync(PagingParam pagingParam)
        {
            var (entities, totalCount) = await _baseRepo.GetListAsync(pagingParam.KeySearch, pagingParam.PageNumber, pagingParam.PageSize, pagingParam.FieldSort, pagingParam.SortAsc,pagingParam.Filters);
            var dtos = _mapper.Map<List<TDto>>(entities);
            return (dtos, totalCount);
        }

        public virtual async Task<Guid> UpdateAsync(TDto dto, Guid id)
        {
            dto.Id = id;
            var modelExist = await _baseRepo.GetAsync(id);
            this.CheckNullModel(modelExist);
            await ValidateBeforeMapUpdateAsync(dto, modelExist);
            _mapper.Map(dto, modelExist);
            await BeforeUpdateAsync(modelExist);
            await this.ValidateBeforeUpdateAsync(modelExist);
            var rs = await _baseRepo.UpdateAsync(modelExist);
            if (rs)
            {
                await AfterUpdateAsync(modelExist);
                return id;
            }
            else
            {
                throw new BaseException();
            }
        }


        protected void CheckNullModel(TModel? model)
        {
            if (model == null)
            {
                var nameCode = "NotFound" + typeof(TModel).Name;
                if (Enum.TryParse(typeof(ServiceResponseCode), nameCode, out object enumValue))
                {
                    ServiceResponseCode code = (ServiceResponseCode)enumValue;
                    throw new BaseException
                    {
                        StatusCode = HttpStatusCode.NotFound,
                        Code = code
                    };
                }
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.NotFound,
                    Code = ServiceResponseCode.Error
                };
            }
        }

        public async Task AfterAddAsync(TModel entity)
        {

        }

        public virtual async Task AfterSaveAsync(TModel entity)
        {

        }

        public virtual async Task AfterUpdateAsync(TModel entity)
        {

        }

        public virtual async Task ValidateBeforeAddAsync(TModel model)
        {

        }

        public virtual async Task ValidateBeforeDeleteAsync(TModel model)
        {

        }

        public virtual async Task ValidateBeforeUpdateAsync(TModel model)
        {
        }
        public virtual async Task ValidateBeforeMapUpdateAsync(TDto dto, TModel model)
        {
        }

        public virtual async Task ValidateBeforeDeleteRangeAsync(List<TModel> models)
        {

        }

        public async Task<bool> DeleteRangeAsync(List<Guid> ids)
        {
            var models = await _baseRepo.GetListByIdsAsync(ids);

            await this.ValidateBeforeDeleteRangeAsync(models);

            return await _baseRepo.DeleteRangeAsync(models);
        }

        public virtual async Task HandleDataBeforeMapAddAsync(TDto dto)
        {

        }

        /// <summary>
        /// tạo task chạy luồng riêng
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        protected Task CreateAndRunTaskAsync(Func<Task> function)
        {
            return Task.Run(async () =>
            {
                try
                {
                    await function().ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                finally
                {

                }
            });
        }

        public async Task CheckPermission(List<UserRole> permissionKeys)
        {
            var isPassPermission = permissionKeys.Count != 0;
            var userId = Guid.Empty;
            if (isPassPermission)
            {
                userId = _contextData?.UserID ?? Guid.Empty;
                if (userId == Guid.Empty)
                {
                    isPassPermission = false;
                }
                else
                {
                    var user = await _baseRepo.GetUserByIdAsync(userId);
                    if (user == null)
                    {
                        isPassPermission = false;
                    }
                    else
                    {
                        if (permissionKeys.Contains(user.RoleID))
                        {
                            isPassPermission = true;
                        }
                        else
                        {
                            isPassPermission = false;
                        }
                    }
                }
            }
            if (!isPassPermission)
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    Code = ServiceResponseCode.Forbidden
                };
            }
        }

        public virtual async Task<string> StoreFileAsync(IFormFile file, string directoryPath, string fileName)
        {
            var folderPath = Path.Combine(_env.ContentRootPath, directoryPath);
            // Kiểm tra và tạo thư mục nếu nó không tồn tại
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            var filePath = Path.Combine(folderPath, fileName);
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            return filePath;
        }
    }
}
