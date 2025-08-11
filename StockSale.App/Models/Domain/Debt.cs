namespace StockSale.App.Models.Domain
{
    public class Debt
    {
        public Guid Id { get; set; }
        public DateTime Date { get; set; }
        public int Import { get; set; }
        public bool Status { get; set; }
        public Client Client { get; set; }
    }
}