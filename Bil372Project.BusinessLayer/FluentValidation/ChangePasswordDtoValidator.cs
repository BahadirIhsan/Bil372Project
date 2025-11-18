using System;
using Bil372Project.BusinessLayer.Dtos;
using Bil372Project.DataAccessLayer;
using FluentValidation;
using Microsoft.EntityFrameworkCore;

namespace Bil372Project.BusinessLayer.FluentValidation;

public class ChangePasswordDtoValidator : AbstractValidator<ChangePasswordDto>
{
    private readonly AppDbContext _db;

    public ChangePasswordDtoValidator(AppDbContext db)
    {
        _db = db;

        RuleFor(x => x.CurrentPassword)
            .NotEmpty().WithMessage("Mevcut şifre zorunludur")
            .MustAsync(async (dto, currentPass, cancellationToken) =>
            {
                var user = await _db.Users.FindAsync(new object?[] { dto.UserId }, cancellationToken);
                if (user == null) return false;
                return user.Password == currentPass; // SENDE HASH VARSA VERIFY KULLAN
            })
            .WithMessage("Mevcut şifre yanlış.");

        RuleFor(x => x.NewPassword)
            .NotEmpty().WithMessage("Yeni şifre zorunludur")
            .MinimumLength(8).WithMessage("Yeni şifre en az 8 karakter olmalıdır")
            .Matches("[0-9]").WithMessage("Yeni şifre en az bir rakam içermelidir")
            .Matches("[a-zA-Z]").WithMessage("Yeni şifre en az bir harf içermelidir")
            .Matches("[^a-zA-Z0-9]").WithMessage("Yeni şifre en az bir özel karakter içermelidir")
            .Must((dto, newPassword) => !string.Equals(newPassword, dto.CurrentPassword, StringComparison.Ordinal))
            .WithMessage("Yeni şifre mevcut şifre ile aynı olamaz");

        RuleFor(x => x.ConfirmNewPassword)
            .Equal(x => x.NewPassword)
            .WithMessage("Yeni şifreler uyuşmuyor.");
    }
}