using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using API.Models;
using AutoMapper;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Services;
using WebFramework.API;
using WebFramework.DTOs;

namespace API.Controllers.V2
{
    [ApiVersion("2")]
    public class UserController : V1.UserController
    {
        public UserController(IUserRepository userRepository, IJwtServices jwtServices,
            UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<Role> roleManager,
            IMapper mapper) : base(userRepository, jwtServices, userManager, signInManager, roleManager, mapper)
        {
        }

        public override Task<ActionResult<ApiResult<UserReadDto>>> Delete(int id, CancellationToken cancellationToken)
        {
            return base.Delete(id, cancellationToken);
        }

        public override Task<ActionResult<ApiResult<IEnumerable<UserReadDto>>>> Get(CancellationToken cancellationToken)
        {
            return base.Get(cancellationToken);
        }

        public override ActionResult<ApiResult<User>> Get(int id)
        {
            return base.Get(id);
        }

        [NonAction]
        public override Task<ActionResult<ApiResult>> Post([FromBody] UserCreateDto userDto, CancellationToken cancellationToken)
        {
            return base.Post(userDto, cancellationToken);
        }

        public override Task<ActionResult<ApiResult>> Put(int id, UserUpdateDto userDto)
        {
            return base.Put(id, userDto);
        }

        public override Task<ActionResult<ApiResult<string>>> Token([Required] TokenRequest tokenRequest)
        {
            return base.Token(tokenRequest);
        }
    }
}
