using System.ComponentModel.DataAnnotations;

namespace MTApp.Models
{
    // Özel kullanıcı modelimiz. IdentityUser'dan kalıtım almaz.
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Kullanıcı Adı zorunludur.")]
        [StringLength(50, ErrorMessage = "Kullanıcı Adı en fazla 50 karakter olabilir.")]
        [Display(Name = "Kullanıcı Adı")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        [StringLength(100)]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre Hash'i zorunludur.")]
        [StringLength(255)] // Şifre hash'i için yeterli uzunluk
        [Display(Name = "Şifre Hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [StringLength(50)]
        [Display(Name = "Rol")]
        public string Role { get; set; } = "User"; // Varsayılan rol: User (Admin, HR vb. eklenebilir)

        [Display(Name = "Kayıt Tarihi")]
        public DateTime RegistrationDate { get; set; } = DateTime.Now;
    }
}