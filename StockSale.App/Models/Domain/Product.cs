using Azure;

namespace StockSale.App.Models.Domain
{
    public class Product
    {
        public Guid Id { get; set; }
        public int NProduct { get; set; }
        public string? Name { get; set; }
        public int Packaging { get; set; }
        public int Stock { get; set; }
        public int Stock_Limit { get; set; }
        public int List_Price { get; set; }
        public int Sell_Price { get; set; }
        public bool IsDeleted { get; set; }

        private string _barcode = string.Empty;
        public string Barcode
        {
            get => _barcode;
            set => _barcode = value ?? string.Empty;
        }

        //relaciones
        public Provider? Provider { get; set; } // Navegación a Provider
        public UnitM? UnitM { get; set; } // Navegación a UnitM

    }
}
