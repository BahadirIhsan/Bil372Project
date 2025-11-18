using System.ComponentModel.DataAnnotations;

namespace Bil372Project.PresentationLayer.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Email zorunludur")]
        [EmailAddress(ErrorMessage = "Geçerli bir email adresi giriniz")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Şifre zorunludur")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        // İstersen: Beni hatırla kutusu
        public bool RememberMe { get; set; }
    }
}