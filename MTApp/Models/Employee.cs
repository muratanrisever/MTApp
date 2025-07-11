using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MTApp.Models
{
    public class Employee
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Sicil Numarası zorunludur.")]
        [StringLength(20, ErrorMessage = "Sicil Numarası en fazla 20 karakter olabilir.")]
        [Display(Name = "Sicil No")]
        public string EmployeeNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ad zorunludur.")]
        [StringLength(50, ErrorMessage = "Ad en fazla 50 karakter olabilir.")]
        [Display(Name = "Ad")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Soyad zorunludur.")]
        [StringLength(50, ErrorMessage = "Soyad en fazla 50 karakter olabilir.")]
        [Display(Name = "Soyad")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "TC Kimlik Numarası zorunludur.")]
        [StringLength(11, MinimumLength = 11, ErrorMessage = "TC Kimlik Numarası 11 karakter olmalıdır.")]
        [Display(Name = "TC Kimlik No")]
        public string NationalId { get; set; } = string.Empty;

        [Required(ErrorMessage = "Doğum Tarihi zorunludur.")]
        [DataType(DataType.Date)]
        [Display(Name = "Doğum Tarihi")]
        public DateTime DateOfBirth { get; set; }

        [Required(ErrorMessage = "Cinsiyet zorunludur.")]
        [StringLength(10, ErrorMessage = "Cinsiyet en fazla 10 karakter olabilir.")]
        [Display(Name = "Cinsiyet")]
        public string Gender { get; set; } = string.Empty;

        [StringLength(20, ErrorMessage = "Medeni Durum en fazla 20 karakter olabilir.")]
        [Display(Name = "Medeni Durum")]
        public string? MaritalStatus { get; set; }

        [StringLength(50, ErrorMessage = "Uyruk en fazla 50 karakter olabilir.")]
        [Display(Name = "Uyruk")]
        public string? Nationality { get; set; }

        [Required(ErrorMessage = "E-posta adresi zorunludur.")]
        [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
        [Display(Name = "E-posta")]
        public string Email { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Geçerli bir telefon numarası girin.")]
        [Display(Name = "Telefon")]
        public string? PhoneNumber { get; set; }

        [StringLength(200, ErrorMessage = "Adres en fazla 200 karakter olabilir.")]
        [Display(Name = "Adres")]
        public string? Address { get; set; }

        [Required(ErrorMessage = "İşe Giriş Tarihi zorunludur.")]
        [DataType(DataType.Date)]
        [Display(Name = "İşe Giriş Tarihi")]
        public DateTime HireDate { get; set; }

        [DataType(DataType.Date)]
        [Display(Name = "İşten Çıkış Tarihi")]
        public DateTime? TerminationDate { get; set; }

        [Required(ErrorMessage = "Departman zorunludur.")]
        [Display(Name = "Departman")]
        public int DepartmentId { get; set; }

        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        [Required(ErrorMessage = "Unvan zorunludur.")]
        [Display(Name = "Unvan")]
        public int TitleId { get; set; }

        [ForeignKey("TitleId")]
        public Title? Title { get; set; }

        [Required(ErrorMessage = "Maaş zorunludur.")]
        [Column(TypeName = "decimal(18, 2)")]
        [Display(Name = "Maaş")]
        public decimal Salary { get; set; }

        [StringLength(255, ErrorMessage = "Fotoğraf URL'si en fazla 255 karakter olabilir.")]
        [Display(Name = "Fotoğraf URL")]
        public string? PhotoUrl { get; set; }

        [Display(Name = "Özgeçmiş")]
        public string? ResumePath { get; set; }

        [Display(Name = "Aktif Mi?")]
        public bool IsActive { get; set; } = true;
    }
}