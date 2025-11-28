using Bil372Project.BusinessLayer.Dtos;
using FluentValidation;

namespace Bil372Project.BusinessLayer.FluentValidation;

public class LoginValidator : AbstractValidator<LoginDto>
{
    public LoginValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email zorunludur.")
            .EmailAddress().WithMessage("Geçerli bir email adresi giriniz.");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre zorunludur.");
    }
}