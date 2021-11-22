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
using AutoMapper.QueryableExtensions;
using AutoMapper;
using WebFramework.DTOs;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;

namespace WebFramework.Filters.Validations.Users
{
    /// <summary>
    /// Using <see cref="IUserRepository"/> in <see cref="IServiceProvider"/> gets user by given id,
    /// if not exist returns not found to client.
    /// if exist saves the user in <see cref="HttpContext.Items"/> with key of <see cref="ItemKey"/>.
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = true)]
    public class TryGetUserByIdValidationAttribute : ActionFilterAttribute
    {
        /// <summary>
        /// The name of the action argument to get user id. Default 'id'
        /// </summary>
        public string UserIdArgumentName { get; set; } = "id";

        /// <summary>
        /// The key to pass the user by <see cref="HttpContext.Items"/>. Default 'user'
        /// </summary>
        public string ItemKey { get; set; } = "user";

        /// <summary>
        /// If true saves the user using <see cref="ItemKey"/> as <see cref="UserReadDto"/>.
        /// <see cref="User"/> otherwise.
        /// </summary>
        public bool GetAsUserReadDto { get; set; } = false;

        public override async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var cancellationToken = context.HttpContext.RequestAborted;

            var userRepository = context.HttpContext.RequestServices.GetService<IUserRepository>();
            var userId = (int) context.ActionArguments[UserIdArgumentName];

            object user;

            if (GetAsUserReadDto)
            {
                var mapper = context.HttpContext.RequestServices.GetService<IMapper>();
                user = await userRepository.TableNoTracking.ProjectTo<UserReadDto>(mapper.ConfigurationProvider)
                    .FirstOrDefaultAsync(u => u.Id == userId, cancellationToken);
            }
            else
            {
                user = await userRepository.GetByIdAsync(cancellationToken, userId);
            }

            if (user is null)
            {
                var notFoundRes = new ApiResult()
                    .WithMessage($"There is no user with id of '{userId}'")
                    .WithStatus(false);

                context.Result = new NotFoundObjectResult(notFoundRes);
            }
            
            context.HttpContext.Items[ItemKey] = user;


            await base.OnActionExecutionAsync(context, next);
        }
    }
}
