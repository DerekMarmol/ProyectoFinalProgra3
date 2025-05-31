// SuperBodega.AdminWeb/Controllers/ReportsController.cs
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SuperBodega.AdminWeb.Models.Reports;

namespace SuperBodega.AdminWeb.Controllers
{
    public class ReportsController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<ReportsController> _logger;

        public ReportsController(IHttpClientFactory clientFactory, ILogger<ReportsController> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public async Task<IActionResult> Sales()
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            
            // Obtener reporte con filtro por defecto (últimos 30 días)
            var filter = new ReportFilterViewModel
            {
                Period = ReportPeriod.Last30Days
            };

            var json = JsonSerializer.Serialize(filter);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await client.PostAsync("api/reports/sales", content);
            
            if (response.IsSuccessStatusCode)
            {
                var reportContent = await response.Content.ReadAsStringAsync();
                var salesReport = JsonSerializer.Deserialize<SalesReportViewModel>(reportContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                ViewBag.Filter = filter;
                return View(salesReport);
            }

            _logger.LogError("Error al obtener reporte de ventas: {StatusCode}", response.StatusCode);
            return View(new SalesReportViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Sales(ReportFilterViewModel filter)
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            
            var json = JsonSerializer.Serialize(filter);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await client.PostAsync("api/reports/sales", content);
            
            if (response.IsSuccessStatusCode)
            {
                var reportContent = await response.Content.ReadAsStringAsync();
                var salesReport = JsonSerializer.Deserialize<SalesReportViewModel>(reportContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                ViewBag.Filter = filter;
                return View(salesReport);
            }

            _logger.LogError("Error al obtener reporte de ventas filtrado: {StatusCode}", response.StatusCode);
            ViewBag.Filter = filter;
            return View(new SalesReportViewModel());
        }

        public async Task<IActionResult> Inventory()
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            var response = await client.GetAsync("api/reports/inventory");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var inventoryReport = JsonSerializer.Deserialize<InventoryReportViewModel>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                return View(inventoryReport);
            }

            _logger.LogError("Error al obtener reporte de inventario: {StatusCode}", response.StatusCode);
            return View(new InventoryReportViewModel());
        }

        public async Task<IActionResult> Customers()
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            
            var filter = new ReportFilterViewModel
            {
                Period = ReportPeriod.Last30Days
            };

            var json = JsonSerializer.Serialize(filter);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await client.PostAsync("api/reports/customers", content);
            
            if (response.IsSuccessStatusCode)
            {
                var reportContent = await response.Content.ReadAsStringAsync();
                var customersReport = JsonSerializer.Deserialize<CustomersReportViewModel>(reportContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                ViewBag.Filter = filter;
                return View(customersReport);
            }

            _logger.LogError("Error al obtener reporte de clientes: {StatusCode}", response.StatusCode);
            return View(new CustomersReportViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Customers(ReportFilterViewModel filter)
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            
            var json = JsonSerializer.Serialize(filter);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await client.PostAsync("api/reports/customers", content);
            
            if (response.IsSuccessStatusCode)
            {
                var reportContent = await response.Content.ReadAsStringAsync();
                var customersReport = JsonSerializer.Deserialize<CustomersReportViewModel>(reportContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                ViewBag.Filter = filter;
                return View(customersReport);
            }

            _logger.LogError("Error al obtener reporte de clientes filtrado: {StatusCode}", response.StatusCode);
            ViewBag.Filter = filter;
            return View(new CustomersReportViewModel());
        }

        // API endpoint para obtener datos para gráficos
        [HttpPost]
        public async Task<IActionResult> GetSalesChartData([FromBody] ReportFilterViewModel filter)
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            
            var json = JsonSerializer.Serialize(filter);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            
            var response = await client.PostAsync("api/reports/sales", content);
            
            if (response.IsSuccessStatusCode)
            {
                var reportContent = await response.Content.ReadAsStringAsync();
                var salesReport = JsonSerializer.Deserialize<SalesReportViewModel>(reportContent,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                var chartData = new
                {
                    dailySales = salesReport.DailySales.Select(d => new { 
                        date = d.Date.ToString("yyyy-MM-dd"), 
                        amount = d.Amount,
                        orders = d.OrderCount 
                    }),
                    topProducts = salesReport.TopProducts.Take(5).Select(p => new { 
                        name = p.ProductName, 
                        revenue = p.TotalRevenue,
                        quantity = p.QuantitySold 
                    }),
                    categoryBreakdown = salesReport.SalesByCategory.Select(c => new { 
                        category = c.CategoryName, 
                        sales = c.TotalSales,
                        percentage = c.Percentage 
                    })
                };
                
                return Json(chartData);
            }

            return Json(new { error = "No se pudieron obtener los datos" });
        }

        [HttpGet]
        public async Task<IActionResult> GetInventoryChartData()
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            var response = await client.GetAsync("api/reports/inventory");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var inventoryReport = JsonSerializer.Deserialize<InventoryReportViewModel>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                var chartData = new
                {
                    categoryInventory = inventoryReport.InventoryByCategory.Select(c => new { 
                        category = c.CategoryName, 
                        value = c.TotalValue,
                        products = c.ProductCount,
                        stock = c.TotalStock 
                    }),
                    stockStatus = new
                    {
                        inStock = inventoryReport.TotalProducts - inventoryReport.LowStockProducts - inventoryReport.OutOfStockProducts,
                        lowStock = inventoryReport.LowStockProducts,
                        outOfStock = inventoryReport.OutOfStockProducts
                    }
                };
                
                return Json(chartData);
            }

            return Json(new { error = "No se pudieron obtener los datos de inventario" });
        }

        public async Task<IActionResult> Dashboard()
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            var response = await client.GetAsync("api/reports/dashboard");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var dashboardData = JsonSerializer.Deserialize<DashboardSummaryViewModel>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                return View(dashboardData);
            }

            _logger.LogError("Error al obtener datos del dashboard: {StatusCode}", response.StatusCode);
            return View(new DashboardSummaryViewModel());
        }

        [HttpGet]
        public async Task<IActionResult> ExportSales(DateTime? startDate, DateTime? endDate, string format = "excel")
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            
            var queryParams = new List<string>();
            if (startDate.HasValue)
                queryParams.Add($"startDate={startDate.Value:yyyy-MM-dd}");
            if (endDate.HasValue)
                queryParams.Add($"endDate={endDate.Value:yyyy-MM-dd}");
            
            var query = queryParams.Any() ? "?" + string.Join("&", queryParams) : "";
            var response = await client.GetAsync($"api/reports/export/sales{query}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var salesReport = JsonSerializer.Deserialize<SalesReportViewModel>(content,
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                if (format.ToLower() == "pdf")
                {
                    // Aquí podrías implementar la generación de PDF
                    // Por ahora retornamos una vista parcial para imprimir
                    return PartialView("_SalesReportPrint", salesReport);
                }
                else
                {
                    // Para Excel, por ahora retornamos CSV
                    var csv = GenerateSalesCsv(salesReport);
                    var bytes = Encoding.UTF8.GetBytes(csv);
                    
                    return File(bytes, "text/csv", $"reporte-ventas-{DateTime.Now:yyyyMMdd}.csv");
                }
            }

            return RedirectToAction(nameof(Sales));
        }

        private string GenerateSalesCsv(SalesReportViewModel report)
        {
            var csv = new StringBuilder();
            csv.AppendLine("Reporte de Ventas");
            csv.AppendLine($"Período,{report.StartDate:dd/MM/yyyy} - {report.EndDate:dd/MM/yyyy}");
            csv.AppendLine($"Total Ventas,{report.TotalSales:C}");
            csv.AppendLine($"Total Órdenes,{report.TotalOrders}");
            csv.AppendLine($"Valor Promedio por Orden,{report.AverageOrderValue:C}");
            csv.AppendLine();
            
            csv.AppendLine("Top Productos");
            csv.AppendLine("Producto,Categoría,Cantidad Vendida,Ingresos Totales");
            foreach (var product in report.TopProducts)
            {
                csv.AppendLine($"{product.ProductName},{product.CategoryName},{product.QuantitySold},{product.TotalRevenue:C}");
            }
            
            csv.AppendLine();
            csv.AppendLine("Ventas por Categoría");
            csv.AppendLine("Categoría,Ventas Totales,Porcentaje");
            foreach (var category in report.SalesByCategory)
            {
                csv.AppendLine($"{category.CategoryName},{category.TotalSales:C},{category.Percentage:F1}%");
            }
            
            return csv.ToString();
        }
    }
}