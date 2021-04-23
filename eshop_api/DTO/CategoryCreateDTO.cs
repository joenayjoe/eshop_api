using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eshop_api.DTO
{
    public class CategoryCreateDTO
    {
        [Required(ErrorMessage = "Category name is required")]
        public string Name { get; set; }
    }
}
