// =============================================================================

// BetaFit.Application - Interface IProductService

// =============================================================================

//  CONCEITO IMPORTANTE: Service Layer (Camada de Serviço)

// A camada Application contém os SERVIÇOS que orquestram as operações.

// Ela é a "ponte" entre os Controllers e os Repositories.

//

// Fluxo: Controller → Service → Repository → Banco de Dados

//

// O Service é responsável por:

// - Orquestrar chamadas ao repositório

// - Mapear Entidades para DTOs (e vice-versa)

// - Aplicar regras de aplicação (validações, etc.)

// =============================================================================

using BetaFit.Application.DTOs;

namespace BetaFit.Application.Interfaces

{

    /// <summary>

    /// Contrato do serviço de Products.

    /// Define as operações de negócio disponíveis para produtos.

    /// </summary>

    public interface IProductService

    {

        Task<IEnumerable<ProductDto>> GetAllAsync();

        Task<ProductDto?> GetByIdAsync(int id);

        Task<IEnumerable<ProductDto>> GetFeaturedAsync();

        Task<IEnumerable<ProductDto>> GetByCategoryAsync(int categoryId);

        Task<ProductDto> CreateAsync(CreateProductDto dto);

        Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto);

        Task<bool> DeleteAsync(int id);

        Task<int> CountAsync();

    }

}
