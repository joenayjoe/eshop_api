using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace eshop_api.DTO
{
    public class CustomerCreateDTO
    {
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
    }
}
