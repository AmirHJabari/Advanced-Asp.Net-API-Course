﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using Common;
using Microsoft.AspNetCore.Identity;

namespace Services
{
    public class JwtServices : IJwtServices
    {
        private readonly SiteSettings _siteSettings;

        public JwtServices(SiteSettings siteSettings)
        {
            this._siteSettings = siteSettings;
        }

        public string Generate(User user)
        {
            var secretKey = Encoding.UTF8.GetBytes(_siteSettings.Jwt.SecretKey);
            var signingCredentials = new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature);

            var encriptionKey = Encoding.UTF8.GetBytes(_siteSettings.Jwt.EncriptKey);
            var encryptingCredentials = new EncryptingCredentials(new SymmetricSecurityKey(encriptionKey), SecurityAlgorithms.Aes128KW, SecurityAlgorithms.Aes128CbcHmacSha256);

            var claims = this.GetClaims(user);

            var descriptor = new SecurityTokenDescriptor()
            {
                Issuer = _siteSettings.Jwt.Issuer,
                Audience = _siteSettings.Jwt.Audience,
                Subject = new ClaimsIdentity(claims),
                IssuedAt = DateTime.UtcNow,
                NotBefore = DateTime.Now.AddMinutes(_siteSettings.Jwt.NotBeforeMinutes),
                Expires = DateTime.Now.AddMinutes(_siteSettings.Jwt.ExpirationMinutes),
                SigningCredentials = signingCredentials,
                EncryptingCredentials = encryptingCredentials
            };

            //JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            //JwtSecurityTokenHandler.DefaultMapInboundClaims = false;
            //JwtSecurityTokenHandler.DefaultOutboundClaimTypeMap.Clear();

            var tokenHandler = new JwtSecurityTokenHandler();
            SecurityToken securityToken = tokenHandler.CreateJwtSecurityToken(descriptor);

            return tokenHandler.WriteToken(securityToken);
        }

        public IEnumerable<Claim> GetClaims(User user)
        {
            var securityStampClaimType = new ClaimsIdentityOptions().SecurityStampClaimType;
            yield return new Claim(ClaimTypes.Name, user.UserName);
            yield return new Claim(ClaimTypes.NameIdentifier, user.Id.ToString());

            var roles = new List<Role>()
            {
                new Role() {Name = "Admin"},
                new Role() {Name = "Reader"},
                new Role() {Name = "Writer"},
            };

            foreach (var role in roles)
                yield return new Claim(ClaimTypes.Role, role.Name);

            yield return new Claim(securityStampClaimType, user.SecurityStamp.ToString());
        }
    }
}
