using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using Entities;

namespace Services
{
    public interface IJwtServices
    {
        Task<string> GenerateAsync(User user);
        Task<IEnumerable<Claim>> GetClaimsAsync(User user);
    }
}