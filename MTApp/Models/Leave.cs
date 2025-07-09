using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MTApp.Models
{
    public enum LeaveStatus
    {
        Pending,    // Beklemede
        Approved,   // Onaylandı
        Rejected    // Reddedildi
    }

    public class Leave
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Personel seçimi zorunludur.")]
        [Display(Name = "Personel")]
        public int EmployeeId { get; set; }

        [ForeignKey("EmployeeId")]
        public Employee? Employee { get; set; }

        [Required(ErrorMessage = "İzin Türü zorunludur.")]
        [StringLength(50, ErrorMessage = "İzin Türü en fazla 50 karakter olabilir.")]
        [Display(Name = "İzin Türü")]
        public string LeaveType { get; set; } = string.Empty; // Yıllık, Hastalık, Doğum vb.

        [Required(ErrorMessage = "Başlangıç Tarihi zorunludur.")]
        [DataType(DataType.Date)]
        [Display(Name = "Başlangıç Tarihi")]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "Bitiş Tarihi zorunludur.")]
        [DataType(DataType.Date)]
        [Display(Name = "Bitiş Tarihi")]
        public DateTime EndDate { get; set; }

        [Required(ErrorMessage = "Gün Sayısı zorunludur.")]
        [Range(0.5, 365, ErrorMessage = "Gün sayısı 0.5 ile 365 arasında olmalıdır.")]
        [Display(Name = "Gün Sayısı")]
        public double NumberOfDays { get; set; }

        [Required(ErrorMessage = "Onay Durumu zorunludur.")]
        [Display(Name = "Onay Durumu")]
        public LeaveStatus Status { get; set; } = LeaveStatus.Pending; // Varsayılan: Beklemede

        [StringLength(500, ErrorMessage = "Açıklama en fazla 500 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "Talep Tarihi")]
        public DateTime RequestDate { get; set; } = DateTime.Now;
    }
}