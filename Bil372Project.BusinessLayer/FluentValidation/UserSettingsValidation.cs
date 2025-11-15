using Bil372Project.BusinessLayer.Dtos;
using Bil372Project.DataAccessLayer;
using FluentValidation;

namespace Bil372Project.BusinessLayer.FluentValidation;

public class UserSettingsValidation : AbstractValidator<RegisterUserDto>
{
    private readonly AppDbContext _db;
    
    public UserSettingsValidation(AppDbContext db)
    {
        _db = db;
        
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email zorunludur")
            .EmailAddress().WithMessage("Geçerli bir email formatı değil")
            .Must(email => !_db.Users.Any(u => u.Email == email))
            .WithMessage("Bu email zaten kayıtlı");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Şifre zorunludur")
            .MinimumLength(8).WithMessage("En az 8 karakter olmalıdır")
            .Matches("[0-9]").WithMessage("En az bir rakam içermelidir")
            .Matches("[a-zA-Z]").WithMessage("En az bir harf içermelidir")
            .Matches("[^a-zA-Z0-9]").WithMessage("En az bir özel karakter içermelidir");

        RuleFor(x => x.ConfirmPassword)
            .Equal(x => x.Password)
            .WithMessage("Şifreler eşleşmiyor");
    }
}