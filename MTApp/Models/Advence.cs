using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MTApp.Models
{
    public enum AdvanceStatus
    {
        Pending,    // Beklemede
        Approved,   // Onaylandı
        Rejected    // Reddedildi
    }

    public class Advance
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Personel seçimi zorunludur.")]
        [Display(Name = "Personel")]
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }

        [Required(ErrorMessage = "Miktar zorunludur.")]
        [Range(0.01, 1000000.00, ErrorMessage = "Miktar 0.01 ile 1.000.000 arasında olmalıdır.")]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Miktar")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Talep Tarihi zorunludur.")]
        [DataType(DataType.Date)]
        [Display(Name = "Talep Tarihi")]
        public DateTime RequestDate { get; set; } = DateTime.Now;

        [Required(ErrorMessage = "Onay Durumu zorunludur.")]
        [Display(Name = "Onay Durumu")]
        public AdvanceStatus Status { get; set; } = AdvanceStatus.Pending; // Varsayılan: Beklemede

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }
    }
}