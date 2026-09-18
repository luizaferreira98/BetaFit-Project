// =============================================================================
// BetaFit.Infraestructure - Seed Data (Dados Iniciais)
// =============================================================================
// Seed Data são os dados iniciais utilizados para popular o banco.
// O método abaixo é idempotente: pode ser executado várias vezes sem duplicar
// produtos, categorias, roles ou usuários.
// =============================================================================

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Text.Json;
using System.Security.Claims;
using BetaFit.Domain.Entities;
using BetaFit.Domain.Enums;
using BetaFit.Infraestructure.Context;

namespace BetaFit.Infraestructure.Identity
{
    /// <summary>
    /// Classe responsável por popular o banco de dados com dados iniciais.
    /// </summary>
    public static class SeedData
    {
        /// <summary>
        /// Popula roles, usuário administrador, categorias, catálogo de produtos,
        /// tamanhos, cores e galeria. Idempotente.
        /// </summary>
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<BetaFitDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // =================================================================
            // 1. SEED DE ROLES (idempotente)
            // =================================================================
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            if (!await roleManager.RoleExistsAsync("Usuario"))
                await roleManager.CreateAsync(new IdentityRole("Usuario"));

            if (!await roleManager.RoleExistsAsync("Funcionario"))
                await roleManager.CreateAsync(new IdentityRole("Funcionario"));

            // =================================================================
            // 2. SEED DO USUÁRIO ADMIN (idempotente)
            // =================================================================
            var adminEmail = "admin@betafit.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");

                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }

            if (adminUser is not null)
            {
                var adminClaims = await userManager.GetClaimsAsync(adminUser);

                if (!adminClaims.Any(c => c.Type == "FullName"))
                    await userManager.AddClaimAsync(adminUser, new Claim("FullName", "Administrador BetaFit"));

                if (!adminClaims.Any(c => c.Type == "BirthDate"))
                    await userManager.AddClaimAsync(adminUser, new Claim("BirthDate", "1990-01-01"));
            }

            // =================================================================
            // 3. SEED DE CATEGORIAS (idempotente)
            // =================================================================
            var categoryDefinitions = new[]
            {
                ("Camisetas", "Camisetas casuais e esportivas"),
                ("Leggings", "Leggings e calças de treino"),
                ("Moletons", "Moletons e casacos"),
                ("Shorts", "Shorts esportivos e casuais"),
                ("Tênis", "Calçados esportivos"),
                ("Acessórios", "Bonés, garrafas, shakers, galões e outros acessórios")
            };

            foreach (var (name, description) in categoryDefinitions)
            {
                if (!await context.Categories.AnyAsync(c => c.Name == name))
                {
                    await context.Categories.AddAsync(new Category
                    {
                        Name = name,
                        Description = description
                    });
                }
            }

            await context.SaveChangesAsync();

            // Busca as categorias para obter os IDs.
            var camisetas = await context.Categories.FirstAsync(c => c.Name == "Camisetas");
            var leggings = await context.Categories.FirstAsync(c => c.Name == "Leggings");
            var moletons = await context.Categories.FirstAsync(c => c.Name == "Moletons");
            var shorts = await context.Categories.FirstAsync(c => c.Name == "Shorts");
            var tenis = await context.Categories.FirstAsync(c => c.Name == "Tênis");
            var acessorios = await context.Categories.FirstAsync(c => c.Name == "Acessórios");

            // =================================================================
            // 4. SEED / ATUALIZAÇÃO DO CATÁLOGO DE PRODUTOS (idempotente por Name)
            // =================================================================
            var products = new List<Product>
            {
                // ---------------- TÊNIS ----------------
                new Product
                {
                    Name = "Tênis Urban Low White",
                    Description = "Tênis esportivo branco, leve e versátil para academia, caminhadas e treinos do dia a dia.",
                    Price = 279.90m,
                    ImageUrl = "/images/products/Tênis Urban Low White.png",
                    ImageUrlsJson = "[\"/images/products/Tênis Urban Low White.png\"]",
                    Gender = Gender.Feminino,
                    CategoryId = tenis.Id,
                    IsFeatured = true,
                    CreatedAt = DateTime.Now
                },
            };

            // Insere produtos ausentes e atualiza os já existentes pelo nome.
            foreach (var seedProduct in products)
            {
                var existingProduct = await context.Products
                    .FirstOrDefaultAsync(p => p.Name == seedProduct.Name);

                if (existingProduct is null)
                {
                    await context.Products.AddAsync(seedProduct);
                    continue;
                }

                existingProduct.Description = seedProduct.Description;
                existingProduct.Price = seedProduct.Price;
                existingProduct.ImageUrl = seedProduct.ImageUrl;
                existingProduct.ImageUrlsJson = seedProduct.ImageUrlsJson;
                existingProduct.Gender = seedProduct.Gender;
                existingProduct.CategoryId = seedProduct.CategoryId;
                existingProduct.IsFeatured = seedProduct.IsFeatured;
            }

            await context.SaveChangesAsync();

            // =================================================================
            // 5. TAMANHOS (idempotente)
            // =================================================================
            var clothingCategoryIds = await context.Categories
                .Where(c =>
                    c.Name == "Camisetas" ||
                    c.Name == "Leggings" ||
                    c.Name == "Moletons" ||
                    c.Name == "Shorts")
                .Select(c => c.Id)
                .ToListAsync();

            var shoeCategory = await context.Categories
                .FirstOrDefaultAsync(c => c.Name == "Tênis");

            var clothingSizes = JsonSerializer.Serialize(new[] { "P", "M", "G", "GG", "XG" });
            var shoeSizes = JsonSerializer.Serialize(
                Enumerable.Range(35, 8).Select(x => x.ToString()).ToList());

            var clothingProducts = await context.Products
                .Where(p => clothingCategoryIds.Contains(p.CategoryId))
                .ToListAsync();

            foreach (var product in clothingProducts)
            {
                product.AvailableSizesJson = clothingSizes;
            }

            if (shoeCategory is not null)
            {
                var shoeProducts = await context.Products
                    .Where(p => p.CategoryId == shoeCategory.Id)
                    .ToListAsync();

                foreach (var shoe in shoeProducts)
                {
                    shoe.AvailableSizesJson = shoeSizes;
                }
            }

            var accessoryCategory = await context.Categories
                .FirstAsync(c => c.Name == "Acessórios");

            var accessoryProducts = await context.Products
                .Where(p => p.CategoryId == accessoryCategory.Id)
                .ToListAsync();

            foreach (var accessory in accessoryProducts)
            {
                accessory.AvailableSizesJson = "[]";
            }

            // =================================================================
            // 6. CORES (idempotente - só preenche se ainda estiver vazio)
            // =================================================================
            foreach (var product in await context.Products.ToListAsync())
            {
                if (!string.IsNullOrWhiteSpace(product.AvailableColorsJson) &&
                    product.AvailableColorsJson != "[]")
                {
                    continue;
                }

                var name = product.Name.ToLowerInvariant();

                var colors =
                    name.Contains("branco") || name.Contains("white")
                        ? new List<string> { "Branco" }
                    : name.Contains("preto") || name.Contains("black")
                        ? new List<string> { "Preto" }
                    : name.Contains("cinza") || name.Contains("grafite")
                        ? new List<string> { "Cinza" }
                    : name.Contains("verde") || name.Contains("neon")
                        ? new List<string> { "Verde neon" }
                    : new List<string> { "Preto", "Branco" };

                product.AvailableColorsJson = JsonSerializer.Serialize(colors);
            }

            // =================================================================
            // 7. GALERIAS ESPECÍFICAS DOS BONÉS (idempotente)
            // =================================================================
            var capProducts = await context.Products
                .Where(p => p.Name.StartsWith("Boné Beta Fit"))
                .ToListAsync();

            var capGallery = new Dictionary<string, List<string>>
            {
                ["Branco"] = new() { "/images/products/Boné Beta Fit Branco.png" },
                ["Cinza"] = new() { "/images/products/Boné Beta Fit Grafite.png" },
                ["Preto"] = new() { "/images/products/Boné Beta Fit Preto.png" },
                ["Verde neon"] = new() { "/images/products/Boné Beta Fit Verde Neon.png" }
            };

            foreach (var cap in capProducts)
            {
                cap.AvailableColorsJson = JsonSerializer.Serialize(capGallery.Keys);
                cap.ColorGalleriesJson = JsonSerializer.Serialize(capGallery);
                cap.ColorImageUrlsJson = JsonSerializer.Serialize(
                    capGallery.ToDictionary(x => x.Key, x => x.Value[0]));
            }

            await context.SaveChangesAsync();

            // =================================================================
            // 8. GALERIA PERSISTENTE (ProductImage) - idempotente
            // =================================================================
            foreach (var product in await context.Products.Include(p => p.Images).ToListAsync())
            {
                if (product.Images.Any())
                {
                    continue;
                }

                List<string> urls = new();

                try
                {
                    urls = JsonSerializer.Deserialize<List<string>>(
                        product.ImageUrlsJson ?? "[]") ?? new();
                }
                catch
                {
                    urls = new();
                }

                if (!urls.Any() && !string.IsNullOrWhiteSpace(product.ImageUrl))
                {
                    urls.Add(product.ImageUrl);
                }

                product.Images = urls
                    .Where(x => !string.IsNullOrWhiteSpace(x))
                    .Distinct(StringComparer.OrdinalIgnoreCase)
                    .Select((url, index) => new ProductImage
                    {
                        Url = url,
                        SortOrder = index,
                        IsPrimary = index == 0
                    })
                    .ToList();
            }

            await context.SaveChangesAsync();
        }
    }
}