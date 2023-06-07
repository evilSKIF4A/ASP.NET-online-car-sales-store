using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace AutoROFL.Models
{
    public class ARDbContext : IdentityDbContext<User>
    {
        public DbSet<Basket> Baskets { get; set; }
        public DbSet<Car> Cars { get; set; }
        public DbSet<PurchaseHistory> PurchaseHistories { get; set; }
        public DbSet<RemoteAccount> RemoteAccounts { get; set; }
        public ARDbContext(DbContextOptions<ARDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
