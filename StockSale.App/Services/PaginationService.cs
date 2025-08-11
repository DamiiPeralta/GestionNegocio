namespace StockSale.App.Services
{
    public class PaginationService
    {
        public PaginatedResult<T> GetPaginatedData<T>(
            IEnumerable<T> data,
            int pageNumber,
            int pageSize,
            string? sortBy,
            string? sortDirection,
            Func<T, bool> filter)
        {
            // Filtrar y ordenar los datos
            var filteredData = data.Where(filter);

            if (!string.IsNullOrEmpty(sortBy))
            {
                filteredData = sortDirection == "desc"
                    ? filteredData.OrderByDescending(x => x.GetType().GetProperty(sortBy).GetValue(x, null))
                    : filteredData.OrderBy(x => x.GetType().GetProperty(sortBy).GetValue(x, null));
            }

            // Paginación
            var totalItems = filteredData.Count();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var paginatedData = filteredData
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Generar las páginas visibles
            var visiblePages = GetVisiblePages(pageNumber, totalPages);

            return new PaginatedResult<T>
            {
                Data = paginatedData,
                PageNumber = pageNumber,
                PageSize = pageSize,
                TotalPages = totalPages,
                VisiblePages = visiblePages
            };
        }

        private List<int> GetVisiblePages(int currentPage, int totalPages)
        {
            var visiblePages = new List<int>();

            // Siempre incluir la primera página
            visiblePages.Add(1);

            // Rango dinámico de páginas
            int startPage = Math.Max(currentPage - 2, 2);
            int endPage = Math.Min(currentPage + 2, totalPages - 1);

            for (int i = startPage; i <= endPage; i++)
            {
                visiblePages.Add(i);
            }

            // Siempre incluir la última página si no está en el rango
            if (totalPages > 1 && !visiblePages.Contains(totalPages))
            {
                visiblePages.Add(totalPages);
            }

            return visiblePages;
        }
    }

    public class PaginatedResult<T>
    {
        public List<T> Data { get; set; }
        public int? TotalRecords { get; set; }
        public int? TotalPages { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public List<int>? VisiblePages { get; set; }
    }
}
