using Microsoft.EntityFrameworkCore;
using StockSale.App.Models.Domain;
using StockSale.App.Data;
using StockSale.App.Repositories.Interfaces; // Asegúrate de que esto apunta al contexto correcto

namespace StockSale.App.Repositories
{
    public class UnitMRepository : IUnitMRepository
    {
        private readonly StockSaleDbContext stockSaleDbContext; // Tu DbContext, el nombre puede variar

        public UnitMRepository(StockSaleDbContext stockSaleDbContext)
        {
            this.stockSaleDbContext = stockSaleDbContext;
        }

        // Obtener todas las unidades de medida
        public async Task<IEnumerable<UnitM>> GetAllAsync()
        {
            return await stockSaleDbContext.UnitsM
                .OrderBy(x => x.Name)
                .ToListAsync(); // Asegúrate de que DbSet<UnitM> esté en el contexto
        }

        // Obtener una unidad de medida por su ID (opcional)
        public async Task<UnitM?> GetAsync(Guid id)
        {
            return await stockSaleDbContext.UnitsM.FindAsync(id);
        }

        // Añadir una nueva unidad de medida
        public async Task AddAsync(UnitM unitM)
        {
            stockSaleDbContext.UnitsM.Add(unitM);
            await stockSaleDbContext.SaveChangesAsync();
        }

        // Actualizar una unidad de medida existente
        public async Task UpdateAsync(UnitM unitM)
        {
            stockSaleDbContext.UnitsM.Update(unitM);
            await stockSaleDbContext.SaveChangesAsync();
        }

        // Eliminar una unidad de medida por su ID
        public async Task DeleteAsync(Guid id)
        {
            var unitM = await stockSaleDbContext.UnitsM.FindAsync(id);
            if (unitM != null)
            {
                stockSaleDbContext.UnitsM.Remove(unitM);
                await stockSaleDbContext.SaveChangesAsync();
            }
        }
    }
}
