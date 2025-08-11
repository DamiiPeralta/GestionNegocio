using StockSale.App.Models.Domain;
using StockSale.App.Repositories.Interfaces;
using System.Collections;

namespace StockSale.App.Services

{
    public class ProductPriceService
    {
        private readonly IProductPriceRepository productPriceRepository;

        public ProductPriceService(IProductPriceRepository productPriceRepository)
        {
            this.productPriceRepository = productPriceRepository;
        }

        public async Task<Product_Price?> UpdateAsync(Product_Price product_Price)
        {
            return await productPriceRepository.UpdateAsync(product_Price);
        }
    }
}
