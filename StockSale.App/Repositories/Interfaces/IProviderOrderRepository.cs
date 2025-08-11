using StockSale.App.Models.Domain;

namespace StockSale.App.Repositories.Interfaces
{
    public interface IProviderOrderRepository
    {
        Task<IEnumerable<ProviderOrder>> GetAllAsync();
        Task<ProviderOrder?> GetAsync(Guid id);
        Task<ProviderOrder> AddAsync(ProviderOrder providerOrder);
        Task<ProviderOrder?> UpdateAsync(ProviderOrder providerOrder);
        Task<ProviderOrder?> DeleteAsync(Guid id);
        Task<ProviderOrder?> GetIsActiveOrder(Guid providerId);
        Task<ProviderOrder?> CloseProviderOrder(Guid providerId);
        Task<List<ProviderOrder>> GetOrdersByProviderIdAsync(Guid providerId);
        Task<ProviderOrder?> LastAdded();
    }
}
