// =============================================================================
// BetaFit.Infraestructure - Seed Data (Dados Iniciais)
// =============================================================================
//  CONCEITO IMPORTANTE: Seed Data
// Seed Data são dados iniciais que são inseridos no banco de dados
// quando a aplicação é executada pela primeira vez.
// Isso é útil para:
// - Ter dados de demonstração (categorias e produtos da loja)
// - Criar o usuário administrador inicial
// - Popular roles padrão
//
// Este método é chamado no Program.cs (da API) durante a inicialização.
// =============================================================================

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
        /// Popula o banco de dados com categorias, produtos e o usuário admin.
        /// Este método é idempotente — pode ser chamado várias vezes sem duplicar dados.
        /// </summary>
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            // Obtém o DbContext do container de Dependency Injection
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<BetaFitDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Aplica migrations pendentes automaticamente
            await context.Database.MigrateAsync();

            // =====================================================================
            // 1. SEED DE CATEGORIAS
            // =====================================================================
            if (!context.Categories.Any())
            {
                var categories = new List<Category>
                {
                    new Category { Name = "Camisetas", Description = "Camisetas casuais e esportivas" },
                    new Category { Name = "Leggings", Description = "Leggings e calças de treino" },
                    new Category { Name = "Moletons", Description = "Moletons e casacos" },
                    new Category { Name = "Shorts", Description = "Shorts esportivos e casuais" },
                    new Category { Name = "Tênis", Description = "Calçados esportivos" },
                    new Category { Name = "Acessórios", Description = "Bonés, meias e acessórios" }
                };

                await context.Categories.AddRangeAsync(categories);
                await context.SaveChangesAsync();
            }

            // =====================================================================
            // 2. SEED DE PRODUTOS
            // =====================================================================
            if (!context.Products.Any())
            {
                // Busca as categorias recém-criadas para obter os IDs
                var camisetas = await context.Categories.FirstAsync(c => c.Name == "Camisetas");
                var leggings = await context.Categories.FirstAsync(c => c.Name == "Leggings");
                var moletons = await context.Categories.FirstAsync(c => c.Name == "Moletons");
                var shorts = await context.Categories.FirstAsync(c => c.Name == "Shorts");
                var tenis = await context.Categories.FirstAsync(c => c.Name == "Tênis");
                var acessorios = await context.Categories.FirstAsync(c => c.Name == "Acessórios");

                var products = new List<Product>
                {
                    new Product
                    {
                        Name = "Camiseta Dry Fit Performance",
                        Description = "Camiseta com tecido de secagem rápida, ideal para treinos de alta intensidade.",
                        Price = 89.90m,
                        ImageUrl = "https://images.unsplash.com/photo-1521572163474-6864f9cf17ab",
                        Gender = Gender.Masculino,
                        CategoryId = camisetas.Id,
                        IsFeatured = true,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Legging High Support",
                        Description = "Legging de cintura alta com compressão, perfeita para treinos funcionais e corrida.",
                        Price = 129.90m,
                        ImageUrl = "https://images.unsplash.com/photo-1594381898411-846e7d193883",
                        Gender = Gender.Feminino,
                        CategoryId = leggings.Id,
                        IsFeatured = true,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Moletom Canguru Beta Fit",
                        Description = "Moletom com capuz e bolso canguru, forro macio para dias frios de treino.",
                        Price = 179.90m,
                        ImageUrl = "https://images.unsplash.com/photo-1556821840-3a63f95609a7",
                        Gender = Gender.Unissex,
                        CategoryId = moletons.Id,
                        IsFeatured = true,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Short Everyday",
                        Description = "Short leve e respirável, com bolso lateral e cordão de ajuste.",
                        Price = 69.90m,
                        ImageUrl = "https://images.unsplash.com/photo-1591195853828-11db59a44f6b",
                        Gender = Gender.Masculino,
                        CategoryId = shorts.Id,
                        IsFeatured = false,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Tênis Runner Pro",
                        Description = "Tênis para corrida com amortecimento em gel e solado antiderrapante.",
                        Price = 349.90m,
                        ImageUrl = "https://images.unsplash.com/photo-1542291026-7eec264c27ff",
                        Gender =    Gender.Unissex,
                        CategoryId = tenis.Id,
                        IsFeatured = true,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Boné Beta Fit Performance",
                        Description = "Boné esportivo ajustável com proteção UV e tecido de secagem rápida.",
                        Price = 49.90m,
                        ImageUrl = "https://images.unsplash.com/photo-1521369909029-2afed882baee",
                        Gender = Gender.Unissex,
                        CategoryId = acessorios.Id,
                        IsFeatured = false,
                        CreatedAt = DateTime.Now
                    }
                };

                await context.Products.AddRangeAsync(products);
                await context.SaveChangesAsync();
            }

            // =====================================================================
            // 3. SEED DE ROLES (Papéis de Usuário)
            // =====================================================================
            //  CONCEITO: Roles no Identity
            // Roles são papéis que definem o nível de acesso do usuário.
            // Exemplo: "Admin" pode gerenciar produtos, "Usuario" só pode visualizar.
            // =====================================================================
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("Usuario"))
            {
                await roleManager.CreateAsync(new IdentityRole("Usuario"));
            }

            // =====================================================================
            // 4. SEED DO USUÁRIO ADMINISTRADOR
            // =====================================================================
            //  CONCEITO: UserManager
            // O UserManager é o serviço do Identity para gerenciar usuários.
            // Ele permite criar, buscar, atualizar e deletar usuários.
            // =====================================================================
            var adminEmail = "admin@betafit.com";
            var adminUser = await userManager.FindByEmailAsync(adminEmail);

            if (adminUser == null)
            {
                adminUser = new IdentityUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true // Confirma o email automaticamente
                };

                // Cria o usuário com a senha padrão
                var result = await userManager.CreateAsync(adminUser, "Admin@123");

                if (result.Succeeded)
                {
                    // Atribui a role "Admin" ao usuário
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}