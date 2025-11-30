using Bil372Project.EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bil372Project.DataAccessLayer.Configurations;

public class UserGoalConfiguration : IEntityTypeConfiguration<UserGoal>
{
    public void Configure(EntityTypeBuilder<UserGoal> builder)
    {
        builder.ToTable("UserGoals");

        builder.HasKey(g => g.Id);

        builder.Property(g => g.TargetWeightKg)
            .HasColumnType("double");

        builder.Property(g => g.MotivationNote)
            .HasMaxLength(1000);

        builder.Property(g => g.UpdatedAt)
            .IsRequired();

        builder.HasOne(g => g.User)
            .WithMany()
            .HasForeignKey(g => g.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}