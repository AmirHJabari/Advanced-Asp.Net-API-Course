using Data.Repositories;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using WebFramework.API;
using Microsoft.AspNetCore.Mvc;
using Entities;

namespace WebFramework.Filters.Validations.Users
{
    /// <summary>
    /// Using <see cref="IUserRepository"/> in <see cref="IServiceProvider"/> gets user by given id,
    /// if not exist returns not found to client.
    /// if exist fills the user argument in the action.
    /// </summary>
    public class TryGetUserByIdValidationAttribute : ActionFilterAttribute
    {
        public string UserIdArgumentName { get; set; } = "id";
        public string UserArgumentName { get; set; } = "user";

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cancellationToken = context.HttpContext.RequestAborted;

            var userRepository = context.HttpContext.RequestServices.GetService<IUserRepository>();
            var userId = context.ActionArguments[UserIdArgumentName];

            var user = await userRepository.GetByIdAsync(cancellationToken, (int)userId);

            if (user is null)
            {
                var notFoundRes = new ApiResult()
                    .WithMessage($"There is no user with id of '{userId}'")
                    .WithStatus(false);

                context.Result = new NotFoundObjectResult(notFoundRes);
            }

            context.ActionArguments[UserArgumentName] = user;

            await base.OnActionExecutionAsync(context, next);
        }
    }
}
