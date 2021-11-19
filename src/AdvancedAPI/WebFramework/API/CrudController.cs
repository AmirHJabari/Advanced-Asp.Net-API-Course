using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebFramework.DTOs;

namespace WebFramework.API
{
    /// <summary>
    /// A generic CRUD controller.
    /// </summary>
    /// <remarks>
    /// Don't use it for entities with Foreign Keys or override and handle actions if it is required.
    /// </remarks>
    /// <typeparam name="TCreateDto">The DTO for creating entity.</typeparam>
    /// <typeparam name="TReadDto">The DTO for reading entity.</typeparam>
    /// <typeparam name="TUpdateDto">The DTO for updating entity.</typeparam>
    /// <typeparam name="TEntity">The entity class.</typeparam>
    /// <typeparam name="TKey">The key of <see cref="TEntity"/>.</typeparam>
    public class CrudController<TCreateDto, TReadDto, TUpdateDto, TEntity, TKey> : BaseApiController
        where TCreateDto : BaseDto<TCreateDto, TEntity, TKey>, new()
        where TReadDto : BaseReadDto<TReadDto, TEntity, TKey>, new()
        where TUpdateDto : BaseDto<TUpdateDto, TEntity, TKey>, new()
        where TEntity : class, IEntity, new()
    {
        protected readonly IRepository<TEntity> repository;
        protected readonly IMapper mapper;

        public CrudController(IRepository<TEntity> repository, IMapper mapper)
        {
            this.repository = repository;
            this.mapper = mapper;
        }

        [HttpGet]
        public virtual async Task<ActionResult<ApiResult<List<TReadDto>>>> Get(CancellationToken cancellationToken)
        {
            var list = await repository.TableNoTracking.ProjectTo<TReadDto>(mapper.ConfigurationProvider)
                            .ToListAsync(cancellationToken);

            return Ok(new ApiResult<List<TReadDto>>()
                            .WithData(list));
        }

        [HttpGet("{id}")]
        public virtual async Task<ActionResult<ApiResult<TReadDto>>> Get(TKey id, CancellationToken cancellationToken)
        {
            var dto = await repository.TableNoTracking.ProjectTo<TReadDto>(mapper.ConfigurationProvider)
                            .FirstOrDefaultAsync(p => p.Id.Equals(id), cancellationToken);

            if (dto is null)
                return NotFound();

            return Ok(new ApiResult<TReadDto>()
                            .WithData(dto));
        }

        [HttpPost]
        public virtual async Task<ActionResult<ApiResult<TReadDto>>> Create(TCreateDto dto, CancellationToken cancellationToken)
        {
            var model = mapper.Map<TEntity>(dto);

            await repository.AddAsync(model, cancellationToken);

            var resultDto = mapper.Map<TReadDto>(model);
            resultDto = await repository.TableNoTracking.ProjectTo<TReadDto>(mapper.ConfigurationProvider)
                                        .FirstOrDefaultAsync(o => o.Id.Equals(resultDto.Id), cancellationToken);

            return Ok(new ApiResult<TReadDto>()
                            .WithData(resultDto));
        }

        [HttpPut("{id}")]
        public virtual async Task<ActionResult<ApiResult<TReadDto>>> Update(TKey id, TUpdateDto dto, CancellationToken cancellationToken)
        {
            var model = await repository.GetByIdAsync(cancellationToken, id);

            if (model is null)
                return NotFound(new ApiResult(false));

            model = mapper.Map(dto, model);

            await repository.UpdateAsync(model, cancellationToken);

            var resultDto = await repository.TableNoTracking.ProjectTo<TReadDto>(mapper.ConfigurationProvider)
                                        .FirstOrDefaultAsync(o => o.Id.Equals(id), cancellationToken);

            return Ok(new ApiResult<TReadDto>()
                            .WithData(resultDto));
        }

        [HttpDelete("{id}")]
        public virtual async Task<ActionResult<ApiResult>> Delete(TKey id, CancellationToken cancellationToken)
        {
            var model = await repository.GetByIdAsync(cancellationToken, id);

            if (model is null)
                return NotFound();

            await repository.DeleteAsync(model, cancellationToken);

            return Ok();
        }
    }

    /// <summary>
    /// A generic CRUD controller.
    /// </summary>
    /// <remarks>
    /// Don't use it for entities with Foreign Keys or override and handle actions if it is required.
    /// </remarks>
    /// <typeparam name="TCreateDto">The DTO for creating entity.</typeparam>
    /// <typeparam name="TReadDto">The DTO for reading entity.</typeparam>
    /// <typeparam name="TUpdateDto">The DTO for updating entity.</typeparam>
    /// <typeparam name="TEntity">The entity class.</typeparam>
    public class CrudController<TCreateDto, TReadDto, TUpdateDto, TEntity> : CrudController<TCreateDto, TReadDto, TUpdateDto, TEntity, int>
        where TCreateDto : BaseDto<TCreateDto, TEntity>, new()
        where TReadDto : BaseReadDto<TReadDto, TEntity>, new()
        where TUpdateDto : BaseDto<TUpdateDto, TEntity>, new()
        where TEntity : class, IEntity, new()
    {
        public CrudController(IRepository<TEntity> repository, IMapper mapper) 
            : base(repository, mapper)
        {  }
    }

    /// <summary>
    /// A generic CRUD controller.
    /// </summary>
    /// <remarks>
    /// Don't use it for entities with Foreign Keys or override and handle actions if it is required.
    /// </remarks>
    /// <typeparam name="TCreateAndUpdateDto">The DTO for creating and updating entity.</typeparam>
    /// <typeparam name="TReadDto">The DTO for reading entity.</typeparam>
    /// <typeparam name="TEntity">The entity class.</typeparam>
    public class CrudController<TCreateAndUpdateDto, TReadDto, TEntity> : CrudController<TCreateAndUpdateDto, TReadDto, TCreateAndUpdateDto, TEntity>
       where TCreateAndUpdateDto : BaseDto<TCreateAndUpdateDto, TEntity>, new()
       where TReadDto : BaseReadDto<TReadDto, TEntity>, new()
        where TEntity : class, IEntity, new()
    {
        public CrudController(IRepository<TEntity> repository, IMapper mapper)
            : base(repository, mapper)
        { }
    }
}
