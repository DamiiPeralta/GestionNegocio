using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockSale.App.Data;
using StockSale.App.Models.Domain;
using StockSale.App.Repositories.Interfaces;
using StockSale.App.Utils;

namespace StockSale.App.Repositories
{
    public class TurnRepository : ITurnRepository
    {
        private readonly StockSaleDbContext stockSaleDbContext;

        public TurnRepository(StockSaleDbContext stockSaleDbContext)
        {
            this.stockSaleDbContext = stockSaleDbContext;
        }
        public async Task<int> CountAsync(string? searchQuery)
        {
            var query = stockSaleDbContext.Turns
                .Where(x => x.IsDeleted == false)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchQuery))
            {
                if (DateTime.TryParse(searchQuery, out DateTime parsedDate))
                {
                    // Si la búsqueda es una fecha, comparamos solo las fechas (sin hora)
                    query = query.Where(x => x.Date.Date == parsedDate.Date);
                }
            }
            return await query.CountAsync();
        }
        public async Task<IEnumerable<Turn>> GetAllAsync()
        {
            return await stockSaleDbContext.Turns
                .Where(t => !t.IsDeleted)
                .OrderBy(x => x.NTurn)
                .Include(t => t.OrderBuys)
                .Select(t => new Turn
                {
                    Id = t.Id,
                    NTurn = t.NTurn,
                    Date = t.Date,
                    Is_Closed = t.Is_Closed,
                    Clients = t.Clients,
                    Initial_Cash = t.Initial_Cash,
                    Closing_Cash = t.Initial_Cash + t.OrderBuys.Sum(ob => ob.FinalTotal),
                    OrderBuys = t.OrderBuys
                })
                .ToListAsync();
        }

        public async Task<Turn?> GetAsync(Guid id)
        {
            var turn = await stockSaleDbContext.Turns
                .Include(t => t.OrderBuys)
                .ThenInclude(ob => ob.Products_Prices)
                .Include(t => t.OrderBuys)
                .Include(t => t.Clients)
                .ThenInclude(t => t.Debts)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (turn != null)
            {
                // Calcula el importe total de las órdenes asociadas al turno
                foreach (var orderBuy in turn.OrderBuys ?? Enumerable.Empty<OrderBuy>())
                {
                    orderBuy.Import = UtilsCode.CalculateImport(orderBuy.Products_Prices ?? Enumerable.Empty<Product_Price>(), pp => pp.PriceSell);
                }
            }

            return turn;
        }

        public async Task<Turn> AddAsync(Turn turn)
        {
            turn.Id = Guid.NewGuid(); // Generar un nuevo ID
            await stockSaleDbContext.Turns.AddAsync(turn);
            await stockSaleDbContext.SaveChangesAsync();
            return turn;
        }

        public async Task<Turn?> UpdateAsync(Turn turn)
        {
            var existingTurn = await stockSaleDbContext.Turns.FindAsync(turn.Id);
            if (existingTurn != null)
            {
                existingTurn.Initial_Cash = turn.Initial_Cash;
                existingTurn.Closing_Cash = turn.Closing_Cash;
                existingTurn.Date = turn.Date;
                existingTurn.OrderBuys = turn.OrderBuys;
                await stockSaleDbContext.SaveChangesAsync();
                return existingTurn;
            }
            return null;
        }

        public async Task<Turn?> DeleteAsync(Guid id)
        {
            var existingTurn = await stockSaleDbContext.Turns.FindAsync(id);
            if (existingTurn != null)
            {
                existingTurn.IsDeleted = true; // Marcar como eliminado
                await stockSaleDbContext.SaveChangesAsync();
                return existingTurn;
            }
            return null;
        }
        private float CalculateClosingCash(Turn turn)
        {
            // Calcular el total de las compras de los clientes
            float orderBuysTotal = turn.OrderBuys?.Sum(ob => ob.Import) ?? 0;

            // No restamos el total de las ProviderOrders, simplemente sumamos el cash inicial y las ventas
            return turn.Initial_Cash + orderBuysTotal;
        }
        public async Task<Turn?> CloseTurnAsync()
        {
            var turn = await GetActualTurn();
            if (turn != null)
            {
                turn.Closing_Cash = CalculateClosingCash(turn);
                turn.Is_Closed = true; // Marcamos el turno como cerrado
                await stockSaleDbContext.SaveChangesAsync();
                return turn;
            }
            return null;
        }

        public async Task<Turn?> GetActualTurn()
        {
            // Obtener el turno que no esté cerrado
            var turn = await stockSaleDbContext.Turns
                .Where(t => !t.Is_Closed)
                .Where(t => !t.IsDeleted)
                .Include(t => t.OrderBuys)
                .ThenInclude(ob => ob.Client)
                .Include(t => t.OrderBuys)
                .ThenInclude(ob => ob.Products_Prices)
                .FirstOrDefaultAsync();

            if (turn != null)
            {
            return turn;
            }
            return null;
}

        public async Task<Turn?> LastAdded()
        {
            if (!await stockSaleDbContext.Turns.AnyAsync())
            {
                return null;
            }
            var maxNum = await stockSaleDbContext.Turns
                .MaxAsync(x => x.NTurn);

            return await stockSaleDbContext.Turns
                .Where(x => x.NTurn == maxNum)
                .FirstOrDefaultAsync();
        }
        public async Task<IEnumerable<CashFlow>?> GetAllCashFlows()
        {
            return await stockSaleDbContext.CashFlows
            .ToListAsync();
        }
        public async Task<CashFlow?> GetCashFlowById(Guid id)
        {
            var cashFlow = await stockSaleDbContext.CashFlows
            .FirstOrDefaultAsync(c => c.Id == id);
            return cashFlow;
        }

        public async Task<CashFlow> AddCashFlow(CashFlow cashFlow)
        {
            await stockSaleDbContext.CashFlows.AddAsync(cashFlow);
            await stockSaleDbContext.SaveChangesAsync();
            return cashFlow;
        }

        public async Task<IEnumerable<CashFlow>?> GetCashFlowByDate(DateTime date)
        {
            return await stockSaleDbContext.CashFlows
            .Where(c => c.Date.Date == date.Date)
            .ToListAsync();
        }
    }
}
