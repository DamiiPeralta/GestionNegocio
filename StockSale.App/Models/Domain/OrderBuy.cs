using Azure;

namespace StockSale.App.Models.Domain
{
    public class OrderBuy
    {
        public Guid Id { get; set; }
        public int NOrderBuy { get; set; }
        public DateTime Date { get; set; }
        public float Import {  get; set; }
        public int Discount { get; set; }
        public float TotalDiscount { get; set; }
        public float FinalTotal { get; set; }
        public bool CashPay { get; set; }

        //Relaciones
        public Client? Client { get; set; }
        public Turn? Turn { get; set; }
        public ICollection<Product_Price>? Products_Prices { get; set; }
        public bool IsClosed { get; set; }
        public bool IsDeleted { get; set; }
    }
}