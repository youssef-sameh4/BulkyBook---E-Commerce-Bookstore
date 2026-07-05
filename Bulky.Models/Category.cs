using System.ComponentModel.DataAnnotations;

namespace Bulky.Models.Models
{
    public class Category
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [StringLength(100,ErrorMessage = "Name cannot exceed 100 characters!")]
        [Display(Name="Category Name")]
        public string Name { get; set; }
        [Range(0,100, ErrorMessage = "Display Order must be between 0 and 100!")]
        [Display(Name = "Display Order")]
        public int DisplayOrder { get; set; }

    }
}
