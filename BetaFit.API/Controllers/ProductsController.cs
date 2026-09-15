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
// POST   /api/products/upload-image        Faz upload de uma imagem local
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
        private readonly IWebHostEnvironment _webHostEnvironment;

        //  CONCEITO: O serviço é injetado automaticamente pelo .NET (DI)
        public ProductsController(
            IProductService productService,
            INotificationService notifications,
            IWebHostEnvironment webHostEnvironment)
        {
            _productService = productService;
            _notifications = notifications;
            _webHostEnvironment = webHostEnvironment;
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
        /// Faz upload de uma imagem local e retorna a URL pública dela.
        /// POST /api/products/upload-image
        /// Usado pelo formulário de produto do Desktop, que permite escolher
        /// um arquivo do computador em vez de digitar a URL manualmente
        /// (mesma ideia do upload de imagens do site institucional).
        /// A imagem é salva em wwwroot/images/products da própria API e
        /// servida como arquivo estático.
        /// </summary>
        [HttpPost("upload-image")]
        [Authorize(Roles = "Admin,Funcionario,Estoquista")]
        [RequestSizeLimit(6 * 1024 * 1024)] // 6 MB — um pouco acima do limite de validação (5 MB)
        public async Task<IActionResult> UploadImage(IFormFile? image)
        {
            if (image == null || image.Length == 0)
                return BadRequest(new { message = "Nenhum arquivo foi enviado." });

            const long tamanhoMaximo = 5 * 1024 * 1024; // 5 MB
            if (image.Length > tamanhoMaximo)
                return BadRequest(new { message = "A imagem excede o tamanho máximo de 5 MB." });

            var extensoesPermitidas = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extensao = Path.GetExtension(image.FileName).ToLowerInvariant();
            if (!extensoesPermitidas.Contains(extensao))
                return BadRequest(new { message = "Use apenas imagens JPG, PNG ou WEBP." });

            // Verificação de "magic bytes": confirma que o conteúdo do arquivo
            // realmente é do tipo de imagem declarado pela extensão (evita que
            // um arquivo malicioso renomeado para .jpg seja aceito).
            await using (var streamValidacao = image.OpenReadStream())
            {
                var header = new byte[12];
                var lidos = await streamValidacao.ReadAsync(header.AsMemory(0, 12));
                bool valido = extensao switch
                {
                    ".jpg" or ".jpeg" => lidos >= 3 && header[0] == 0xFF && header[1] == 0xD8 && header[2] == 0xFF,
                    ".png" => lidos >= 8 && header[0] == 0x89 && header[1] == 0x50 && header[2] == 0x4E && header[3] == 0x47,
                    ".webp" => lidos >= 12 && header[0] == (byte)'R' && header[1] == (byte)'I' && header[2] == (byte)'F' && header[3] == (byte)'F'
                               && header[8] == (byte)'W' && header[9] == (byte)'E' && header[10] == (byte)'B' && header[11] == (byte)'P',
                    _ => false
                };

                if (!valido)
                    return BadRequest(new { message = "O arquivo enviado não parece ser uma imagem válida." });
            }

            // wwwroot pode não existir fisicamente ainda em um projeto novo —
            // criamos a pasta de destino se necessário.
            var pastaWebRoot = _webHostEnvironment.WebRootPath
                ?? Path.Combine(_webHostEnvironment.ContentRootPath, "wwwroot");
            var pastaDestino = Path.Combine(pastaWebRoot, "images", "products");
            Directory.CreateDirectory(pastaDestino);

            var nomeArquivo = $"{Guid.NewGuid():N}{extensao}";
            var caminhoCompleto = Path.Combine(pastaDestino, nomeArquivo);

            await using (var stream = new FileStream(caminhoCompleto, FileMode.Create))
            {
                await image.CopyToAsync(stream);
            }

            // URL absoluta (host da própria API), assim a imagem é exibida
            // corretamente independente de quem estiver consumindo o produto.
            var url = $"{Request.Scheme}://{Request.Host}/images/products/{nomeArquivo}";
            return Ok(new { url });
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