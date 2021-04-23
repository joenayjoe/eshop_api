using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace eshop_api.Models
{
    public class Customer : EntityBase
    {
        public int Id { get; set; }

        [Required]
        [DataType(DataType.Text), MaxLength(50)]
        public string FirstName { get; set; }

        [DataType(DataType.Text), MaxLength(50)]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        [DataType(DataType.EmailAddress), MaxLength(50)]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password), MaxLength(50)]
        public string Password { get; set; }

        public List<Order> Orders { get; set; } = new List<Order>();

        public List<ShoppingCartRecord> ShoppingCartRecords { get; set; } = new List<ShoppingCartRecord>();
    }
}
