using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WebFramework.DTOs.Category
{
    public class CategoryReadDto : BaseReadDto<CategoryReadDto, Entities.Category>
    {
        public string Name { get; set; }
        public int? ParentCategoryId { get; set; }
        public string ParentCategoryName { get; set; }
    }
}
