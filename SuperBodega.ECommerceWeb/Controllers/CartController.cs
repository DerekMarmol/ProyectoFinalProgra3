using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SuperBodega.ECommerceWeb.Models;

namespace SuperBodega.ECommerceWeb.Controllers
{
    public class CartController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;
        private readonly ILogger<CartController> _logger;
        
        // Clave para el carrito en la sesión
        private const string CartSessionKey = "Cart";

        public CartController(IHttpClientFactory clientFactory, ILogger<CartController> logger)
        {
            _clientFactory = clientFactory;
            _logger = logger;
        }

        public IActionResult Index()
        {
            var cart = GetCartFromSession();
            return View(cart);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, int quantity = 1)
        {
            var cart = GetCartFromSession();
            
            // Verificar si el producto ya está en el carrito
            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            
            if (existingItem != null)
            {
                // Actualizar cantidad
                existingItem.Quantity += quantity;
            }
            else
            {
                // Obtener información del producto
                var client = _clientFactory.CreateClient("SuperBodegaAPI");
                var response = await client.GetAsync($"api/catalog/{productId}");
                
                if (response.IsSuccessStatusCode)
                {
                    var content = await response.Content.ReadAsStringAsync();
                    var product = JsonSerializer.Deserialize<ProductViewModel>(content,
                        new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                    
                    // Agregar nuevo ítem al carrito
                    cart.Items.Add(new CartItemViewModel
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        ProductPrice = product.Price,
                        Quantity = quantity
                    });
                }
                else
                {
                    _logger.LogError("No se pudo obtener el producto: {StatusCode}", response.StatusCode);
                    return RedirectToAction("Index", "Shop");
                }
            }
            
            // Guardar carrito en sesión
            SaveCartToSession(cart);
            
            return RedirectToAction(nameof(Index));
        }
        
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            var cart = GetCartFromSession();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            
            if (item != null)
            {
                if (quantity > 0)
                {
                    item.Quantity = quantity;
                }
                else
                {
                    cart.Items.Remove(item);
                }
                
                SaveCartToSession(cart);
            }
            
            return RedirectToAction(nameof(Index));
        }
        
        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = GetCartFromSession();
            var item = cart.Items.FirstOrDefault(i => i.ProductId == productId);
            
            if (item != null)
            {
                cart.Items.Remove(item);
                SaveCartToSession(cart);
            }
            
            return RedirectToAction(nameof(Index));
        }
        
        public IActionResult Checkout()
        {
            var cart = GetCartFromSession();
            
            if (cart.Items.Count == 0)
            {
                return RedirectToAction(nameof(Index));
            }
            
            return View(new CheckoutViewModel());
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Checkout(CheckoutViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            
            var cart = GetCartFromSession();
            
            if (cart.Items.Count == 0)
            {
                return RedirectToAction(nameof(Index));
            }
            
            try
            {
                var client = _clientFactory.CreateClient("SuperBodegaAPI");
                
                // 1. Crear el cliente primero
                var customerRequest = new 
                {
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    Phone = model.Phone,
                    Address = model.Address
                };
                
                var customerJson = JsonSerializer.Serialize(customerRequest);
                var customerContent = new StringContent(customerJson, Encoding.UTF8, "application/json");
                
                var customerResponse = await client.PostAsync("api/customers", customerContent);
                
                if (!customerResponse.IsSuccessStatusCode)
                {
                    var errorContent = await customerResponse.Content.ReadAsStringAsync();
                    _logger.LogError("Error al crear cliente: {StatusCode}, {Error}", 
                        customerResponse.StatusCode, errorContent);
                    
                    ModelState.AddModelError("", "Error al crear el cliente. Por favor, inténtelo de nuevo.");
                    return View(model);
                }
                
                // Obtener el cliente creado
                var customerResponseContent = await customerResponse.Content.ReadAsStringAsync();
                var customer = JsonSerializer.Deserialize<JsonElement>(customerResponseContent, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                
                int customerId = customer.GetProperty("id").GetInt32();
                
                // 2. Ahora crear la venta con el ID del cliente
                var saleRequest = new
                {
                    SaleDate = DateTime.UtcNow,
                    CustomerId = customerId, // Usar el ID del cliente recién creado
                    CustomerName = $"{model.FirstName} {model.LastName}",
                    TotalAmount = cart.Total,
                    Status = 0, // Pending
                    Reference = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}",
                    SaleDetails = cart.Items.Select(i => new
                    {
                        ProductId = i.ProductId,
                        ProductName = i.ProductName,
                        Quantity = i.Quantity,
                        UnitPrice = i.ProductPrice,
                        TotalPrice = i.TotalPrice
                    }).ToList()
                };
                
                var saleJson = JsonSerializer.Serialize(saleRequest);
                _logger.LogInformation("Enviando datos de venta: {Json}", saleJson);
                
                var saleContent = new StringContent(saleJson, Encoding.UTF8, "application/json");
                
                var saleResponse = await client.PostAsync("api/sales", saleContent);
                
                if (saleResponse.IsSuccessStatusCode)
                {
                    // Limpiar el carrito
                    HttpContext.Session.Remove(CartSessionKey);
                    
                    // Redirigir a la página de confirmación
                    return RedirectToAction(nameof(OrderConfirmation));
                }
                else
                {
                    var errorContent = await saleResponse.Content.ReadAsStringAsync();
                    _logger.LogError("Error en el checkout: {StatusCode}, {Error}", 
                        saleResponse.StatusCode, errorContent);
                    
                    ModelState.AddModelError("", $"Error al procesar el pedido: {errorContent}");
                    return View(model);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al procesar el checkout");
                ModelState.AddModelError("", "Error al procesar el pedido. Por favor, inténtelo de nuevo.");
                return View(model);
            }
        }
        
        public IActionResult OrderConfirmation()
        {
            return View();
        }
        
        // Métodos auxiliares para manejar el carrito en la sesión
        private CartViewModel GetCartFromSession()
        {
            var cartJson = HttpContext.Session.GetString(CartSessionKey);
            return string.IsNullOrEmpty(cartJson) 
                ? new CartViewModel() 
                : JsonSerializer.Deserialize<CartViewModel>(cartJson);
        }
        
        private void SaveCartToSession(CartViewModel cart)
        {
            var cartJson = JsonSerializer.Serialize(cart);
            HttpContext.Session.SetString(CartSessionKey, cartJson);
        }
    }
}