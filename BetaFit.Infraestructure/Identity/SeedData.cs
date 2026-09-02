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
                var tenis = await context.Categories.FirstAsync(c => c.Name == "Tênis");
                var acessorios = await context.Categories.FirstAsync(c => c.Name == "Acessórios");

                var products = new List<Product>
                {
                    // ---------------- TÊNIS ----------------
                    new Product
                    {
                        Name = "Tênis Urban Low White",
                        Description = "Tênis casual branco com detalhe preto no calcanhar, cabedal em couro sintético e solado emborrachado leve. Combina com o dia a dia e treinos leves.",
                        Price = 259.90m,
                        ImageUrl = "/images/products/betafit_tenis_feminino_branco_preto.jpg",
                        ImageUrlsJson = "[\"/images/products/betafit_tenis_feminino_branco_preto.jpg\"]",
                        Gender = Gender.Feminino,
                        CategoryId = tenis.Id,
                        IsFeatured = true,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Tênis Runner Flex Black",
                        Description = "Tênis de corrida all black, cabedal em mesh respirável e entressola com amortecimento macio para treinos de alta performance.",
                        Price = 329.90m,
                        ImageUrl = "/images/products/betafit_tenis_feminino_all_black.jpg",
                        ImageUrlsJson = "[\"/images/products/betafit_tenis_feminino_all_black.jpg\"]",
                        Gender = Gender.Feminino,
                        CategoryId = tenis.Id,
                        IsFeatured = false,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Tênis Runner Panther Black",
                        Description = "Tênis de corrida masculino com logo Beta Fit no lateral, solado espesso com bom amortecimento e mesh reforçado para treinos intensos.",
                        Price = 349.90m,
                        ImageUrl = "/images/products/betafit_tenis_masculino_all_black.jpg",
                        ImageUrlsJson = "[\"/images/products/betafit_tenis_masculino_all_black.jpg\"]",
                        Gender = Gender.Masculino,
                        CategoryId = tenis.Id,
                        IsFeatured = true,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Tênis Runner Panther White",
                        Description = "Versão branca do Runner Panther, com solado em duas cores e cabedal em mesh macio — leveza e respiro para longas distâncias.",
                        Price = 349.90m,
                        ImageUrl = "/images/products/betafit_tenis_masculino_branco_preto.jpg",
                        ImageUrlsJson = "[\"/images/products/betafit_tenis_masculino_branco_preto.jpg\"]",
                        Gender = Gender.Masculino,
                        CategoryId = tenis.Id,
                        IsFeatured = false,
                        CreatedAt = DateTime.Now
                    },

                    // ---------------- CAMISETAS / REGATAS ----------------
                    new Product
                    {
                        Name = "Regata Dry Performance Preta",
                        Description = "Regata masculina em tecido dry fit com textura sport, corte reto e logo Beta discreto no peito. Ideal para treinos de alta intensidade.",
                        Price = 79.90m,
                        ImageUrl = "/images/products/betafit_regata_preta_textura.jpg",
                        ImageUrlsJson = "[\"/images/products/betafit_regata_preta_textura.jpg\"]",
                        Gender = Gender.Masculino,
                        CategoryId = camisetas.Id,
                        IsFeatured = true,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Regata Dry Performance Cinza",
                        Description = "Regata masculina cinza mescla com logo do urso Beta Fit, tecido leve e respirável para academia e corrida.",
                        Price = 79.90m,
                        ImageUrl = "/images/products/betafit_regata_cinza_frente_urso.jpg",
                        ImageUrlsJson = "[\"/images/products/betafit_regata_cinza_frente_urso.jpg\"]",
                        Gender = Gender.Masculino,
                        CategoryId = camisetas.Id,
                        IsFeatured = false,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Camiseta Oversized Vista Sua Disciplina",
                        Description = "Camiseta oversized preta unissex com estampa 'Vista Sua Disciplina' nas costas. Modelagem ampla e tecido encorpado.",
                        Price = 119.90m,
                        ImageUrl = "/images/products/blusa_dupla_modelos_fitness_beta.jpg",
                        ImageUrlsJson = "[\"/images/products/blusa_dupla_modelos_fitness_beta.jpg\"]",
                        Gender = Gender.Unissex,
                        CategoryId = camisetas.Id,
                        IsFeatured = true,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Camiseta Oversized Move Forward",
                        Description = "Camiseta oversized preta unissex com estampa gráfica neon 'Move Forward' nas costas. Algodão premium e caimento solto.",
                        Price = 119.90m,
                        ImageUrl = "/images/products/betafit_progression_line_clean.jpg",
                        ImageUrlsJson = "[\"/images/products/betafit_progression_line_clean.jpg\"]",
                        Gender = Gender.Unissex,
                        CategoryId = camisetas.Id,
                        IsFeatured = false,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Camiseta Oversized Core Discipline",
                        Description = "Camiseta oversized preta unissex com estampa de cronômetro nas costas, remetendo ao foco e à disciplina do treino.",
                        Price = 119.90m,
                        ImageUrl = "/images/products/betafit_core_discipline_clean.jpg",
                        ImageUrlsJson = "[\"/images/products/betafit_core_discipline_clean.jpg\"]",
                        Gender = Gender.Unissex,
                        CategoryId = camisetas.Id,
                        IsFeatured = false,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Camiseta Oversized Elite Neon",
                        Description = "Camiseta oversized preta masculina com estampa 'BETA' fragmentada em neon nas costas. Visual streetwear para dentro e fora da academia.",
                        Price = 119.90m,
                        ImageUrl = "/images/products/betafit_elite_neon_clean.jpg",
                        ImageUrlsJson = "[\"/images/products/betafit_elite_neon_clean.jpg\"]",
                        Gender = Gender.Masculino,
                        CategoryId = camisetas.Id,
                        IsFeatured = false,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Camiseta Oversized Power Elements",
                        Description = "Camiseta oversized branca masculina com estampa 'Power Elements' nas costas. Tecido encorpado e caimento solto.",
                        Price = 119.90m,
                        ImageUrl = "/images/products/betafit_power_elements_robusto_rosto.jpg",
                        ImageUrlsJson = "[\"/images/products/betafit_power_elements_robusto_rosto.jpg\"]",
                        Gender = Gender.Masculino,
                        CategoryId = camisetas.Id,
                        IsFeatured = true,
                        CreatedAt = DateTime.Now
                    },

                    // ---------------- LEGGINGS / CALÇAS DE TREINO ----------------
                    new Product
                    {
                        Name = "Legging Cargo Neon",
                        Description = "Legging feminina verde neon com bolso cargo lateral e capuz combinando em preto. Estilo statement para treinos funcionais.",
                        Price = 149.90m,
                        ImageUrl = "/images/products/betafit_calca_feminina_neon.jpg",
                        ImageUrlsJson = "[\"/images/products/betafit_calca_feminina_neon.jpg\"]",
                        Gender = Gender.Feminino,
                        CategoryId = leggings.Id,
                        IsFeatured = true,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Legging Performance Grafite",
                        Description = "Legging feminina grafite com compressão e logo do urso Beta Fit em neon, combina com o conjunto de treino de alta performance.",
                        Price = 149.90m,
                        ImageUrl = "/images/products/betafit_calca_feminina_grafite.jpg",
                        ImageUrlsJson = "[\"/images/products/betafit_calca_feminina_grafite.jpg\"]",
                        Gender = Gender.Feminino,
                        CategoryId = leggings.Id,
                        IsFeatured = false,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Calça Jogger Essential Cinza",
                        Description = "Calça jogger masculina cinza mescla, cordão de ajuste na cintura e punho elástico. Conforto para o pré e pós-treino.",
                        Price = 139.90m,
                        ImageUrl = "/images/products/betafit_calca_cinza_com_camiseta.jpg",
                        ImageUrlsJson = "[\"/images/products/betafit_calca_cinza_com_camiseta.jpg\"]",
                        Gender = Gender.Masculino,
                        CategoryId = leggings.Id,
                        IsFeatured = false,
                        CreatedAt = DateTime.Now
                    },

                    // ---------------- MOLETONS ----------------
                    new Product
                    {
                        Name = "Moletom Canguru Beta",
                        Description = "Moletom masculino preto com capuz, bolso canguru com zíper e detalhe neon na gola. Forro macio para dias frios de treino.",
                        Price = 199.90m,
                        ImageUrl = "/images/products/betafit_moletom_masculino.jpg",
                        ImageUrlsJson = "[\"/images/products/betafit_moletom_masculino.jpg\"]",
                        Gender = Gender.Masculino,
                        CategoryId = moletons.Id,
                        IsFeatured = true,
                        CreatedAt = DateTime.Now
                    },

                    // ---------------- ACESSÓRIOS ----------------
                    new Product
                    {
                        Name = "Boné Beta Fit Branco",
                        Description = "Boné aba curva branco com logo Beta Fit bordado, ajuste traseiro e tecido de secagem rápida.",
                        Price = 59.90m,
                        ImageUrl = "/images/products/betafit_bone_branco.jpg",
                        ImageUrlsJson = "[\"/images/products/betafit_bone_branco.jpg\"]",
                        Gender = Gender.Unissex,
                        CategoryId = acessorios.Id,
                        IsFeatured = false,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Boné Beta Fit Grafite",
                        Description = "Boné aba curva grafite com friso neon e logo Beta Fit bordado, visual discreto e esportivo.",
                        Price = 59.90m,
                        ImageUrl = "/images/products/betafit_bone_grafite.jpg",
                        ImageUrlsJson = "[\"/images/products/betafit_bone_grafite.jpg\"]",
                        Gender = Gender.Unissex,
                        CategoryId = acessorios.Id,
                        IsFeatured = false,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Boné Beta Fit Verde Neon",
                        Description = "Boné aba curva verde neon, peça statement da coleção, com logo Beta Fit bordado em preto e branco.",
                        Price = 64.90m,
                        ImageUrl = "/images/products/betafit_bone_verde_neon.jpg",
                        ImageUrlsJson = "[\"/images/products/betafit_bone_verde_neon.jpg\"]",
                        Gender = Gender.Unissex,
                        CategoryId = acessorios.Id,
                        IsFeatured = true,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Boné Beta Fit Preto",
                        Description = "Boné aba curva all black com logo Beta Fit bordado tom sobre tom, o clássico da linha de acessórios.",
                        Price = 59.90m,
                        ImageUrl = "/images/products/betafit_bone_preto.jpg",
                        ImageUrlsJson = "[\"/images/products/betafit_bone_preto.jpg\"]",
                        Gender = Gender.Unissex,
                        CategoryId = acessorios.Id,
                        IsFeatured = false,
                        CreatedAt = DateTime.Now
                    },
                    new Product
                    {
                        Name = "Shaker Pro 700ml",
                        Description = "Coqueteleira Beta Fit de 700ml com misturador interno, tampa com trava e corpo resistente a impacto. Essencial para o pré e pós-treino.",
                        Price = 39.90m,
                        ImageUrl = "/images/products/betafit_shaker_pro.jpg",
                        ImageUrlsJson = "[\"/images/products/betafit_shaker_pro.jpg\"]",
                        Gender = Gender.Unissex,
                        CategoryId = acessorios.Id,
                        IsFeatured = true,
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