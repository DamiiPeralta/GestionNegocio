using System.Collections.Generic;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using StockSale.App.Data;
using StockSale.App.Models.Domain;

namespace StockSale.App.Utils
{
    public static class JsonSeeder
    {
        public static async Task SeedFromJsonFilesAsync(StockSaleDbContext context)
        {
            string directoryPath = Path.Combine(Directory.GetCurrentDirectory(), "Data");

            if (!Directory.Exists(directoryPath))
            {
                Console.WriteLine("⚠️ No se encontró la carpeta de datos.");
                return;
            }

            string[] jsonFiles = Directory.GetFiles(directoryPath, "*.json");

            if (jsonFiles.Length == 0)
            {
                Console.WriteLine("⚠️ No se encontraron archivos JSON en la carpeta.");
                return;
            }

            foreach (var filePath in jsonFiles)
            {
                string fileName = Path.GetFileNameWithoutExtension(filePath).ToLower();

                try
                {
                    switch (fileName)
                    {
                        case "clients":
                            await SeedEntities<Client>(context, filePath, context.Clients, c => c.Name);
                            break;

                        case "providers":
                            await SeedEntities<Provider>(context, filePath, context.Providers, p => p.Name);
                            break;

                        case "products":
                            await SeedProducts(context, filePath);
                            break;

                        default:
                            Console.WriteLine($"⚠️ No hay una entidad definida para {fileName}.json");
                            break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error procesando {fileName}.json: {ex.Message}");
                }
            }
        }

        private static async Task SeedProducts(StockSaleDbContext context, string filePath)
        {
            try
            {
                string jsonData = await File.ReadAllTextAsync(filePath);
                var products = JsonSerializer.Deserialize<List<Product>>(jsonData);

                if (products == null || products.Count == 0)
                {
                    Console.WriteLine($"⚠️ El archivo {Path.GetFileName(filePath)} está vacío o tiene datos inválidos.");
                    return;
                }

                // Crear un diccionario con los proveedores existentes en la base de datos
                var providersDict = await context.Providers
                    .ToDictionaryAsync(p => p.Name, p => p.Id);

                foreach (var product in products)
                {
                    if (!string.IsNullOrEmpty(product.Provider?.Name) &&
                        providersDict.TryGetValue(product.Provider.Name, out var providerId))
                    {
                        product.Provider.Id = providerId; // Asigna el ID del proveedor
                        product.Provider = null; // Evita conflictos con Entity Framework
                    }
                    else
                    {
                        Console.WriteLine($"⚠️ No se encontró el proveedor '{product.Provider?.Name}' para el producto '{product.Name}'.");
                    }
                }

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
                Console.WriteLine($"✅ Se agregaron {products.Count} productos desde {Path.GetFileName(filePath)}.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al procesar productos desde {Path.GetFileName(filePath)}: {ex.Message}");
            }
        }

        private static async Task SeedEntities<T>(StockSaleDbContext context, string filePath, DbSet<T> dbSet, Func<T, object> keySelector) where T : class
        {
            try
            {
                string jsonData = await File.ReadAllTextAsync(filePath);
                var entities = JsonSerializer.Deserialize<List<T>>(jsonData);

                if (entities == null || entities.Count == 0)
                {
                    Console.WriteLine($"⚠️ El archivo {Path.GetFileName(filePath)} está vacío o tiene datos inválidos.");
                    return;
                }

                var existingKeys = await dbSet.AsNoTracking()
                    .Select(e => keySelector(e))
                    .ToListAsync();
                var newEntities = entities.Where(e => !existingKeys.Contains(keySelector(e))).ToList();

                if (newEntities.Any())
                {
                    await dbSet.AddRangeAsync(newEntities);
                    await context.SaveChangesAsync();
                    Console.WriteLine($"✅ Se agregaron {newEntities.Count} registros en {typeof(T).Name} desde {Path.GetFileName(filePath)}.");
                }
                else
                {
                    Console.WriteLine($"✅ No hay nuevos datos para agregar en {typeof(T).Name}.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error al procesar {Path.GetFileName(filePath)}: {ex.Message}");
            }
        }
    }
}