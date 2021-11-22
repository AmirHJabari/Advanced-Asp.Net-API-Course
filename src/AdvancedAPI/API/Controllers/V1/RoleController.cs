using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WebFramework.API;
using WebFramework.DTOs.Role;
using Common;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers.V1
{
    [ApiVersion("1")]
    public class RoleController : CrudController<RoleDto, RoleReadDto, Entities.Role>
    {
        private readonly RoleManager<Role> _roleManager;

        public RoleController(IRepository<Role> repository, IMapper mapper, RoleManager<Role> roleManager)
            : base(repository, mapper)
        {
            this._roleManager = roleManager;
        }

        public override async Task<ActionResult<ApiResult<RoleReadDto>>> Create(RoleDto dto, CancellationToken cancellationToken)
        {
            var role = mapper.Map<Role>(dto);
            
            var res = await _roleManager.CreateAsync(role);
            
            if (!res.Succeeded)
            {
                return BadRequest(new ApiResult<IEnumerable<IdentityError>>(false)
                                            .WithCode(ApiResultStatusCode.InvalidInputs)
                                            .WithData(res.Errors));
            }
            
            var resultDto = mapper.Map<RoleReadDto>(role);

            return Ok(new ApiResult<RoleReadDto>()
                            .WithData(resultDto));
        }

        public override async Task<ActionResult<ApiResult<RoleReadDto>>> Update(int id, RoleDto roleDto, CancellationToken cancellationToken)
        {
            var role = await repository.GetByIdAsync(cancellationToken, id);

            if (role is null)
                return NotFound(new ApiResult(false));

            role = mapper.Map(roleDto, role);

            var res = await _roleManager.UpdateAsync(role);

            if (!res.Succeeded)
            {
                return BadRequest(new ApiResult<IEnumerable<IdentityError>>(false)
                                            .WithCode(ApiResultStatusCode.InvalidInputs)
                                            .WithData(res.Errors));
            }

            var resultDto = mapper.Map<RoleReadDto>(role);

            return Ok(new ApiResult<RoleReadDto>()
                            .WithData(resultDto));
        }
    }
}
