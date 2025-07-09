using System.ComponentModel.DataAnnotations;

namespace MTApp.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Departman Adı zorunludur.")]
        [StringLength(100, ErrorMessage = "Departman Adı en fazla 100 karakter olabilir.")]
        [Display(Name = "Departman Adı")]
        public string Name { get; set; } = string.Empty;

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        // Navigation Property: Bu departmana bağlı çalışanlar
        public ICollection<Employee>? Employees { get; set; }
    }
}