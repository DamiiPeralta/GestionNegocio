using StockSale.App.Models.Domain;

namespace StockSale.App.Repositories.Interfaces
{
    public interface IClientRepository
    {
        Task<IEnumerable<Client>> GetAllAsync();
        Task<Client?> GetAsync(Guid id);
        Task<Client> AddAsync(Client client);
        Task<Client?> UpdateAsync(Client client);
        Task<Client?> DeleteAsync(Guid id);
        Task<Client?> LastAdded();
        Task<Debt> AddDebt(Debt debt);
        Task<IEnumerable<Debt>> GetDebtsByDate(DateTime date);
    }
}
