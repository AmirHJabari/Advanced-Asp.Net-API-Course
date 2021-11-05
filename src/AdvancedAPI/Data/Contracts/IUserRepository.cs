using Entities;
using System.Threading;
using System.Threading.Tasks;

namespace Data.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetUserByUserNameAndPasswordAsync(string userName, string password, CancellationToken cancellationToken);
        Task AddAsync(User user, string password, CancellationToken cancellationToken, bool saveNow = true);
        Task<bool> IsUserNameExistAsync(string userName, CancellationToken cancellationToken);
        Task<bool> IsUserExistAsync(int id, CancellationToken cancellationToken);
        Task UpdateSecurityStampAsync(User user, CancellationToken cancellationToken);
    }
}