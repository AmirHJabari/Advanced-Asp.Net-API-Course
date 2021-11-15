using Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace WebFramework.DTOs
{
    public class UserDto : IValidatableObject
    {
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Range(1, 150)]
        public int Age { get; set; }
        public Gender Gender { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            this.UserName = this.UserName.Trim().ToLower();
            this.Email = this.Email.Trim().ToLower();

            if (UserName.Equals("test", StringComparison.OrdinalIgnoreCase))
                yield return new ValidationResult("UserName can not be 'test'", new[] { nameof(UserName) });

            if (Password.Equals("123456"))
                yield return new ValidationResult($"Password can not be {Password}", new[] { nameof(Password) });

            if (Gender == Gender.Male && Age > 30)
                yield return new ValidationResult("Men older than 30 years are not allowed", new[] { nameof(Gender), nameof(Age) });
        }
    }
}
