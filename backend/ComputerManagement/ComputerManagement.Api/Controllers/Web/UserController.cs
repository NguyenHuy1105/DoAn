using ComputerManagement.BO.DTO;
using ComputerManagement.BO.DTO.Users;
using ComputerManagement.BO.Models;
using ComputerManagement.Common.Enums;
using ComputerManagement.Service;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace ComputerManagement.Controllers.Web
{
    [Authorize]
    public class UserController : BaseController<UserDto, User>
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService) : base(userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost("login")]
        public  async Task<IActionResult> Login([FromBody] UserLogin userLogin)
        {
            var rs = await _userService.Login(userLogin);
            return Ok(rs);
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public  async Task<IActionResult> Register([FromBody] UserRegister userRegister)
        {
            var rs = await _userService.Register(userRegister);
            return Ok(rs);
        }

        [HttpPut("ChangePassword")]
        public  async Task<IActionResult> ChangePassword([FromBody] UserChangePassword userChangePassword)
        {
            var rs = await _userService.ChangePassword(userChangePassword);

            return Ok(rs);
        }

        [AllowAnonymous]
        [HttpPut("Logout")]
        public IActionResult Logout()
        {
            var rs = new ServiceResponse();
            _userService.Logout();
            return Ok(rs);
        }

        [HttpPut("check-login")]
        public virtual IActionResult CheckLogin()
        {
            var rs = new ServiceResponse();
            return Ok(rs);
        }

        /// <summary>
        /// cập với user với quyền admin
        /// </summary>
        /// <param name="userUpdateByAdmin"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPut("update-by-admin/{id}")]
        public async Task<IActionResult> UpdateByAdmin([FromBody] UserUpdateByAdmin userUpdateByAdmin, [FromRoute] Guid id)
        {
            var rs = new ServiceResponse();
            rs.Data = await _userService.UpdateUserByAdminAsync(userUpdateByAdmin, id);
            return Ok(rs);
        }

        [HttpPut("UpdateState/{userId}/{userState}")]
        public async Task<IActionResult> UpdateState([FromRoute] Guid userId, [FromRoute] UserState userState)
        {
            var rs = new ServiceResponse();
            rs.Data = await _userService.UpdateStateAsync(userId, userState);
            return Ok(rs);
        }
    }
}
