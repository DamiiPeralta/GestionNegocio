using Microsoft.EntityFrameworkCore;
using StockSale.App.Data;
using StockSale.App.Models.Domain;
using StockSale.App.Repositories.Interfaces;

namespace StockSale.App.Repositories
{
    public class ProductPriceRepository : IProductPriceRepository
    {
        private readonly StockSaleDbContext stockSaleDbContext;

        public ProductPriceRepository(StockSaleDbContext stockSaleDbContext)
        {
            this.stockSaleDbContext = stockSaleDbContext;
        }

        public async Task<IEnumerable<Product_Price>> GetAllAsync()
        {
            return await stockSaleDbContext.Products_Price.Include(p => p.Product)
                .ThenInclude(p => p.UnitM)
                .OrderBy(p => p.Product.NProduct)
                .ToListAsync();
        }

        public async Task<Product_Price?> GetByIdAsync(Guid id)
        {
            return await stockSaleDbContext.Products_Price.Include(p => p.Product)
                .ThenInclude(p => p.UnitM)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<Product_Price> AddAsync(Product_Price productPrice)
        {
            productPrice.Id = Guid.NewGuid(); // Generar nuevo Id
            await stockSaleDbContext.Products_Price.AddAsync(productPrice);
            await stockSaleDbContext.SaveChangesAsync();
            return productPrice; 
        }

        public async Task<Product_Price?> UpdateAsync(Product_Price productPrice)
        {
            stockSaleDbContext.Products_Price.Update(productPrice);
            await stockSaleDbContext.SaveChangesAsync();
            return productPrice;
        }

        public async Task<Product_Price?> DeleteAsync(Guid id)
        {
            var productPrice = await stockSaleDbContext.Products_Price.FindAsync(id);
            if (productPrice != null)
            {
                stockSaleDbContext.Products_Price.Remove(productPrice);
                await stockSaleDbContext.SaveChangesAsync();
            }
            return productPrice;
        }

        public async Task<bool> ExistsAsync(Guid id)
        {
            return await stockSaleDbContext.Products_Price.AnyAsync(p => p.Id == id);
        }
    }
}
