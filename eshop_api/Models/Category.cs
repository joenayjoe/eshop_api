using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eshop_api.Models
{
    [Index(nameof(Category.Name), IsUnique = true)]
    public class Category : EntityBase
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        [DataType(DataType.Text), MaxLength(50)]
        public string Name { get; set; }

        public IList<Product> Products { get; set; } = new List<Product>();
    }
}
