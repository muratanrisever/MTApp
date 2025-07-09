using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MTApp.Models
{
    // IdentityUser sınıfını genişleterek kendi kullanıcı modelimizi oluşturuyoruz.
    // Buraya eklemek istediğiniz kullanıcıya özel alanları ekleyebilirsiniz.
    public class ApplicationUser : IdentityUser
    {
        [PersonalData] // Bu alanın kişisel veri olduğunu belirtir.
        [Display(Name = "Ad")]
        [StringLength(50)]
        public string? FirstName { get; set; }

        [PersonalData]
        [Display(Name = "Soyad")]
        [StringLength(50)]
        public string? LastName { get; set; }

        [Display(Name = "Sicil No")]
        [StringLength(20)]
        public string? EmployeeNumber { get; set; } // Personel sicil numarası

        [Display(Name = "Aktif Mi?")]
        public bool IsActive { get; set; } = true; // Kullanıcının aktif olup olmadığı
    }
}