using Microsoft.EntityFrameworkCore;
using StockSale.App.Data;
using StockSale.App.Models.Domain;
using StockSale.App.Repositories.Interfaces;
using StockSale.App.Services;
using StockSale.App.Utils;

namespace StockSale.App.Repositories
{
    public class ProviderOrderRepository : IProviderOrderRepository
    {
        private readonly StockSaleDbContext stockSaleDbContext;
        private readonly ProductService productService;

        public ProviderOrderRepository(StockSaleDbContext stockSaleDbContext, ProductService productService)
        {
            this.stockSaleDbContext=stockSaleDbContext;
            this.productService = productService;
        }
        public async Task<ProviderOrder> AddAsync(ProviderOrder providerOrder)
        {
            await stockSaleDbContext.ProviderOrders.AddAsync(providerOrder);
            await stockSaleDbContext.SaveChangesAsync();
            return providerOrder;
        }

        public async Task<ProviderOrder?> DeleteAsync(Guid id)
        {
            var existingProviderOrder = await stockSaleDbContext.ProviderOrders.FindAsync(id);
            if(existingProviderOrder != null)
            {
                existingProviderOrder.IsDeleted = true;
                await stockSaleDbContext.SaveChangesAsync();
                return existingProviderOrder;
            }
            return null;
        }

        public async Task<IEnumerable<ProviderOrder>> GetAllAsync()
        {
            return await stockSaleDbContext.ProviderOrders
                .Where(x => x.IsDeleted == false)
                .OrderBy(x => x.NProviderOrder)
                .Include(x => x.Provider)
                .Include(x => x.Products_Prices)
                .ToListAsync();
        }

        public async Task<ProviderOrder?> GetAsync(Guid id)
        {
            var providerOrder = await stockSaleDbContext.ProviderOrders
                    .Where(x => x.IsDeleted == false)
                    .Include(x => x.Provider)
                    .Include(po => po.Products_Prices)
                    .ThenInclude(pp => pp.Product)
                    .FirstOrDefaultAsync(x => x.Id == id);

            if (providerOrder != null)
            {
                // Calcula el importe para la orden específica
                providerOrder.Import = UtilsCode.CalculateImport(providerOrder.Products_Prices ?? Enumerable.Empty<Product_Price>(), pp => pp.PriceList);
            }

            return providerOrder;
        }

        public async Task<ProviderOrder?> UpdateAsync(ProviderOrder providerOrder)
        {
            var existingProviderOrder = await stockSaleDbContext.ProviderOrders
                .FirstOrDefaultAsync(x => x.Id == providerOrder.Id);
            if(existingProviderOrder != null)
            {
                existingProviderOrder.Date = providerOrder.Date;
                existingProviderOrder.Import = providerOrder.Import;

                await stockSaleDbContext.SaveChangesAsync();
                return existingProviderOrder;
            }
            return null;
        }

        public async Task<ProviderOrder?> GetIsActiveOrder(Guid id)
        {
            // Busca la orden de proveedor activa para un proveedor específico
            return await stockSaleDbContext.ProviderOrders
                .Include(x => x.Provider)
                .Include(x => x.Products_Prices)
                .ThenInclude(x => x.Product.UnitM)
                .FirstOrDefaultAsync(x => x.Provider.Id == id && !x.IsClosed);
        }

        public async Task<ProviderOrder?> CloseProviderOrder(Guid providerId)
        {
            var actualOrder = await GetIsActiveOrder(providerId);
            if (actualOrder != null)
            {
                var isStock = await productService.AddStockFromOrder(actualOrder.Products_Prices.ToList());
                actualOrder.IsClosed = true;
                var updatedOrder = await UpdateAsync(actualOrder);
                if (updatedOrder != null)
                {
                    return actualOrder;
                }
            }
            return null;
        }
        public async Task<List<ProviderOrder>> GetOrdersByProviderIdAsync(Guid providerId)
        {
            var providerOrders = await stockSaleDbContext.ProviderOrders
                    .Where(x => x.IsDeleted == false)
                    .OrderBy(x => x.NProviderOrder)
                    .Include(po => po.Provider)
                    .Include(po => po.Products_Prices) // Incluimos los precios relacionados
                    .Where(po => po.Provider.Id == providerId)
                    .ToListAsync();

            // Calculamos el importe para cada orden
            foreach (var order in providerOrders)
            {
                order.Import = UtilsCode.CalculateImport(order.Products_Prices, pp => pp.PriceList);
            }

            return providerOrders;
        }

        public async Task<ProviderOrder?> LastAdded()
        {
            if (!await stockSaleDbContext.ProviderOrders.AnyAsync())
            {
                return null;
            }
            var maxNum = await stockSaleDbContext.ProviderOrders
                .MaxAsync(x => x.NProviderOrder);

            return await stockSaleDbContext.ProviderOrders
                .Where(x => x.NProviderOrder == maxNum)
                .FirstOrDefaultAsync();
        }
    }
}
