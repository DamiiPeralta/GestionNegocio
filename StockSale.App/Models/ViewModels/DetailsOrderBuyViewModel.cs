using StockSale.App.Models.Domain;

namespace StockSale.App.Models.ViewModels
{
    public class DetailsOrderBuyViewModel
    {
            public Guid Id { get; set; }
            public int NOrderBuy { get; set; }
            public DateTime Date { get; set; }
            public float Import { get; set; }
            public int Discount { get; set; }
            public float TotalImport { get; set; }
            public float TotalDiscount { get; set; }
            public float FinalTotal { get; set; }
            public Client? Client { get; set; }
            public Turn? Turn { get; set; }
            public bool IsClosed { get; set; }
            public ICollection<Product_Price>? Products_Prices { get; set; }
        }
    }
