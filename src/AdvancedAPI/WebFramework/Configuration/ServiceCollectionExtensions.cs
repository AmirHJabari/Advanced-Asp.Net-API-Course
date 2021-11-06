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
                        OnAuthenticationFailed = context =>
                        {
                            var t = context.Exception.GetType();

                            context.Response.StatusCode = StatusCodes.Status401Unauthorized;

                            switch (context.Exception)
                            {
                                case SecurityTokenDecryptionFailedException:
                                case ArgumentException:
                                default:
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
                            }
                        },
                        OnChallenge = context =>
                        {
                            context.HandleResponse();

                            if (context.AuthenticateFailure is null && !context.Response.HasStarted)
                            {
                                context.Response.StatusCode = StatusCodes.Status401Unauthorized;
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

                            Guid.TryParse(securityStamp, out Guid ss);
                            if (!user.SecurityStamp.Equals(ss))
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
    }
}
