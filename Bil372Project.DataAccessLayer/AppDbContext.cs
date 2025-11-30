using Bil372Project.EntityLayer.Entities;
using Microsoft.EntityFrameworkCore;

namespace Bil372Project.DataAccessLayer;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<AppUser> Users { get; set; }
    public DbSet<UserMeasure> UserMeasures { get; set; }
    public DbSet<MeasurementForMl> MeasurementsForMl { get; set; }
    public DbSet<UserDietPlan> UserDietPlans { get; set; }         
    public DbSet<UserGoal> UserGoals { get; set; }
    public DbSet<AuditLog> AuditLogs { get; set; }


    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}