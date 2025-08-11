using StockSale.App.Models.Domain;

namespace StockSale.App.Models.ViewModels
{
    public class AddOrderBuyViewModel
    {
        public DateTime Date { get; set; }
        public float Import {  get; set; }
        public int Discount { get; set; }
        public Client? Client {  get; set; }
        
        public Turn? Turn { get; set; }
        
        public ICollection<Product?> Products { get; set; }


    }
}
