// =============================================================================
// BetaFit.Infraestructure - CategoryRepository
// =============================================================================
// Implementação do repositório de categorias.
// Segue o mesmo padrão do ProductRepository.
// =============================================================================

using Microsoft.EntityFrameworkCore;
using BetaFit.Domain.Entities;
using BetaFit.Domain.Interfaces;
using BetaFit.Infraestructure.Context;

namespace BetaFit.Infraestructure.Repositories
{
    /// <summary>
    /// Implementação do repositório de Categorias usando Entity Framework Core.
    /// </summary>
    public class CategoryRepository : ICategoryRepository
    {
        private readonly BetaFitDbContext _context;

        public CategoryRepository(BetaFitDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Include(c => c.Products) // Inclui os produtos para contar
                .OrderBy(c => c.Name)
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<int> CountAsync()
        {
            return await _context.Categories.CountAsync();
        }
    }
}