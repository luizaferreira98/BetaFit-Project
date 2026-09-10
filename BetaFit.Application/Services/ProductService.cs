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
using System.Text.Json;

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

            ValidateExtras(dto.ColorGalleries,dto.SizeMeasurements);
            ValidateInventory(dto.Price,dto.SalePrice,dto.Sku,dto.Variants,dto.AvailableSizes,dto.AvailableColors);
            // Garante que o produto está sendo vinculado a uma categoria existente.

            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId)

                ?? throw new InvalidOperationException("Categoria informada não foi encontrada.");

            if (string.IsNullOrWhiteSpace(dto.Name)) throw new InvalidOperationException("Informe o nome do produto.");
            if (string.IsNullOrWhiteSpace(dto.Description)) throw new InvalidOperationException("Informe a descrição do produto.");
            if (dto.Price < 0) throw new InvalidOperationException("O preço não pode ser negativo.");
            if (dto.Stock < 0) throw new InvalidOperationException("O estoque não pode ser negativo.");

            // Mapeia o DTO de criação para a entidade Product

            var product = new Product

            {

                Name = dto.Name,

                Description = dto.Description,

                Price = dto.Price, SalePrice=dto.SalePrice,Sku=dto.Sku.Trim(),LowStockThreshold=dto.LowStockThreshold,VariantsJson=JsonSerializer.Serialize(dto.Variants),

                Stock = dto.Variants.Count>0?dto.Variants.Sum(v=>v.Stock):Math.Max(0, dto.Stock),

                ImageUrl = dto.ImageUrl ?? dto.ImageUrls.FirstOrDefault(),

                ImageUrlsJson = JsonSerializer.Serialize((dto.ImageUrls.Any() ? dto.ImageUrls : (dto.ImageUrl is null ? new List<string>() : new List<string> { dto.ImageUrl })).Distinct()),

                AvailableSizesJson = JsonSerializer.Serialize(dto.AvailableSizes.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).ToList()),
                AvailableColorsJson = JsonSerializer.Serialize(dto.AvailableColors.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Take(20).ToList()),
                ColorGalleriesJson = JsonSerializer.Serialize(dto.ColorGalleries),
                SizeMeasurementsJson = JsonSerializer.Serialize(dto.SizeMeasurements),
                MeasurementsAreDemo = dto.MeasurementsAreDemo,
                ColorImageUrlsJson = JsonSerializer.Serialize(NormalizeColorImages(dto.AvailableColors, dto.ColorImageUrls)),

                Gender = dto.Gender,

                CategoryId = dto.CategoryId,

                IsFeatured = dto.IsFeatured,

                IsActive = dto.IsActive,

                CreatedAt = DateTime.Now

            };

            var createImages = (dto.ImageUrls.Any() ? dto.ImageUrls : (dto.ImageUrl is null ? new List<string>() : new List<string> { dto.ImageUrl }))
                .Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            product.Images = createImages.Select((url, index) => new ProductImage { Url = url, SortOrder = index, IsPrimary = index == 0 }).ToList();
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

            ValidateExtras(dto.ColorGalleries,dto.SizeMeasurements);
            ValidateInventory(dto.Price,dto.SalePrice,dto.Sku,dto.Variants,dto.AvailableSizes,dto.AvailableColors);
            var product = await _productRepository.GetByIdAsync(id);

            if (product == null) return null;

            var category = await _categoryRepository.GetByIdAsync(dto.CategoryId)

                ?? throw new InvalidOperationException("Categoria informada não foi encontrada.");

            if (string.IsNullOrWhiteSpace(dto.Name)) throw new InvalidOperationException("Informe o nome do produto.");
            if (string.IsNullOrWhiteSpace(dto.Description)) throw new InvalidOperationException("Informe a descrição do produto.");
            if (dto.Price < 0) throw new InvalidOperationException("O preço não pode ser negativo.");
            if (dto.Stock < 0) throw new InvalidOperationException("O estoque não pode ser negativo.");

            // Atualiza os campos do produto com os dados do DTO

            product.Name = dto.Name;

            product.Description = dto.Description;

            product.Price = dto.Price;product.SalePrice=dto.SalePrice;product.Sku=dto.Sku.Trim();product.LowStockThreshold=dto.LowStockThreshold;product.VariantsJson=JsonSerializer.Serialize(dto.Variants);
            product.Stock = dto.Variants.Count>0?dto.Variants.Sum(v=>v.Stock):Math.Max(0, dto.Stock);

            var updateImages = (dto.ImageUrls.Any() ? dto.ImageUrls : (dto.ImageUrl is null ? new List<string>() : new List<string> { dto.ImageUrl }))
                .Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
            product.Images = updateImages.Select((url, index) => new ProductImage { Url = url, SortOrder = index, IsPrimary = index == 0 }).ToList();
            product.ImageUrl = updateImages.FirstOrDefault();
            product.ImageUrlsJson = JsonSerializer.Serialize(updateImages);
            product.AvailableSizesJson = JsonSerializer.Serialize(dto.AvailableSizes.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).ToList());
            product.AvailableColorsJson = JsonSerializer.Serialize(dto.AvailableColors.Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()).Distinct(StringComparer.OrdinalIgnoreCase).Take(20).ToList());
            product.ColorGalleriesJson = JsonSerializer.Serialize(dto.ColorGalleries);
            product.SizeMeasurementsJson = JsonSerializer.Serialize(dto.SizeMeasurements);
            product.MeasurementsAreDemo = dto.MeasurementsAreDemo;
            product.ColorImageUrlsJson = JsonSerializer.Serialize(NormalizeColorImages(dto.AvailableColors, dto.ColorImageUrls));

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

        private static void ValidateInventory(decimal price,decimal? sale,string sku,List<ProductVariant> variants,List<string> sizes,List<string> colors){
 if(sale.HasValue&&(sale<=0||sale>=price))throw new InvalidOperationException("O preço de oferta deve ser positivo e menor que o preço original.");
 if(variants.Count>600||variants.Any(v=>v.Stock<0||v.Stock>100000||v.Sku.Length>80)||variants.Sum(v=>(long)v.Stock)>100000)throw new InvalidOperationException("Estoque das variações inválido.");
 if(variants.Select(v=>(v.Size+"|"+v.Color).ToLowerInvariant()).Distinct().Count()!=variants.Count)throw new InvalidOperationException("Há variações repetidas.");
 if(variants.Any(v=>sizes.Count>0?!sizes.Contains(v.Size):v.Size!="")||variants.Any(v=>colors.Count>0?!colors.Contains(v.Color):v.Color!=""))throw new InvalidOperationException("A variação deve usar os tamanhos e cores cadastrados.");
 if(variants.Count>0&&variants.Count!=Math.Max(1,sizes.Count)*Math.Max(1,colors.Count))throw new InvalidOperationException("Preencha o estoque de todas as combinações de tamanho e cor.");
 }
 private static void ValidateExtras(Dictionary<string,List<string>> galleries,Dictionary<string,Dictionary<string,decimal>> measurements)
        {
            if(galleries is null||galleries.Count>20||galleries.Any(g=>g.Value is null||g.Value.Count>20||g.Value.Any(url=>url.Length>500||!(url.StartsWith("/")&&!url.StartsWith("//")||Uri.TryCreate(url,UriKind.Absolute,out var u)&&u.Scheme=="https"))))throw new InvalidOperationException("Galeria por cor inválida.");
            if(measurements is null||measurements.Count>30||measurements.Any(s=>s.Value is null||s.Value.Count>12||s.Value.Any(m=>m.Key.Length>80||m.Value<=0||m.Value>500)))throw new InvalidOperationException("Tabela de medidas inválida.");
        }
        private static List<string> DeserializeList(string? json, params string[] fallback)
        {
            try
            {
                var values = JsonSerializer.Deserialize<List<string>>(json ?? "[]") ?? new();
                if (values.Count > 0) return values;
            }
            catch (JsonException) { }
            return fallback.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        private static ProductDto MapToDto(Product product)

        {

            return new ProductDto

            {

                Id = product.Id,

                Name = product.Name,

                Description = product.Description,

                Price = product.Price, SalePrice=product.SalePrice,Sku=product.Sku,LowStockThreshold=product.LowStockThreshold,Variants=VariantInventory.Read(product),

                Stock = product.Stock,

                ImageUrl = product.ImageUrl,
                ImageUrls = (product.Images?.OrderBy(x => x.SortOrder).Select(x => x.Url).Where(x => !string.IsNullOrWhiteSpace(x)).ToList())?.Count > 0
                    ? product.Images.OrderBy(x => x.SortOrder).Select(x => x.Url).ToList()
                    : DeserializeList(product.ImageUrlsJson, product.ImageUrl ?? string.Empty),
                AvailableSizes = DeserializeList(product.AvailableSizesJson),
                AvailableColors = DeserializeList(product.AvailableColorsJson),
                ColorGalleries = JsonSerializer.Deserialize<Dictionary<string,List<string>>>(product.ColorGalleriesJson)??new(),
                SizeMeasurements = JsonSerializer.Deserialize<Dictionary<string,Dictionary<string,decimal>>>(product.SizeMeasurementsJson)??new(),
                MeasurementsAreDemo = product.MeasurementsAreDemo,
                ColorImageUrls = DeserializeDictionary(product.ColorImageUrlsJson),

                Gender = product.Gender,

                CategoryId = product.CategoryId,

                CategoryName = product.Category?.Name ?? string.Empty,

                IsFeatured = product.IsFeatured,

                IsActive = product.IsActive,

                CreatedAt = product.CreatedAt

            };

        }

        private static Dictionary<string, string> NormalizeColorImages(IEnumerable<string>? colors, IDictionary<string, string>? images)
        {
            var allowed = new HashSet<string>((colors ?? Array.Empty<string>()).Where(x => !string.IsNullOrWhiteSpace(x)).Select(x => x.Trim()), StringComparer.OrdinalIgnoreCase);
            return (images ?? new Dictionary<string, string>())
                .Where(x => allowed.Contains(x.Key) && !string.IsNullOrWhiteSpace(x.Value))
                .GroupBy(x => x.Key.Trim(), StringComparer.OrdinalIgnoreCase)
                .ToDictionary(x => x.Key, x => x.Last().Value.Trim(), StringComparer.OrdinalIgnoreCase);
        }

        private static Dictionary<string, string> DeserializeDictionary(string? json)
        {
            try { return JsonSerializer.Deserialize<Dictionary<string, string>>(json ?? "{}") ?? new(StringComparer.OrdinalIgnoreCase); }
            catch (JsonException) { return new(StringComparer.OrdinalIgnoreCase); }
        }

    }

}
