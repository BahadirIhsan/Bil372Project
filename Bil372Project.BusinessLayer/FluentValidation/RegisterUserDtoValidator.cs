using Bil372Project.BusinessLayer.Dtos;
using Bil372Project.DataAccessLayer;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Bil372Project.BusinessLayer.FluentValidation;

public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
{
    public RegisterUserDtoValidator(AppDbContext db)
    {
        RuleFor(x => x.FullName)
            .NotEmpty().WithMessage("Ad soyad zorunludur")
            .MaximumLength(150).WithMessage("Ad soyad 150 karakterden uzun olamaz");

        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email zorunludur")
            .EmailAddress().WithMessage("Geçerli bir email formatı değil")
            .MustAsync(async (email, cancellationToken) =>
                !await db.Users.AnyAsync(u => u.Email == email, cancellationToken))
            .WithMessage("Bu email zaten kayıtlı");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre zorunludur")
            .MinimumLength(8).WithMessage("Şifre en az 8 karakter olmalıdır")
            .Matches("[0-9]").WithMessage("Şifre en az bir rakam içermelidir")
            .Matches("[a-zA-Z]").WithMessage("Şifre en az bir harf içermelidir")
            .Matches("[^a-zA-Z0-9]").WithMessage("Şifre en az bir özel karakter içermelidir");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("Şifreler eşleşmiyor");
    }
}
