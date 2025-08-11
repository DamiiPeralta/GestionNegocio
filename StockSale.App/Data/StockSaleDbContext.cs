using Microsoft.EntityFrameworkCore;
using StockSale.App.Models.Domain;

namespace StockSale.App.Data
{
    public class StockSaleDbContext : DbContext
    {
        public StockSaleDbContext(DbContextOptions<StockSaleDbContext> options) : base(options)
        {
        }

        public DbSet<Client> Clients { get; set; }
        public DbSet<Debt> Debts { get; set; }
        public DbSet<Fiado> Fiados { get; set; }
        public DbSet<OrderBuy> OrderBuys { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<Product_Price> Products_Price { get; set; }
        public DbSet<Provider> Providers { get; set; }
        public DbSet<ProviderOrder> ProviderOrders { get; set; }
        public DbSet<Turn> Turns { get; set; }
        public DbSet<UnitM> UnitsM { get; set; }
        public DbSet<CashFlow> CashFlows { get; set; }
    }
}
