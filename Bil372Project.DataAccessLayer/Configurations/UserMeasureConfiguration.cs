using Bil372Project.EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bil372Project.DataAccessLayer.Configurations;

public class UserMeasureConfiguration : IEntityTypeConfiguration<UserMeasure>
{
    public void Configure(EntityTypeBuilder<UserMeasure> entity)
    {
        entity.ToTable("UserMeasures");

        entity.HasKey(m => m.Id);

        entity.Property(m => m.Gender)
            .HasMaxLength(20);

        entity.Property(m => m.Allergies)
            .HasMaxLength(500);

        entity.Property(m => m.Diseases)
            .HasMaxLength(500);

        entity.Property(m => m.DislikedFoods)
            .HasMaxLength(500);

        entity.Property(m => m.HeightCm)
            .HasColumnType("double");

        entity.Property(m => m.WeightKg)
            .HasColumnType("double");

        entity.Property(m => m.Bmi)
            .HasColumnType("double");

        entity.Property(m => m.UpdatedAt)
            .IsRequired();

        // User ilişki (1 User - 1 Measure)
        entity.HasOne(m => m.User)
            .WithMany()                 // 1 kullanıcının çok ölçümü
            .HasForeignKey(m => m.UserId)
            .OnDelete(DeleteBehavior.Cascade);

    }
}