using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SuperBodega.ECommerceWeb.Models;

namespace SuperBodega.ECommerceWeb.Controllers
{
    public class ShopController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<ShopController> _logger;

        public ShopController(IHttpClientFactory clientFactory, ILogger<ShopController> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task<IActionResult> Index(int categoryId = 0, int page = 1, int pageSize = 8)
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            
            // Obtener categorías
            var categories = await GetCategoriesAsync(client);
            ViewBag.Categories = categories;
            
            // Obtener productos
            string url = categoryId > 0 
                ? $"api/catalog/category/{categoryId}?page={page}&pageSize={pageSize}" 
                : $"api/catalog?page={page}&pageSize={pageSize}";
            
            var response = await client.GetAsync(url);
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var products = JsonSerializer.Deserialize<List<ProductViewModel>>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                ViewBag.CurrentCategory = categoryId;
                ViewBag.CurrentPage = page;
                ViewBag.PageSize = pageSize;
                
                return View(products);
            }
            
            _logger.LogError("Error al obtener productos: {StatusCode}", response.StatusCode);
            return View(new List<ProductViewModel>());
        }
        
        public async Task<IActionResult> Details(int id)
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            var response = await client.GetAsync($"api/catalog/{id}");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<ProductViewModel>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                return View(product);
            }
            
            return NotFound();
        }
        
        private async Task<List<CategoryViewModel>> GetCategoriesAsync(HttpClient client)
        {
            var response = await client.GetAsync("api/categories");
            
            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                return JsonSerializer.Deserialize<List<CategoryViewModel>>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
            
            return new List<CategoryViewModel>();
        }
    }
}