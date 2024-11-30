
using ApplicationCore.Books;
using ApplicationCore.Documents;
using ApplicationCore.Entities.Books;
using ApplicationCore.Registries;
using ApplicationUsers;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityFramework
{
    public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
    {

        public DbSet<Contragent> Contragents { get; set; } = null!;
        public DbSet<CargoType> CargoTypes { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<Invoice> Invoices { get; set; } = null!;
        public DbSet<Numerator> Numerators { get; set; } = null!;
        public DbSet<LogisticPrice> LogisticPrices { get; set; } = null!;
        public DbSet<DeliveryContract> DeliveryContracts { get; set; } = null!;
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
    }
}
