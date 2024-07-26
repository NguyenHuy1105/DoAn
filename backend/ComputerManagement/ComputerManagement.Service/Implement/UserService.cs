using ComputerManagement.BO.DTO;
using ComputerManagement.BO.DTO.Users;
using ComputerManagement.BO.Lib.Interface;
using ComputerManagement.BO.Models;
using ComputerManagement.Common.Configs;
using ComputerManagement.Common.Enums;
using ComputerManagement.Common.Exceptions;
using ComputerManagerment.Repos.Interface;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace ComputerManagement.Service.Implement
{
    public class UserService : BaseService<UserDto, User>, IUserService
    {
        private readonly IJwtGenerator _jwtGenerate;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IUserRepop _userRepo;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly BaseUrlConfig _baseUrlConfig;
        public UserService(IServiceProvider serviceProvider, IUserRepop userRepo, BaseUrlConfig baseUrlConfig) : base(serviceProvider, userRepo)
        {
            _userRepo = userRepo;
            _jwtGenerate = serviceProvider.GetService(typeof(IJwtGenerator)) as IJwtGenerator;
            _passwordHasher = serviceProvider.GetService(typeof(IPasswordHasher)) as IPasswordHasher;
            _httpContextAccessor = serviceProvider.GetService(typeof(IHttpContextAccessor)) as IHttpContextAccessor;
            _baseUrlConfig = baseUrlConfig;
        }

        public async Task<ServiceResponse> Login(UserLogin userLogin)
        {
            var rs = new ServiceResponse();
            var userExist = await _userRepo.GetQueryable()
                                    .Where(u => u.Username == userLogin.Username || u.Email == userLogin.Username)
                                    .FirstOrDefaultAsync() ?? throw new BaseException
                                    {
                                        StatusCode = HttpStatusCode.BadRequest,
                                        Code = ServiceResponseCode.WrongLogin
                                    };
            bool verifyPassword = _passwordHasher.Verify(userExist.Password, userLogin.Password);
            if (verifyPassword)
            {
                // check state user
                if(userExist.State == UserState.Pending)
                {
                    throw new BaseException
                    {
                        StatusCode = HttpStatusCode.Conflict,
                        Code = ServiceResponseCode.UserPending
                    };
                }
                if(userExist.State == UserState.Revoked)
                {
                    throw new BaseException
                    {
                        StatusCode = HttpStatusCode.Conflict,
                        Code = ServiceResponseCode.UserRevoked
                    };
                }
                var accessToken = _jwtGenerate.GenerateJwt(userExist);

               // var httpContext = _httpContextAccessor.HttpContext;

                // Gán Access Token vào cookie
                //httpContext.Response.Cookies.Append("AccessToken", accessToken, new CookieOptions
                //{
                //    HttpOnly = true, // Chỉ có thể truy cập từ phía máy chủ
                //    Secure = true, // Chỉ gửi cookie qua kết nối HTTPS
                //    SameSite = SameSiteMode.Strict // Giảm nguy cơ tấn công CSRF
                //                                   // Có thể thêm các thuộc tính khác của cookie ở đây
                //});
                rs.Data = new
                {
                    AccessToken = accessToken,
                    Id = userExist.Id,
                    Email = userExist.Email,
                    Fullname = userExist.Fullname,
                    Username = userExist.Username,
                    RoleID = userExist.RoleID,
                };
                return rs;
            }
            else
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    Code = ServiceResponseCode.WrongLogin
                };
            }
        }

        public async Task<ServiceResponse> Register(UserRegister userRegister)
        {
            var rs = new ServiceResponse();
            var useExistByUsername = await _userRepo.GetQueryable().Where(u => u.Username == userRegister.Username).FirstOrDefaultAsync();
            if (useExistByUsername != null)
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Code = ServiceResponseCode.UsernameConflict,
                };
            }

            var useExistByEmail = await _userRepo.GetQueryable().Where(u => u.Email == userRegister.Email).FirstOrDefaultAsync();
            if (useExistByEmail != null)
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Code = ServiceResponseCode.EmailConflict,
                };
            }

            var user = _mapper.Map<User>(userRegister);
            user.Password = _passwordHasher.Hash(user.Password);
            user.RoleID = UserRole.Teacher;
            // xử lý phân quyền và trạng thái làm sau
            //user.RoleID = (int)UserRole.Teacher;
            //user.State = UserState.Active;
            base.BeforeAddAsync(user);
            var isSuccess =  await _userRepo.AddAsync(user);
            rs.Data = isSuccess;
            if(isSuccess)
            {
                
                await this.CreateAndRunTaskAsync(async() =>
                {
                    var filePath = "D:\\da\\source\\backend\\ComputerManagement\\ComputerManagement.Common\\Templates\\CreateAccountSuccess.html";
                    var subject = "Tạo tài khoản thành công";
                    var bodySend = File.ReadAllText(filePath);
                    await _emailService.SenEmailAsync(user.Email, subject,bodySend);
                });
            }
            return rs;
        }

        public async Task<ServiceResponse> ChangePassword(UserChangePassword userChangePassword)
        {
            var rs = new ServiceResponse();
            var user = await _userRepo.GetAsync(_contextData.UserID) ?? throw new BaseException
            {
                StatusCode = HttpStatusCode.NotFound,
                Code = ServiceResponseCode.NotFoundUser
            };

            var veryfyPassword = _passwordHasher.Verify(user.Password, userChangePassword.OldPassword);
            if (!veryfyPassword)
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.Conflict,
                    Code = ServiceResponseCode.WrongLogin
                };
            }
            user.Password = _passwordHasher.Hash(userChangePassword.NewPassword);
            base.BeforeUpdateAsync(user);
            rs.Data = await _userRepo.UpdateAsync(user);

            return rs;
        }

        public Task<ServiceResponse> ResetPassword(UserResetPassword userResetPassword)
        {
            throw new NotImplementedException();
        }

        public void Logout()
        {
            //var httpContext = _httpContextAccessor.HttpContext;
            //if (httpContext != null)
            //{
            //    if (httpContext.Request.Cookies["AccessToken"] != null)
            //    {
            //        var cookieOptions = new CookieOptions
            //        {
            //            Expires = DateTime.Now.AddDays(-1)
            //        };
            //        httpContext.Response.Cookies.Append("AccessToken", "", cookieOptions);
            //    }
            //}
        }

        public async Task<bool> UpdateUserByAdminAsync(UserUpdateByAdmin userUpdateByAdmin, Guid id)
        {
            // check author
            //if(_contextData.RoleID != UserRole.Admin)
            //{
            //    throw new BaseException
            //    {
            //        StatusCode = HttpStatusCode.Forbidden,
            //        Code = ServiceResponseCode.Forbidden
            //    };
            //}

            var userAuth = await _userRepo.GetAsync(_contextData.UserID);
            if(userAuth?.RoleID != UserRole.Admin)
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    Code = ServiceResponseCode.Forbidden
                };
            }
            var userExist = await _userRepo.GetAsync(id) ?? throw new BaseException
            {
                StatusCode = HttpStatusCode.NotFound,
                Code = ServiceResponseCode.NotFoundUser
            };

            _mapper.Map(userUpdateByAdmin, userExist);

            await this.BeforeUpdateAsync(userExist);

            return await _userRepo.UpdateAsync(userExist);
        }

        public async Task<bool> UpdateStateAsync(Guid userId, UserState userState)
        {
            var userAuth = await _userRepo.GetAsync(_contextData.UserID);
            if (userAuth?.RoleID != UserRole.Admin)
            {
                throw new BaseException
                {
                    StatusCode = HttpStatusCode.Forbidden,
                    Code = ServiceResponseCode.Forbidden
                };
            }
            var userExist = await _userRepo.GetAsync(userId) ?? throw new BaseException
            {
                StatusCode = HttpStatusCode.NotFound,
                Code = ServiceResponseCode.NotFoundUser
            };
            var rs = true;
            if(userExist.State != userState)
            {
                userExist.State = userState;
                await this.BeforeUpdateAsync(userExist);

                 rs = await _userRepo.UpdateAsync(userExist);
                if (rs)
                {
                    await this.CreateAndRunTaskAsync(async () =>
                    {
                        if(userState == UserState.Active)
                        {
                            var filePath = "D:\\da\\source\\backend\\ComputerManagement\\ComputerManagement.Common\\Templates\\AccountActive.html";
                            var subject = "Tài khoản đã được kích hoạt";
                            var bodySend = File.ReadAllText(filePath);
                            // Thay thế chuỗi ##UrlLogin## bằng đường dẫn đăng nhập
                            bodySend = bodySend.Replace("##UrlLogin##", $"{_baseUrlConfig.Frontend}/login");
                            await _emailService.SenEmailAsync(userExist.Email, subject, bodySend);
                        }else if(userState == UserState.Revoked)
                        {
                            var filePath = "D:\\da\\source\\backend\\ComputerManagement\\ComputerManagement.Common\\Templates\\AccountRevoked.html";
                            var subject = "Tài khoản đã bị thu hồi";
                            var bodySend = File.ReadAllText(filePath);
                            // Thay thế chuỗi ##UrlLogin## bằng đường dẫn đăng nhập
                            bodySend = bodySend.Replace("##EmailSystem##", _emailService.GetEmailConfig().FromEmail);
                            await _emailService.SenEmailAsync(userExist.Email, subject, bodySend);
                        }
                        
                    });
                }
            }
            
            return rs;
        }
    }
}
