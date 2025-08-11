using StockSale.App.Models.Domain;

namespace StockSale.App.Repositories.Interfaces
{
    public interface IProductPriceRepository
    {
        Task<IEnumerable<Product_Price>> GetAllAsync();
        Task<Product_Price?> GetByIdAsync(Guid id);
        Task<Product_Price> AddAsync(Product_Price productPrice);
        Task<Product_Price?> UpdateAsync(Product_Price productPrice);
        Task<Product_Price?> DeleteAsync(Guid id);
        Task<bool> ExistsAsync(Guid id);
    }
}
