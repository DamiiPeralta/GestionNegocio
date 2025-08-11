using Microsoft.AspNetCore.Mvc;
using StockSale.App.Services;

namespace StockSale.App.Controllers
{
    public class StockLimitController : Controller
    {
        private readonly PaginationService _paginationService;
        private readonly ProviderService _providerService;
        private readonly ProductService _productService;

        public StockLimitController(PaginationService paginationService, ProviderService providerService, ProductService productService)
        {
            _paginationService = paginationService;
            _providerService = providerService;
            _productService = productService;
        }
        [HttpGet]
        public async Task<IActionResult> ListPaginated(
        string? searchString,
        string? sortBy,
        string? sortDirection,
        int pageSize = 10,
        int pageNumber = 1)
        {
            // Validar parámetros
            if (pageSize <= 0) pageSize = 10;
            if (pageNumber <= 0) pageNumber = 1;

            // Obtener los productos con límite de stock
            var products = await _productService.HasStockLimit();

            // Validar que haya productos
            if (products == null || !products.Any())
            {
                ViewBag.VisiblePages = new List<int>();
                ViewBag.PageNumber = 1;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = 1;

                return View(new PaginatedProviderProductsViewModel
                {
                    ProvidersWithProducts = new List<ProviderProductsViewModel>(),
                });
            }

            // Agrupar los productos por proveedor
            var groupedProducts = products.GroupBy(p => p.Provider?.Name ?? "Sin Proveedor");

            // Crear la estructura para la vista
            var providersWithProducts = groupedProducts.Select(group => new ProviderProductsViewModel
            {
                ProviderName = group.Key,
                Products = _paginationService.GetPaginatedData(
                    group.ToList(),
                    pageNumber,
                    pageSize,
                    sortBy,
                    sortDirection,
                    p => string.IsNullOrEmpty(searchString) ||
                         p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            }).ToList();

            // Validar que haya datos en los resultados paginados
            var paginatedResult = providersWithProducts.FirstOrDefault()?.Products;
            if (paginatedResult == null)
            {
                ViewBag.VisiblePages = new List<int>();
                ViewBag.PageNumber = 1;
                ViewBag.PageSize = pageSize;
                ViewBag.TotalPages = 1;

                return View(new PaginatedProviderProductsViewModel
                {
                    ProvidersWithProducts = providersWithProducts
                });
            }

            // Configurar valores para la vista
            ViewBag.VisiblePages = paginatedResult.VisiblePages ?? new List<int>();
            ViewBag.PageNumber = paginatedResult.PageNumber ?? 1;
            ViewBag.PageSize = paginatedResult.PageSize ?? pageSize;
            ViewBag.TotalPages = paginatedResult.TotalPages ?? 1;

            // Preparar el modelo para la vista
            var viewModel = new PaginatedProviderProductsViewModel
            {
                ProvidersWithProducts = providersWithProducts,
            };

            return View(viewModel);
        }
    }
}

