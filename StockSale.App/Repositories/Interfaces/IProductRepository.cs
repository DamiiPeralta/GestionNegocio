using StockSale.App.Models.Domain;

namespace StockSale.App.Repositories.Interfaces
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product?> GetAsync(Guid id);
        Task<Product> AddAsync(Product product);
        Task<Product?> UpdateAsync(Product product);
        Task<Product?> DeleteAsync(Guid id);
        Task<IEnumerable<Product>> HasStockLimit();
        Task<Product?> LastAdded();
    }
}
