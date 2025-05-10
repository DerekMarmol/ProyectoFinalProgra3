using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using SuperBodega.Core.DTOs;
using SuperBodega.Core.Interfaces;
using SuperBodega.Core.Models;
using SuperBodega.Core.Services;

namespace SuperBodega.Infrastructure.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<CategoryDto> CreateCategoryAsync(CategoryDto categoryDto)
        {
            var category = new Category
            {
                Name = categoryDto.Name,
                Description = categoryDto.Description
            };

            await _categoryRepository.AddAsync(category);

            categoryDto.Id = category.Id;
            return categoryDto;
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category != null)
            {
                await _categoryRepository.DeleteAsync(category);
            }
        }

        public async Task<IEnumerable<CategoryDto>> GetAllCategoriesAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            
            return categories.Select(c => new CategoryDto
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description
            });
        }

        public async Task<CategoryDto> GetCategoryByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return null;

            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                Description = category.Description
            };
        }

        public async Task UpdateCategoryAsync(CategoryDto categoryDto)
        {
            var category = await _categoryRepository.GetByIdAsync(categoryDto.Id);
            if (category != null)
            {
                category.Name = categoryDto.Name;
                category.Description = categoryDto.Description;

                await _categoryRepository.UpdateAsync(category);
            }
        }
    }
}