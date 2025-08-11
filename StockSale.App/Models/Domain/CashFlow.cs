namespace StockSale.App.Models.Domain
{
    public class CashFlow
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public int Import { get; set; }
        public bool Income { get; set; }
        public string Comment { get; set; }
    }
}
