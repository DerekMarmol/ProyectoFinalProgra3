using System.Text;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using SuperBodega.AdminWeb.Models;

namespace SuperBodega.AdminWeb.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly IHttpClientFactory _clientFactory;

        public CategoriesController(IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
        }

        public async Task<IActionResult> Index()
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            var response = await client.GetAsync("api/categories");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var categories = JsonSerializer.Deserialize<List<CategoryViewModel>>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(categories);
            }

            return View(new List<CategoryViewModel>());
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CategoryViewModel category)
        {
            if (ModelState.IsValid)
            {
                var client = _clientFactory.CreateClient("SuperBodegaAPI");
                var json = JsonSerializer.Serialize(category);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PostAsync("api/categories", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(category);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            var response = await client.GetAsync($"api/categories/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var category = JsonSerializer.Deserialize<CategoryViewModel>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(category);
            }

            return NotFound();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, CategoryViewModel category)
        {
            if (id != category.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                var client = _clientFactory.CreateClient("SuperBodegaAPI");
                var json = JsonSerializer.Serialize(category);
                var content = new StringContent(json, Encoding.UTF8, "application/json");
                var response = await client.PutAsync($"api/categories/{id}", content);

                if (response.IsSuccessStatusCode)
                {
                    return RedirectToAction(nameof(Index));
                }
            }

            return View(category);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            var response = await client.GetAsync($"api/categories/{id}");

            if (response.IsSuccessStatusCode)
            {
                var content = await response.Content.ReadAsStringAsync();
                var category = JsonSerializer.Deserialize<CategoryViewModel>(content, 
                    new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
                return View(category);
            }

            return NotFound();
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var client = _clientFactory.CreateClient("SuperBodegaAPI");
            var response = await client.DeleteAsync($"api/categories/{id}");

            if (response.IsSuccessStatusCode)
            {
                return RedirectToAction(nameof(Index));
            }

            return NotFound();
        }
    }
}