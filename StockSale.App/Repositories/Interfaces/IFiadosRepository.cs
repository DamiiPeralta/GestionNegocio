using StockSale.App.Models.Domain;
using StockSale.App.Models.ViewModels;

namespace StockSale.App.Repositories.Interfaces
{
    public interface IFiadosRepository
    {
        Task<IEnumerable<Fiado>> GetAllAsync();
        Task<Fiado?> GetAsync(Guid id);
        Task<Fiado> AddAsync(Fiado fiado);
        Task<Fiado?> UpdateAsync(Fiado fiado);
        Task<Fiado?> DeleteAsync(Guid id);
        Task<Fiado?> LastAdded();
        Task<IEnumerable<Fiado>?> GetFiadosByDate(DateTime date);
    }
}
