// =============================================================================
// BetaFit.API - ProductsController
// =============================================================================
//  CONCEITO IMPORTANTE: API Controller
// Um API Controller é responsável por receber requisições HTTP
// e retornar respostas em formato JSON.
//
// Endpoints REST deste controller:
// GET    /api/products                     Lista todos os produtos
// GET    /api/products/{id}                Busca um produto pelo Id
// GET    /api/products/featured            Lista produtos em destaque
// GET    /api/products/category/{catId}    Lista produtos de uma categoria
// POST   /api/products                     Cria um novo produto
// PUT    /api/products/{id}                Atualiza um produto existente
// DELETE /api/products/{id}                Remove um produto
// =============================================================================

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BetaFit.Application.DTOs;
using BetaFit.Application.Interfaces;
using BetaFit.API.Services;

namespace BetaFit.API.Controllers
{
    /// <summary>
    /// Controller REST para operações com Products.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductService _productService;
        private readonly INotificationService _notifications;

        //  CONCEITO: O serviço é injetado automaticamente pelo .NET (DI)
        public ProductsController(IProductService productService, INotificationService notifications)
        {
            _productService = productService;
            _notifications = notifications;
        }

        /// <summary>
        /// Retorna todos os produtos.
        /// GET /api/products
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
        {
            var products = await _productService.GetAllAsync();
            return Ok(products);
        }

        /// <summary>
        /// Busca um produto específico pelo Id.
        /// GET /api/products/{id}
        /// </summary>
        [HttpGet("{id}")]
        public async Task<ActionResult<ProductDto>> GetById(int id)
        {
            var product = await _productService.GetByIdAsync(id);

            if (product == null)
                return NotFound(new { message = "Produto não encontrado." });

            return Ok(product);
        }

        /// <summary>
        /// Retorna apenas os produtos em destaque.
        /// GET /api/products/featured
        /// </summary>
        [HttpGet("featured")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetFeatured()
        {
            var products = await _productService.GetFeaturedAsync();
            return Ok(products);
        }

        /// <summary>
        /// Retorna os produtos de uma categoria específica.
        /// GET /api/products/category/{categoryId}
        /// </summary>
        [HttpGet("category/{categoryId}")]
        public async Task<ActionResult<IEnumerable<ProductDto>>> GetByCategory(int categoryId)
        {
            var products = await _productService.GetByCategoryAsync(categoryId);
            return Ok(products);
        }

        /// <summary>
        /// Cria um novo produto.
        /// POST /api/products
        /// Requer autenticação (somente admin pode criar produtos).
        /// </summary>
        [HttpPost]
        [Authorize(Roles = "Admin,Funcionario,Estoquista")]
        public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
        {
            try
            {
                var product = await _productService.CreateAsync(dto);
                await _notifications.ProductCreatedAsync(product, HttpContext.RequestAborted);

                // Retorna 201 Created com a URL do recurso criado
                return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
            }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }

        /// <summary>
        /// Atualiza um produto existente.
        /// PUT /api/products/{id}
        /// </summary>
        [HttpPut("{id}")]
        [Authorize(Roles = "Admin,Funcionario,Estoquista")]
        public async Task<ActionResult<ProductDto>> Update(int id, [FromBody] UpdateProductDto dto)
        {
            try
            {
                var product = await _productService.UpdateAsync(id, dto);

                if (product == null)
                return NotFound(new { message = "Produto não encontrado." });

                return Ok(product);
            }
            catch (InvalidOperationException ex) { return BadRequest(new { message = ex.Message }); }
        }

        /// <summary>
        /// Remove um produto.
        /// DELETE /api/products/{id}
        /// </summary>
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Funcionario,Estoquista")]
        public async Task<ActionResult> Delete(int id)
        {
            var deleted = await _productService.DeleteAsync(id);

            if (!deleted)
                return NotFound(new { message = "Produto não encontrado." });

            return NoContent(); // Retorna 204 No Content (sucesso sem corpo)
        }
    }
}
