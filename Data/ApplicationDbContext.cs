using Microsoft.EntityFrameworkCore;
using OrgkSetra.Models;
namespace OrgkSetra.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }
        public DbSet<Customer> Customers { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Customer>(entity => { entity.HasKey(k => k.CustomerId); });
        }
    }
}
