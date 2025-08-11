using StockSale.App.Models.Domain;

namespace StockSale.App.Models.ViewModels
{
    public class TurnDetailsViewModel
    {
        public Turn Turn { get; set; }
        public ICollection<CashFlow> CashFlows { get; set; }
        public ICollection<Fiado> Fiados { get; set; }
        public ICollection<Debt> Debts { get; set; }
    }
}
