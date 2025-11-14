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

        entity.HasIndex(u => u.Email)
            .IsUnique();

        // İleride ilişkiler de burada:
        // entity.HasMany(u => u.DietPrograms)...
    }
}