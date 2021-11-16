using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using Entities;
using WebFramework.DTOs;

namespace API.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            #region nullable mappers
            CreateMap<int?, int>().ConvertUsing((src, dest) => src ?? dest);
            CreateMap<Gender?, Gender>().ConvertUsing((src, dest) => src ?? dest);
            #endregion

            CreateMap<UserCreateDto, User>();
            CreateMap<UserUpdateDto, User>()
                .ForAllMembers(opts => opts.Condition((src, dest, srcMember) => srcMember != null));

            CreateMap<User, UserReadDto>();
        }
    }
}
