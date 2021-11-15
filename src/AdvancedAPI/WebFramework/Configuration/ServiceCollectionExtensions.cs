using System.Text.Json;
using WebFramework.API;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Common;
using Common.Exceptions;
using System.Net;
using Data.Repositories;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using Common.Utilities;
using Microsoft.AspNetCore.Http;
using Entities;
using Data;

namespace WebFramework.Configuration
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddJwtAuthentication(this IServiceCollection services, JwtSettings settings)
        {
            services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                var secretKey = Encoding.UTF8.GetBytes(settings.SecretKey);
                var encriptionKey = Encoding.UTF8.GetBytes(settings.EncriptKey);

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
                    ValidIssuer = settings.Issuer,

                    TokenDecryptionKey = new SymmetricSecurityKey(encriptionKey),
                };

                options.IncludeErrorDetails = false;
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.Events = new JwtBearerEvents()
                {
                    OnChallenge = context =>
                    {
                        context.HandleResponse();

                        if (context.Response.HasStarted)
                            return Task.CompletedTask;

                        context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                        switch (context.AuthenticateFailure)
                        {
                            case SecurityTokenDecryptionFailedException:
                            case ArgumentException:
                                return context.Response.WriteAsJsonAsync(
                                    new ApiResult(false)
                                        .WithCode(ApiResultStatusCode.InvalidToken)
                                        .WithMessage("Token is not valid."),
                                    context.HttpContext.RequestAborted);

                            case SecurityTokenExpiredException:
                                return context.Response.WriteAsJsonAsync(
                                    new ApiResult(false)
                                        .WithCode(ApiResultStatusCode.TokenExpired)
                                        .WithMessage("Token is expired."),
                                    context.HttpContext.RequestAborted);

                            case null when !context.Response.HasStarted:
                                return context.Response.WriteAsJsonAsync(
                                    new ApiResult(false)
                                        .WithCode(ApiResultStatusCode.TokenRequired)
                                        .WithMessage("You are unauthorized to access this resource."),
                                    context.HttpContext.RequestAborted);
                        }

                        return Task.CompletedTask;
                    },
                    OnForbidden = context =>
                    {
                        context.Response.StatusCode = StatusCodes.Status403Forbidden;
                        return context.Response.WriteAsJsonAsync(
                            new ApiResult(false)
                                .WithCode(ApiResultStatusCode.AccessDenied)
                                .WithMessage("You don't have access to this resource."),
                            context.HttpContext.RequestAborted);
                    },
                    OnTokenValidated = async context =>
                    {
                        //var applicationSignInManager = context.HttpContext.RequestServices.GetRequiredService<IApplicationSignInManager>();
                        var userRepository = context.HttpContext.RequestServices.GetRequiredService<IUserRepository>();

                        var claimsIdentity = context.Principal.Identity as ClaimsIdentity;
                        if (claimsIdentity.Claims?.Any() != true)
                            context.Fail("This token has no claims.");

                        var securityStamp = claimsIdentity.FindFirstValue(new ClaimsIdentityOptions().SecurityStampClaimType);
                        //if (!securityStamp.HasValue())
                        //    context.Fail("This token has no secuirty stamp");

                        //Find user and token from database and perform your custom validation
                        var userId = claimsIdentity.GetUserId<int>();
                        var user = await userRepository.GetByIdAsync(context.HttpContext.RequestAborted, userId);

                        if (!user.IsActive)
                        {
                            string msg = "Account is restricted.";
                            await context.Response.WriteAsJsonAsync(new ApiResult(false, msg, ApiResultStatusCode.AccountRestricted), context.HttpContext.RequestAborted);
                            context.Fail(msg);
                            return;
                        }

                        if (!user.SecurityStamp.Equals(securityStamp, StringComparison.OrdinalIgnoreCase))
                        {
                            string msg = "Token secuirty stamp is not valid.";
                            await context.Response.WriteAsJsonAsync(new ApiResult(false, msg, ApiResultStatusCode.TokenStampHasChanged), context.HttpContext.RequestAborted);
                            context.Fail(msg);
                            return;
                        }

                        //var validatedUser = await applicationSignInManager.ValidateSecurityStampAsync(context.Principal);
                        //if (validatedUser == null)
                        //    context.Fail("Token secuirty stamp is not valid.");

                        await userRepository.UpdateLastActivityDateAsync(user, context.HttpContext.RequestAborted);
                    }
                };
            });

            return services;
        }

        public static IServiceCollection AddCustomIdentity(this IServiceCollection services, IdentitySettings settings)
        {
            services.AddIdentity<User, Role>(options =>
            {
                // Password
                options.Password.RequireDigit = settings.PasswordRequireDigit;
                options.Password.RequiredLength = settings.PasswordRequiredLength;
                options.Password.RequireNonAlphanumeric = settings.PasswordRequireNonAlphanume;
                options.Password.RequireUppercase = settings.PasswordRequireUppercase;
                options.Password.RequireLowercase = settings.PasswordRequireLowercase;
                options.Password.RequiredUniqueChars = settings.PasswordRequiredUniqueChars;

                // Lockout
                options.Lockout.AllowedForNewUsers = settings.LockoutAllowedForNewUsers;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(settings.DefaultLockoutMinutes);
                options.Lockout.MaxFailedAccessAttempts = settings.LockoutMaxFailedAccessAttempts;

                // User
                options.User.RequireUniqueEmail = settings.UserRequireUniqueEmail;
                options.User.AllowedUserNameCharacters = settings.AllowedUserNameCharacters;
            }).AddEntityFrameworkStores<ApplicationDbContext>()
            .AddDefaultTokenProviders();

            return services;
        }
    }
}
