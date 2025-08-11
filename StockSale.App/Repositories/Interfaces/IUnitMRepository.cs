using StockSale.App.Models.Domain;

namespace StockSale.App.Repositories.Interfaces
{
    public interface IUnitMRepository
    {
        Task<IEnumerable<UnitM>> GetAllAsync();
        Task<UnitM?> GetAsync(Guid id); // Opcional si necesitas obtener un solo elemento
        Task AddAsync(UnitM unitM); // Para agregar una nueva unidad
        Task UpdateAsync(UnitM unitM); // Para actualizar
        Task DeleteAsync(Guid id); // Para eliminar
    }
}
