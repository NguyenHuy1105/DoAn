using ComputerManagement.BO.DTO;
using ComputerManagement.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace ComputerManagement.API.Controllers.Agent
{
    [Route("api-agent/[controller]")]
    [ApiController]
    public abstract partial class BaseController<TDto, TModel>(IBaseService<TDto, TModel> baseService) : ControllerBase
    {
        protected IBaseService<TDto, TModel> _baseService = baseService;
    }
}
