using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Data;
using Data.Repositories;
using Entities;
using Microsoft.AspNetCore.Identity;

namespace Services.SeedData
{
    public class UserSeedData : ISeedData
    {
        private readonly IUserRepository _repository;
        private readonly ApplicationDbContext _dbContext;

        public UserSeedData(IUserRepository repository, ApplicationDbContext dbContext)
        {
            this._repository = repository;
            this._dbContext = dbContext;
        }
        public void InitializeData()
        {
            Console.WriteLine(nameof(UserSeedData));
            if (!_repository.TableNoTracking.Any(u => u.UserName == "admin"))
            {
                #region Add User
                string email = "amirhamzehjabari@outlook.com";
                string userName = "admin";
                var user = new Entities.User
                {
                    Age = 19,
                    Email = email,
                    NormalizedEmail = email.ToUpper(),
                    FirstName = "Amir H.",
                    LastName = "Jabari",
                    UserName = userName,
                    NormalizedUserName = userName.ToUpper(),
                    Gender = Entities.Gender.Male,
                    IsActive = true,
                    SecurityStamp = Guid.NewGuid().ToString(),
                    PasswordHash = "AQAAAAEAACcQAAAAELYR5nFnfAI/5wInm1PQVZ8i//ja2SV69uc58FoqG2WvQjUVwUnhN3ulVqYkpNe7OA=="
                };
                this._repository.Add(user);
                #endregion

                #region Add Role
                string roleName = "admin";
                var role = new Role()
                {
                    Name = roleName,
                    NormalizedName = roleName.ToUpper(),
                    Description = "the administrator"
                };
                var roles = this._dbContext.Set<Role>();

                if (!roles.Any(r => r.Name == role.Name))
                    roles.Add(role);

                _dbContext.SaveChanges();
                #endregion

                #region Assign Role
                var userRoles = _dbContext.Set<IdentityUserRole<int>>();
                role = roles.FirstOrDefault(r => r.Name == roleName);

                userRoles.Add(new IdentityUserRole<int>()
                {
                    UserId = user.Id,
                    RoleId = role.Id
                });

                _dbContext.SaveChanges();
                #endregion
            }
        }
    }
}
