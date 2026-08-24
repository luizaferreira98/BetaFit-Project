// =============================================================================

// BetaFit.Application - Interface ICategoryService

// =============================================================================

using BetaFit.Application.DTOs;

namespace BetaFit.Application.Interfaces

{

    /// <summary>

    /// Contrato do serviço de Categorias.

    /// </summary>

    public interface ICategoryService

    {

        Task<IEnumerable<CategoryDto>> GetAllAsync();

        Task<CategoryDto?> GetByIdAsync(int id);

        Task<CategoryDto> CreateAsync(CreateCategoryDto dto);

        Task<CategoryDto?> UpdateAsync(int id, UpdateCategoryDto dto);

        Task<bool> DeleteAsync(int id);

        Task<int> CountAsync();

    }

}
