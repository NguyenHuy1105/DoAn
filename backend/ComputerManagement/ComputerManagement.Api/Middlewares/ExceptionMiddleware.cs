using ComputerManagement.BO;
using ComputerManagement.BO.DTO;
using ComputerManagement.Common.Enums;
using ComputerManagement.Common.Exceptions;
using System.Net;

namespace ComputerManagement.Api.Middlewares
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        public ExceptionMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {

                await HandleExceptionAsync(context, ex);
            }
        }

        private async Task HandleExceptionAsync(HttpContext context, Exception ex)
        {
            context.Response.ContentType = "application/json";
            if (ex is BaseException baseException)
            {
                // Set the response status code and content type
                context.Response.StatusCode = (int)baseException.StatusCode;
                var exceptionRes = new ServiceResponse
                {
                    Code = baseException.Code,
                    Message = baseException.Message,
                    Data = baseException.Data,
                    Success = false,
                };

                // Convert the error response to JSON
                var responseJson = CmJsonConvert.SerializeObject(exceptionRes);

                // Write the JSON response to the HTTP response
                await context.Response.WriteAsync(responseJson);
            }else
            {
                // Set the response status code and content type
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                // Write the JSON response to the HTTP response
                await context.Response.WriteAsync(CmJsonConvert.SerializeObject(new ServiceResponse
                {
                    Success = false,
                    Code = ServiceResponseCode.Exception,
                    Message = ex.Message,
                    Data = null
                }));
            }
        }
    }
}
