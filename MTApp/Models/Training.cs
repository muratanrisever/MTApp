using System.ComponentModel.DataAnnotations;

namespace MTApp.Models
{
    public class Training
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Eğitim Adı zorunludur.")]
        [StringLength(200, ErrorMessage = "Eğitim Adı en fazla 200 karakter olabilir.")]
        [Display(Name = "Eğitim Adı")]
        public string Name { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Açıklama en fazla 1000 karakter olabilir.")]
        [Display(Name = "Açıklama")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Eğitim Tarihi zorunludur.")]
        [DataType(DataType.Date)]
        [Display(Name = "Eğitim Tarihi")]
        public DateTime Date { get; set; }

        [StringLength(100, ErrorMessage = "Eğitim Şirketi/Kurumu en fazla 100 karakter olabilir.")]
        [Display(Name = "Eğitim Veren Şirket/Kurum")]
        public string? Provider { get; set; }

        [Display(Name = "Katılımcı Sayısı")]
        [Range(0, int.MaxValue, ErrorMessage = "Katılımcı sayısı negatif olamaz.")]
        public int ParticipantCount { get; set; }

    }
}