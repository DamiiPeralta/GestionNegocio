using Microsoft.EntityFrameworkCore;
using StockSale.App.Data;
using StockSale.App.Models.Domain;
using StockSale.App.Models.ViewModels;
using StockSale.App.Repositories.Interfaces;

namespace StockSale.App.Repositories
{
    public class FiadosRepository : IFiadosRepository
    {
        private readonly StockSaleDbContext stockSaleDbContext;

        public FiadosRepository(StockSaleDbContext stockSaleDbContext)
        {
            this.stockSaleDbContext = stockSaleDbContext;
        }
        public async Task<Fiado> AddAsync(Fiado fiado)
        {
            await stockSaleDbContext.Fiados.AddAsync(fiado);
            await stockSaleDbContext.SaveChangesAsync();
            return fiado;
        }
        public async Task<Fiado?> DeleteAsync(Guid id)
        {
            var existingFiado = await stockSaleDbContext.Fiados.FindAsync(id);
            if (existingFiado != null)
            {
                stockSaleDbContext.Fiados.Remove(existingFiado);
                await stockSaleDbContext.SaveChangesAsync();
                return existingFiado;
            }
            return null;
        }

        public async Task<IEnumerable<Fiado>> GetAllAsync()
        {
            return await stockSaleDbContext.Fiados
                .OrderBy(x => x.NFiado)
                .ToListAsync();
        }

        public async Task<Fiado?> GetAsync(Guid id)
        {
            return await stockSaleDbContext.Fiados
            .FindAsync(id);
        }

        public async Task<Fiado?> UpdateAsync(Fiado fiado)
        {
            var existingFiado = await stockSaleDbContext.Fiados
                .FirstOrDefaultAsync(x => x.Id == fiado.Id);
            if (existingFiado != null)
            {
                existingFiado.Name = fiado.Name;
                existingFiado.Date = fiado.Date;
                existingFiado.Import = fiado.Import;
                existingFiado.IsPay = fiado.IsPay;
                existingFiado.LastDatePay = fiado.LastDatePay;
                await stockSaleDbContext.SaveChangesAsync();
                return existingFiado;
            }
            return null;
        }

        public async Task<Fiado?> LastAdded()
        {
            if (!await stockSaleDbContext.Fiados.AnyAsync())
            {
                return null;
            }
            var maxNum = await stockSaleDbContext.Fiados
                .MaxAsync(x => x.NFiado);

            return await stockSaleDbContext.Fiados
                .Where(x => x.NFiado == maxNum)
                .FirstOrDefaultAsync();
        }

        public async Task<IEnumerable<Fiado>?> GetFiadosByDate(DateTime date)
        {
            return await stockSaleDbContext.Fiados
            .Where(f => f.Date.Date == date.Date)
            .Where(f => f.LastDatePay.Date == date.Date)
            .ToListAsync();
        }
    }
}
