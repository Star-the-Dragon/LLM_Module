using Microsoft.EntityFrameworkCore;
using LLM_Module.Data;

namespace LLM_Module.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
        public DbSet<Company> Company { get; set; }
        public DbSet<Capability> Capabilities { get; set; }
        public DbSet<Support> Support { get; set; }
        public DbSet<Order> Orders { get; set; }
        public DbSet<Customer> Customer { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasDefaultSchema("public");
            modelBuilder.Entity<Company>().ToTable("companies");
            modelBuilder.Entity<Capability>().ToTable("capabilities");
        }
    }
}
