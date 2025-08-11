using StockSale.App.Models.Domain;

namespace StockSale.App.Models.ViewModels
{
    public class ActualProviderOrderViewModel
    {
        public Guid Id { get; set; }
        public DateTime? Date { get; set; }
        public float? Import { get; set; }
        public int? Quantity { get; set; }
        public bool SelectedProduct { get; set; }

        //Relaciones
        public Provider Provider { get; set; }
        public ICollection<Product_Price>? Products_Prices { get; set; }
        public ICollection<Product>? Products { get; set; }
        public Guid? SelectedProductId { get; set; }
    }
}