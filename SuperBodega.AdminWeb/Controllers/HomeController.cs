using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using SuperBodega.AdminWeb.Models;

namespace SuperBodega.AdminWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly IHttpClientFactory _clientFactory;

        public HomeController(ILogger<HomeController> logger, IHttpClientFactory clientFactory)
        {
            _logger = logger;
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var dashboard = new DashboardViewModel();
            var client = _clientFactory.CreateClient("SuperBodegaAPI");

            try
            {
                // Obtener conteo de categorías
                var categoriesResponse = await client.GetAsync("api/categories");
                if (categoriesResponse.IsSuccessStatusCode)
                {
                    var content = await categoriesResponse.Content.ReadAsStringAsync();
                    var categories = JsonSerializer.Deserialize<List<CategoryViewModel>>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    dashboard.CategoryCount = categories?.Count ?? 0;
                }

                // Obtener conteo de productos
                var productsResponse = await client.GetAsync("api/products");
                if (productsResponse.IsSuccessStatusCode)
                {
                    var content = await productsResponse.Content.ReadAsStringAsync();
                    var products = JsonSerializer.Deserialize<List<ProductViewModel>>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    dashboard.ProductCount = products?.Count ?? 0;
                }

                // Obtener conteo de proveedores
                var suppliersResponse = await client.GetAsync("api/suppliers");
                if (suppliersResponse.IsSuccessStatusCode)
                {
                    var content = await suppliersResponse.Content.ReadAsStringAsync();
                    var suppliers = JsonSerializer.Deserialize<List<SupplierViewModel>>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    dashboard.SupplierCount = suppliers?.Count ?? 0;
                }

                // Obtener ventas del mes actual
                var today = DateTime.Today;
                var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
                var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
                
                var salesResponse = await client.GetAsync($"api/sales/date-range?startDate={firstDayOfMonth:yyyy-MM-dd}&endDate={lastDayOfMonth:yyyy-MM-dd}");
                if (salesResponse.IsSuccessStatusCode)
                {
                    var content = await salesResponse.Content.ReadAsStringAsync();
                    var sales = JsonSerializer.Deserialize<List<SaleViewModel>>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    dashboard.MonthlySales = sales?.Sum(s => s.TotalAmount) ?? 0;
                }

                // Obtener actividades recientes (usamos las últimas ventas, actualizaciones de productos, etc.)
                // Aquí simularemos con las últimas ventas o productos
                dashboard.RecentActivities = new List<RecentActivityItem>();
                
                // Intentar obtener las ventas más recientes
                var recentSalesResponse = await client.GetAsync("api/sales");
                if (recentSalesResponse.IsSuccessStatusCode)
                {
                    var content = await recentSalesResponse.Content.ReadAsStringAsync();
                    var sales = JsonSerializer.Deserialize<List<SaleViewModel>>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    if (sales != null && sales.Any())
                    {
                        foreach (var sale in sales.OrderByDescending(s => s.SaleDate).Take(3))
                        {
                            dashboard.RecentActivities.Add(new RecentActivityItem
                            {
                                ActivityType = "sale",
                                Action = "completed",
                                Description = $"Venta #{sale.Reference} por {sale.TotalAmount:C} a {sale.CustomerName}",
                                Timestamp = sale.SaleDate
                            });
                        }
                    }
                }
                
                // Si necesitamos más actividades, podemos obtener productos recientes
                if (dashboard.RecentActivities.Count < 3)
                {
                    var products = JsonSerializer.Deserialize<List<ProductViewModel>>(
                        await productsResponse.Content.ReadAsStringAsync(),
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    if (products != null)
                    {
                        foreach (var product in products.OrderByDescending(p => p.Id).Take(3 - dashboard.RecentActivities.Count))
                        {
                            dashboard.RecentActivities.Add(new RecentActivityItem
                            {
                                ActivityType = "product",
                                Action = "added",
                                Description = $"Se ha agregado el producto \"{product.Name}\" al inventario.",
                                Timestamp = DateTime.Now.AddHours(-new Random().Next(1, 12))
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener datos para el dashboard");
            }

            return View(dashboard);
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}