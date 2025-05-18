using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SuperBodega.AdminWeb.Models;

namespace SuperBodega.AdminWeb.Controllers
{
    public class SuppliersController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public SuppliersController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            var response = await client.GetAsync("api/suppliers");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var suppliers = JsonSerializer.Deserialize<List<SupplierViewModel>>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(suppliers);
            }

            return View(new List<SupplierViewModel>());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(SupplierViewModel supplier)
        {
            if (ModelState.IsValid)
            {
                var client = _clientFactory.CreateClient("SuperBodegaAPI");
                var json = JsonSerializer.Serialize(supplier);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/suppliers", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(supplier);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            var response = await client.GetAsync($"api/suppliers/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var supplier = JsonSerializer.Deserialize<SupplierViewModel>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(supplier);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, SupplierViewModel supplier)
        {
            if (id != supplier.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var client = _clientFactory.CreateClient("SuperBodegaAPI");
                var json = JsonSerializer.Serialize(supplier);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"api/suppliers/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(supplier);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            var response = await client.GetAsync($"api/suppliers/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var supplier = JsonSerializer.Deserialize<SupplierViewModel>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(supplier);
            }

            return NotFound();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            var response = await client.DeleteAsync($"api/suppliers/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }
    }
}