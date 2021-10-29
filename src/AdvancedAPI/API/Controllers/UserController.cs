using Common.Utilities;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
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

        public UserController(IUserRepository userRepository)
        {
            this._userRepository = userRepository;
        }

        [HttpGet]
        public async Task<ApiResult<IEnumerable<User>>> Get(CancellationToken cancellationToken)
        {
            var users = await _userRepository.TableNoTracking.ToListAsync(cancellationToken);

            return new ApiResult<IEnumerable<User>>()
                .WithData(users);
        }

        [HttpGet("{id:int}")]
        [TryGetUserByIdValidation]
        public ActionResult<ApiResult<User>> Get(int id, CancellationToken cancellationToken,
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
                LastLogin = DateTime.UtcNow,
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

            user.LastLogin = DateTime.UtcNow;

            await _userRepository.UpdateAsync(user, cancellationToken);

            var res = new ApiResult();

            return Ok(res);
        }

        [HttpDelete("{id:int}")]
        [TryGetUserByIdValidation]
        public async Task<ActionResult<ApiResult>> Delete(int id, CancellationToken cancellationToken,
            [BindNever] User user)
        {
            await _userRepository.DeleteAsync(user, cancellationToken);

            var res = new ApiResult<User>()
                .WithData(user);

            return Ok(res);
        }
    }
}
