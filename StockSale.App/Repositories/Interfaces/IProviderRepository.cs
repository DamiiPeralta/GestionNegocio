using StockSale.App.Models.Domain;

namespace StockSale.App.Repositories.Interfaces
{
    public interface IProviderRepository
    {
        Task<IEnumerable<Provider>> GetAllAsync();
        Task<Provider?> GetAsync(Guid id);
        Task<Provider> AddAsync(Provider provider);
        Task<int> CountAsync(string? searchQuery);
        Task<Provider?> UpdateAsync(Provider provider);
        Task<Provider?> DeleteAsync(Guid id);
        Task<IEnumerable<Provider>> GetAllPaginatedAsync(string? searchQuery, string? sortBy, string? sortDirection, int pageNumber, int pageSize);
        Task<Provider?> LastAdded();
    }
}
