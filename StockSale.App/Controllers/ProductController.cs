using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using OfficeOpenXml;
using StockSale.App.Data;
using StockSale.App.Models.Domain;
using StockSale.App.Models.ViewModels;
using StockSale.App.Repositories;
using StockSale.App.Repositories.Interfaces;
using StockSale.App.Services;
using System.Diagnostics;
using System.Linq;

namespace StockSale.App.Controllers
{
    public class ProductController : Controller
    {
        private readonly ProductService productService;
        private readonly ProviderService providerService;
        private readonly IUnitMRepository unitMRepository;
        private readonly PaginationService paginationService;
        private readonly ExcelService excelService;
        private readonly StockSaleDbContext stockSaleDbContext;

        public ProductController(ProductService productService,
                                 ProviderService providerService,
                                 IUnitMRepository unitMRepository,
                                 PaginationService paginationService,
                                 ExcelService excelService,
                                 StockSaleDbContext stockSaleDbContext)
        {
            this.productService = productService;
            this.providerService = providerService;
            this.unitMRepository = unitMRepository;
            this.paginationService = paginationService;
            this.excelService = excelService;
            this.stockSaleDbContext = stockSaleDbContext;
        }

        // Este método devuelve la vista con los proveedores y unidades de medida cargadas
        [HttpGet]
        public async Task<IActionResult> Add()
        {
            var providers = await providerService.GetAllAsync(); // Obtener proveedores
            var unitsM = await unitMRepository.GetAllAsync(); // Obtener unidades de medida

            // Crear un ViewModel para pasar los datos a la vista
            var viewModel = new AddProductViewModel
            {
                Providers = providers,
                UnitsM = unitsM
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Add(AddProductViewModel model)
        {
            var lastAdded = await productService.LastAdded();
            int N = 1;
            if (lastAdded != null)
            {
                N = lastAdded.NProduct + 1;
            }

            var provider = await providerService.GetAsync(model.ProviderId.Value);
            var unitM = await unitMRepository.GetAsync(model.UnitMId.Value);

            var product = new Product
            {
                NProduct = N,
                Name = model.Name,
                Packaging = model.Packaging,
                Stock = model.Stock,
                Stock_Limit = model.Stock_Limit,
                List_Price = model.List_Price,
                Sell_Price = model.Sell_Price,
                Provider = provider,
                IsDeleted = false,
                UnitM = unitM,
                Barcode = model.Barcode  // <-- Agregado aquí
            };

            await productService.AddAsync(product);
            return RedirectToAction("ListPaginated");
        }


        [HttpPost]
        public async Task<IActionResult> UploadExcel(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                TempData["Error"] = "Por favor, selecciona un archivo Excel válido.";
                return RedirectToAction("ListPaginated");
            }

            try
            {
                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);

                    // Llamar al servicio para procesar el archivo Excel
                    string resultMessage = await excelService.ReadProductsFromExcel(stream);

                    if (string.IsNullOrWhiteSpace(resultMessage))
                    {
                        TempData["Error"] = "El archivo Excel no contiene productos válidos o ocurrió un error.";
                        return RedirectToAction("ListPaginated");
                    }

                    // Lógica para asignar NProduct
                    var lastAdded = await productService.LastAdded();  // Obtener el último producto agregado
                    int N = 1;  // Valor inicial para NProduct
                    if (lastAdded != null)
                    {
                        N = lastAdded.NProduct + 1;  // Incrementar el NProduct basándote en el último valor
                    }

                    // Aquí puedes acceder a los productos importados y asignar el NProduct
                    var products = await stockSaleDbContext.Products.ToListAsync(); // Obtén todos los productos importados o procesados
                    foreach (var product in products)
                    {
                        // Asignar el NProduct con el valor calculado
                        product.NProduct = N++;

                        // Aquí puedes agregar más lógica si necesitas hacer algo adicional con cada producto
                    }

                    // Guardar los cambios en la base de datos
                    await stockSaleDbContext.SaveChangesAsync();

                    // Mensaje de éxito
                    TempData["Success"] = resultMessage;
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = $"Error al procesar el archivo: {ex.Message}";
            }

            return RedirectToAction("ListPaginated");
        }


        // GET: Product/Edit/{id}
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var product = await productService.GetAsync(id);
            var providers = await providerService.GetAllAsync();
            var unitsM = await unitMRepository.GetAllAsync();

            if (product != null)
            {
                var viewModel = new EditProductViewModel
                {
                    Id = product.Id,
                    NProduct = product.NProduct,
                    Name = product.Name,
                    Packaging = product.Packaging,
                    Stock = product.Stock,
                    Stock_Limit = product.Stock_Limit,
                    List_Price = product.List_Price,
                    Sell_Price = product.Sell_Price,
                    Barcode = product.Barcode,  // <-- asignado acá
                    Provider = product.Provider,
                    UnitM = product.UnitM,
                    Providers = providers,
                    UnitsM = unitsM,
                    ProviderId = product.Provider?.Id,
                    UnitMId = product.UnitM?.Id,
                    IsDeleted = product.IsDeleted
                };
                return View(viewModel);
            }

            return NotFound();
        }

        // POST: Product/Edit
        [HttpPost]
        public async Task<IActionResult> Edit(EditProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                model.Providers = await providerService.GetAllAsync();
                model.UnitsM = await unitMRepository.GetAllAsync();
                return View(model);
            }

            var existingProduct = await productService.GetAsync(model.Id);
            if (existingProduct == null)
            {
                return NotFound();
            }

            existingProduct.NProduct = model.NProduct;
            existingProduct.Name = model.Name;
            existingProduct.Packaging = model.Packaging;
            existingProduct.Stock = model.Stock;
            existingProduct.Stock_Limit = model.Stock_Limit;
            existingProduct.List_Price = model.List_Price;
            existingProduct.Sell_Price = model.Sell_Price;
            existingProduct.Barcode = model.Barcode; // <-- actualizar barcode

            if (model.ProviderId != null)
            {
                var provider = await providerService.GetAsync(model.ProviderId.Value);
                existingProduct.Provider = provider;
            }
            else
            {
                existingProduct.Provider = null;
            }

            if (model.UnitMId != null)
            {
                var unitM = await unitMRepository.GetAsync(model.UnitMId.Value);
                existingProduct.UnitM = unitM;
            }
            else
            {
                existingProduct.UnitM = null;
            }

            var updated = await productService.UpdateAsync(existingProduct);
            if (updated != null)
            {
                return RedirectToAction("ListPaginated");
            }

            model.Providers = await providerService.GetAllAsync();
            model.UnitsM = await unitMRepository.GetAllAsync();
            ModelState.AddModelError("", "Error al actualizar el producto.");
            return View(model);
        }

        // Envía el formulario para eliminar ese producto, recibe un id y borra el producto o devuelve un mensaje de error
        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var deletedProduct = await productService.DeleteAsync(id);
            if (deletedProduct != null)
            {
                // Mostrar un mensaje de éxito
                return RedirectToAction("ListPaginated");
            }

            // Mostrar un mensaje de error
            return RedirectToAction("Edit", new { id = id });
        }

[HttpPost]
public async Task<IActionResult> RecoverProduct(Guid id)
{
    // Intentar obtener el producto por su ID
    var product = await productService.GetAsync(id);
    if (product != null)
    {
        // Cambiar el estado de eliminado a falso
        product.IsDeleted = false;
        // Actualizar el producto en el repositorio
            var recoveredProduct = await productService.UpdateAsync(product);
            if(recoveredProduct != null)
        {
            return RedirectToAction("ListPaginated");
        }
        return RedirectToAction("Edit", new { id = id });
    }
    return RedirectToAction("ListPaginated");
}

        [HttpGet]
        public async Task<IActionResult> ListPaginated(
    string? barcodeSearch,
    int pageNumber = 1,
    int pageSize = 10,
    string? sortBy = "Name",
    string? sortDirection = "asc")
        {
            // Traigo todos los productos a memoria (tu productService devuelve IEnumerable)
            var allProducts = (await productService.GetAllAsync()).ToList();

            // Si hay texto de búsqueda, filtrar por Barcode, Name o Packaging (packaging es int -> ToString())
            if (!string.IsNullOrWhiteSpace(barcodeSearch))
            {
                var q = barcodeSearch.Trim();
                allProducts = allProducts
                    .Where(p =>
                        (!string.IsNullOrEmpty(p.Barcode) && p.Barcode.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                        (!string.IsNullOrEmpty(p.Name) && p.Name.Contains(q, StringComparison.OrdinalIgnoreCase)) ||
                        // Packaging es int en tu modelo -> convertir a string para buscar coincidencias
                        p.Packaging.ToString().Contains(q, StringComparison.OrdinalIgnoreCase)
                    )
                    .ToList();
            }

            // Ordenado según parámetros
            allProducts = sortBy?.ToLower() switch
            {
                "name" => sortDirection == "asc" ? allProducts.OrderBy(p => p.Name).ToList() : allProducts.OrderByDescending(p => p.Name).ToList(),
                "stock" => sortDirection == "asc" ? allProducts.OrderBy(p => p.Stock).ToList() : allProducts.OrderByDescending(p => p.Stock).ToList(),
                "list_price" => sortDirection == "asc" ? allProducts.OrderBy(p => p.List_Price).ToList() : allProducts.OrderByDescending(p => p.List_Price).ToList(),
                "sell_price" => sortDirection == "asc" ? allProducts.OrderBy(p => p.Sell_Price).ToList() : allProducts.OrderByDescending(p => p.Sell_Price).ToList(),
                _ => allProducts.OrderBy(p => p.Name).ToList(),
            };

            // Paginación segura aunque no haya items
            int totalItems = allProducts.Count;
            int totalPages = totalItems == 0 ? 1 : (int)Math.Ceiling(totalItems / (double)pageSize);

            if (pageNumber < 1) pageNumber = 1;
            if (pageNumber > totalPages) pageNumber = totalPages;

            var pagedProducts = allProducts
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // cálculo de páginas visibles (max 5)
            int startPage = Math.Max(1, pageNumber - 2);
            int endPage = Math.Min(totalPages, startPage + 4);
            var visiblePages = Enumerable.Range(startPage, endPage - startPage + 1).ToList();

            // Pasar datos a la vista
            ViewBag.PageNumber = pageNumber;
            ViewBag.PageSize = pageSize;
            ViewBag.TotalPages = totalPages;
            ViewBag.VisiblePages = visiblePages;
            ViewBag.SortBy = sortBy;
            ViewBag.SortDirection = sortDirection;
            ViewBag.BarcodeSearch = barcodeSearch;

            if (!pagedProducts.Any() && !string.IsNullOrEmpty(barcodeSearch))
            {
                ViewBag.Message = "No se encontró ningún producto con ese código de barras o texto.";
            }

            return View(pagedProducts);
        }

    }
}
