using Microsoft.AspNetCore.Mvc;
using StockSale.App.Services;
using StockSale.App.Models.ViewModels;
using StockSale.App.Models.Domain;
using System.Threading.Tasks;
using StockSale.App.Repositories;
using StockSale.App.Repositories.Interfaces;
using StockSale.App.Data;

namespace StockSale.App.Controllers
{
    public class TurnController : Controller
    {
        private readonly TurnService _turnService;
        private readonly OrderBuyService orderBuyService;
        private readonly PaginationService paginationService;
        private readonly FiadosService fiadosService;
        private readonly ClientService clientService;

        public TurnController(TurnService turnService, OrderBuyService orderBuyService, PaginationService paginationService, FiadosService fiadosService, ClientService clientService)
        {
            _turnService = turnService;
            this.orderBuyService = orderBuyService;
            this.paginationService = paginationService;
            this.fiadosService = fiadosService;
            this.clientService = clientService;
        }
        [HttpGet]
        public IActionResult StartNewTurn()
        {

            return View();

        }
        [HttpPost]
        public async Task<IActionResult> StartNewTurn(TurnViewModel model)
        {
            var newTurn = await _turnService.StartTurnAsync(model.Initial_Cash);
            if (newTurn != null)
            {
                return RedirectToAction("ActualTurn");
            }

            return View();

        }

        [HttpGet]
        public async Task<IActionResult> ActualTurn(
            string searchString,
            string? sortBy,
            string? sortDirection,
            int pageSize = 10,
            int pageNumber = 1)
        {
            var actualTurn = await _turnService.GetActualTurn();

            if (actualTurn != null)
            {
                // Total de órdenes asociadas al turno actual
                var totalOrders = actualTurn.OrderBuys.Count;

                // Aplicar el servicio de paginación
                var paginatedResult = paginationService.GetPaginatedData(
                    actualTurn.OrderBuys,
                    pageNumber,
                    pageSize,
                    sortBy,
                    sortDirection,
                    t => string.IsNullOrEmpty(searchString) ||
                         t.Client.Name.Contains(searchString, StringComparison.OrdinalIgnoreCase) // Ejemplo de filtro
                );

                var turnViewModel = new TurnViewModel
                {
                    NTurn = actualTurn.NTurn,
                    Id = actualTurn.Id,
                    ActualTurn = actualTurn,
                    Actual_Cash = actualTurn.Actual_Cash,
                    Initial_Cash = actualTurn.Initial_Cash,
                    OrderBuys = paginatedResult.Data
                };

                // Pasar datos de paginación a la vista
                ViewBag.VisiblePages = paginatedResult.VisiblePages;
                ViewBag.TotalPages = paginatedResult.TotalPages;
                ViewBag.PageNumber = paginatedResult.PageNumber;
                ViewBag.PageSize = paginatedResult.PageSize;

                return View(turnViewModel);
            }
            else
            {
                return RedirectToAction("StartNewTurn");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Details(Guid id)
        {
            var turn = await _turnService.GetTurnAsync(id);
            if(turn != null)
            {
                var cashFlows = await _turnService.GetCashFlowByDate(turn.Date.Date);
                var fiados = await fiadosService.GetFiadosByDate(turn.Date.Date);
                var debts = await clientService.GetDebtsByDate(turn.Date.Date);
                var turnDetails = new TurnDetailsViewModel()
                { 
                    Turn = turn,
                    CashFlows = cashFlows.ToList(),
                    Fiados = fiados.ToList(),
                    Debts = debts.ToList()
                };
                return View(turnDetails);
            }
            return View(null);
        }

        [HttpPost]
        public async Task<IActionResult> Details(Turn detailTurn)
        {
            var turn = new Turn
            {
                NTurn = detailTurn.NTurn,
                Id = detailTurn.Id,
                Actual_Cash = detailTurn.Actual_Cash,
                Closing_Cash = detailTurn.Closing_Cash,
                Initial_Cash = detailTurn.Initial_Cash,
                OrderBuys = detailTurn.OrderBuys.ToList(),
                Date = detailTurn.Date,
                Is_Closed = detailTurn.Is_Closed,
            };
            var updatedTurn = await _turnService.UpdateAsync(turn);
            if (updatedTurn != null)
            {
                //success
                return View(updatedTurn);
            }
            //error
            return View(detailTurn);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(Guid id)
        {
            var turn = await _turnService.DeleteAsync(id);
            if (turn != null)
            {
                return RedirectToAction("ListPaginated");
            }
            return RedirectToAction("Details", id);
        }

        [HttpPost]
        public async Task<IActionResult> CloseTurn()
        {
            var turn = await _turnService.CloseTurnAsync();

            if (turn != null)
            {
                return RedirectToAction("ListPaginated");
            }
            else
            {
                return RedirectToAction("ActualTurn");
            }
        }

        [HttpGet]
        public async Task<IActionResult> ListPaginated(
            string searchString,
            string? sortBy,
            string? sortDirection,
            int pageSize = 10,
            int pageNumber = 1)
        {
            var allProviderOrders = await _turnService.GetAllTurnsAsync();

            // Utilizar el servicio de paginado
            var paginatedResult = paginationService.GetPaginatedData(
                allProviderOrders,
                pageNumber,
                pageSize,
                sortBy,
                sortDirection,
                t => string.IsNullOrEmpty(searchString) ||
                     (DateTime.TryParse(searchString, out var parsedDate) && t.Date.Date == parsedDate.Date)
            );

            // Pasar datos a la vista
            ViewBag.VisiblePages = paginatedResult.VisiblePages;
            ViewBag.TotalPages = paginatedResult.TotalPages;
            ViewBag.PageNumber = paginatedResult.PageNumber;
            ViewBag.PageSize = paginatedResult.PageSize;

            return View(paginatedResult.Data);
        }

        [HttpGet]
        public async Task<IActionResult> CashFlowList(
            string searchString,
            string? sortBy,
            string? sortDirection,
            int pageSize = 10,
            int pageNumber = 1)
        {
            // Si searchString no está definido, asignamos la fecha actual
            if (string.IsNullOrEmpty(searchString))
            {
                searchString = DateTime.Now.ToString("yyyy-MM-dd"); // Formato de fecha que se espera en el filtro
            }

            var cashFlow = await _turnService.GetAllCashFlows();
            var paginatedResult = paginationService.GetPaginatedData(
            cashFlow,
            pageNumber,
            pageSize,
            sortBy,
            sortDirection,
            t => string.IsNullOrEmpty(searchString) ||
            (DateTime.TryParse(searchString, out var parsedDate) && t.Date.Date == parsedDate.Date)
            );

            // Pasar datos a la vista
            ViewBag.VisiblePages = paginatedResult.VisiblePages;
            ViewBag.TotalPages = paginatedResult.TotalPages;
            ViewBag.PageNumber = paginatedResult.PageNumber;
            ViewBag.PageSize = paginatedResult.PageSize;

            return View(paginatedResult.Data);
        }

        [HttpPost]
        public async Task<IActionResult> AddCashFlow(int import, bool isIncome, string? comment)
        {
            var newCashFlow = await _turnService.AddCashFlow(import, isIncome, comment);
            if(newCashFlow)
            {
                //show success
                return RedirectToAction("CashFlowList");
            }
            else
            {
                //show error
                return RedirectToAction("CashFlowList");
            }
        }

        [HttpGet]
        public async Task<IActionResult> Resume(
    DateTime? from = null,
    DateTime? to = null,
    string? period = null, // "day", "week", "month", "year"
    int pageSize = 10,
    int pageNumber = 1
)
        {
            var allTurns = await _turnService.GetAllTurnsAsync();
            var allOrderBuys = allTurns
                .SelectMany(t => t.OrderBuys ?? new List<OrderBuy>())
                .Where(ob => ob.IsClosed && !ob.IsDeleted)
                .ToList();

            var allCashFlows = await _turnService.GetAllCashFlows(); // Obtenemos todos los cashflows

            DateTime today = DateTime.Today;

            if (!string.IsNullOrEmpty(period))
            {
                switch (period.ToLower())
                {
                    case "day":
                        allOrderBuys = allOrderBuys.Where(ob => ob.Date.Date == today).ToList();
                        allCashFlows = allCashFlows.Where(cf => cf.Date.Date == today).ToList();
                        break;

                    case "week":
                        var startOfWeek = today.AddDays(-(int)today.DayOfWeek);
                        allOrderBuys = allOrderBuys.Where(ob => ob.Date.Date >= startOfWeek && ob.Date.Date <= today).ToList();
                        allCashFlows = allCashFlows.Where(cf => cf.Date.Date >= startOfWeek && cf.Date.Date <= today).ToList();
                        break;

                    case "month":
                        allOrderBuys = allOrderBuys.Where(ob => ob.Date.Month == today.Month && ob.Date.Year == today.Year).ToList();
                        allCashFlows = allCashFlows.Where(cf => cf.Date.Month == today.Month && cf.Date.Year == today.Year).ToList();
                        break;

                    case "year":
                        allOrderBuys = allOrderBuys.Where(ob => ob.Date.Year == today.Year).ToList();
                        allCashFlows = allCashFlows.Where(cf => cf.Date.Year == today.Year).ToList();
                        break;
                }
            }
            else if (from.HasValue && to.HasValue)
            {
                allOrderBuys = allOrderBuys
                    .Where(ob => ob.Date.Date >= from.Value.Date && ob.Date.Date <= to.Value.Date)
                    .ToList();

                allCashFlows = allCashFlows
                    .Where(cf => cf.Date.Date >= from.Value.Date && cf.Date.Date <= to.Value.Date)
                    .ToList();
            }

            float totalImporte = allOrderBuys.Sum(ob => ob.FinalTotal);
            float totalGanancia = 0f;

            foreach (var order in allOrderBuys)
            {
                if (order.Products_Prices != null)
                {
                    foreach (var pp in order.Products_Prices)
                    {
                        totalGanancia += (pp.PriceSell - pp.PriceList) * pp.Quantity;
                    }
                }
            }

            float totalDescuentos = allOrderBuys.Sum(ob => ob.TotalDiscount);
            float totalGastos = allCashFlows.Where(cf => !cf.Income).Sum(cf => cf.Import);

            // Paginación de las órdenes
            var paginatedResult = paginationService.GetPaginatedData(
                allOrderBuys,
                pageNumber,
                pageSize,
                sortBy: null,
                sortDirection: null,
                filter: null
            );

            var resumen = new ResumeViewModel
            {
                TotalImporte = totalImporte,
                TotalGanancia = totalGanancia,
                TotalDescuentos = totalDescuentos,
                TotalGastos = totalGastos,
                Orders = paginatedResult.Data,
                CashFlows = allCashFlows.ToList()
            };

            ViewBag.VisiblePages = paginatedResult.VisiblePages;
            ViewBag.TotalPages = paginatedResult.TotalPages;
            ViewBag.PageNumber = paginatedResult.PageNumber;
            ViewBag.PageSize = paginatedResult.PageSize;

            return View(resumen);
        }

    }
}

