using StockSale.App.Models.Domain;
using StockSale.App.Services;

public class ProviderProductsViewModel
{
    public string ProviderName { get; set; }
    public PaginatedResult<Product> Products { get; set; }
}

public class PaginatedProviderProductsViewModel
{
    public List<ProviderProductsViewModel> ProvidersWithProducts { get; set; }
}
