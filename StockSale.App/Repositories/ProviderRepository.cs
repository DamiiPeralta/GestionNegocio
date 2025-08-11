using Azure;
using Microsoft.EntityFrameworkCore;
using StockSale.App.Data;
using StockSale.App.Models.Domain;
using StockSale.App.Repositories.Interfaces;

namespace StockSale.App.Repositories
{
    public class ProviderRepository : IProviderRepository
    {
        private readonly StockSaleDbContext stockSaleDbContext;

        public ProviderRepository(StockSaleDbContext stockSaleDbContext)
        {
            this.stockSaleDbContext = stockSaleDbContext;
        }
        public async Task<Provider> AddAsync(Provider provider)
        {
            await stockSaleDbContext.AddAsync(provider);
            await stockSaleDbContext.SaveChangesAsync();
            return provider;
        }
        public async Task<int> CountAsync(string? searchQuery)
        {
            var query = stockSaleDbContext.Providers.AsQueryable();

            // Filtering
            if (string.IsNullOrWhiteSpace(searchQuery) == false)
            {
                query = query.Where(x => x.Name.Contains(searchQuery));
            }

            return await query.CountAsync();
        }
        public async Task<Provider?> DeleteAsync(Guid id)
        {
            var existingProvider = await stockSaleDbContext.Providers
                .Where(x => x.IsDeleted == false)
                .Include(x => x.ProviderOrders)
                .Include(x => x.Products)
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id);
            if (existingProvider != null)
            {
                existingProvider.IsDeleted = true; // Marcar como eliminado
                await stockSaleDbContext.SaveChangesAsync();
                return existingProvider;
            }
            return null;
        }
        public async Task<IEnumerable<Provider>> GetAllAsync()
        {
            return await stockSaleDbContext.Providers
                .Where(x => x.IsDeleted == false)
                .OrderBy(x => x.NProvider)
                .Include(x => x.ProviderOrders)
                .Include(x => x.Products)
                .Where(x => x.IsDeleted == false)
                .ToListAsync();
        }

        public async Task<Provider?> GetAsync(Guid id)
        {
            return await stockSaleDbContext.Providers
                .Where(x => x.IsDeleted == false)
                .Include(x => x.ProviderOrders)
                .Include(x => x.Products)
                .ThenInclude(x => x.UnitM)
                .Where(x => x.IsDeleted == false)
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        public async Task<Provider?> UpdateAsync(Provider provider)
        {
            var existingProvider = await stockSaleDbContext.Providers
                .FirstOrDefaultAsync(x => x.Id == provider.Id);

            if (existingProvider != null)
            {
                existingProvider.Name = provider.Name;
                existingProvider.Phone = provider.Phone;
                existingProvider.Email = provider.Email;
                existingProvider.Company = provider.Company;
                existingProvider.Products = provider.Products;
                existingProvider.ProviderOrders = provider.ProviderOrders;

                await stockSaleDbContext.SaveChangesAsync();
                return existingProvider;
            }
            return null;
        }
        public async Task<IEnumerable<Provider>> GetAllPaginatedAsync(
        string? searchQuery,
        string? sortBy,
        string? sortDirection,
        int pageNumber = 1,
        int pageSize = 100)
        {
            var query = stockSaleDbContext.Providers
                .Where(x => x.IsDeleted == false)
                .AsQueryable();

            // Filtering
            if (string.IsNullOrWhiteSpace(searchQuery) == false)
            {
                query = query.Where(x => x.Name.Contains(searchQuery) ||
                                         x.Company.Contains(searchQuery));
            }

            // Sorting
            if (string.IsNullOrWhiteSpace(sortBy) == false)
            {
                var isDesc = string.Equals(sortDirection, "Desc", StringComparison.OrdinalIgnoreCase);

                if (string.Equals(sortBy, "Name", StringComparison.OrdinalIgnoreCase))
                {
                    query = isDesc ? query.OrderByDescending(x => x.Name) : query.OrderBy(x => x.Name);
                }

                if (string.Equals(sortBy, "DisplayName", StringComparison.OrdinalIgnoreCase))
                {
                    query = isDesc ? query.OrderByDescending(x => x.Company) : query.OrderBy(x => x.Company);
                }
            }
            // Pagination
            // Skip 0 Take 5 -> Page 1 of 5 results
            // Skip 5 Take next 5 -> Page 2 of 5 results
            var skipResults = (pageNumber - 1) * pageSize;
            query = query.Skip(skipResults).Take(pageSize);

            return await query.ToListAsync();
        }
        public async Task<Provider?> LastAdded()
        {
            if (!await stockSaleDbContext.Providers.AnyAsync())
            {
                return null;
            }
            var maxNum = await stockSaleDbContext.Providers
                .MaxAsync(x => x.NProvider);

            return await stockSaleDbContext.Providers
                .Where(x => x.NProvider == maxNum)
                .FirstOrDefaultAsync();
        }
    }
}