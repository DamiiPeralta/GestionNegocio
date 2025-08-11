using OfficeOpenXml;
using StockSale.App.Models.Domain;
using StockSale.App.Data; // Asegúrate de importar el contexto
using Microsoft.EntityFrameworkCore;
using StockSale.App.Repositories;

namespace StockSale.App.Services
{
    public class ExcelService
    {
        private readonly StockSaleDbContext _dbContext;

        public ExcelService(StockSaleDbContext stockSaleDbContext)
        {
            _dbContext = stockSaleDbContext;
        }

        public async Task<string> ReadProductsFromExcel(Stream fileStream)
        {
            int importedCount = 0;
            int updatedCount = 0;

            List<Product> products = new List<Product>();

            ExcelPackage.LicenseContext = LicenseContext.NonCommercial;

            using (var package = new ExcelPackage(fileStream))
            {
                var worksheet = package.Workbook.Worksheets[0];
                int rowCount = worksheet.Dimension.Rows;

                for (int row = 2; row <= rowCount; row++)
                {
                    // Ubicaciones - Columnas
                    var name = worksheet.Cells[row, 1].Text?.Trim();
                    var packagingText = worksheet.Cells[row, 2].Text;
                    var stockText = worksheet.Cells[row, 3].Text;
                    var stockLimitText = worksheet.Cells[row, 4].Text;
                    var listPriceText = worksheet.Cells[row, 5].Text;
                    var sellPriceText = worksheet.Cells[row, 6].Text;
                    var unitMText = worksheet.Cells[row, 7].Text?.Trim();
                    var providerText = worksheet.Cells[row, 8].Text?.Trim();

                    // Validaciones
                    if (string.IsNullOrWhiteSpace(name) ||
                        !int.TryParse(packagingText, out int packaging) ||
                        !int.TryParse(stockText, out int stock) ||
                        !int.TryParse(stockLimitText, out int stockLimit) ||
                        !int.TryParse(listPriceText, out int listPrice) ||
                        !int.TryParse(sellPriceText, out int sellPrice))
                    {
                        continue;
                    }

                    // Buscar UnitM si existe en la base de datos
                    UnitM? unitM = null;
                    if (!string.IsNullOrWhiteSpace(unitMText))
                    {
                        unitM = await _dbContext.UnitsM.FirstOrDefaultAsync(u => u.Name == unitMText);
                    }

                    // Buscar Provider si existe en la base de datos
                    Provider? provider = null;
                    if (!string.IsNullOrWhiteSpace(providerText))
                    {
                        provider = await _dbContext.Providers.FirstOrDefaultAsync(p => p.Name == providerText);
                    }

                    // Verificar si el producto ya existe en la base de datos
                    var existingProduct = await _dbContext.Products
                        .FirstOrDefaultAsync(p => p.Name == name && p.Provider == provider);

                    if (existingProduct != null)
                    {
                        existingProduct.Stock += stock;
                        existingProduct.Stock_Limit = stockLimit;
                        existingProduct.List_Price = listPrice;
                        existingProduct.Sell_Price = sellPrice;
                        existingProduct.Packaging = packaging;
                        existingProduct.UnitM = unitM;

                        updatedCount++;
                    }
                    else
                    {
                        // Si no existe, crearlo
                        Product product = new Product
                        {
                            Id = Guid.NewGuid(),
                            Name = name,
                            Packaging = packaging,
                            Stock = stock,
                            Stock_Limit = stockLimit,
                            List_Price = listPrice,
                            Sell_Price = sellPrice,
                            UnitM = unitM,
                            Provider = provider,
                            IsDeleted = false
                        };
                        _dbContext.Products.Add(product);

                        importedCount++;
                    }
                }
            }

            // Guardar los cambios en la base de datos
            await _dbContext.SaveChangesAsync();

            // Generar el mensaje
            string resultMessage = string.Empty;

            if (importedCount > 0 && updatedCount > 0)
            {
                resultMessage = $"Se han importado {importedCount} y actualizado {updatedCount} productos correctamente.";
            }
            else if (importedCount > 0)
            {
                resultMessage = $"Se han importado {importedCount} productos correctamente.";
            }
            else if (updatedCount > 0)
            {
                resultMessage = $"Se han actualizado {updatedCount} productos correctamente.";
            }

            return resultMessage;
        }
    }
}