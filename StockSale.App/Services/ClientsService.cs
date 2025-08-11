using StockSale.App.Models.Domain;
using StockSale.App.Repositories.Interfaces;

namespace StockSale.App.Services
{
    public class ClientService
    {
        private readonly IClientRepository clientRepository;

        public ClientService(IClientRepository clientRepository)
        {
            this.clientRepository = clientRepository;
        }

        public async Task<Client> AddAsync(Client client)
        {
            return await clientRepository.AddAsync(client);
        }

        public async Task<Client?> DeleteClient(Guid id)
        {
            var client = await clientRepository.GetAsync(id);
            return await clientRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Client>> GetAll()
        {
            return await clientRepository.GetAllAsync();
        }

        public async Task<Client?> GetAsync(Guid id)
        {
            var client = await clientRepository.GetAsync(id);
            return client;
        }

        public async Task<Client?> UpdateClient(Client client)
        {
            var existingClient = await clientRepository.GetAsync(client.Id);
            return await clientRepository.UpdateAsync(client);
        }
        public async Task<Client?> LastAdded()
        {
            return await clientRepository.LastAdded();
        }

        public async Task<bool> AddDebt(Guid id, bool isDebt, int import)
        {
            var client = await clientRepository.GetAsync(id);
            if(client != null)
            {
                var newDebt = new Debt
                {
                    Client = client,
                    Date = DateTime.Now,
                    Import = import,
                    Status = isDebt
                };
                var newDebtAdded = await clientRepository.AddDebt(newDebt);
                if (newDebtAdded != null)
                {
                    return true;
                }
            }
            return false;
        }

        public async Task<IEnumerable<Debt>> GetDebtsByDate(DateTime date)
        {
            return await clientRepository.GetDebtsByDate(date);
        }
    }
}
