using Microsoft.EntityFrameworkCore;
using LLM_Module.Data;

namespace LLM_Module.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Company> Companies { get; set; }
        public DbSet<Capability> Capabilities { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("stansharing");
            modelBuilder.Entity<Company>().ToTable("companies");
            modelBuilder.Entity<Capability>().ToTable("capabilities");
        }
    }
}
