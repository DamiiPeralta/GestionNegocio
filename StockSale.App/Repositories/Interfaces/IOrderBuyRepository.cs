using StockSale.App.Models.Domain;

namespace StockSale.App.Repositories.Interfaces
{
    public interface IOrderBuyRepository
    {
        Task<IEnumerable<OrderBuy>> GetAllAsync();
        Task<OrderBuy?> GetAsync(Guid id);
        Task<OrderBuy> AddAsync(OrderBuy orderBuy);
        Task<OrderBuy?> UpdateAsync(OrderBuy orderBuy);
        Task<OrderBuy?> DeleteAsync(Guid id);
        Task<OrderBuy?> GetOpenOrderAsync();
        Task<OrderBuy?> LastAdded();
    }
}
