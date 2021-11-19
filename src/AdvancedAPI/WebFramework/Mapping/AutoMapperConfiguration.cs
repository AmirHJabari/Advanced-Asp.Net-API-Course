using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.Extensions.DependencyInjection;

namespace WebFramework.Mapping
{
    public static class AutoMapperConfiguration
    {
        /// <summary>
        /// Adds <see cref="Profile"/>s and classes that implemented <see cref="ICustomMapping"/>.
        /// </summary>
        /// <param name="services">The services to add AutoMapper to it.</param>
        /// <param name="customMappingAssembly">Assemblies that contains classes that implemented <see cref="ICustomMapping"/>.</param>
        /// <param name="profileAssemblies">Assemblies that contains AutoMapper <see cref="Profile"/> class</param>
        public static void InitializeAutoMapper(this IServiceCollection services, Assembly customMappingAssembly = null, params Assembly[] profileAssemblies)
        {
            //See http://docs.automapper.org/en/stable/Configuration.html
            //And https://code-maze.com/automapper-net-core/

            services.AddAutoMapper(config =>
            {
                config.AddCustomMappingProfile();
                if (customMappingAssembly is not null)
                    config.AddCustomMappingProfile(customMappingAssembly);

                config.Advanced.BeforeSeal(configProvicer =>
                {
                    configProvicer.CompileMappings();
                });
            }, profileAssemblies);
        }

        /// <summary>
        /// Adds the classes that implemented <see cref="ICustomMapping"/> in <see cref="Assembly.GetEntryAssembly"/>.
        /// </summary>
        public static void AddCustomMappingProfile(this IMapperConfigurationExpression config)
        {
            config.AddCustomMappingProfile(Assembly.GetEntryAssembly());
        }

        /// <summary>
        /// Adds the classes that implemented <see cref="ICustomMapping"/>.
        /// </summary>
        /// <param name="assemblies">Assemblies that contains classes with <see cref="ICustomMapping"/></param>
        public static void AddCustomMappingProfile(this IMapperConfigurationExpression config, params Assembly[] assemblies)
        {
            var allTypes = assemblies.SelectMany(a => a.ExportedTypes);

            var list = allTypes.Where(type => type.IsClass && !type.IsAbstract &&
                type.GetInterfaces().Contains(typeof(ICustomMapping)))
                .Select(type => (ICustomMapping)Activator.CreateInstance(type));

            var profile = new CustomMappingProfile(list);

            config.AddProfile(profile);
        }
    }
}
