using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace eshop_api.Models
{
    public class ShoppingCartRecord : EntityBase
    {
        public int Id { get; set; }

        public int Quantity { get; set; }

        [NotMapped, DataType(DataType.Currency)]
        public decimal LineItemTotal { get; set; }

        public int CustomerId { get; set; }

        public Customer Customer { get; set; }

        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
