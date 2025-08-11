using StockSale.App.Models.Domain;

namespace StockSale.App.Repositories.Interfaces
{
    public interface ITurnRepository
    {
        Task<IEnumerable<Turn>> GetAllAsync();
        Task<Turn?> GetAsync(Guid id);
        Task<Turn> AddAsync(Turn turn);
        Task<Turn?> UpdateAsync(Turn turn);
        Task<Turn?> DeleteAsync(Guid id);
        Task<Turn?> CloseTurnAsync();
        Task<Turn?> GetActualTurn();
        Task<Turn?> LastAdded();
        Task<IEnumerable<CashFlow>?> GetAllCashFlows();
        Task<CashFlow?> GetCashFlowById(Guid id);
        Task<CashFlow> AddCashFlow(CashFlow cashFlow);
        Task<IEnumerable<CashFlow>?> GetCashFlowByDate(DateTime date);
    }
}
