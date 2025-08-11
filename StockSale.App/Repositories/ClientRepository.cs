using StockSale.App.Data;
using StockSale.App.Models.Domain;
using Microsoft.EntityFrameworkCore;
using StockSale.App.Repositories.Interfaces;

namespace StockSale.App.Repositories
{
    public class ClientRepository : IClientRepository
    {
        private readonly StockSaleDbContext stockSaleDbContext;

        public ClientRepository(StockSaleDbContext stockSaleDbContext)
        {
            this.stockSaleDbContext = stockSaleDbContext;
        }

        public async Task<Client> AddAsync(Client client)
        {
            await stockSaleDbContext.Clients.AddAsync(client);
            await stockSaleDbContext.SaveChangesAsync();
            return client;
        }

        public async Task<Client?> DeleteAsync(Guid id)
        {
            var existingClient = await stockSaleDbContext.Clients.FindAsync(id);
            if (existingClient != null)
            {
                existingClient.IsDeleted = true;
                await stockSaleDbContext.SaveChangesAsync();
                return existingClient;
            }
            return null;
        }

        public async Task<IEnumerable<Client>> GetAllAsync()
        {
            return await stockSaleDbContext.Clients
                .Where(x => x.IsDeleted == false)
                .OrderBy(x => x.NClient)
                .Include(x => x.OrderBuys)
                .Include(x => x.Turns)
                .ToListAsync();
        }

        public async Task<Client?> GetAsync(Guid id)
        {
            return await stockSaleDbContext.Clients
                .Include(x => x.OrderBuys)
                .Include(x => x.Turns)
                .Include(x => x.Debts)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Client?> UpdateAsync(Client client)
        {
            var existingClient = await stockSaleDbContext.Clients
                .FirstOrDefaultAsync(x => x.Id == client.Id);
            if (existingClient != null)
            {
                existingClient.Name = client.Name;
                existingClient.Email = client.Email;
                existingClient.Dni = client.Dni;
                existingClient.Phone = client.Phone;
                existingClient.Company = client.Company;
                existingClient.Cuil_t = client.Cuil_t;
                existingClient.Condition_Iva = client.Condition_Iva;
                existingClient.Discount = client.Discount;
                existingClient.Debts = client.Debts;

                await stockSaleDbContext.SaveChangesAsync();
                return existingClient;
            }

            return null;
        }
        public async Task<Client?> LastAdded()
        {
            if (!await stockSaleDbContext.Clients.AnyAsync())
            {
                return null;
            }
            var maxNum = await stockSaleDbContext.Clients
                .MaxAsync(x => x.NClient);

            return await stockSaleDbContext.Clients
                .Where(x => x.NClient == maxNum)
                .FirstOrDefaultAsync();
        }

        public async Task<Debt> AddDebt(Debt debt)
        {
            await stockSaleDbContext.Debts.AddAsync(debt);
            await stockSaleDbContext.SaveChangesAsync();
            return debt;
        }

        public async Task<IEnumerable<Debt>> GetDebtsByDate(DateTime date)
        {
            return await stockSaleDbContext.Debts
            .Where(d => d.Date.Date == date.Date)
            .ToListAsync();
        }
    }
}
