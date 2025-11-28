using Bil372Project.EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bil372Project.DataAccessLayer.Configurations;

public class UserDietPlanConfiguration : IEntityTypeConfiguration<UserDietPlan>
{
    public void Configure(EntityTypeBuilder<UserDietPlan> entity)
    {
        entity.ToTable("UserDietPlans");

        entity.HasKey(p => p.Id);

        entity.Property(p => p.Breakfast)
            .HasMaxLength(2000);

        entity.Property(p => p.Lunch)
            .HasMaxLength(2000);

        entity.Property(p => p.Dinner)
            .HasMaxLength(2000);

        entity.Property(p => p.Snack)
            .HasMaxLength(2000);

        entity.Property(p => p.GeneratedAt)
            .IsRequired();

        entity.Property(p => p.ModelVersion)
            .HasMaxLength(100);

        // 1 UserMeasure -> n UserDietPlan
        entity.HasOne(p => p.UserMeasure)
            .WithMany(m => m.DietPlans)
            .HasForeignKey(p => p.UserMeasureId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}