using StockSale.App.Models.Domain;
using StockSale.App.Repositories;
using StockSale.App.Repositories.Interfaces;

namespace StockSale.App.Services
{
    public class OrderBuyService
    {
        private readonly IOrderBuyRepository orderBuyRepository;
        private readonly IProductRepository productRepository;
        private readonly IProductPriceRepository productPriceRepository;
        private readonly IClientRepository clientRepository;

        public OrderBuyService(
            IOrderBuyRepository orderBuyRepository,
            IProductRepository productRepository,
            IProductPriceRepository productPriceRepository,
            IClientRepository clientRepository
                )
        {
            this.orderBuyRepository=orderBuyRepository;
            this.productRepository=productRepository;
            this.productPriceRepository=productPriceRepository;
            this.clientRepository = clientRepository;
        }
        public async Task<IEnumerable<OrderBuy>> GetAllAsync() 
        {
            return await orderBuyRepository.GetAllAsync();
        }

        public async Task<OrderBuy?> GetAsync(Guid id)
        {
            return await orderBuyRepository.GetAsync(id);
        }

        public async Task<OrderBuy> AddAsync(OrderBuy order)
        {
            var orderBuy = await orderBuyRepository.AddAsync(order);
            return orderBuy;
        }
        public async Task<OrderBuy?> DeleteAsync(Guid id)
        {
            return await orderBuyRepository.DeleteAsync(id);
        }
        public async Task<OrderBuy?> CloseOrder(Guid id, bool cashpay)
        {
            var order = await orderBuyRepository.GetAsync(id);
            if (order != null)
            {
                // Asegurar que CashPay se actualiza correctamente
                order.CashPay = cashpay;

                // Cerrar la orden
                order.IsClosed = true;

                var updatedOrder = await orderBuyRepository.UpdateAsync(order);
                return updatedOrder;
            }
            return null;
        }

        public Product_Price? ExistProduct(List<Product_Price> productsPrices, Guid productId)
        {
            foreach(var product in productsPrices )
            {
                if(product.Product.Id == productId)
                {
                    return product;
                }
            }
            return null;
        }

        public async Task<bool> AddProductPrice(Guid orderBuyId, Guid productId , int quantity)
        {
            var product = await productRepository.GetAsync(productId);
            var order = await orderBuyRepository.GetAsync(orderBuyId);
            if( product != null && order != null)
            {
                
                var productPrice = new Product_Price()
                {
                    Product = product,
                    Quantity = quantity,
                    PriceList = product.List_Price,
                    PriceSell = product.Sell_Price
                };
                await productPriceRepository.AddAsync(productPrice);
                order.Products_Prices.Add(productPrice);
                var updatedOrder = await orderBuyRepository.UpdateAsync(order);
                if(updatedOrder != null)
                {
                    return true;
                }
            }
            return false;
            
        }

        public async Task<OrderBuy?> AddClientToOrder(Guid clientId)
        {
            var actualOrder = await GetActiveOrder();
            var client = await clientRepository.GetAsync(clientId);
            if (actualOrder != null && client != null)
            {
                actualOrder.Client = client;
                actualOrder.Discount = client.Discount;
                var updatedOrder = await orderBuyRepository.UpdateAsync(actualOrder);
                return updatedOrder;
            }
            return null;
        }
        public async Task<Client?> RemoveClientFromOrder()
        {
            var actualOrder = await GetActiveOrder();
            if (actualOrder != null)
            {
                if(actualOrder.Client != null)
                {
                var client = actualOrder.Client;
                actualOrder.Client = null;
                actualOrder.Discount = 0;
                var updatedOrder = await orderBuyRepository.UpdateAsync(actualOrder);
                if(updatedOrder != null)
                    {
                        return client;
                    }
                }
            }
            return null;
        }

        public async Task<bool> DeleteProductPrice(Guid productPriceId)
        {
            var productPrice = await productPriceRepository.GetByIdAsync(productPriceId);
            if(productPrice != null)
            {
                await productPriceRepository.DeleteAsync(productPriceId);
                return true;
            }
            return false;
        }
        public async Task<OrderBuy?> GetActiveOrder()
        {
            return await orderBuyRepository.GetOpenOrderAsync();
        }

        public async Task<OrderBuy?> LastAdded()
        {
            return await orderBuyRepository.LastAdded();
        }

        public async Task<OrderBuy?> UpdateAsync(OrderBuy orderBuy)
        {
            return await orderBuyRepository.UpdateAsync(orderBuy);
        }

        public async Task<IEnumerable<OrderBuy>> ListOnPageAsync(int pageNumber = 1)
        {
            const int pageSize = 5;

            // Trae todos los proveedores
            var allOrderBuys = (await orderBuyRepository.GetAllAsync()).ToList();

            // Calcula el total de proveedores
            var totalOrderBuys = allOrderBuys.Count;

            // Calcula el total de páginas
            var totalPages = (int)Math.Ceiling(totalOrderBuys / (double)pageSize);

            // Obtiene los proveedores para la página actual (salta los anteriores y toma solo 5)
            var orderBuysOnPage = allOrderBuys
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            // Devuelve un resultado que incluye los proveedores de la página actual y el total de páginas
            return orderBuysOnPage;
        }

        public async Task<float?> GetImport(Guid id, bool cashpay)
        {
            var orderBuy = await orderBuyRepository.GetAsync(id);
            if (orderBuy != null)
            {
                var total = orderBuy.Products_Prices.Sum(p => p.PriceSell * p.Quantity);

                if (cashpay)
                {
                    orderBuy.Discount = Math.Min(orderBuy.Discount + 10, 100);
                }

                orderBuy.TotalDiscount = total * orderBuy.Discount / 100;
                orderBuy.Import = total;
                orderBuy.FinalTotal = total - orderBuy.TotalDiscount;

                var updated = await orderBuyRepository.UpdateAsync(orderBuy);
                if (updated != null)
                {
                    return orderBuy.FinalTotal;
                }
            }
            return null;
        }
    }
}
