using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eshop_api.DTO
{
    public class ProductCreateDTO
    {
        [Required(ErrorMessage = "Product name is required")]
        [MaxLength(250)]
        public string Name { get; set; }

        [MaxLength(3800)]
        public string Description { get; set; }

        [MaxLength(50)]
        public string ModelName { get; set; }

        public bool IsFeatured { get; set; }

        [MaxLength(50)]
        public string ModelNumber { get; set; }

        [MaxLength(150)]
        public string ProductImage { get; set; }

        [DataType(DataType.Currency)]
        public decimal UnitCost { get; set; }

        [DataType(DataType.Currency)]
        public decimal CurrentPrice { get; set; }

        public int UnitInStock { get; set; }

        [Required]
        public int CategoryId { get; set; }
    }
}
