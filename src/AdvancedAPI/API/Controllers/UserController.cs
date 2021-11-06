using Common.Utilities;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Authorization;
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

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private IUserRepository _userRepository;
        private readonly IJwtServices _jwtServices;

        public UserController(IUserRepository userRepository, Services.IJwtServices jwtServices)
        {
            this._userRepository = userRepository;
            this._jwtServices = jwtServices;
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<ApiResult<IEnumerable<User>>> Get(CancellationToken cancellationToken)
        {
            var userName = this.User.FindFirstValue(ClaimTypes.Name);
            var role = this.User.FindAll(ClaimTypes.Role);

            var users = await _userRepository.TableNoTracking.ToListAsync(cancellationToken);

            return new ApiResult<IEnumerable<User>>()
                .WithData(users);
        }

        [HttpGet("{id:int}")]
        [TryGetUserByIdValidation]
        [Authorize(Roles = "Forbidden")] // for test
        public ActionResult<ApiResult<User>> Get(int id,
            [BindNever] User user)
        {
            var res = new ApiResult<User>()
                .WithData(user);
            
            return Ok(res);
        }

        [HttpPost]
        [UserNameIsExistValidation]
        public async Task<ActionResult<ApiResult>> Post([FromBody] UserDto userDto,
            CancellationToken cancellationToken)
        {
            var user = new User()
            {
                UserName = userDto.UserName,
                FirstName = userDto.FirstName,
                LastName = userDto.LastName,
                Gender = userDto.Gender,
                Age = userDto.Age,
                LoginDate = DateTimeOffset.UtcNow,
                LastActivityDate = DateTimeOffset.UtcNow,
                IsActive = true,
            };

            await _userRepository.AddAsync(user, userDto.Password, cancellationToken);

            var res = new ApiResult<object>()
                .WithData(new { user.Id });

            return CreatedAtAction(nameof(Get), new { id = user.Id }, res);
        }

        [HttpPut("{id:int}")]
        // [UserNameIsExistValidation]
        [TryGetUserByIdValidation]
        public async Task<ActionResult<ApiResult>> Put(int id,
            UserDto userDto, CancellationToken cancellationToken,
            [FromRoute][BindNever] User user)
        {
            user.UserName = userDto.UserName;
            user.PasswordHash = SecurityHelper.GetSha256Hash(userDto.Password);
            user.FirstName = userDto.FirstName;
            user.LastName = userDto.LastName;
            user.Age = userDto.Age;
            user.Gender = userDto.Gender;

            user.LastActivityDate = DateTimeOffset.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);

            var res = new ApiResult();

            return Ok(res);
        }

        [HttpDelete("{id:int}")]
        [TryGetUserByIdValidation]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResult>> Delete(int id, CancellationToken cancellationToken,
            [BindNever] User user)
        {
            await _userRepository.DeleteAsync(user, cancellationToken);

            var res = new ApiResult<User>()
                .WithData(user);

            return Ok(res);
        }

        [HttpGet("[action]")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResult>> Token([Required] string username, [Required] string password, CancellationToken cancellationToken)
        {
            User user = await _userRepository.GetUserByUserNameAndPasswordAsync(username, password, cancellationToken);
            
            if (user is null)
                return Unauthorized(new ApiResult(false, "UserName or password is invalid.", ApiResultStatusCode.WrongUsernameOrPassword));
            else
            {
                return Ok(new ApiResult<object>()
                    .WithData(new { Token = _jwtServices.Generate(user) }));
            }
        }
    }
}
