using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework.DTOs.Category
{
    public class CategoryDto : BaseDto<CategoryDto, Entities.Category>, IValidatableObject
    {
        [Required]
        [StringLength(50)]
        public string Name { get; set; }

        public int? ParentCategoryId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            Name = Name.Trim().ToLower();
            yield break;
        }
    }
}
