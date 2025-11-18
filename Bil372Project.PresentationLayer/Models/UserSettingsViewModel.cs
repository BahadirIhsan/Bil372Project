using System.ComponentModel.DataAnnotations;

namespace Bil372Project.PresentationLayer.Models;

public class UserSettingsViewModel
{
    [Required(ErrorMessage = "Ad soyad zorunludur")]
    [Display(Name = "Ad Soyad")]
    public string FullName { get; set; } = string.Empty;

    [Required(ErrorMessage = "Email zorunludur")]
    [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
    public string Email { get; set; } = string.Empty;

    [Phone(ErrorMessage = "Geçerli bir telefon numarası giriniz")]
    [Display(Name = "Telefon Numarası")]
    public string? PhoneNumber { get; set; }

    [DataType(DataType.Date)]
    [Display(Name = "Doğum Tarihi")]
    public DateTime? BirthDate { get; set; }

    public string? City { get; set; }
    public string? Country { get; set; }

    [Display(Name = "Hakkımda")]
    [MaxLength(500, ErrorMessage = "Hakkımda alanı en fazla 500 karakter olabilir")]
    public string? Bio { get; set; }
}