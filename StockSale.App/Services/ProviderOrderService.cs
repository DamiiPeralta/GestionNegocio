using StockSale.App.Models.Domain;
using StockSale.App.Repositories.Interfaces;

namespace StockSale.App.Services
{
    public class ProviderOrderService
    {
        private readonly IProviderOrderRepository providerOrderRepository;
        private readonly IProviderRepository providerRepository;
        private readonly IProductPriceRepository productPriceRepository;
        private readonly IProductRepository productRepository;
        private readonly ITurnRepository turnRepository;

        public ProviderOrderService(IProviderOrderRepository providerOrderRepository, IProviderRepository providerRepository, IProductPriceRepository productPriceRepository, IProductRepository productRepository, ITurnRepository turnRepository)
        {
            this.providerOrderRepository = providerOrderRepository;
            this.providerRepository = providerRepository;
            this.productPriceRepository = productPriceRepository;
            this.productRepository = productRepository;
            this.turnRepository = turnRepository;
        }
        public async Task<ProviderOrder> AddAsync(ProviderOrder providerOrder)
        {
            var lastAdded = await providerOrderRepository.LastAdded();
            int N = 1;
            if (lastAdded != null)
            {
                N = lastAdded.NProviderOrder + 1;
            }
            var newProviderOrder = new ProviderOrder()
            {
                NProviderOrder = N,
                Id = providerOrder.Id,
                Date = providerOrder.Date,
                Provider = providerOrder.Provider
            };
            return await providerOrderRepository.AddAsync(newProviderOrder);
        }

        public async Task<bool> AddProductPrice(Guid providerOrderId, Guid productId, int quantity)
        {
            var product = await productRepository.GetAsync(productId);
            var providerOrder = await providerOrderRepository.GetAsync(providerOrderId);
            if (product != null && providerOrder != null)
            {

                var productPrice = new Product_Price()
                {
                    Product = product,
                    Quantity = quantity,
                    PriceList = product.List_Price,
                    PriceSell = product.Sell_Price
                };
                await productPriceRepository.AddAsync(productPrice);
                providerOrder.Products_Prices.Add(productPrice);
                var updatedOrder = await providerOrderRepository.UpdateAsync(providerOrder);
                if (updatedOrder != null)
                {
                    return true;
                }

            }
            return false;

        }

        public async Task<bool> UpdateProductPrice(Guid providerOrderId, Product_Price updatedProductPrice)
        {
            var providerOrder = await providerOrderRepository.GetAsync(providerOrderId);

            if (providerOrder != null)
            {
                // Buscar el producto en la lista de la orden
                var existingProductPrice = providerOrder.Products_Prices
                    .FirstOrDefault(pp => pp.Product.Id == updatedProductPrice.Product.Id);

                if (existingProductPrice != null)
                {
                    // Actualizar la cantidad y otros detalles si es necesario
                    existingProductPrice.Quantity = updatedProductPrice.Quantity;
                    existingProductPrice.PriceList = updatedProductPrice.PriceList;
                    existingProductPrice.PriceSell = updatedProductPrice.PriceSell;

                    // Guardar los cambios en la base de datos
                    var updatedOrder = await providerOrderRepository.UpdateAsync(providerOrder);
                    return updatedOrder != null;
                }
            }

            return false;
        }

        public async Task<IEnumerable<ProviderOrder>> GetAllAsync()
        { 
            return await providerOrderRepository.GetAllAsync();
        }

        public async Task<ProviderOrder?> GetAsync(Guid id)
        {
            return await providerOrderRepository.GetAsync(id);
        }

        public async Task<ProviderOrder?> CloseProviderOrder(Guid providerId)
        {
            return await providerOrderRepository.CloseProviderOrder(providerId);

        }

        public async Task<ProviderOrder?> UpdateAsync(ProviderOrder providerOrder)
        {
            return await providerOrderRepository.UpdateAsync(providerOrder);
        }

        public async Task<ProviderOrder?> DeleteAsync(Guid id)
        {
            return await providerOrderRepository.DeleteAsync(id);
        }

        public async Task<ProviderOrder?> GetIsActiveOrder(Guid id)
        {
            return await providerOrderRepository.GetIsActiveOrder(id);
        }

        public async Task<List<ProviderOrder>> GetOrdersByProviderIdAsync(Guid id)
        {
            // Llama al repositorio para obtener las órdenes del proveedor
            return await providerOrderRepository.GetOrdersByProviderIdAsync(id);
        }

        public async Task<ProviderOrder?> GetOrCreateIsActiveOrder(Guid id)
        {
            var activeOrder = await providerOrderRepository.GetIsActiveOrder(id);
            if (activeOrder != null)
            {
                return activeOrder;
            }
            var lastAdded = await providerOrderRepository.LastAdded();
            int N = 1;
            if (lastAdded != null)
            {
                N = lastAdded.NProviderOrder + 1;
            }
            var provider = await providerRepository.GetAsync(id);
            if(provider != null)
            {
            var newOrder = new ProviderOrder
            {
                NProviderOrder = N,
                Id = Guid.NewGuid(),
                Date = DateTime.Now,
                Provider = provider,
                IsClosed = false
            };
            await providerOrderRepository.AddAsync(newOrder);
            return await providerOrderRepository.GetIsActiveOrder(id);
            }
            return null;
        }
    }
}
