using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using SuperBodega.AdminWeb.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace SuperBodega.AdminWeb.Controllers
{
    public class ProductsController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<ProductsController> _logger;

        public ProductsController(IHttpClientFactory clientFactory, ILogger<ProductsController> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            var response = await client.GetAsync("api/products");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var products = JsonSerializer.Deserialize<List<ProductViewModel>>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(products);
            }

            return View(new List<ProductViewModel>());
        }

        public async Task<IActionResult> Create()
        {
            await LoadViewBagData();
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductViewModel product)
        {
            try
            {
                // Validar manualmente solo los campos que enviamos a la API
                var modelStateForApi = new ModelStateDictionary();
                if (string.IsNullOrEmpty(product.Name))
                    modelStateForApi.AddModelError(nameof(product.Name), "El nombre es requerido");
                if (string.IsNullOrEmpty(product.Description))
                    modelStateForApi.AddModelError(nameof(product.Description), "La descripción es requerida");
                if (product.Price <= 0)
                    modelStateForApi.AddModelError(nameof(product.Price), "El precio debe ser mayor que cero");
                if (product.Stock < 0)
                    modelStateForApi.AddModelError(nameof(product.Stock), "El stock no puede ser negativo");
                if (product.CategoryId <= 0)
                    modelStateForApi.AddModelError(nameof(product.CategoryId), "La categoría es requerida");
                if (product.SupplierId <= 0)
                    modelStateForApi.AddModelError(nameof(product.SupplierId), "El proveedor es requerido");
                if (modelStateForApi.IsValid)
                {
                    var client = _clientFactory.CreateClient("SuperBodegaAPI");
                    
                    // Obtener el nombre de la categoría
                    var categoryResponse = await client.GetAsync($"api/categories/{product.CategoryId}");
                    if (categoryResponse.IsSuccessStatusCode)
                    {
                        var categoryContent = await categoryResponse.Content.ReadAsStringAsync();
                        var category = JsonSerializer.Deserialize<CategoryViewModel>(categoryContent,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        product.CategoryName = category.Name;
                    }
                    
                    // Obtener el nombre del proveedor
                    var supplierResponse = await client.GetAsync($"api/suppliers/{product.SupplierId}");
                    if (supplierResponse.IsSuccessStatusCode)
                    {
                        var supplierContent = await supplierResponse.Content.ReadAsStringAsync();
                        var supplier = JsonSerializer.Deserialize<SupplierViewModel>(supplierContent,
                            new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                        product.SupplierName = supplier.Name;
                    }
                    
                    var json = JsonSerializer.Serialize(product);
                    _logger.LogInformation("Enviando datos de producto: {Json}", json);
                    
                    var content = new StringContent(json, Encoding.UTF8, "application/json");
                    var response = await client.PostAsync("api/products", content);
                    if (response.IsSuccessStatusCode)
                    {
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        var errorContent = await response.Content.ReadAsStringAsync();
                        _logger.LogError("Error al crear producto: {StatusCode}, {Error}", 
                            response.StatusCode, errorContent);
                        
                        ModelState.AddModelError("", $"Error al crear producto: {errorContent}");
                    }
                }
                else
                {
                    foreach (var error in modelStateForApi.Values.SelectMany(v => v.Errors))
                    {
                        ModelState.AddModelError("", error.ErrorMessage);
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Excepción al crear producto");
                ModelState.AddModelError("", "Error al crear producto. Detalles: " + ex.Message);
            }
            await LoadViewBagData();
            return View(product);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            var response = await client.GetAsync($"api/products/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<ProductViewModel>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                await LoadViewBagData();
                return View(product);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductViewModel product)
        {
            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var client = _clientFactory.CreateClient("SuperBodegaAPI");
                var json = JsonSerializer.Serialize(product);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"api/products/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            await LoadViewBagData();
            return View(product);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            var response = await client.GetAsync($"api/products/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var product = JsonSerializer.Deserialize<ProductViewModel>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(product);
            }

            return NotFound();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            var response = await client.DeleteAsync($"api/products/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }

        private async Task LoadViewBagData()
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            
            // Cargar categorías
            var categoriesResponse = await client.GetAsync("api/categories");
            if (categoriesResponse.IsSuccessStatusCode)
            {
                var content = await categoriesResponse.Content.ReadAsStringAsync();
                var categories = JsonSerializer.Deserialize<List<CategoryViewModel>>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                ViewBag.Categories = new SelectList(categories, "Id", "Name");
            }
            
            // Cargar proveedores
            var suppliersResponse = await client.GetAsync("api/suppliers");
            if (suppliersResponse.IsSuccessStatusCode)
            {
                var content = await suppliersResponse.Content.ReadAsStringAsync();
                var suppliers = JsonSerializer.Deserialize<List<SupplierViewModel>>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                ViewBag.Suppliers = new SelectList(suppliers, "Id", "Name");
            }
        }
    }
}