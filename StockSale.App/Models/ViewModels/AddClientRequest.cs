using Microsoft.AspNetCore.Mvc.Rendering;

namespace StockSale.App.Models.ViewModels
{
    public class AddClientRequest
    {
        public string? Name { get; set; }
        public string? Email { get; set; }
        public string? Dni { get; set; }
        public string? Phone { get; set; }
        public string? Company { get; set; }
        public string? Cuil_t { get; set; }
        public string? Condition_Iva { get; set; }
        public int Discount { get; set; }
    }
}