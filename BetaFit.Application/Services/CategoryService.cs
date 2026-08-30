// =============================================================================
// BetaFit.Application - CategoryService
// =============================================================================
// Implementação do serviço de categorias.
// Segue o mesmo padrão do ProductService.
// =============================================================================

using BetaFit.Application.DTOs;
using BetaFit.Application.Interfaces;
using BetaFit.Domain.Entities;
using BetaFit.Domain.Interfaces;

namespace BetaFit.Application.Services
{
    /// <summary>
    /// Serviço de Categorias — lógica de aplicação para operações com categorias.
    /// </summary>
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;

        public CategoryService(ICategoryRepository categoryRepository)
        {
            _categoryRepository = categoryRepository;
        }

        public async Task<IEnumerable<CategoryDto>> GetAllAsync()
        {
            var categories = await _categoryRepository.GetAllAsync();
            return categories.Select(MapToDto);
        }

        public async Task<CategoryDto?> GetByIdAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            return category == null ? null : MapToDto(category);
        }

        public async Task<CategoryDto> CreateAsync(CreateCategoryDto dto)
        {
            var category = new Category
            {
                Name = dto.Name,
                IsActive = true
            };

            await _categoryRepository.AddAsync(category);
            return MapToDto(category);
        }

        public async Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto dto)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return null;

            category.Name = dto.Name;
            category.IsActive = dto.IsActive;

            await _categoryRepository.UpdateAsync(category);
            return MapToDto(category);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null) return false;

            // Regra da loja: uma categoria com produtos vinculados não pode
            // ser removida, para não deixar produtos "órfãos" no catálogo.
            if (category.Products.Any())
                return false;

            await _categoryRepository.DeleteAsync(id);
            return true;
        }

        public async Task<int> CountAsync()
        {
            return await _categoryRepository.CountAsync();
        }

        /// <summary>
        /// Mapeia uma entidade Category para CategoryDto.
        /// </summary>
        private static CategoryDto MapToDto(Category category)
        {
            return new CategoryDto
            {
                Id = category.Id,
                Name = category.Name,
                IsActive = category.IsActive,
                ProductCount = category.Products?.Count ?? 0,
                CreatedAt = category.CreatedAt
            };
        }
    }
}