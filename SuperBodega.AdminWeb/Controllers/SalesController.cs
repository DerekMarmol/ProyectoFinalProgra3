using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SuperBodega.AdminWeb.Models;

namespace SuperBodega.AdminWeb.Controllers
{
    public class SalesController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<SalesController> _logger;

        public SalesController(IHttpClientFactory clientFactory, ILogger<SalesController> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        // Vista principal de ventas
        public async Task<IActionResult> Index()
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            
            try
            {
                var response = await client.GetAsync("api/sales");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var sales = JsonSerializer.Deserialize<List<SaleViewModel>>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    return View(sales);
                }
                else
                {
                    _logger.LogError("Error al obtener ventas: {StatusCode}", response.StatusCode);
                    return View(new List<SaleViewModel>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ventas");
                return View(new List<SaleViewModel>());
            }
        }

        // Vista de ventas pendientes
        public async Task<IActionResult> Pending()
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            
            try
            {
                var response = await client.GetAsync("api/sales");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var allSales = JsonSerializer.Deserialize<List<SaleViewModel>>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    // Filtrar solo las ventas pendientes (Status = 0 = Pending)
                    var pendingSales = allSales?.Where(s => s.Status == 0).ToList() ?? new List<SaleViewModel>();
                    
                    return View(pendingSales);
                }
                else
                {
                    _logger.LogError("Error al obtener ventas pendientes: {StatusCode}", response.StatusCode);
                    return View(new List<SaleViewModel>());
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener ventas pendientes");
                return View(new List<SaleViewModel>());
            }
        }

        // Actualizar estado de una venta
        [HttpPost]
        public async Task<IActionResult> UpdateStatus(int saleId, int newStatus)
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            
            try
            {
                var json = JsonSerializer.Serialize(newStatus);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                
                var response = await client.PutAsync($"api/sales/{saleId}/status", content);
                
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Estado de la venta actualizado correctamente.";
                }
                else
                {
                    TempData["ErrorMessage"] = "Error al actualizar el estado de la venta.";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar estado de venta {SaleId}", saleId);
                TempData["ErrorMessage"] = "Error al actualizar el estado de la venta.";
            }
            
            return RedirectToAction(nameof(Index));
        }

        // Ver detalles de una venta
        public async Task<IActionResult> Details(int id)
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            
            try
            {
                var response = await client.GetAsync($"api/sales/{id}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var sale = JsonSerializer.Deserialize<SaleViewModel>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    return View(sale);
                }
                else if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return NotFound();
                }
                else
                {
                    _logger.LogError("Error al obtener detalles de venta: {StatusCode}", response.StatusCode);
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener detalles de venta {SaleId}", id);
                return RedirectToAction(nameof(Index));
            }
        }
    }
}