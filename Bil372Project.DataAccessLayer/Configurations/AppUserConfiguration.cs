using Bil372Project.EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bil372Project.DataAccessLayer.Configurations;

public class AppUserConfiguration : IEntityTypeConfiguration<AppUser>
{
    public void Configure(EntityTypeBuilder<AppUser> entity)
    {
        entity.ToTable("Users");
        
        entity.HasKey(u => u.Id);

        entity.Property(u => u.FullName)
            .HasMaxLength(100)
            .IsRequired();
    
        entity.Property(u => u.Email)
            .HasMaxLength(150)
            .IsRequired();

        entity.Property(u => u.Password)
            .HasMaxLength(200)
            .IsRequired();
        
        entity.Property(u => u.IsAdmin)
            .IsRequired()
            .HasDefaultValue(false);

        entity.HasIndex(u => u.Email)
            .IsUnique();

        entity.Property(u => u.PhoneNumber)
            .HasMaxLength(20);

        entity.Property(u => u.City)
            .HasMaxLength(100);

        entity.Property(u => u.Country)
            .HasMaxLength(100);

        entity.Property(u => u.Bio)
            .HasMaxLength(500);
        
        entity.Property(u => u.UpdatedAt)
            .IsConcurrencyToken();
        
        // 1 User -> n UserMeasure
        entity.HasMany(u => u.UserMeasures)
            .WithOne(m => m.User)
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}