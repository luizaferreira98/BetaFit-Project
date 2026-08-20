// =============================================================================

// BetaFit.Application - ProductService

// =============================================================================

//  CONCEITO IMPORTANTE: Implementação do Serviço

// Esta classe IMPLEMENTA a interface IProductService.

// Ela usa o repositório (IProductRepository) para acessar o banco de dados

// e converte as entidades em DTOs antes de retornar para o controller.

//

// MAPEAMENTO MANUAL:

// Neste projeto, fazemos o mapeamento Entidade → DTO manualmente.

// Em projetos maiores, você pode usar bibliotecas como AutoMapper.

// =============================================================================

using BetaFit.Application.DTOs;

using BetaFit.Application.Interfaces;

using BetaFit.Domain.Entities;

using BetaFit.Domain.Interfaces;

namespace BetaFit.Application.Services

{

    /// <summary>

    /// Serviço de Products — contém a lógica de aplicação para operações com

    /// produtos do catálogo Beta Fit. O preço é demonstrativo: não existe

    /// carrinho, checkout ou pagamento real.

    /// </summary>

    public class ProductService : IProductService

    {

        //  CONCEITO: Injeção de Dependência

        // O repositório é injetado via construtor. Isso permite que o .NET

        // forneça automaticamente a implementação correta em tempo de execução.

        private readonly IProductRepository _productRepository;

        private readonly ICategoryRepository _categoryRepository;

        public ProductService(IProductRepository productRepository, ICategoryRepository categoryRepository)

        {

            _productRepository = productRepository;

            _categoryRepository = categoryRepository;

        }

        /// <summary>

        /// Retorna todos os produtos convertidos em DTOs.

        /// </summary>

        public async Task<IEnumerable<ProductDto>> GetAllAsync()

        {

            var products = await _productRepository.GetAllAsync();

            return products.Select(MapToDto);

        }

        /// <summary>

        /// Busca um produto pelo Id e retorna como DTO.

        /// </summary>

        public async Task<ProductDto?> GetByIdAsync(int id)

        {

            var product = await _productRepository.GetByIdAsync(id);

            return product == null ? null : MapToDto(product);

        }

        /// <summary>

        /// Retorna os produtos em destaque.

        /// </summary>

        public async Task<IEnumerable<ProductDto>> GetFeaturedAsync()

        {

            var products = await _productRepository.GetFeaturedAsync();

            return products.Select(MapToDto);

        }

        /// <summary>

        /// Retorna os produtos de uma categoria específica.

        /// </summary>

        public async Task<IEnumerable<ProductDto>> GetByCategoryAsync(int categoryId)

        {

            var products = await _productRepository.GetByCategoryAsync(categoryId);

            return products.Select(MapToDto);

        }

        /// <summary>

        /// Cria um novo produto a partir do DTO de criação.

        /// </summary>

        public async Task<ProductDto> CreateAsync(CreateProductDto dto)

        {

            // Garante que o produto está sendo vinculado a uma categoria existente.

            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId)

                ?? throw new InvalidOperationException("Categoria informada não foi encontrada.");

            // Mapeia o DTO de criação para a entidade Product

            var product = new Product

            {

                Name = dto.Name,

                Description = dto.Description,

                Price = dto.Price,

                ImageUrl = dto.ImageUrl,

                Gender = dto.Gender,

                CategoryId = dto.CategoryId,

                IsFeatured = dto.IsFeatured,

                IsActive = true,

                CreatedAt = DateTime.Now

            };

            await _productRepository.AddAsync(product);

            product.Category = category;

            // Retorna o produto criado como DTO

            return MapToDto(product);

        }

        /// <summary>

        /// Atualiza um produto existente.

        /// </summary>

        public async Task<ProductDto?> UpdateAsync(int id, UpdateProductDto dto)

        {

            var product = await _productRepository.GetByIdAsync(id);

            if (product == null) return null;

            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId)

                ?? throw new InvalidOperationException("Categoria informada não foi encontrada.");

            // Atualiza os campos do produto com os dados do DTO

            product.Name = dto.Name;

            product.Description = dto.Description;

            product.Price = dto.Price;

            product.ImageUrl = dto.ImageUrl;

            product.Gender = dto.Gender;

            product.CategoryId = dto.CategoryId;

            product.IsFeatured = dto.IsFeatured;

            product.IsActive = dto.IsActive;

            await _productRepository.UpdateAsync(product);

            product.Category = category;

            return MapToDto(product);

        }

        /// <summary>

        /// Remove um produto pelo Id.

        /// </summary>

        public async Task<bool> DeleteAsync(int id)

        {

            var product = await _productRepository.GetByIdAsync(id);

            if (product == null) return false;

            await _productRepository.DeleteAsync(id);

            return true;

        }

        /// <summary>

        /// Retorna o total de produtos.

        /// </summary>

        public async Task<int> CountAsync()

        {

            return await _productRepository.CountAsync();

        }

        // =====================================================================

        // MÉTODO PRIVADO DE MAPEAMENTO

        // =====================================================================

        //  CONCEITO: Mapeamento Entidade → DTO

        // Este método converte uma entidade Product em um ProductDto.

        // Ele é privado porque só é usado internamente pelo serviço.

        // =====================================================================

        private static ProductDto MapToDto(Product product)

        {

            return new ProductDto

            {

                Id = product.Id,

                Name = product.Name,

                Description = product.Description,

                Price = product.Price,

                ImageUrl = product.ImageUrl,

                Gender = product.Gender,

                CategoryId = product.CategoryId,

                CategoryName = product.Category?.Name ?? string.Empty,

                IsFeatured = product.IsFeatured,

                IsActive = product.IsActive,

                CreatedAt = product.CreatedAt

            };

        }

    }

}
