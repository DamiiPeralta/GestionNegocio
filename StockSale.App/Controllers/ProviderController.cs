using Microsoft.AspNetCore.Mvc;
using StockSale.App.Models.ViewModels;
using StockSale.App.Models.Domain;
using System.Globalization;
using StockSale.App.Services;
using StockSale.App.Repositories.Interfaces;
using StockSale.App.Repositories;

namespace StockSale.App.Controllers
{

    public class ProviderController : Controller
    {
        private readonly ProviderService providerService;
        private readonly IProviderOrderRepository providerOrderRepository;
        private readonly PaginationService paginationService;
        private readonly ProductService productService;

        public ProviderController(ProviderService providerService, IProviderOrderRepository providerOrderRepository, PaginationService paginationService, ProductService productService)
        {
            this.providerService = providerService;
            this.providerOrderRepository = providerOrderRepository;
            this.paginationService = paginationService;
            this.productService = productService;
        }
        //Este devuelve la vista que tiene el formulario para añadir proveedores
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        //Este envia el formulario de proveedores una vez que este completado, trata de crear un proveedores en la base de datos o devuelve un error
        [HttpPost]
        public async Task<IActionResult> Add(AddProviderRequest addProviderRequest)
        {
            var lastAdded = await providerService.LastAdded();
            int N = 1;
            if(lastAdded != null)
            {
                N = lastAdded.NProvider + 1;
            }
            var provider = new Provider
            {
                NProvider = N,
                Name = addProviderRequest.Name,
                Phone = addProviderRequest.Phone,
                Email = addProviderRequest.Email,
                Company = addProviderRequest.Company
            };
            // Intentar añadir el proveedor
            try
            {
                await providerService.AddAsync(provider);
            }
            catch (Exception ex)
            {
                // Manejo de errores (loggear o mostrar un mensaje de error)
                ModelState.AddModelError(string.Empty, "Error al añadir el cliente. Intente nuevamente.");
                return View(addProviderRequest);
            }
            return RedirectToAction("ListPaginated");
        }

        [HttpPost]
        public async Task<IActionResult> ChangeProvider(Guid providerId, Guid selectedProviderId)
        {
            var products = await providerService.GetProductsByProvider(providerId);
            var newProvider = await providerService.GetAsync(selectedProviderId);

            foreach (var product in products)
            {
                product.Provider = newProvider;
                await productService.UpdateAsync(product);
            }

            return RedirectToAction("GetProductsByProvider", new { id = newProvider.Id });
        }

        //Devuelve una vista que tiene el formulario para editar o eliminar a los proveedores
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var provider = await providerService.GetAsync(id);

            if (provider != null)
            {
                var model = new Provider
                {
                    Id = provider.Id,
                    NProvider = provider.NProvider,
                    Name = provider.Name,
                    Phone = provider.Phone,
                    Email = provider.Email,
                    Company = provider.Company
                };
                return View(model);
            }
            return View(null);
        }
        //envia el formulario para eliminar a ese proveedor, recibe un id y borra ese proveedor o devuelve un mensaje de error
        [HttpPost]
        public async Task<IActionResult> Delete(Provider provider)
        {
            var deletedProvider = await providerService.DeleteProviderAsync(provider.Id);
            if (deletedProvider != null)
            {
                //mensaje de exit
                return RedirectToAction("ListPaginated");
            }
            //mensaje de error
            return RedirectToAction("Edit", new { id = provider.Id });
        }
        //envia el formulario con los nuevos datos y edita a ese proveedores o devuelve un error
        [HttpPost]
        public async Task<IActionResult> Edit(Provider provider)
        {
            var providerDomainModel = new Provider
            {
                Id = provider.Id,
                Name = provider.Name,
                Phone = provider.Phone,
                Email = provider.Email,
                Company = provider.Company,
            };

            var updatedProvider = await providerService.UpdateAsync(providerDomainModel);

            if (updatedProvider != null)
            {
                //mensaje de exito
                return RedirectToAction("ListPaginated");
            }
            //mensaje de error
            return RedirectToAction("Edit", new { id = provider.Id });
        }

        [HttpGet]
        public async Task<IActionResult> GetProductsByProvider(
    Guid id,
    string? searchString,
    string? searchQuery,
    string? sortBy,
    string? sortDirection,
    int pageSize = 10,
    int pageNumber = 1)
        {
            var allProducts = await providerService.GetProductsByProvider(id);
            if (allProducts == null)
            {
                return View(null);
            }

            // Usar el PaginationService para obtener los datos paginados
            var paginationService = new PaginationService();
            var paginatedResult = paginationService.GetPaginatedData(
                allProducts,
                pageNumber,
                pageSize,
                sortBy,
                sortDirection,
                p => string.IsNullOrEmpty(searchString) ||
                     p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
            );

            // Preparar el modelo de la vista
            var getProductsByProviderViewModel = new GetProductsByProviderViewModel
            {
                Products = paginatedResult.Data,
                Providers = await providerService.GetAllAsync(),
                SelectedProviderId = null,
                ProviderId = id
            };

            // Pasar datos de paginación a la vista
            ViewBag.ProviderId = id;
            ViewBag.TotalPages = paginatedResult.TotalPages;
            ViewBag.SearchQuery = searchQuery;
            ViewBag.SortBy = sortBy;
            ViewBag.SortDirection = sortDirection;
            ViewBag.PageSize = pageSize;
            ViewBag.PageNumber = paginatedResult.PageNumber;
            ViewBag.VisiblePages = paginatedResult.VisiblePages;

            return View(getProductsByProviderViewModel);
        }


        [HttpGet]
        [Route("Provider/ListPaginated")]
        public async Task<IActionResult> ListPaginated(
            string searchString,
            string? searchQuery,
            string? sortBy,
            string? sortDirection,
            int pageSize = 10,
            int pageNumber = 1)
        {
            var allProviders = await providerService.GetAllAsync();

            // Utilizar el servicio de paginado
            var paginatedResult = paginationService.GetPaginatedData(
                allProviders,
                pageNumber,
                pageSize,
                sortBy,
                sortDirection,
                p => string.IsNullOrEmpty(searchString) ||
                     p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) ||
                     p.Company.Contains(searchString, StringComparison.OrdinalIgnoreCase)
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
