// =============================================================================
// BetaFit.API - CategoriesController
// =============================================================================
// Controller REST para operações com Categorias.
//
// Endpoints:
// GET    /api/categories        Lista todas as categorias
// GET    /api/categories/{id}   Busca uma categoria pelo Id
// POST   /api/categories        Cria uma nova categoria
// PUT    /api/categories/{id}   Atualiza uma categoria
// DELETE /api/categories/{id}   Remove uma categoria
// =============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BetaFit.Application.DTOs;
using BetaFit.Application.Interfaces;

namespace BetaFit.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoriesController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        /// <summary>
        /// Retorna todas as categorias.
        /// GET /api/categories
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
        {
            var categories = await _categoryService.GetAllAsync();
            return Ok(categories);
        }

        /// <summary>
        /// Busca uma categoria específica pelo Id.
        /// GET /api/categories/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryDto>> GetById(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);

            if (category == null)
                return NotFound(new { message = "Categoria não encontrada." });

            return Ok(category);
        }

        /// <summary>
        /// Cria uma nova categoria.
        /// POST /api/categories
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Funcionario,Estoquista")]
        public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryDto dto)
        {
            CategoryDto? category;try{category = await _categoryService.CreateAsync(dto);}catch(InvalidOperationException ex){return BadRequest(new{message=ex.Message});}
            return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
        }

        /// <summary>
        /// Atualiza uma categoria existente.
        /// PUT /api/categories/{id}
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Funcionario,Estoquista")]
        public async Task<ActionResult<CategoryDto>> Update(int id, [FromBody] UpdateCategoryDto dto)
        {
            CategoryDto? category;try{category = await _categoryService.UpdateAsync(id,dto);}catch(InvalidOperationException ex){return BadRequest(new{message=ex.Message});}

            if (category == null)
                return NotFound(new { message = "Categoria não encontrada." });

            return Ok(category);
        }

        /// <summary>
        /// Remove uma categoria.
        /// DELETE /api/categories/{id}
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Funcionario,Estoquista")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _categoryService.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "Categoria não encontrada." });

            return NoContent();
        }
    }
}
