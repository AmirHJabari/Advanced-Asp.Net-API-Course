﻿using API.Models;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Common;
using Common.Utilities;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using WebFramework.API;
using WebFramework.DTOs;
using WebFramework.Filters;
using WebFramework.Filters.Validations.Users;

namespace API.Controllers.V1
{
    [ApiVersion("1")]
    public class UserController : BaseApiController
    {
        private IUserRepository _userRepository;
        private readonly IJwtServices _jwtServices;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly IMapper _mapper;

        public UserController(IUserRepository userRepository, Services.IJwtServices jwtServices,
            UserManager<User> userManager, SignInManager<User> signInManager, RoleManager<Role> roleManager,
            IMapper mapper)
        {
            this._userRepository = userRepository;
            this._jwtServices = jwtServices;
            this._userManager = userManager;
            this._signInManager = signInManager;
            this._roleManager = roleManager;
            this._mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult<ApiResult<IEnumerable<UserReadDto>>>> Get(CancellationToken cancellationToken)
        {
            var users = await _userRepository.TableNoTracking.ProjectTo<UserReadDto>(_mapper.ConfigurationProvider)
                .ToListAsync(cancellationToken);

            return Ok(new ApiResult<IEnumerable<UserReadDto>>()
                                .WithData(users));
        }

        [HttpGet("{id:int}")]
        [TryGetUserByIdValidation(GetAsUserReadDto = true, ItemKey = "userDto")]
        // [Authorize(Roles = "Forbidden")] // for test
        public virtual ActionResult<ApiResult<User>> Get(int id)
        {
            UserReadDto userDto = HttpContext.Items[nameof(userDto)] as UserReadDto;

            var res = new ApiResult<UserReadDto>()
                .WithData(userDto);

            return Ok(res);
        }

        [HttpPost]
        //[UserNameIsExistValidation]
        public virtual async Task<ActionResult<ApiResult>> Post([FromBody] UserCreateDto userDto,
            CancellationToken cancellationToken)
        {
            var user = _mapper.Map<User>(userDto);

            var result = await _userManager.CreateAsync(user, userDto.Password);

            if (!result.Succeeded)
            {
                //var code = result.GetApiResultCode();
                //string message = result.GetErrorMessages();

                return BadRequest(new ApiResult<IEnumerable<IdentityError>>(false)
                                            .WithCode(ApiResultStatusCode.InvalidInputs)
                                            .WithData(result.Errors));
            }

            var res = new ApiResult<object>()
                .WithData(new { user.Id });

            return CreatedAtAction(nameof(Get), new { id = user.Id }, res);
        }

        [HttpPut("{id:int}")]
        // [UserNameIsExistValidation]
        [TryGetUserByIdValidation]
        public virtual async Task<ActionResult<ApiResult>> Put(int id,
            UserUpdateDto userDto)
        {
            User user = HttpContext.Items[nameof(user)] as User;

            _mapper.Map(userDto, user);

            var result = await _userManager.UpdateAsync(user);

            if (!result.Succeeded)
            {
                return BadRequest(
                    new ApiResult<IEnumerable<IdentityError>>(false)
                        .WithCode(ApiResultStatusCode.InvalidInputs)
                        .WithData(result.Errors)
                );
            }

            var res = new ApiResult();
            return Ok(res);
        }

        [HttpDelete("{id:int}")]
        [TryGetUserByIdValidation]
        [Authorize(Roles = "admin")]
        public virtual async Task<ActionResult<ApiResult<UserReadDto>>> Delete(int id,
            CancellationToken cancellationToken)
        {
            User user = HttpContext.Items[nameof(user)] as User;

            await _userRepository.DeleteAsync(user, cancellationToken);
            var userDto = _mapper.Map<UserReadDto>(user);
            var res = new ApiResult<UserReadDto>()
                .WithData(userDto);

            return Ok(res);
        }

        [HttpPost("[action]")]
        public virtual async Task<ActionResult<ApiResult<string>>> Token([Required] TokenRequest tokenRequest)
        {
            User user = await _userManager.FindByNameAsync(tokenRequest.UserName);

            if (user is null || !(await _userManager.CheckPasswordAsync(user, tokenRequest.Password)))
                return Unauthorized(new ApiResult(false, "UserName or password is invalid.", ApiResultStatusCode.WrongUserNameOrPassword));
            else
            {
                return Ok(new ApiResult<string>()
                    .WithData(await _jwtServices.GenerateAsync(user)));
            }
        }
    }
}
