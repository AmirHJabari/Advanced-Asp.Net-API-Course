using System.Collections.Generic;
using System.Security.Claims;
using Entities;

namespace Services
{
    public interface IJwtServices
    {
        string Generate(User user);
        IEnumerable<Claim> GetClaims(User user);
    }
}