using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Entities
{
    public class User : BaseEntity<int>
    {
        public User()
        {
            this.IsActive = true;
        }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }
        [Required]
        [StringLength(100)]
        public string PasswordHash { get; set; }

        [Required]
        [StringLength(50)]
        public string FirstName { get; set; }
        [Required]
        [StringLength(50)]
        public string LastName { get; set; }

        [Range(1, 150)]
        public int Age { get; set; }
        public Gender Gender { get; set; }
        public bool IsActive { get; set; }
        public DateTimeOffset LastLogin { get; set; }

        public ICollection<Post> Posts { get; set; }
    }

    public enum Gender
    {
        [Display(Name = "هیچکدام")]
        None,

        [Display(Name = "زن")]
        Female,

        [Display(Name = "مرد")]
        Male,
    }
}
