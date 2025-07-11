using System.ComponentModel.DataAnnotations;

namespace MTApp.Models
{
    public class Title
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Unvan Adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Unvan Adı en fazla 100 karakter olabilir.")]
        [Display(Name = "Unvan Adı")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        public ICollection<Employee>? Employees { get; set; }
    }
}