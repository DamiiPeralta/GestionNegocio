using StockSale.App.Models.Domain;

namespace StockSale.App.Models.ViewModels
{
    public class EditProductViewModel
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

        // Agregá Barcode
        public string? Barcode { get; set; }

        //Relaciones
        public Provider? Provider { get; set; }
        public UnitM? UnitM { get; set; }

        public Guid? ProviderId { get; set; }
        public Guid? UnitMId { get; set; }

        // Listas de opciones
        public IEnumerable<Provider>? Providers { get; set; }
        public IEnumerable<UnitM>? UnitsM { get; set; }
    }
}
