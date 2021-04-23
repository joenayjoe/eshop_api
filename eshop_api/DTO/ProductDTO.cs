using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eshop_api.DTO
{
    public class ProductDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string ModelName { get; set; }

        public bool IsFeatured { get; set; }

        public string ModelNumber { get; set; }

        public string ProductImage { get; set; }

        public string ProductImageLarge { get; set; }


        public string ProductImageThumb { get; set; }

        public decimal UnitCost { get; set; }

        public decimal CurrentPrice { get; set; }

        public int UnitInStock { get; set; }

        public CategoryDTO Category { get; set; }

    }
}
