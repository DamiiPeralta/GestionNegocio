using StockSale.App.Data;
using StockSale.App.Models.Domain;
using Microsoft.EntityFrameworkCore;
using StockSale.App.Services;
using StockSale.App.Repositories.Interfaces;

namespace StockSale.App.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly StockSaleDbContext stockSaleDbContext;
        private readonly IProviderRepository providerRepository;
        private readonly IUnitMRepository unitMRepository;

        public ProductRepository(StockSaleDbContext stockSaleDbContext,IProviderRepository providerRepository ,IUnitMRepository unitMRepository)
        {
            this.stockSaleDbContext = stockSaleDbContext;
            this.providerRepository = providerRepository;
            this.unitMRepository= unitMRepository;
        }

        // Añadir un nuevo producto
        public async Task<Product> AddAsync(Product product)
        {
            await stockSaleDbContext.Products.AddAsync(product);
            await stockSaleDbContext.SaveChangesAsync();
            return product;
        }

        // Eliminar un producto por su ID
        public async Task<Product?> DeleteAsync(Guid id)
        {
            var existingProduct = await stockSaleDbContext.Products.FindAsync(id);
            if (existingProduct != null)
            {
                existingProduct.IsDeleted = true;
                await stockSaleDbContext.SaveChangesAsync();
                return existingProduct;
            }
            return null;
        }

        // Obtener todos los productos
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await stockSaleDbContext.Products
                .Where(x => x.IsDeleted == false)
                .OrderBy(x => x.NProduct)
                .Include(p => p.Provider)   // Incluye la relación con Provider
                .Include(p => p.UnitM)      // Incluye la relación con UnitM
                .ToListAsync();
        }

        // Obtener un producto por su ID
        public async Task<Product?> GetAsync(Guid id)
        {
            var product = await stockSaleDbContext.Products
                .Include(p => p.Provider)   // Incluye la relación con Provider
                .Include(p => p.UnitM)      // Incluye la relación con UnitM
                .FirstOrDefaultAsync(p => p.Id == id);  // Cambia FindAsync por FirstOrDefaultAsync para poder incluir relaciones

            return product;
        }

        // Actualizar un producto existente
        public async Task<Product?> UpdateAsync(Product product)
        {
            var existingProduct = await GetAsync(product.Id);

            if (existingProduct != null)
            {
                existingProduct.Name = product.Name;
                existingProduct.Packaging = product.Packaging;
                existingProduct.Stock = product.Stock;
                existingProduct.Stock_Limit = product.Stock_Limit;
                existingProduct.List_Price = product.List_Price;
                existingProduct.Sell_Price = product.Sell_Price;

                if (product.Provider != null)
                {
                    existingProduct.Provider = await providerRepository.GetAsync(product.Provider.Id);
                }
                if (product.UnitM != null)
                {
                    existingProduct.UnitM = await unitMRepository.GetAsync(product.UnitM.Id);
                }

                await stockSaleDbContext.SaveChangesAsync();
                return existingProduct;
            }

            return null;
        }
        public async Task<Product?> LastAdded()
        {
            if (!await stockSaleDbContext.Products.AnyAsync())
            {
                return null;
            }
            var maxNum = await stockSaleDbContext.Products
                .MaxAsync(x => x.NProduct);

            return await stockSaleDbContext.Products
                .Where(x => x.NProduct == maxNum)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Product>> HasStockLimit()
        {
            var products = await stockSaleDbContext.Products
                .Where(x => x.Stock <= x.Stock_Limit)
                .Include(p => p.Provider)
                .Include(p => p.UnitM)
                .ToListAsync();
            return products;
        }
    }
}
