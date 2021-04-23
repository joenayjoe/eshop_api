using System.ComponentModel.DataAnnotations;

namespace eshop_api.Models
{
    public class OrderDetail : EntityBase
    {
        public int Id { get; set; }

        public int Quantity { get; set; }

        [DataType(DataType.Currency)]
        public decimal UnitCost { get; set; }

        public decimal? LineItemTotal { get; set; }

        [Required]
        public int OrderId { get; set; }

        public Order Order { get; set; }

        [Required]
        public int ProductId { get; set; }
        public Product Product { get; set; }
    }
}
