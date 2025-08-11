using Azure;

namespace StockSale.App.Models.Domain
{
    public class UnitM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        
        //Relaciones
        public ICollection<Product>? Products { get; set; }
    }
}
