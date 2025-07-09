using System.ComponentModel.DataAnnotations;

namespace MTApp.Models
{
    public class Announcement
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Başlık zorunludur.")]
        [StringLength(200, ErrorMessage = "Başlık en fazla 200 karakter olabilir.")]
        [Display(Name = "Başlık")]
        public string Title { get; set; } = string.Empty;

        [Required(ErrorMessage = "İçerik zorunludur.")]
        [Display(Name = "İçerik")]
        public string Content { get; set; } = string.Empty;

        [Required(ErrorMessage = "Yayın Tarihi zorunludur.")]
        [DataType(DataType.Date)]
        [Display(Name = "Yayın Tarihi")]
        public DateTime PublishDate { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        [Display(Name = "Bitiş Tarihi")]
        public DateTime? EndDate { get; set; } // İsteğe bağlı, duyurunun ne zaman sona ereceği
    }
}