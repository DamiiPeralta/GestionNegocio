using Microsoft.AspNetCore.Mvc;
using StockSale.App.Models.Domain;
using StockSale.App.Models.ViewModels;
using StockSale.App.Services;

namespace StockSale.App.Controllers
{
    public class ProviderOrderController : Controller
    {
    
        private readonly ProviderOrderService providerOrderService;
        private readonly ProviderService providerService;
        private readonly ProductService productService;
        private readonly PaginationService paginationService;


        public ProviderOrderController(ProviderOrderService providerOrderService, 
               ProviderService providerService, 
               ProductService productService,
               PaginationService paginationService)
        {
       
            this.providerOrderService = providerOrderService;
            this.providerService = providerService;
            this.productService = productService;
            this.paginationService = paginationService;
        }

        // Este envía el formulario de ProviderOrder una vez que esté completado, 
        // trata de crear un ProviderOrder en la base de datos o devuelve un error
        [HttpPost]
        public async Task<IActionResult> AddOrder(AddProviderOrderViewModel addProviderOrderViewModel)
        {
            var providers = await providerService.GetAllAsync();
            var providerOrderViewModel = new AddProviderOrderViewModel
            {
                Providers = providers,
            };

            var newOrder = new ProviderOrder
            {
                Date = DateTime.Now,
                Import = 0,
                IsClosed = false
            };

            var selectedProvider = await providerService.GetAsync(addProviderOrderViewModel.ProviderId);
            if(selectedProvider != null)
            {
                newOrder.Provider = selectedProvider;
                try
                {
                    await providerOrderService.AddAsync(newOrder);
                    return RedirectToAction("GetActualProviderOrder", selectedProvider.Id);
                }
                catch
                {
                    ModelState.AddModelError(string.Empty, "Error al añadir la Orden. Intente nuevamente.");
                    return RedirectToAction("Provider/ListPaginated");
                }
            }
            else
            {
                return RedirectToAction("Provider/ListPaginated");
            }
        }

        [HttpGet]
        public async Task<IActionResult> CheckProviderOrder(Guid id, Guid providerOrderId)
        {
            var provider = await providerService.GetAsync(id);
            var providerOrder = await providerOrderService.GetAsync(providerOrderId);

            if (providerOrder == null || provider == null)
            {
                return NotFound($"Proveedor con ID {id} o la orden con ID {providerOrderId} no encontrado.");
            }

            if (providerOrder.IsClosed = false)
            {
                return Ok(providerOrder); // Si está activa, se devuelve la orden
            }
            else
            {
                ViewBag.Message = $"La orden del proveedor con ID {providerOrderId} no está activa.";
                return View("InactiveOrderView"); // Muestra la vista para órdenes inactivas
            }
        }

        [HttpPost]
        public async Task<IActionResult> CloseOrder(Guid providerId)
        {
            var closedOrder = await providerOrderService.CloseProviderOrder(providerId);
                if (closedOrder != null)
                {
                    return RedirectToAction("ListPaginated");
                }
                else
                {
                    return RedirectToAction("GetActualProviderOrder");
                }
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deletedOrder = await providerOrderService.DeleteAsync(id);
            if (deletedOrder != null)
            {
                return RedirectToAction("ListPaginated", "Provider");
            }
            return RedirectToAction("Details", id);
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var order = await providerOrderService.GetAsync(id);
            return View(order);
        }

        [HttpPost]
        public async Task<IActionResult> Details(ProviderOrder detailOrder)
        {
            var order = new ProviderOrder
            {
                NProviderOrder = detailOrder.NProviderOrder,
                Id = detailOrder.Id,
                Date = detailOrder.Date,
                Import = detailOrder.Import,
                Provider = detailOrder.Provider,
                Products_Prices = detailOrder.Products_Prices,
                IsClosed = detailOrder.IsClosed,
            };
            var updatedOrder = await providerOrderService.UpdateAsync(order);
            if(updatedOrder != null)
            {
                // success
                return View(updatedOrder);
            }
            else
            {
                // error
                return View(detailOrder);
            }
        }

        [HttpGet]
        [Route("ProviderOrder/ListByProvider/{id}")]
        public async Task<IActionResult> ListByProvider(
        Guid id,
        string? searchString,
        string? sortBy,
        string? sortDirection,
        int pageSize = 10,
        int pageNumber = 1) // 'id' es el 'provider.Id'
        {
            var providerOrders = await providerOrderService.GetOrdersByProviderIdAsync(id);

            var paginatedResult = paginationService.GetPaginatedData(
                providerOrders,
                pageNumber,
                pageSize,
                sortBy,
                sortDirection,
                p => string.IsNullOrEmpty(searchString)
            );
            ViewBag.VisiblePages = paginatedResult.VisiblePages;
            ViewBag.TotalPages = paginatedResult.TotalPages;
            ViewBag.PageNumber = paginatedResult.PageNumber;
            ViewBag.PageSize = paginatedResult.PageSize;

            return View(paginatedResult.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetActualProviderOrder(Guid id)
        {
            var provider = await providerService.GetAsync(id);
            if (provider != null)
            {
                var createOrder = await providerOrderService.GetOrCreateIsActiveOrder(id);
                var activeOrder = await providerOrderService.GetIsActiveOrder(id);
                // Recuperamos la orden actual y los productos
                var products = provider.Products.AsQueryable();

                if (activeOrder != null)
                {
                    if (activeOrder.Products_Prices == null)
                    {
                        activeOrder.Products_Prices = new List<Product_Price>();
                    }

                    // Creamos el modelo para pasar a la vista
                    var model = new ActualProviderOrderViewModel()
                    {
                        Date = activeOrder.Date,
                        Import = activeOrder.Import,
                        Provider = activeOrder.Provider,
                        Products = products.ToList(),
                        Products_Prices = activeOrder.Products_Prices.ToList(),
                    };

                    // Retornamos la vista con el modelo ensamblado
                    return View(model);
                }
                
                return RedirectToAction("Providers/ListPaginated");
            }
            return RedirectToAction("Providers/ListPaginated");
        }

        [HttpGet]
        public async Task<IActionResult> AddProductsToProviderOrder(Guid id, string searchString, int pageNumber = 1, int pageSize = 5, string sortBy = "Name", string sortDirection = "asc")
        {
            var actualOrder = await providerOrderService.GetIsActiveOrder(id);
            if (actualOrder == null) return RedirectToAction("AddOrder");

            var provider = await providerService.GetAsync(id);
            var products = provider?.Products ?? new List<Product>(); // Evitar null

            var paginatedResult = paginationService.GetPaginatedData(
                products,
                pageNumber,
                pageSize,
                sortBy,
                sortDirection,
                p => string.IsNullOrEmpty(searchString) ||
                     p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
            );

            // Pasar datos de paginación a ViewBag
            ViewBag.VisiblePages = paginatedResult.VisiblePages;
            ViewBag.TotalPages = paginatedResult.TotalPages;
            ViewBag.PageNumber = paginatedResult.PageNumber ?? 1;
            ViewBag.PageSize = paginatedResult.PageSize ?? 5;

            var model = new ActualProviderOrderViewModel
            {
                Id = actualOrder.Id, // Asignar el ID de la orden
                Date = actualOrder.Date,
                Import = actualOrder.Import,
                Provider = actualOrder.Provider,
                Products = products.ToList(),
                Products_Prices = actualOrder.Products_Prices.ToList(),
                SelectedProductId = null,
                Quantity = 0
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddProductsToProviderOrder(ActualProviderOrderViewModel actualProviderOrderViewModel, List<Guid> SelectedProductId, List<int> Quantities, Guid id, string searchString, int pageNumber = 1, int pageSize = 5, string sortBy = "Name", string sortDirection = "asc")
        {
            var actualOrder = await providerOrderService.GetIsActiveOrder(id);
            var provider = await providerService.GetAsync(id);

            if (actualOrder == null || SelectedProductId == null || Quantities == null)
            {
                return RedirectToAction("AddOrder");
            }

            // Filtrar los productos seleccionados con la checkbox
            for (int i = 0; i < SelectedProductId.Count; i++)
            {
                var productId = SelectedProductId[i];
                var quantity = Quantities[i];

                if (quantity < 1) continue; // Validar que la cantidad sea válida

                var existingProductPrice = actualOrder.Products_Prices
                    .FirstOrDefault(pp => pp.Product.Id == productId);

                if (existingProductPrice != null)
                {
                    // Si ya existe el producto en la orden, actualizar cantidad
                    existingProductPrice.Quantity = quantity;
                    await providerOrderService.UpdateProductPrice(actualOrder.Id, existingProductPrice);
                }
                else
                {
                    // Si no existe, agregarlo
                    await providerOrderService.AddProductPrice(actualOrder.Id, productId, quantity);
                }
            }

            // Redirigir al GET para mantener la paginación y filtros
            return RedirectToAction("AddProductsToProviderOrder", new
            {
                id = id,
                searchString = searchString,
                pageNumber = pageNumber,
                pageSize = pageSize,
                sortBy = sortBy,
                sortDirection = sortDirection
            });
        }

        [HttpGet]
        public async Task<IActionResult> ListPaginated(
        string searchString,
        string? sortBy,
        string? sortDirection,
        int pageSize = 10,
        int pageNumber = 1)
        {
            var allProviderOrders = await providerOrderService.GetAllAsync();
            foreach (var order in allProviderOrders)
            {
                var total = 0f;
                if (order.Products_Prices != null)
                {
                    foreach (var product in order.Products_Prices)
                    {
                        total += product.PriceList * product.Quantity; // Calcular el total para cada producto
                    }
                }
                order.Import = total; // Asignar el total al ProviderOrder
            }
            // Utilizar el servicio de paginado
            var paginatedResult = paginationService.GetPaginatedData(
                allProviderOrders,
                pageNumber,
                pageSize,
                sortBy,
                sortDirection,
                t => string.IsNullOrEmpty(searchString) ||
                     (DateTime.TryParse(searchString, out var parsedDate) && t.Date.Date == parsedDate.Date) ||
                     (t.Provider != null && t.Provider.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase))
            );

            // Pasar datos a la vista
            ViewBag.VisiblePages = paginatedResult.VisiblePages;
            ViewBag.TotalPages = paginatedResult.TotalPages;
            ViewBag.PageNumber = paginatedResult.PageNumber;
            ViewBag.PageSize = paginatedResult.PageSize;

            return View(paginatedResult.Data);
        }
    }
}
