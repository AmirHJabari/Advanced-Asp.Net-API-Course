using System.Linq;
using Common.Exceptions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using WebFramework.API;

namespace WebFramework.Middlewares
{
    public static class CustomExceptionHandlerMiddlewareExtensions
    {
        public static IApplicationBuilder UseCustomExceptionHandler(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<CustomExceptionHandlerMiddleware>();
        }
    }

    public class CustomExceptionHandlerMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IWebHostEnvironment _env;
        private readonly ILogger<CustomExceptionHandlerMiddleware> _logger;

        public CustomExceptionHandlerMiddleware(RequestDelegate next,
            IWebHostEnvironment env,
            ILogger<CustomExceptionHandlerMiddleware> logger)
        {
            _next = next;
            _env = env;
            _logger = logger;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            string message = null;
            Dictionary<string, object> data = null;
            HttpStatusCode httpStatusCode = HttpStatusCode.InternalServerError;
            ApiResultStatusCode apiStatusCode = ApiResultStatusCode.None;

            try
            {
                await _next(context);
            }
            catch (AppException ex)
            {
                _logger.LogError(ex, ex.Message);

                httpStatusCode = ex.HttpStatusCode;
                apiStatusCode = (ApiResultStatusCode)ex.ApiStatusCode;
                message = ex.Message;

                if (_env.IsDevelopment())
                {
                    var dic = ToDic(ex);
                    dic = AddInnerEx(dic, ex);

                    if (ex.AdditionalData != null)
                        dic.Add("AdditionalData", ex.AdditionalData);

                    data = dic;
                }
                await WriteToResponseAsync();
            }
            catch (SecurityTokenExpiredException exception)
            {
                _logger.LogError(exception, exception.Message);

                message = exception.Message;
                SetUnAuthorizeResponse(exception);
                await WriteToResponseAsync();
            }
            catch (UnauthorizedAccessException exception)
            {
                _logger.LogError(exception, exception.Message);

                message = exception.Message;
                SetUnAuthorizeResponse(exception);
                await WriteToResponseAsync();
            }
            //catch (InvalidOperationException) { } // System.InvalidOperationException: StatusCode cannot be set because the response has already started.
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);

                message = ex.Message;
                if (_env.IsDevelopment())
                {
                    var dic = ToDic(ex);
                    dic = AddInnerEx(dic, ex);
                    data = dic;
                }

                await WriteToResponseAsync();
            }

            async Task WriteToResponseAsync()
            {
                if (context.Response.HasStarted)
                    throw new InvalidOperationException("The response has already started, the http status code middleware will not be executed.");

                object result;

                if (data is not null)
                    result = new ApiResult<Dictionary<string, object>>(false, message, data, apiStatusCode);
                else
                    result = new ApiResult(false, message, apiStatusCode);

                context.Response.StatusCode = (int)httpStatusCode;
                await context.Response.WriteAsJsonAsync(result);
            }

            void SetUnAuthorizeResponse(Exception ex)
            {
                httpStatusCode = HttpStatusCode.Unauthorized;

                if (_env.IsDevelopment())
                {
                    var dic = ToDic(ex);
                    dic = AddInnerEx(dic, ex);

                    if (ex is SecurityTokenExpiredException tokenException)
                        dic.Add("Expires", tokenException.Expires);

                    data = dic;
                }
            }

            Dictionary<string, object> ToDic(Exception ex)
            {
                return new Dictionary<string, object>
                {
                    ["Exception"] = $"{ex.GetType()} | {ex.Message}",
                    ["StackTraceLines"] = ex.StackTrace.Split(Environment.NewLine)
                };
            }

            Dictionary<string, object> AddInnerEx(Dictionary<string, object> dic, Exception ex)
            {
                if (ex.InnerException != null)
                {
                    dic.Add("InnerException.Exception", $"{ex.InnerException.GetType()} | {ex.InnerException.Message}");
                    dic.Add("InnerException.StackTraceLines", ex.InnerException.StackTrace.Split(Environment.NewLine));
                }
                return dic;
            }
        }
    }
}
