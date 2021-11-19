using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using WebFramework.Mapping;

namespace WebFramework.DTOs
{
    public abstract class BaseReadDto<TDto, TEntity, TKey> : BaseDto<TDto, TEntity, TKey>
        where TDto : class, new()
        where TEntity : IEntity, new()
    {
        public virtual TKey Id { get; set; }
    }

    public abstract class BaseReadDto<TDto, TEntity> : BaseReadDto<TDto, TEntity, int>
        where TDto : BaseReadDto<TDto, TEntity>, new()
        where TEntity : IEntity, new()
    {
    }
}
