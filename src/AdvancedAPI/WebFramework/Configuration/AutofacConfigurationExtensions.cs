using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.DI;
using Data.Repositories;
using Microsoft.Extensions.DependencyInjection;

namespace WebFramework.Configuration
{
    public static class AutofacConfigurationExtensions
    {
        public static void AddServices(this ContainerBuilder containerBuilder)
        {
            // not avaliable in aspnet IOC e.g
            // 1> Property Injection
            // 2> Assembly Scanning + Auto Registeration
            // 3> Interception

            containerBuilder.RegisterGeneric(typeof(Repository<>)).As(typeof(IRepository<>)).InstancePerLifetimeScope();

            var commonAssembly = typeof(Common.SiteSettings).Assembly;
            var entitiesAssembly = typeof(Entities.IEntity).Assembly;
            var dataAssembly = typeof(Data.ApplicationDbContext).Assembly;
            var servicesAssembly = typeof(Services.JwtServices).Assembly;

            containerBuilder.RegisterAssemblyTypes(commonAssembly, entitiesAssembly, dataAssembly, servicesAssembly)
                .AssignableTo<IScopedDependency>()
                .AsImplementedInterfaces()
                .InstancePerLifetimeScope();

            containerBuilder.RegisterAssemblyTypes(commonAssembly, entitiesAssembly, dataAssembly, servicesAssembly)
                .AssignableTo<ITransientDependency>()
                .AsImplementedInterfaces()
                .InstancePerDependency();

            containerBuilder.RegisterAssemblyTypes(commonAssembly, entitiesAssembly, dataAssembly, servicesAssembly)
                .AssignableTo<ISingletonDependency>()
                .AsImplementedInterfaces()
                .SingleInstance();
        }

        /// <summary>
        /// For versions below Asp.Net Core 3.0
        /// </summary>
        [Obsolete]
        public static IServiceProvider BuildAotofacServiceProvider(this IServiceCollection services)
        {
            var containerBuilder = new ContainerBuilder();
            containerBuilder.Populate(services);

            // Register > As > Lifetime
            containerBuilder.AddServices();

            var container = containerBuilder.Build();
            return new AutofacServiceProvider(container);
        }
    }
}
