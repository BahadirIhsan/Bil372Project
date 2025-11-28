using Bil372Project.EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Bil372Project.DataAccessLayer.Configurations;

public class MeasurementForMlConfiguration : IEntityTypeConfiguration<MeasurementForMl>
{
    public void Configure(EntityTypeBuilder<MeasurementForMl> entity)
    {
        entity.ToTable("MeasurementsForMl");

        entity.HasKey(m => m.Id);

        entity.Property(m => m.Bmi)
            .HasColumnType("double");

        entity.Property(m => m.DailyCalorieTarget)
            .HasColumnType("double");

        entity.Property(m => m.ProteinGrams)
            .HasColumnType("double");

        entity.Property(m => m.FatGrams)
            .HasColumnType("double");

        entity.Property(m => m.SugarGrams)
            .HasColumnType("double");

        entity.Property(m => m.SodiumMg)
            .HasColumnType("double");

        entity.Property(m => m.CalculatedAt)
            .IsRequired();

        // 1-1: UserMeasure <-> MeasurementForMl
        entity.HasOne(m => m.UserMeasure)
            .WithOne(um => um.MeasurementForMl)
            .HasForeignKey<MeasurementForMl>(m => m.UserMeasureId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}