using Microsoft.AspNetCore.Mvc;
using StockSale.App.Models.Domain;
using StockSale.App.Repositories;
using StockSale.App.Repositories.Interfaces;
using StockSale.App.Services;

namespace StockSale.App.Services
{
    public class ProviderService
    {
        private readonly IProviderRepository providerRepository;
        private readonly ProductService productService;

        public ProviderService(IProviderRepository providerRepository, ProductService productService)
        {
            this.providerRepository = providerRepository;
            this.productService = productService;
        }
        public async Task<IEnumerable<Provider>> GetAllAsync()
        {
            return await providerRepository.GetAllAsync();
        }

        public async Task<Provider?> GetAsync(Guid id)
        {
            return await providerRepository.GetAsync(id);
        }

        public async Task<Provider> AddAsync(Provider provider)
        {
            return await providerRepository.AddAsync(provider);
        }
        public async Task<int> CountProviderAsync(string? searchQuery)
        {
            return await providerRepository.CountAsync(searchQuery);
        }
        public async Task<Provider> UpdateAsync(Provider provider)
        {
            if (provider != null)
            {
                return await providerRepository.UpdateAsync(provider);
            }
            return null;
        }
        public async Task<IEnumerable<Product>?> GetProductsByProvider(Guid id)
        {
            var provider = await providerRepository.GetAsync(id);
            if (provider != null)
            {
                var products = await productService.ListOnPageAsync(id); // Aquí ya no es nullable
                return products;
            }
            return null;
        }
        public async Task<Provider?> LastAdded()
        {
            return await providerRepository.LastAdded();
        }
        public async Task<Provider?> DeleteProviderAsync(Guid id)
        {
            var providerDeleted = await providerRepository.DeleteAsync(id);
            if(providerDeleted != null)
            {
                var providerProducts = providerDeleted.Products;
                if(providerProducts != null)
                {
                    foreach (var product in providerProducts)
                    {
                        var productDeleted = await productService.DeleteAsync(product.Id);
                    }
                    return providerDeleted;
                }
                return providerDeleted;
            }
            return null;
        }
        public async Task<IEnumerable<Provider>> ListPaginated(
          string? searchQuery,
          string? sortBy,
          string? sortDirection,
          int pageNumber,
          int pageSize)
        {
            return await providerRepository.GetAllPaginatedAsync(
                searchQuery, sortBy, sortDirection, pageNumber, pageSize);
        }

    }
}
