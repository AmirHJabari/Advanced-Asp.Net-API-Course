using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Mvc;
using WebFramework.API;
using WebFramework.DTOs.Role;

namespace API.Controllers.V1
{
    public class RoleController : CrudController<RoleDto, RoleReadDto, Entities.Role>
    {
        public RoleController(IRepository<Role> repository, IMapper mapper) : base(repository, mapper)
        {
        }
    }
}
