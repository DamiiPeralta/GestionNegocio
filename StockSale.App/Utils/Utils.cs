using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using StockSale.App.Data;
using StockSale.App.Models.Domain;

namespace StockSale.App.Utils
{
    public class UtilsCode
    {
        public static float CalculateImport(IEnumerable<Product_Price> productPrices, Func<Product_Price, float> priceSelector)
        {
            return productPrices
                .Where(pp => pp != null)
                .Sum(pp => priceSelector(pp) * pp.Quantity);
        }

        public static string ModalProviderOrder(IEnumerable<ProviderOrder> providerOrders, Guid id)
        {
            var order = providerOrders.FirstOrDefault(po => po.Id == id);

            if (order == null)
            {
                return "No se encontraron datos de la orden.";
            }

            return $"¿Estás seguro de que deseas cerrar la orden?";
        }

        public static string ModalTurn(IEnumerable<Turn> turns, Guid id)
        {
            var turn = turns.FirstOrDefault(t => t.Id == id);

            if (turn == null)
            {
                return "No se encontraron datos del turno.";
            }

            return $"¿Estás seguro de que deseas cerrar el turno?";
        }

        public static string ModalDebts(IEnumerable<Debt> debts, Guid id, string action)
        {
            var debt = debts.FirstOrDefault(d => d.Id == id);

            if (debt == null && action == "RemoveDebt")
            {
                return "No se encontraron datos de la deuda.";
            }

            return action == "AddDebt"
                ? "¿Estás seguro de que deseas agregar esta deuda?"
                : "¿Estás seguro de que deseas quitar esta deuda?";
        }
    }
}