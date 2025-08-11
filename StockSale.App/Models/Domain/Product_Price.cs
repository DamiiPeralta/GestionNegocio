namespace StockSale.App.Models.Domain
{
    public class Product_Price
    {
        public Guid Id { get; set; }
        public float PriceList { get; set; }
        public float PriceSell { get; set; }
        public int Quantity { get; set; }
        public Product? Product { get; set; }
    }
}
