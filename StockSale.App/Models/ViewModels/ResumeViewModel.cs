using StockSale.App.Models.Domain;

public class ResumeViewModel
{
    public float TotalImporte { get; set; }
    public float TotalGanancia { get; set; }
    public float TotalDescuentos { get; set; }
    public float TotalGastos { get; set; }
    public List<OrderBuy> Orders { get; set; }
    public List<CashFlow> CashFlows { get; set; }
}
