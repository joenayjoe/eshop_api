using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshop_api.DTO
{
    public class OrderDetailDTO
    {
        public int Id { get; set; }

        public int Quantity { get; set; }

        public decimal UnitCost { get; set; }

        public decimal LineItemTotal { get; set; }
        public ProductDTO Product { get; set; }
    }
}
