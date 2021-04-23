namespace eshop_api.DTO
{
    public class ShoppingCartRecordDTO
    {
        public int Id { get; set; }

        public int Quantity { get; set; }
        public decimal LineItemTotal { get; set; }
        public ProductDTO Product { get; set; }
    }
}
