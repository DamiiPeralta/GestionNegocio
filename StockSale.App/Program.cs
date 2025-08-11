using Microsoft.EntityFrameworkCore;
using StockSale.App.Data;
using StockSale.App.Repositories;
using StockSale.App.Repositories.Interfaces;
using StockSale.App.Services;
using StockSale.App.Utils;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

builder.Services.AddDbContext<StockSaleDbContext>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("StockSaleDbConnectionString"))
);
//Add Repositories
builder.Services.AddScoped<IClientRepository, ClientRepository>();
builder.Services.AddScoped<IProviderRepository, ProviderRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IUnitMRepository, UnitMRepository>();
builder.Services.AddScoped<IProviderOrderRepository, ProviderOrderRepository>();
builder.Services.AddScoped<IProductPriceRepository, ProductPriceRepository>();
builder.Services.AddScoped<IOrderBuyRepository, OrderBuyRepository>();
builder.Services.AddScoped<IFiadosRepository, FiadosRepository>();
builder.Services.AddScoped<ITurnRepository, TurnRepository>();
//Add Services
builder.Services.AddScoped<ClientService>();
builder.Services.AddScoped<TurnService>();
builder.Services.AddScoped<ProviderOrderService>();
builder.Services.AddScoped<FiadosService>();
builder.Services.AddScoped<OrderBuyService>();
builder.Services.AddScoped<ProductPriceService>();
builder.Services.AddScoped<ProductService>();
builder.Services.AddScoped<ProviderService>();
builder.Services.AddScoped<PaginationService>();
builder.Services.AddScoped<ExcelService>();
var app = builder.Build();

// 🔹 Ejecutar el Seeder para cargar los archivos JSON al iniciar la app
using (var scope = app.Services.CreateScope())
{
    var services = scope.ServiceProvider;
    var context = services.GetRequiredService<StockSaleDbContext>();

    // Ejecutar la función de carga de datos JSON
    await JsonSeeder.SeedFromJsonFilesAsync(context);
}

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
