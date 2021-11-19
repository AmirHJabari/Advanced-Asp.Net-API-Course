using Data.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebFramework.API;
using Entities;
using WebFramework.DTOs.Category;
using AutoMapper;

namespace API.Controllers.V1
{
    public class CategoryController : CrudController<CategoryDto, CategoryReadDto, Category>
    {
        public CategoryController(IRepository<Category> repository, IMapper mapper) 
            : base(repository, mapper)
        {  }
    }
}
