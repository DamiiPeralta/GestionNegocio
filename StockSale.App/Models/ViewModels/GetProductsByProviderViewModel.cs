using StockSale.App.Models.Domain;

namespace StockSale.App.Models.ViewModels
{
    public class GetProductsByProviderViewModel
    {
        public Guid ProviderId { get; set; }
        public Guid? SelectedProviderId { get; set; }
        public IEnumerable<Provider?> Providers { get; set; }
        public IEnumerable<Product?> Products { get; set; }
    }
}
