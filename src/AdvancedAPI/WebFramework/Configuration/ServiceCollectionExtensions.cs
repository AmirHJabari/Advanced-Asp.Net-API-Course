using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Common;

namespace WebFramework.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, JwtSettings settings)
        {
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    var secretKey = Encoding.UTF8.GetBytes(settings.SecretKey);
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        ClockSkew = TimeSpan.FromMinutes(settings.ClockSkewMinutes), // default: 5m
                        RequireSignedTokens = true,

                        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
                        ValidateIssuerSigningKey = true,

                        RequireExpirationTime = true,
                        ValidateLifetime = true,

                        ValidateAudience = true, // default: false
                        ValidAudience = settings.Audience,

                        ValidateIssuer = true, // default: false
                        ValidIssuer = settings.Issuer
                    };

                    options.RequireHttpsMetadata = true;
                    options.SaveToken = true;
                });

            return services;
        }
    }
}
