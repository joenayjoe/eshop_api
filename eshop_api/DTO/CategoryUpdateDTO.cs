using System.ComponentModel.DataAnnotations;

namespace eshop_api.DTO
{
    public class CategoryUpdateDTO
    {
        [Required(ErrorMessage = "Category ID cannot be empty")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Category name is required")]
        public string Name { get; set; }
    }
}
