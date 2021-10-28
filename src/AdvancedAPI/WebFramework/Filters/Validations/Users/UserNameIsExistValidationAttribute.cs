using System.Reflection;
using Data;
using Data.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WebFramework.API;
using WebFramework.DTOs;

namespace WebFramework.Filters.Validations.Users
{
    /// <summary>
    /// Using <see cref="IUserRepository"/> in <see cref="IServiceProvider"/> checks is there any user with given UserName,
    /// if exist returns bad request to client, else lets action to execute.
    /// </summary>
    public class UserNameIsExistValidationAttribute : ActionFilterAttribute
    {
        public string UserNameArgumentName { get; set; } = "userDto.UserName";

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string[] userNameExtension = UserNameArgumentName.Split('.');

            object objUserName = null;

            foreach (var arg in userNameExtension)
            {
                if (objUserName is null)
                    objUserName = context.ActionArguments[arg];

                else
                    objUserName = objUserName.GetType().GetProperty(arg).GetValue(objUserName);
            }

            string userName = objUserName as string;

            var userRepository = context.HttpContext.RequestServices.GetService<IUserRepository>();
            var cancellationToken = context.HttpContext.RequestAborted;

            if (userRepository is not null)
            {
                bool isExist = await userRepository.IsUserNameExistAsync(userName, cancellationToken);
                if (isExist)
                {
                    var badRes = new ApiResult()
                        .WithMessage($"UserName '{userName}' already exist")
                        .WithStatus(false)
                        .WithCode(ApiResultStatusCode.UserNameExist);
                    
                    context.Result = new BadRequestObjectResult(badRes);
                }
            }
            else
            {
                Console.WriteLine("'IUserRepository' is not registered for this HttpContext.");
            }

            await base.OnActionExecutionAsync(context, next);
        }
    }
}
