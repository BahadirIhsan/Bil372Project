using Bil372Project.BusinessLayer.Dtos;
using Bil372Project.DataAccessLayer;
using FluentValidation;

namespace Bil372Project.BusinessLayer.FluentValidation;

public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    private readonly AppDbContext _db;

    public ChangePasswordDtoValidator(AppDbContext db)
    {
        _db = db;

        RuleFor(x => x.CurrentPassword)
            .NotEmpty()
            .Must((dto, currentPass) =>
            {
                var user = _db.Users.FirstOrDefault(u => u.Id == dto.UserId);
                if (user == null) return false;
                return user.Password == currentPass; // SENDE HASH VARSA VERIFY KULLAN
            })
            .WithMessage("Mevcut şifre yanlış.");

        RuleFor(x => x.NewPassword)
            .NotEmpty()
            .MinimumLength(8)
            .Matches("[0-9]")
            .Matches("[a-zA-Z]")
            .Matches("[^a-zA-Z0-9]");

        RuleFor(x => x.ConfirmNewPassword)
            .Equal(x => x.NewPassword)
            .WithMessage("Yeni şifreler uyuşmuyor.");
    }
}