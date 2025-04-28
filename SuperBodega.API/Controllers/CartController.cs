using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SuperBodega.API.Models;
using SuperBodega.Core.DTOs;
using SuperBodega.Core.Models;
using SuperBodega.Core.Services;

namespace SuperBodega.API.Controllers
{
    [ApiController]
    [Route("api/cart")]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;
        private readonly IAsyncSaleService _asyncSaleService;
        private readonly ICustomerService _customerService;

        public CartController(ICartService cartService, IAsyncSaleService asyncSaleService, ICustomerService customerService)
        {
            _cartService = cartService;
            _asyncSaleService = asyncSaleService;
            _customerService = customerService;
        }

        [HttpGet("{cartId}")]
        public async Task<ActionResult<IEnumerable<CartItemDto>>> GetCart(string cartId)
        {
            var cartItems = await _cartService.GetCartItemsAsync(cartId);
            return Ok(cartItems);
        }

        [HttpPost("{cartId}/items")]
        public async Task<ActionResult<CartItemDto>> AddItemToCart(string cartId, [FromBody] AddCartItemRequest request)
        {
            var cartItem = await _cartService.AddItemToCartAsync(cartId, request.ProductId, request.Quantity);
            if (cartItem == null)
                return NotFound("Product not found");
            
            return Ok(cartItem);
        }

        [HttpPut("{cartId}/items/{productId}")]
        public async Task<IActionResult> UpdateCartItemQuantity(string cartId, int productId, [FromBody] UpdateCartItemRequest request)
        {
            var result = await _cartService.UpdateCartItemQuantityAsync(cartId, productId, request.Quantity);
            if (!result)
                return NotFound();
            
            return NoContent();
        }

        [HttpDelete("{cartId}/items/{productId}")]
        public async Task<IActionResult> RemoveCartItem(string cartId, int productId)
        {
            var result = await _cartService.RemoveItemFromCartAsync(cartId, productId);
            if (!result)
                return NotFound();
            
            return NoContent();
        }

        [HttpPost("{cartId}/checkout")]
        public async Task<ActionResult<SaleDto>> Checkout(string cartId, [FromBody] CheckoutRequest request)
        {
            var cartItems = await _cartService.GetCartItemsAsync(cartId);
            if (!cartItems.Any())
                return BadRequest("Cart is empty");
            
            // Buscar o crear cliente
            CustomerDto customer;
            if (request.CustomerId.HasValue)
            {
                customer = await _customerService.GetCustomerByIdAsync(request.CustomerId.Value);
                if (customer == null)
                    return NotFound("Customer not found");
            }
            else
            {
                // Crear nuevo cliente
                customer = new CustomerDto
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    Email = request.Email,
                    Phone = request.Phone,
                    Address = request.Address
                };
                
                customer = await _customerService.CreateCustomerAsync(customer);
            }
            
            // Crear venta
            var sale = new SaleDto
            {
                SaleDate = DateTime.UtcNow,
                CustomerId = customer.Id,
                CustomerName = $"{customer.FirstName} {customer.LastName}",
                TotalAmount = cartItems.Sum(ci => ci.ProductPrice * ci.Quantity),
                Status = SaleStatus.Pending,
                Reference = $"ORD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 8)}",
                SaleDetails = cartItems.Select(ci => new SaleDetailDto
                {
                    ProductId = ci.ProductId,
                    ProductName = ci.ProductName,
                    Quantity = ci.Quantity,
                    UnitPrice = ci.ProductPrice,
                    TotalPrice = ci.ProductPrice * ci.Quantity
                }).ToList()
            };
            
            // Usar el servicio asíncrono para crear la venta
            var createdSale = await _asyncSaleService.CreateSaleAsync(sale);
            
            // Limpiar el carrito
            await _cartService.ClearCartAsync(cartId);
            
            return Ok(createdSale);
        }
    }
}