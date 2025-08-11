using StockSale.App.Models.Domain;

namespace StockSale.App.Models.ViewModels
{
    public class AddProviderOrderViewModel
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }

        //Relaciones
        public Guid ProviderId { get; set; }
   
        public IEnumerable<Provider?> Providers { get; set; }
    }
}
