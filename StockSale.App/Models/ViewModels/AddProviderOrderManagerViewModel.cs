using StockSale.App.Models.Domain;
using System.Collections;

namespace StockSale.App.Models.ViewModels
{
    public class AddProviderOrderManagerViewModel
    {
        public IEnumerable<Provider>? Providers { get; set; }
        public Guid? ProviderId { get; set; }
        public IEnumerable<Product>? Products { get; set; }
        public DateTime? Date { get; set; }
        public Guid? ProviderOrderId { get; set; }
        public int Import { get; set; }
    }
}
