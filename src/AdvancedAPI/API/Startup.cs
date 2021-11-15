using ElmahCore;
using ElmahCore.Sql;
using ElmahCore.Mvc;
using Data;
using Data.Repositories;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebFramework.Middlewares;
using WebFramework.Configuration;
using Microsoft.AspNetCore.Mvc.Authorization;
using Common;
using Autofac;
using Autofac.Extensions.DependencyInjection;

namespace API
{
    public class Startup
    {
        private readonly SiteSettings _settings;
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            _settings = configuration.GetSection(nameof(SiteSettings)).Get<SiteSettings>();
        }

        public IConfiguration Configuration { get; }
        public ILifetimeScope AutofacContainer { get; private set; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddApplicationDbContext(Configuration);
            services.AddMinimalControllers();

            //services.AddSwaggerGen(c =>
            //{
            //    c.SwaggerDoc("v1", new OpenApiInfo { Title = "API", Version = "v1" });
            //});
            services.AddCustomElmah(_settings.ElamahPath, Configuration);

            services.AddSingleton(this._settings);

            services.AddCustomIdentity(_settings.Identity);
            services.AddJwtAuthentication(_settings.Jwt);
        }

        /// <summary>
        /// ConfigureContainer is where you can register things directly with Autofac.
        /// This runs after <see cref="ConfigureServices"/> so the things here will override registrations made in <see cref="ConfigureServices"/>.
        /// Don't build the container; that gets done for you by the factory.
        /// </summary>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            builder.AddServices();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            app.UseCustomExceptionHandler();

            if (env.IsDevelopment())
            {
                //app.UseDeveloperExceptionPage();
                //app.UseSwagger();
                //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "API v1"));
            }

            app.UseRouting();
            app.UseHttpsRedirection();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseElmah();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
