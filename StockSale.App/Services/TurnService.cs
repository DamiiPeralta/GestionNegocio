using Microsoft.AspNetCore.Mvc;
using StockSale.App.Data;
using StockSale.App.Models.Domain;
using StockSale.App.Repositories;
using StockSale.App.Repositories.Interfaces;

namespace StockSale.App.Services
{
    public class TurnService
    {
        private readonly ITurnRepository _turnRepository;

        public TurnService(ITurnRepository turnRepository)
        {
            _turnRepository = turnRepository;
        }

        public async Task<Turn> StartTurnAsync(float initialCash)
        {
            var lastAdded = await _turnRepository.LastAdded();
            int N = 1;
            if (lastAdded != null)
            {
                N = lastAdded.NTurn + 1;
            }
            var turn = new Turn
            {
                NTurn = N,
                Date = DateTime.Now,
                Initial_Cash = initialCash,
                Closing_Cash = 0,
                Clients = new List<Client>(),
                OrderBuys = new List<OrderBuy>(),
            };

            return await _turnRepository.AddAsync(turn);
        }

        public async Task<Turn?> CloseTurnAsync()
        {
            return await _turnRepository.CloseTurnAsync();
        }
        public async Task<IEnumerable<Turn>> GetAllTurnsAsync()
        {
            return await _turnRepository.GetAllAsync();
        }
        public async Task<Turn?> GetActualTurn()
        {
            return await _turnRepository.GetActualTurn();
        }

        public async Task<Turn?> GetTurnAsync(Guid id)
        {
            return await _turnRepository.GetAsync(id);
        }
        public async Task<Turn?> DeleteAsync(Guid id)
        {
            return await _turnRepository.DeleteAsync(id);
        }
        public async Task<Turn?> UpdateAsync(Turn turn)
        {
            return await _turnRepository.UpdateAsync(turn);      
        }
        public async Task<Turn?> LastAdded()
        {
            return await _turnRepository.LastAdded();
        }
        public async Task<IEnumerable<CashFlow>?> GetAllCashFlows()
        {
            return await _turnRepository.GetAllCashFlows();
        }

        public async Task<CashFlow?> GetCashFlowById (Guid id)
        {
            return await _turnRepository.GetCashFlowById(id);
        }

        public async Task<bool> AddCashFlow(int import, bool isIncome, string? comment)
        {
            var cashFlow = new CashFlow
            {
                Date = DateTime.Now,
                Import = import,
                Income = isIncome,
                Comment = "Sin comentario"
            };
            if (comment != null)
            {
                cashFlow.Comment = comment;
            }
            var cashFlowAdd = await _turnRepository.AddCashFlow(cashFlow);
            if(cashFlowAdd != null)
            {
            return true;
            }
            return false;
        }

        public async Task<IEnumerable<CashFlow>?> GetCashFlowByDate(DateTime date)
        {
            return await _turnRepository.GetCashFlowByDate(date);
        }
    }
}