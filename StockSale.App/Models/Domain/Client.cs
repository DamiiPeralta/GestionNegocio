using Azure;
using System.ComponentModel.DataAnnotations;

namespace StockSale.App.Models.Domain
{
    public class Client
    {
        public Guid Id { get; set; }
        public int NClient {get; set; }
        public string Name { get; set; }
        public string? Email { get; set; }
        public string? Dni { get; set; }
        public string? Phone { get; set; }
        public string? Company { get; set; }
        public string? Cuil_t { get; set; }
        public string? Condition_Iva { get; set; }

        [Range(0, 100, ErrorMessage = "El descuento debe estar entre 0 y 100.")]
        public int Discount { get; set; }

        public ICollection<Debt> Debts { get; set; }

        // Relaciones
        public ICollection<OrderBuy>? OrderBuys { get; set; }
        public ICollection<Turn>? Turns { get; set; }

        public bool IsDeleted { get; set; }
    }
}
