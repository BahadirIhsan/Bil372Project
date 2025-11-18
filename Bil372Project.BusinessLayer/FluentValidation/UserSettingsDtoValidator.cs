using Bil372Project.BusinessLayer.Dtos;
using FluentValidation;

namespace Bil372Project.BusinessLayer.FluentValidation;

public class UserSettingsDtoValidator : AbstractValidator<UserSettingsDto>
{
    public UserSettingsDtoValidator()
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Ad soyad zorunludur")
            .MaximumLength(150).WithMessage("Ad soyad 150 karakterden uzun olamaz");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email zorunludur")
            .EmailAddress().WithMessage("Geçerli bir email formatı değil");

        RuleFor(x => x.PhoneNumber)
            .MaximumLength(30).WithMessage("Telefon en fazla 30 karakter olabilir")
            .When(x => !string.IsNullOrWhiteSpace(x.PhoneNumber));

        RuleFor(x => x.City)
            .MaximumLength(100).WithMessage("Şehir en fazla 100 karakter olabilir")
            .When(x => !string.IsNullOrWhiteSpace(x.City));

        RuleFor(x => x.Country)
            .MaximumLength(100).WithMessage("Ülke en fazla 100 karakter olabilir")
            .When(x => !string.IsNullOrWhiteSpace(x.Country));

        RuleFor(x => x.Bio)
            .MaximumLength(500).WithMessage("Hakkımda alanı en fazla 500 karakter olabilir")
            .When(x => !string.IsNullOrWhiteSpace(x.Bio));
    }
}
