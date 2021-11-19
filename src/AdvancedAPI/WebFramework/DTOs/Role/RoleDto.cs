using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework.DTOs.Role
{
    public class RoleDto : BaseDto<RoleDto, Entities.Role>
    {
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string Name { get; set; }
        [Required]
        [StringLength(100, MinimumLength = 10)]
        public string Description { get; set; }
    }
}
