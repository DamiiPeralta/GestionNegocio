namespace StockSale.App.Models.Domain
{
    public class Provider
    {
        public Guid Id { get; set; }
        public int NProvider { get; set; }
        public string Name { get; set; }
        public Boolean IsDeleted { get; set; }
        public string? Phone { get; set; }
        public string? Email { get; set; }
        public string? Company { get; set; }

        //Relaciones
        public ICollection<Product>? Products { get; set; }
        public ICollection<ProviderOrder>? ProviderOrders { get; set; }
    }
}
