using Microsoft.AspNetCore.Mvc;
using StockSale.App.Models.Domain;
using StockSale.App.Models.ViewModels;
using StockSale.App.Services;

namespace StockSale.App.Controllers
{
    public class ClientController : Controller
    {
        
        private readonly OrderBuyService orderBuyService;
        private readonly PaginationService paginationService;
        private readonly ClientService clientService;

        public ClientController(OrderBuyService orderBuyService, PaginationService paginationService, ClientService clientService)
        {
            this.orderBuyService = orderBuyService;
            this.paginationService = paginationService;
            this.clientService = clientService;
        }
        //Este devuelve la vista que tiene el formulario para añadir clientes
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }
        //Este envia el formulario de clientes una vez que este completado, trata de crear un cliente en la base de datos o devuelve un error
        [HttpPost]
        public async Task<IActionResult> Add(AddClientRequest addClientRequest)
        {
            var lastAdded = await clientService.LastAdded();
            int N = 1;
            if (lastAdded != null)
            {
                N = lastAdded.NClient + 1;
            }
            // Crear el nuevo cliente con los datos proporcionados
            var client = new Client
            {
                NClient = N,
                Name = addClientRequest.Name,
                Email = addClientRequest.Email,
                Dni = addClientRequest.Dni,
                Phone = addClientRequest.Phone,
                Company = addClientRequest.Company,
                Condition_Iva = addClientRequest.Condition_Iva,
                Cuil_t = addClientRequest.Cuil_t,
                Discount = addClientRequest.Discount,
                IsDeleted = false,
                Debts = new List<Debt>() // Inicializar la deuda en 0
            };

            // Intentar añadir el cliente
            try
            {
                await clientService.AddAsync(client);
            }
            catch (Exception ex)
            {
                // Manejo de errores (loggear o mostrar un mensaje de error)
                ModelState.AddModelError(string.Empty, "Error al añadir el cliente. Intente nuevamente.");
                return View(addClientRequest);
            }

            // Redirigir a la misma acción o a otra vista
            return RedirectToAction("ListPaginated");
        }

        [HttpGet]
        public async Task<IActionResult> DebtsList(
        Guid id,
        string? searchString,
        string? sortBy,
        string? sortDirection,
        string? action = "AddDebt", // Acción por defecto
        int pageSize = 10,
        int pageNumber = 1)
        {
            // Obtener el cliente por ID
            var client = await clientService.GetAsync(id);

            if (client != null)
            {
                // Obtener la lista de deudas del cliente
                var debts = client.Debts; // Asegúrate de que esta propiedad existe y contiene las deudas

                // Utilizar el servicio de paginación
                var paginatedResult = paginationService.GetPaginatedData(
                    debts,
                    pageNumber,
                    pageSize,
                    sortBy,
                    sortDirection,
                    d => string.IsNullOrEmpty(searchString) ||
                         (DateTime.TryParse(searchString, out var parsedDate) && d.Date.Date == parsedDate.Date)
                );

                // Pasar datos de paginación a la vista
                ViewBag.VisiblePages = paginatedResult.VisiblePages;
                ViewBag.TotalPages = paginatedResult.TotalPages;
                ViewBag.PageNumber = paginatedResult.PageNumber;
                ViewBag.PageSize = paginatedResult.PageSize;

                // Pasar la acción al ViewBag para que el modal pueda utilizarla
                ViewBag.Action = action;
                ViewBag.ClientId = id; // Para asegurarnos de tener el ID del cliente en el modal

                return View(paginatedResult.Data);
            }

            // Si el cliente no existe, manejar el caso de error (puede ser redirigir o mostrar un mensaje)
            return NotFound("Cliente no encontrado.");
        }

        [HttpPost]
        public async Task<IActionResult> AddDebt(Guid id, bool isDebt, int import)
        {
            var newDebt = await clientService.AddDebt(id, isDebt, import);
            if(newDebt)
            {
                //show success
                return RedirectToAction("DebtsList", new { id = id });
            }
            else
            {
                //show error
                return RedirectToAction("DebtsList", new { id = id });
            }
        }

        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
         {
            var client = await clientService.GetAsync(id);
            if(client != null)
            {
                var model = new Client
                {
                    NClient = client.NClient,
                    Id = client.Id,
                    Name = client.Name,
                    Email = client.Email,
                    Dni = client.Dni,
                    Phone = client.Phone,
                    Company = client.Company,
                    Cuil_t = client.Cuil_t,
                    Condition_Iva = client.Condition_Iva,
                    Discount = client.Discount
                };
                return View(model);
            }
            return View(null);
        }
        //envia el formulario con los nuevos datos y edita a ese clinte o devuelve un error
        [HttpPost]
        public async Task<IActionResult> Edit(Client client)
        {
            var clientDomainModel = new Client
            {
                Id = client.Id,
                Name = client.Name,
                Email = client.Email,
                Dni = client.Dni,
                Phone = client.Phone,
                Company = client.Company,
                Cuil_t = client.Cuil_t,
                Condition_Iva = client.Condition_Iva,
                Discount = client.Discount
            };

            //Submit information to repository to update
            var updateClient = await clientService.UpdateClient(clientDomainModel);

            if (updateClient != null)
            {
                //aca mostra un mensaje de exito
                return RedirectToAction("ListPaginated");
            }
            //aca un mensaje de error
            return RedirectToAction("Edit");
        }
        //envia el formulario para eliminar a ese cliente, recibe un id y borra ese cliente o devuelve un mensaje de error
        [HttpPost]
        public async Task<IActionResult> Delete(Client client)
        {
            var deletedClient = await clientService.DeleteClient(client.Id);
            if (deletedClient != null)
            {
                //aca mostra un mensaje de exito
                return RedirectToAction("ListPaginated");
            }

            //aca mostra un mensaje de error
            return RedirectToAction("Edit", new { id = client.Id });
        }

        [HttpGet]
        public async Task<IActionResult> ListPaginated(
            string searchString,
            string? searchQuery,
            string? sortBy,
            string? sortDirection,
            int pageSize = 10,
            int pageNumber = 1)
        {
            var allClients = await clientService.GetAll();
            var activeOrder = await orderBuyService.GetActiveOrder();
            var hasActiveOrder = activeOrder != null;
            // Utilizar el servicio de paginado
            var paginatedResult = paginationService.GetPaginatedData(
                allClients,
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
            ViewBag.HasActiveOrder = hasActiveOrder;

            return View(paginatedResult.Data);
        }
    }
}