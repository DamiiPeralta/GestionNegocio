using StockSale.App.Models.Domain;
using StockSale.App.Services;
using StockSale.App.Repositories.Interfaces;
using StockSale.App.Repositories;

namespace StockSale.App.Services
{
    public class ProductService
    {
        private readonly IProductRepository productRepository;
        private readonly IProviderRepository providerRepository;

        public ProductService(IProductRepository productRepository, IProviderRepository providerRepository)
        {
            this.productRepository = productRepository;
            this.providerRepository = providerRepository;
        }
        public async Task<bool> CheckStock(Guid productId, int quantity)
        {
            var product = await productRepository.GetAsync(productId);
            if (product != null && product.Stock >= quantity)
            {
                return true;
            }
            return false;
        }

        public async Task<Product> AddAsync(Product product)
        {
            return await productRepository.AddAsync(product);
        }

        public async Task<bool> AddStock(Guid productId, int quantity)
        {
            var product = await productRepository.GetAsync(productId);
            if (product != null)
            {
                product.Stock = product.Stock += quantity;
                    var updatedProduct = await productRepository.UpdateAsync(product);
                    if(updatedProduct != null)
                    {
                        return true;
                    }
            }
                    return false;
        }
        public async Task<bool> AddStockFromOrder(List<Product_Price> product_Prices)
        {
            foreach (var product in product_Prices)
            {
                var isStock = await this.AddStock(product.Product.Id, product.Quantity);
                if (!isStock)
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<bool> RemoveStock(Guid productId, int quantity)
        {
            var product = await productRepository.GetAsync(productId);
            if (product != null)
            {
                var isStock = await this.CheckStock(productId, quantity);
                if (isStock)
                {
                    product.Stock = product.Stock -= quantity;
                    var updatedProduct = await productRepository.UpdateAsync(product);
                    if (updatedProduct != null)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        public async Task<bool> RemoveStockFromOrder(List<Product_Price> product_Prices)
        {
            // handle error
            foreach (var product in product_Prices)
            {
                var isStock = await this.CheckStock(product.Product.Id, product.Quantity);
                if (!isStock)
                {
                    return false;
                }
            }
            foreach (var product in product_Prices)
            {
                var isStock = await this.RemoveStock(product.Product.Id, product.Quantity);
                if (!isStock)
                {
                    return false;
                }
            }
            return true;
        }

        public async Task<Product?> DeleteAsync(Guid id)
        {
            return await productRepository.DeleteAsync(id);
        }

        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await productRepository.GetAllAsync();
        }

        public async Task<Product?> GetAsync(Guid id)
        {
            return await productRepository.GetAsync(id);
        }

        public async Task<Product?> LastAdded()
        {
            return await productRepository.LastAdded();
        }

        public async Task<Product?> UpdateAsync(Product product)
        {
            return await productRepository.UpdateAsync(product);
        }
        public async Task<IEnumerable<Product>?> GetProductsByProvider(Guid id)
        {
            var provider = await providerRepository.GetAsync(id);
            var products = provider.Products;
            return products;
        }

        public async Task<IEnumerable<Product>> ListOnPageAsync(Guid? id, int pageNumber = 1)
        {
            const int pageSize = 5;

            // Trae todos los proveedores
            var allProducts = (await productRepository.GetAllAsync()).ToList();

            if (id.HasValue)
            {
                var provider = await providerRepository.GetAsync(id.Value);
                if (provider != null)
                {
                    allProducts = (await this.GetProductsByProvider(provider.Id))?.ToList() ?? new List<Product>();
                }
            }

            // Calcula el total de proveedores
            var totalProducts = allProducts.Count;

            // Calcula el total de páginas
            var totalPages = (int)Math.Ceiling(totalProducts / (double)pageSize);

            // Obtiene los proveedores para la página actual (salta los anteriores y toma solo 5)
            var productsOnPage = allProducts
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Devuelve un resultado que incluye los proveedores de la página actual y el total de páginas
            return productsOnPage;
        }

        public async Task<IEnumerable<Product>> HasStockLimit()
        {
            return await productRepository.HasStockLimit();
        }
    }
}
