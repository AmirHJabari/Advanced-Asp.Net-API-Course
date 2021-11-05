using Microsoft.EntityFrameworkCore;
using Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Common.Utilities;

namespace Data.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext dbContext) : base(dbContext)
        { }

        public Task AddAsync(User user, string password, CancellationToken cancellationToken, bool saveNow = true)
        {
            user.PasswordHash = SecurityHelper.GetSha256Hash(password);
            return base.AddAsync(user, cancellationToken, saveNow);
        }

        public Task<bool> IsUserNameExistAsync(string userName, CancellationToken cancellationToken)
        {
            return this.TableNoTracking.AnyAsync(u => u.UserName == userName, cancellationToken);
        }

        public Task UpdateSecurityStampAsync(User user, CancellationToken cancellationToken)
        {
            user.SecurityStamp = Guid.NewGuid();
            return this.UpdateAsync(user, cancellationToken);
        }

        public Task<User> GetUserByUserNameAndPasswordAsync(string userName, string password, CancellationToken cancellationToken)
        {
            string passwordHash = SecurityHelper.GetSha256Hash(password);

            return Entities.FirstOrDefaultAsync(u =>
                        u.UserName == userName && u.PasswordHash == passwordHash,
                        cancellationToken);
        }

        public Task<bool> IsUserExistAsync(int id, CancellationToken cancellationToken)
        {
            return this.TableNoTracking.AnyAsync(u => u.Id == id, cancellationToken);
        }
    }
}
