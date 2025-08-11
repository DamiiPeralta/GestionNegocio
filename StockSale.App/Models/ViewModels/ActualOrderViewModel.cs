using StockSale.App.Models.Domain;

namespace StockSale.App.Models.ViewModels
{
    public class ActualOrderViewModel
    {
        public Guid? Id { get; set; }
        public int NOrderBuy { get; set; }
        public DateTime? Date { get; set; }
        public float? Import { get; set; }
        public int? Discount { get; set; }
        public float? TotalWDisc { get; set; }
        public float? DiscountApplied { get; set; }
        public bool CashPay{ get; set; }

        public string? SearchTerm { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 5;
        public int TotalPages { get; set; }

        //Relaciones
        public Client? Client { get; set; }
        public Turn? Turn { get; set; }
        public ICollection<Product_Price> Products_Prices { get; set; }
        public ICollection<Product?> Products { get; set; }

        public Guid? SelectedProductId { get; set; }
        public int? Quantity { get; set; }
    }
}
