using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace MTApp.Models
{
    public class ApplicationUser : IdentityUser
    {
        [PersonalData]
        [Display(Name = "Ad")]
        [StringLength(50)]
        public string? FirstName { get; set; }

        [PersonalData]
        [Display(Name = "Soyad")]
        [StringLength(50)]
        public string? LastName { get; set; }

        [Display(Name = "Sicil No")]
        [StringLength(20)]
        public string? EmployeeNumber { get; set; }

        [Display(Name = "Aktif Mi?")]
        public bool IsActive { get; set; } = true;
    }
}