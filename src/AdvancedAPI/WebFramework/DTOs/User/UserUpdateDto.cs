using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Entities;

namespace WebFramework.DTOs
{
    public class UserUpdateDto : IValidatableObject
    {
        [StringLength(50)]
        public string UserName { get; set; }

        [StringLength(100)]
        [EmailAddress]
        public string Email { get; set; }

        [StringLength(50)]
        public string FirstName { get; set; }
        [StringLength(50)]
        public string LastName { get; set; }

        [Range(1, 150)]
        public int? Age { get; set; }
        public Gender? Gender { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            this.UserName = this.UserName?.Trim().ToLower();
            this.Email = this.Email?.Trim().ToLower();

            if (UserName?.Equals("test", StringComparison.OrdinalIgnoreCase) == true)
                yield return new ValidationResult("UserName can not be 'test'", new[] { nameof(UserName) });
        }
    }
}
