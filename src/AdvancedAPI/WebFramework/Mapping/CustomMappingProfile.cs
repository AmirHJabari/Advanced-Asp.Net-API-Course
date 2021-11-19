using AutoMapper;
using System.Collections.Generic;

namespace WebFramework.Mapping
{
    public class CustomMappingProfile : Profile
    {
        public CustomMappingProfile(IEnumerable<ICustomMapping> haveCustomMappings)
        {
            foreach (var item in haveCustomMappings)
                item.CreateMappings(this);
        }
    }
}
