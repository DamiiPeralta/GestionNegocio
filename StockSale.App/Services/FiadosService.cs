using StockSale.App.Models.Domain;
using StockSale.App.Models.ViewModels;
using StockSale.App.Repositories;
using StockSale.App.Repositories.Interfaces;

namespace StockSale.App.Services
{
    public class FiadosService
    {
        private readonly IFiadosRepository fiadosRepository;

        public FiadosService(IFiadosRepository fiadosRepository)
        {
            this.fiadosRepository = fiadosRepository;
        }
        
    public async Task<Fiado> AddAsync(Fiado fiado)
    {

            return await fiadosRepository.AddAsync(fiado);
    }
    public async Task<IEnumerable<Fiado>> GetAllAsync()
        {
            return await fiadosRepository.GetAllAsync();
        }

    public async Task<Fiado?> GetAsync(Guid id)
        {
            return await fiadosRepository.GetAsync(id);
        }
        public async Task<Fiado?> DeleteAsync(Guid id)
        {
            return await fiadosRepository.DeleteAsync(id);
        }
    public async Task<Fiado?> LastAdded()
        {
            return await fiadosRepository.LastAdded();
        }
        public async Task<Fiado?> UpdateAsync(Fiado fiado)
        {
            return await fiadosRepository.UpdateAsync(fiado);
        }

        public async Task<IEnumerable<Fiado>?> GetFiadosByDate(DateTime date)
        {
            return await fiadosRepository.GetFiadosByDate(date);
        }

        public async Task<Fiado?> FiadosPay(Guid id)
        {
            var fiado = await fiadosRepository.GetAsync(id);
            if(fiado != null)
            {
                var newFiado = new Fiado()
                {
                    NFiado = fiado.NFiado,
                    Id = fiado.Id,
                    Name = fiado.Name,
                    Date = fiado.Date,
                    Import = fiado.Import,
                    IsPay = true
                };
                var updatedFiado = await fiadosRepository.UpdateAsync(newFiado);
                return updatedFiado;
            }
            return null;
        }
    }
}
