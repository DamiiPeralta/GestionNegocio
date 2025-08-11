using StockSale.App.Models.Domain;

namespace StockSale.App.Models.ViewModels
{
    public class EditProviderOrderViewModel
    {
        public Guid Id { get; set; }
        public int NProviderOrder { get; set; }
        public DateTime Date { get; set; }

        public float Import { get; set; }

        //Relaciones

        public Guid ProviderId { get; set; }

        public IEnumerable<Provider?> Providers { get; set; }
    }
}
