using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Mvc.Filters;
using WebFramework.DTOs;

namespace WebFramework.Filters.Validations
{
    /// <summary>
    /// This is a Generic Validation Attribute which is not sopurtted in C# 9 and right now it is available
    /// in preview version of C# 10.
    /// </summary>
    /// <remarks>
    /// I will update it after that it released.
    /// </remarks>
    public class TryGetEntityByIdValidationAttribute<TEntity, TReadDto, TKey>
        //                                                              : ActionFilterAttribute  --\
        //where TEntity : class, IEntity, new()                                                  ---\ Error
        //where TReadDto : BaseReadDto<TReadDto, TEntity, TKey>, new()                           ---/
    {
        public TryGetEntityByIdValidationAttribute()
        {  }
    }
}
