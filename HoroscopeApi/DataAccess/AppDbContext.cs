using HoroscopeApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace HoroscopeApi.DataAccess;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<HoroscopeQuery> HoroscopeQueries => Set<HoroscopeQuery>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(u => u.Id);

            entity.Property(u => u.Username)
                  .IsRequired()
                  .HasMaxLength(50);

            entity.HasIndex(u => u.Username)
                  .IsUnique();

            entity.Property(u => u.Email)
                  .IsRequired()
                  .HasMaxLength(150);

            entity.Property(u => u.PasswordHash)
                  .IsRequired();

            entity.HasMany(u => u.Queries)
                  .WithOne(q => q.User)
                  .HasForeignKey(q => q.UserId)
                  .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<HoroscopeQuery>(entity =>
        {
            entity.HasKey(q => q.Id);

            entity.Property(q => q.Sign)
                  .IsRequired()
                  .HasMaxLength(20);

            entity.Property(q => q.ResultText)
                  .IsRequired();

            entity.HasIndex(q => q.Sign);
        });
    }
}
