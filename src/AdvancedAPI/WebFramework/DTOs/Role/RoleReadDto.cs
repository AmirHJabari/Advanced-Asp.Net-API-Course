using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework.DTOs.Role
{
    public class RoleReadDto : BaseReadDto<RoleReadDto, Entities.Role>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
