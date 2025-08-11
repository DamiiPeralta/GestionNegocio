using StockSale.App.Models.Domain;

namespace StockSale.App.Models.ViewModels
{
    public class AddFiadosViewModel
    {
        public int NFiado { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public DateTime Date { get; set; }
        public int Import { get; set; }
        public bool IsPay { get; set; }
        public DateTime LastDatePay { get; set; }
    }
}