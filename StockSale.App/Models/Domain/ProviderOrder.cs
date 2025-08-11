using Azure;

namespace StockSale.App.Models.Domain
{
    public class ProviderOrder
    {
        public Guid Id { get; set; }
        public int NProviderOrder { get; set; }
        public DateTime Date { get; set; }
        public float Import {  get; set; }

        //Relaciones
        public ICollection<Product_Price>? Products_Prices { get; set; }
        public Guid ProviderId { get; set; }
        public Provider? Provider { get; set; }
        public bool IsClosed { get; set; }
        public bool IsDeleted { get; set; }
    }
}
