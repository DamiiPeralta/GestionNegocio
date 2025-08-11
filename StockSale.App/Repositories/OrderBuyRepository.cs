using StockSale.App.Data;
using StockSale.App.Models.Domain;
using Microsoft.EntityFrameworkCore;
using StockSale.App.Repositories.Interfaces;

namespace StockSale.App.Repositories
{
    public class OrderBuyRepository : IOrderBuyRepository
    {
        private readonly StockSaleDbContext stockSaleDbContext;

        public OrderBuyRepository(StockSaleDbContext stockSaleDbContext)
        {
            this.stockSaleDbContext = stockSaleDbContext;
        }
        public async Task<OrderBuy> AddAsync(OrderBuy orderBuy)
        {
            await stockSaleDbContext.OrderBuys.AddAsync(orderBuy);
            await stockSaleDbContext.SaveChangesAsync();
            return orderBuy;
        }

        public async Task<OrderBuy?> DeleteAsync(Guid id)
        {
            var existingOrder = await stockSaleDbContext.OrderBuys.FindAsync(id);
            if (existingOrder != null)
            {
                existingOrder.IsDeleted = true;
                await stockSaleDbContext.SaveChangesAsync();
                return existingOrder;
            }
            return null;
        }

        public async Task<IEnumerable<OrderBuy>> GetAllAsync()
        {
            return await stockSaleDbContext.OrderBuys
                .Where(t => !t.IsDeleted)
                .OrderBy(x => x.NOrderBuy)
                .Include(x => x.Client)
                .Include(x => x.Turn)
                .Include(x => x.Products_Prices)
                .ToListAsync();
        }

        public async Task<OrderBuy?> GetAsync(Guid id)
        {
            return await stockSaleDbContext.OrderBuys
                .Include(x => x.Client)
                .Include(x => x.Turn)
                .Include(x => x.Products_Prices)
                .ThenInclude(x => x.Product)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<OrderBuy?> UpdateAsync(OrderBuy orderBuy)
        {
            var existingOrderBuy = await stockSaleDbContext.OrderBuys
                .FirstOrDefaultAsync(x => x.Id == orderBuy.Id);
            if (existingOrderBuy != null)
            {
                existingOrderBuy.Date = orderBuy.Date;
                existingOrderBuy.Import = orderBuy.Import;
                existingOrderBuy.Discount = orderBuy.Discount;
                existingOrderBuy.TotalDiscount = orderBuy.TotalDiscount;
                existingOrderBuy.Client = orderBuy.Client;
                existingOrderBuy.Turn = orderBuy.Turn;

                await stockSaleDbContext.SaveChangesAsync();
                return existingOrderBuy;
            }

            return null;
        }

        public async Task<OrderBuy?> GetOpenOrderAsync()
        {
            return await stockSaleDbContext.OrderBuys
                .Include(x => x.Client)
                .Include(x => x.Turn)
                .Include(x => x.Products_Prices)
                .ThenInclude(x => x.Product)
                .Where(ob => !ob.IsClosed)
                .FirstOrDefaultAsync();
        }

        public async Task<OrderBuy?> LastAdded()
        {
            if (!await stockSaleDbContext.OrderBuys.AnyAsync())
            {
                return null;
            }
            var maxNum = await stockSaleDbContext.OrderBuys
                .MaxAsync(x => x.NOrderBuy);

            return await stockSaleDbContext.OrderBuys
                .Where(x => x.NOrderBuy == maxNum)
                .FirstOrDefaultAsync();
        }
    }
}
