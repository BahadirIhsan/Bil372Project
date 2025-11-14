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
    public DbSet<UserMeasure> UserMeasures { get; set; }   // 🔹 BURAYI EKLE
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // AppUserConfiguration vs. hepsini otomatik uygular
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(AppDbContext).Assembly);

        base.OnModelCreating(modelBuilder);
    }
}