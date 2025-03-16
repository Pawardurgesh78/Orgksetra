using Microsoft.EntityFrameworkCore;
using OrgkSetra.Models;
namespace OrgkSetra.Data
{
    public class CartDbContext : DbContext 
    {
        public CartDbContext(DbContextOptions<CartDbContext> options) : base(options) { }
        public DbSet<Cart_Session> Cart_Session {  get; set; }
        public DbSet<CartItem> CartItems { get;  set; }
        public DbSet<DeliveryAddress> DeliveryAddress { get; set; }
        public DbSet<Customer> Customers { get; set; }
        public DbSet<Orders> Orders { get; set; }   
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<CartItem>().HasOne(b => b.Session)
                                           .WithMany(b => b.cartItems)
                                           .HasForeignKey(b => b.SessionId);
        }
    }
    public class OrgksetraAdmin : DbContext
    {
        public OrgksetraAdmin(DbContextOptions<OrgksetraAdmin> options) : base(options) { }
        public DbSet<Item> Items { get; set; }
        public DbSet<Image> Images { get; set; }
    }
}
