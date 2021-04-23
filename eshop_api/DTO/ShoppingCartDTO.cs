using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshop_api.DTO
{
    public class ShoppingCartDTO
    {
        public CustomerDTO Customer { get; set; }

        public IEnumerable<ShoppingCartRecordDTO> CartItems { get; set; }
    }
}
