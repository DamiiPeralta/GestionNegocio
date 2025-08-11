using Microsoft.AspNetCore.Mvc;
using StockSale.App.Models.Domain;
using StockSale.App.Models.ViewModels;
using StockSale.App.Services;

namespace StockSale.App.Controllers
{
    public class OrderBuyController : Controller
    {
        private readonly TurnService turnService;
        private readonly OrderBuyService orderBuyService;
        private readonly ProductService productService;
        private readonly ProductPriceService productPriceService;

        public OrderBuyController(
                TurnService turnService, 
                OrderBuyService orderBuyService, 
                ProductService productService,
                ProductPriceService productPriceService)
        {
            this.turnService=turnService;
            this.orderBuyService=orderBuyService;
            this.productService=productService;
            this.productPriceService = productPriceService;
        }
        //Este devuelve la vista que tiene el formulario para añadir ordenes
        [HttpGet]
        public async Task<IActionResult> AddOrder()
        {
            var lastAdded = await orderBuyService.LastAdded();
            int N = 1;
            if (lastAdded != null)
            {
                N = lastAdded.NOrderBuy + 1;
            }
            var actualTurn = await turnService.GetActualTurn();
            if (actualTurn != null)
            {
                var activeOrder = await orderBuyService.GetActiveOrder();
                if (activeOrder == null)
                {
                    var addNewOrder = new OrderBuy()
                    {
                        NOrderBuy = N,
                        Id = Guid.NewGuid(),
                        Date = DateTime.Now,
                        Import = 0,
                        Discount = 0,
                        Client = null,
                        Turn = actualTurn,
                        IsClosed = false,
                        IsDeleted = false,
                        Products_Prices = new List<Product_Price>()
                    };
                    await orderBuyService.AddAsync(addNewOrder);
                }
                return RedirectToAction("GetActualOrder");
            }
            return RedirectToAction("ListPaginated", "Turn");
        }

        [HttpPost]
        public async Task<IActionResult> AddClientToOrder(Guid clientId)
        {
            var addedClientToOrder = await orderBuyService.AddClientToOrder(clientId);
            if (addedClientToOrder != null)
            {
                // Show Success
                return RedirectToAction("GetActualOrder");
            }
            // Show Error
            return RedirectToAction("GetActualOrder");
        }
        
        [HttpPost]
        public async Task<IActionResult> RemoveClientFromOrder()
        {
            var deletedClient = await orderBuyService.RemoveClientFromOrder();
            if (deletedClient != null)
            {
                // Show Success
                return RedirectToAction("GetActualOrder");
            }
            // Show Error
            return RedirectToAction("GetActualOrder");
        }

        [HttpPost]
        public async Task<IActionResult> CloseOrder(Guid id, bool cashpay)
        {
            // Obtén la orden activa si no se proporcionó un ID válido
            var order = await orderBuyService.GetActiveOrder();
            var import = await orderBuyService.GetImport(id, cashpay);
            if (order != null && import != null)
            {
                var removedStockFromOrder = await productService.RemoveStockFromOrder(order.Products_Prices.ToList());
                if (removedStockFromOrder == true)
                {
                var closeOrder = await orderBuyService.CloseOrder(order.Id, cashpay);
                if (closeOrder != null)
                {
                    return RedirectToAction("ActualTurn", "Turn");
                }
                }
            }
            return RedirectToAction("GetActualOrder");
        }

        [HttpGet]
        public async Task<IActionResult> GetActualOrder()
        {
            var actualOrder = await orderBuyService.GetActiveOrder();
            var products = await productService.GetAllAsync();
            if (actualOrder != null)
            {

                // Calcula el importe y descuentos
                float import = actualOrder.Import;
                int discount = actualOrder.Discount;
                float discountApplied = import * discount / 100;
                float totalWDisc = import - discountApplied;

                var model = new ActualOrderViewModel()
                {
                    NOrderBuy = actualOrder.NOrderBuy,
                    Id = actualOrder.Id,
                    Date = actualOrder.Date,
                    Import = import,
                    Discount = discount,
                    DiscountApplied = discountApplied,
                    TotalWDisc = totalWDisc,
                    Client = actualOrder.Client,
                    Turn = actualOrder.Turn,
                    Products = products.ToList(),
                    Products_Prices = actualOrder.Products_Prices.ToList(),
                };

                return View(model);
            }

            return View(null);
        }

        [HttpGet]
        public async Task<IActionResult> AddProductsToOrder(string? searchTerm, int pageNumber = 1, int pageSize = 5)
        {
            var actualOrder = await orderBuyService.GetActiveOrder();
            if (actualOrder == null)
                return View(null);

            // Obtener todos los productos
            var allProducts = await productService.GetAllAsync();

            // Filtrar por búsqueda si existe
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                allProducts = allProducts.Where(p =>
                    (p.Name != null && p.Name.Contains(searchTerm, StringComparison.OrdinalIgnoreCase)) ||
                    (p.Barcode != null && p.Barcode.Contains(searchTerm, StringComparison.OrdinalIgnoreCase))
                ).ToList();
            }

            // Calcular total páginas
            var totalProducts = allProducts.Count();
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            // Aplicar paginación
            var productsOnPage = allProducts
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var model = new ActualOrderViewModel()
            {
                NOrderBuy = actualOrder.NOrderBuy,
                Date = actualOrder.Date,
                Import = actualOrder.Import,
                Discount = actualOrder.Discount,
                Client = actualOrder.Client,
                Turn = actualOrder.Turn,
                Products = productsOnPage,
                Products_Prices = actualOrder.Products_Prices.ToList(),
                SearchTerm = searchTerm,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AddProductsToOrder(ActualOrderViewModel actualOrderViewModel)
        {
            var actualOrder = await orderBuyService.GetActiveOrder();
            var products = await productService.GetAllAsync();

            if (actualOrder == null || !actualOrderViewModel.SelectedProductId.HasValue)
            {
                return RedirectToAction("AddOrder");
            }

            var selectedProduct = products.FirstOrDefault(p => p.Id == actualOrderViewModel.SelectedProductId.Value);
            if (selectedProduct == null)
            {
                // Error: El producto no existe
                ViewBag.ErrorMessage = "El producto seleccionado no existe.";
                return View(BuildViewModel(actualOrder, products, actualOrderViewModel));
            }

            if (actualOrderViewModel.Quantity.HasValue && actualOrderViewModel.Quantity.Value > selectedProduct.Stock)
            {
                // Error: Stock insuficiente
                ViewBag.ErrorMessage = $"Stock insuficiente. Solo hay {selectedProduct.Stock} unidades disponibles.";
                return View(BuildViewModel(actualOrder, products, actualOrderViewModel));
            }

            var existProductPrice = orderBuyService.ExistProduct(
                actualOrder.Products_Prices.ToList(),
                actualOrderViewModel.SelectedProductId.Value
            );

            if (existProductPrice != null)
            {
                existProductPrice.Quantity = actualOrderViewModel.Quantity.Value;
                var productPrice = await productPriceService.UpdateAsync(existProductPrice);

                if (productPrice != null)
                {
                    actualOrder = await orderBuyService.GetActiveOrder();
                    return View(BuildViewModel(actualOrder, products));
                }
            }
            else
            {
                // Agregar un nuevo producto a la orden
                var addedProduct = await orderBuyService.AddProductPrice(
                    actualOrder.Id,
                    actualOrderViewModel.SelectedProductId.Value,
                    actualOrderViewModel.Quantity.Value
                );

                if (addedProduct)
                {
                    // Recargar la orden actualizada y regresar la vista
                    actualOrder = await orderBuyService.GetActiveOrder();
                    return View(BuildViewModel(actualOrder, products));
                }
            }

            // Si algo falla, redirigir al formulario de agregar orden
            return RedirectToAction("AddOrder");
        }
        // Método auxiliar para construir el modelo de vista
        private ActualOrderViewModel BuildViewModel(OrderBuy actualOrder, IEnumerable<Product> products, ActualOrderViewModel? inputModel = null)
        {
            return new ActualOrderViewModel
            {
                NOrderBuy = actualOrder.NOrderBuy,
                Id = actualOrder.Id,
                Date = actualOrder.Date,
                Import = actualOrder.Import,
                Discount = actualOrder.Discount,
                Client = actualOrder.Client,
                Turn = actualOrder.Turn,
                Products = products.ToList(),
                Products_Prices = actualOrder.Products_Prices.ToList(),
                SelectedProductId = inputModel?.SelectedProductId,
                Quantity = inputModel?.Quantity ?? 0
            };
        }


        //envia el formulario para eliminar a ese ordenes, recibe un id y borra ese ordenes o devuelve un mensaje de error
        [HttpPost]
        public async Task<IActionResult> DeleteProductPrice(Guid productPriceId)
        {
            var productPriceDeleted = await orderBuyService.DeleteProductPrice(productPriceId);
            if(productPriceDeleted)
            {
                //show success 
                return RedirectToAction("GetActualOrder");
            }
            //show miss
            return RedirectToAction("GetActualOrder");
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id, bool cashpay)
        {
            var order = await orderBuyService.GetAsync(id);
            var importGotted = await orderBuyService.GetImport(id, cashpay);
            if (order == null)
            {
                return NotFound();
            }
            var sortedProducts = order.Products_Prices
            .OrderBy(pp => pp.Product.NProduct)
            .ToList();
            var viewModel = new OrderBuy
            {
                NOrderBuy = order.NOrderBuy,
                Id = order.Id,
                Date = order.Date,
                Import = order.Import,
                Discount = order.Discount,
                TotalDiscount = order.TotalDiscount,
                FinalTotal = order.FinalTotal,
                Client = order.Client,
                Turn = order.Turn,
                IsClosed = order.IsClosed,
                CashPay = order.CashPay,
                Products_Prices = sortedProducts
            };

            return View(viewModel);
        }
    }
}
