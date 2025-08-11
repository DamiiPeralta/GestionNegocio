using StockSale.App.Models.Domain;

namespace StockSale.App.Models.ViewModels
{
    public class CheckProviderOrderViewModel
    {
        public List<Provider> Providers { get; set; } 
        public Guid ProviderId { get; set; }
        public Guid ProviderOrderId { get; set; }
    }
}
