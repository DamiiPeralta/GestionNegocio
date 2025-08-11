using Microsoft.AspNetCore.Mvc;
using StockSale.App.Models.Domain;
using StockSale.App.Models.ViewModels;
using StockSale.App.Services;

namespace StockSale.App.Controllers
{
    public class FiadosController : Controller
    {
        private readonly FiadosService fiadosService;
        private readonly PaginationService paginationService;

        public FiadosController(FiadosService fiadosService, PaginationService paginationService)
        {
            this.fiadosService = fiadosService;
            this.paginationService = paginationService;
        }

        //Este devuelve la vista que tiene el formulario para añadir fiados
        [HttpGet]
        public IActionResult Add()
        {
            return View();
        }

        //Este envia el formulario de clientes una vez que este completado, trata de crear un cliente en la base de datos o devuelve un error
        [HttpPost]
        public async Task<IActionResult> Add(AddFiadosViewModel addFiadosViewModel)
        {
            var lastAdded = await fiadosService.LastAdded();
            int N = 1;
            if (lastAdded != null)
            {
                N = lastAdded.NFiado + 1;
            }
            var fiado = new Fiado
            {
                NFiado = N,
                Id = Guid.NewGuid(),
                Name = addFiadosViewModel.Name,
                Date = addFiadosViewModel.Date,
                Import = addFiadosViewModel.Import,
                IsPay = false,
                LastDatePay = DateTime.Now
            };

            try
            {
                await fiadosService.AddAsync(fiado);
            }
            catch (Exception ex) 
            {
                ModelState.AddModelError(string.Empty, "Error al añadir el fiado. Intente nuevamente.");
                return View(addFiadosViewModel);
            }
            return RedirectToAction("ListPaginated");
        }

        //Devuelve la vista con la lista de todos los clientes
        [HttpGet]
        public async Task<IActionResult> List()
        {
            var fiados = await fiadosService.GetAllAsync();
            return View(fiados);
        }

        //Devuelve una vista que tiene el formulario para editar o eliminar a los clientes
        [HttpGet]
        public async Task<IActionResult> Edit(Guid id)
        {
            var fiado = await fiadosService.GetAsync(id);
            if (fiado != null)
            {
                var model = new Fiado
                {
                    NFiado = fiado.NFiado,
                    Id = fiado.Id,
                    Name = fiado.Name,
                    Date = fiado.Date,
                    Import = fiado.Import,
                    IsPay = fiado.IsPay,
                    LastDatePay = fiado.LastDatePay
                };
                return View(model);
            }
            return View(null);
        }

        //envia el formulario con los nuevos datos y edita a ese clinte o devuelve un error
        [HttpPost]
        public async Task<IActionResult> Edit(Fiado fiado)
        {
            //Submit information to repository to update
            var updateFiado = await fiadosService.UpdateAsync(fiado);

            if (updateFiado != null)
            {
                //aca mostra un mensaje de exito
                return RedirectToAction("ListPaginated");
            }
            //aca un mensaje de error
            return RedirectToAction("Edit");
        }


        //envia el formulario para eliminar a ese cliente, recibe un id y borra ese cliente o devuelve un mensaje de error
        [HttpPost]
        public async Task<IActionResult> Delete(Fiado fiado)
        {
            var deletedFiado = await fiadosService.DeleteAsync(fiado.Id);
            if (deletedFiado != null)
            {
                //aca mostra un mensaje de exito
                return RedirectToAction("ListPaginated");
            }

            //aca mostra un mensaje de error
            return RedirectToAction("Edit", new { id = fiado.Id });
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
            var allClients = await fiadosService.GetAllAsync();

            // Utilizar el servicio de paginado
            var paginatedResult = paginationService.GetPaginatedData(
                allClients,
                pageNumber,
                pageSize,
                sortBy,
                sortDirection,
                p => string.IsNullOrEmpty(searchString) ||
                     p.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase)
            );

            // Pasar datos a la vista
            ViewBag.VisiblePages = paginatedResult.VisiblePages;
            ViewBag.TotalPages = paginatedResult.TotalPages;
            ViewBag.PageNumber = paginatedResult.PageNumber;
            ViewBag.PageSize = paginatedResult.PageSize;

            return View(paginatedResult.Data);
        }

        [HttpPost]
        public async Task<IActionResult> FiadoPayDebt (Guid id)
        {
            
            var fiadoPay = await fiadosService.FiadosPay(id);
            if(fiadoPay != null)
            {
            fiadoPay.LastDatePay = DateTime.Now;
            var updatedFiadoPay = await fiadosService.UpdateAsync(fiadoPay);
            if(updatedFiadoPay != null)
                {
                //show success
               return RedirectToAction("ListPaginated");
                }
            }
                //show error
               return RedirectToAction("ListPaginated");
        }
    }
}
