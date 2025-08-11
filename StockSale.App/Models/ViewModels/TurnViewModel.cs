using StockSale.App.Models.Domain;
using System.ComponentModel.DataAnnotations;

namespace StockSale.App.Models.ViewModels
{
    public class TurnViewModel
    {
        public Guid Id { get; set; }
        public int NTurn { get; set; }
        public Turn? ActualTurn { get; set; }   
        [Required]
        [Range(0, float.MaxValue, ErrorMessage = "El efectivo inicial no puede ser negativo.")]
        public float Initial_Cash { get; set; }

        public float Actual_Cash { get; set; }

        public List<OrderBuy?> OrderBuys { get; set; } = new List<OrderBuy?>(); // Inicializa la lista
    }
}
