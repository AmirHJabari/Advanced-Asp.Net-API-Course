using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
    public class IdentitySettings
    {
        // Password
        public bool PasswordRequireDigit { get; set; }
        public int PasswordRequiredLength { get; set; }
        public bool PasswordRequireNonAlphanume { get; set; }
        public bool PasswordRequireUppercase { get; set; }
        public bool PasswordRequireLowercase { get; set; }
        public int PasswordRequiredUniqueChars { get; set; }

        // Lockout
        public bool LockoutAllowedForNewUsers { get; set; }
        public int DefaultLockoutMinutes { get; set; }
        public int LockoutMaxFailedAccessAttempts { get; set; }

        // User
        public bool UserRequireUniqueEmail { get; set; }
        public string AllowedUserNameCharacters { get; set; }
    }
}
