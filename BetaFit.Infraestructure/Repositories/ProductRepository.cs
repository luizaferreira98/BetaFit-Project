// =============================================================================
// BetaFit.Infraestructure - ProductRepository
// =============================================================================
//  CONCEITO: Repositório (Repository Pattern)
// O repositório encapsula toda a lógica de acesso a dados.
// Ele usa o DbContext do Entity Framework para executar as operações.
//
// Benefícios do Repository Pattern:
// - Centraliza o acesso a dados em um único lugar
// - Facilita a manutenção e testes
// - A camada Application não precisa conhecer o EF Core
// =============================================================================

using Microsoft.EntityFrameworkCore;
using BetaFit.Domain.Entities;
using BetaFit.Domain.Interfaces;
using BetaFit.Infraestructure.Context;

namespace BetaFit.Infraestructure.Repositories
{
    /// <summary>
    /// Implementação do repositório de Products usando Entity Framework Core.
    /// </summary>
    public class ProductRepository : IProductRepository
    {
        private readonly BetaFitDbContext _context;

        public ProductRepository(BetaFitDbContext context)
        {
            _context = context;
        }

        /// <summary>
        /// Retorna todos os produtos incluindo a categoria relacionada.
        ///  CONCEITO: Include() — carrega dados de tabelas relacionadas (JOIN).
        /// </summary>
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images.OrderBy(i => i.SortOrder))
                .OrderByDescending(p => p.CreatedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Busca um produto pelo Id incluindo sua categoria.
        /// </summary>
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images.OrderBy(i => i.SortOrder))
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        /// <summary>
        /// Retorna apenas os produtos marcados como destaque.
        ///  CONCEITO: Where() — filtra registros (equivalente ao WHERE do SQL).
        /// </summary>
        public async Task<IEnumerable<Product>> GetFeaturedAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images.OrderBy(i => i.SortOrder))
                .Where(p => p.IsFeatured)  // WHERE IsFeatured = true
                .ToListAsync();
        }

        /// <summary>
        /// Retorna todos os produtos de uma categoria específica.
        /// </summary>
        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Images.OrderBy(i => i.SortOrder))
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }

        /// <summary>
        /// Adiciona um novo produto ao banco de dados.
        ///  CONCEITO: AddAsync() + SaveChangesAsync()
        /// AddAsync() marca a entidade para inserção.
        /// SaveChangesAsync() executa o INSERT no banco de dados.
        /// </summary>
        public async Task AddAsync(Product product)
        {
            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Atualiza um produto existente.
        ///  CONCEITO: Update() marca a entidade como modificada.
        /// SaveChangesAsync() executa o UPDATE no banco.
        /// </summary>
        public async Task UpdateAsync(Product product)
        {
            var existingImages = await _context.ProductImages.Where(x => x.ProductId == product.Id).ToListAsync();
            _context.ProductImages.RemoveRange(existingImages);
            var newImages = product.Images ?? new List<ProductImage>();
            foreach (var image in newImages)
            {
                image.Id = 0;
                image.ProductId = product.Id;
            }
            // O produto já está sendo rastreado após GetByIdAsync. Adicionar as
            // imagens explicitamente evita que o EF tente atualizar registros que
            // acabaram de ser removidos da galeria anterior.
            await _context.ProductImages.AddRangeAsync(newImages);
            await _context.SaveChangesAsync();
        }

        /// <summary>
        /// Remove um produto do banco de dados.
        /// </summary>
        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        /// <summary>
        /// Retorna o total de produtos cadastrados.
        ///  CONCEITO: CountAsync() — executa COUNT(*) no banco.
        /// </summary>
        public async Task<int> CountAsync()
        {
            return await _context.Products.CountAsync();
        }
    }
}
