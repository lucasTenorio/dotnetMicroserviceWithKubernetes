using Microsoft.EntityFrameworkCore;
using CommandService.Models;

namespace CommandService.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> opt) : base(opt)
        {
            
        }
        public DbSet<Platform> Platforms => Set<Platform>();
        public DbSet<Command> Commands => Set<Command>();

        protected override void OnModelCreating(ModelBuilder modelBuilder){
            modelBuilder.Entity<Platform>()
                .HasMany(p => p.Commands)
                .WithOne(p => p.Platform!)
                .HasForeignKey(p => p.PlatformId);
            
            modelBuilder.Entity<Command>()
                .HasOne(c => c.Platform)
                .WithMany(c => c.Commands)
                .HasForeignKey(c => c.PlatformId);
        }
    }
}