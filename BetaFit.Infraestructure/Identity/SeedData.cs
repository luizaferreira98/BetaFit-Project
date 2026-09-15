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
            // 2.1. SEED DOS ARQUIVOS DE PRODUTO DO CATÁLOGO
            // Garante que todas as imagens existentes no catálogo tenham produto,
            // categoria e tipo (Gender), inclusive quando o banco já foi populado.
            // =====================================================================
            var catalogAssets = new[]
            {
                new { File = "Camisa Asta Costa.png", Name = "Camisa Asta Costa", Category = "Camisetas", Gender = Gender.Unissex, Price = 99.90m },
                new { File = "Camisa Asta Frente.png", Name = "Camisa Asta Frente", Category = "Camisetas", Gender = Gender.Unissex, Price = 99.90m },
                new { File = "Camisa Nami Frente.png", Name = "Camisa Nami Frente", Category = "Camisetas", Gender = Gender.Unissex, Price = 99.90m },
                new { File = "Camisa Nami.png", Name = "Camisa Nami", Category = "Camisetas", Gender = Gender.Unissex, Price = 99.90m },
                new { File = "Camisa Satoro Gojo Frente.png", Name = "Camisa Satoro Gojo Frente", Category = "Camisetas", Gender = Gender.Unissex, Price = 99.90m },
                new { File = "Camisa Satoro Gojo.png", Name = "Camisa Satoro Gojo", Category = "Camisetas", Gender = Gender.Unissex, Price = 99.90m },
                new { File = "Camisa Zoro Costa.png", Name = "Camisa Zoro Costa", Category = "Camisetas", Gender = Gender.Unissex, Price = 99.90m },
                new { File = "Camisa Zoro Frente.png", Name = "Camisa Zoro Frente", Category = "Camisetas", Gender = Gender.Unissex, Price = 99.90m },
                new { File = "Gemini_Generated_Image_66mtea66mtea66mt (1).jpg", Name = "Gemini Generated Image 66mtea66mtea66mt", Category = "Camisetas", Gender = Gender.Unissex, Price = 99.90m },
                new { File = "Gemini_Generated_Image_p2umigp2umigp2um (1).jpg", Name = "Gemini Generated Image P2umigp2umigp2um", Category = "Camisetas", Gender = Gender.Unissex, Price = 99.90m },
                new { File = "betafit_bone_branco.png", Name = "Betafit Bone Branco", Category = "Acessórios", Gender = Gender.Unissex, Price = 49.90m },
                new { File = "betafit_bone_grafite.png", Name = "Betafit Bone Grafite", Category = "Acessórios", Gender = Gender.Unissex, Price = 49.90m },
                new { File = "betafit_bone_preto.png", Name = "Betafit Bone Preto", Category = "Acessórios", Gender = Gender.Unissex, Price = 49.90m },
                new { File = "betafit_bone_verde_neon.png", Name = "Betafit Bone Verde Neon", Category = "Acessórios", Gender = Gender.Unissex, Price = 49.90m },
                new { File = "betafit_calca_cinza_com_camiseta.png", Name = "Betafit Calca Cinza Com Camiseta", Category = "Leggings", Gender = Gender.Unissex, Price = 149.90m },
                new { File = "betafit_calca_feminina_grafite.png", Name = "Betafit Calca Feminina Grafite", Category = "Leggings", Gender = Gender.Feminino, Price = 149.90m },
                new { File = "betafit_calca_feminina_neon.png", Name = "Betafit Calca Feminina Neon", Category = "Leggings", Gender = Gender.Feminino, Price = 149.90m },
                new { File = "betafit_core_discipline_clean.png", Name = "Betafit Core Discipline Clean", Category = "Camisetas", Gender = Gender.Unissex, Price = 99.90m },
                new { File = "betafit_galao_grafite_urso_9x16.png", Name = "Betafit Galao Grafite Urso 9x16", Category = "Acessórios", Gender = Gender.Unissex, Price = 49.90m },
                new { File = "betafit_galao_preto_beta_9x16.png", Name = "Betafit Galao Preto Beta 9x16", Category = "Acessórios", Gender = Gender.Unissex, Price = 49.90m },
                new { File = "betafit_moletom_masculino.png", Name = "Betafit Moletom Masculino", Category = "Moletons", Gender = Gender.Masculino, Price = 199.90m },
                new { File = "betafit_power_elements_robusto.png", Name = "Betafit Power Elements Robusto", Category = "Camisetas", Gender = Gender.Unissex, Price = 99.90m },
                new { File = "betafit_progression_line_clean.png", Name = "Betafit Progression Line Clean", Category = "Camisetas", Gender = Gender.Unissex, Price = 99.90m },
                new { File = "betafit_regata_cinza_frente_urso.png", Name = "Betafit Regata Cinza Frente Urso", Category = "Camisetas", Gender = Gender.Unissex, Price = 99.90m },
                new { File = "betafit_regata_cinza_textura.png", Name = "Betafit Regata Cinza Textura", Category = "Camisetas", Gender = Gender.Unissex, Price = 99.90m },
                new { File = "betafit_regata_preta_textura.png", Name = "Betafit Regata Preta Textura", Category = "Camisetas", Gender = Gender.Unissex, Price = 99.90m },
                new { File = "betafit_shaker_branco_urso_9x16.png", Name = "Betafit Shaker Branco Urso 9x16", Category = "Acessórios", Gender = Gender.Unissex, Price = 49.90m },
                new { File = "betafit_shaker_preto_beta_9x16.png", Name = "Betafit Shaker Preto Beta 9x16", Category = "Acessórios", Gender = Gender.Unissex, Price = 49.90m },
                new { File = "betafit_short_feminino_9x16.png.png", Name = "Betafit Short Feminino 9x16.png", Category = "Shorts", Gender = Gender.Feminino, Price = 89.90m },
                new { File = "betafit_short_feminino_branco.png", Name = "Betafit Short Feminino Branco", Category = "Shorts", Gender = Gender.Feminino, Price = 89.90m },
                new { File = "betafit_short_masculino_9x16.png", Name = "Betafit Short Masculino 9x16", Category = "Shorts", Gender = Gender.Masculino, Price = 89.90m },
                new { File = "betafit_short_masculino_branco.png", Name = "Betafit Short Masculino Branco", Category = "Shorts", Gender = Gender.Masculino, Price = 89.90m },
                new { File = "composicao_dois_modelos_costas_c (1).png", Name = "Composicao Dois Modelos Costas C", Category = "Camisetas", Gender = Gender.Unissex, Price = 99.90m },
                new { File = "foto_direita_9x16_corpo.jpg - Cr (1).png", Name = "Foto Direita 9x16 Corpo.jpg Cr", Category = "Tênis", Gender = Gender.Unissex, Price = 299.90m },
                new { File = "foto_individual_01.png - Crop (1).png", Name = "Foto Individual 01.png Crop", Category = "Tênis", Gender = Gender.Unissex, Price = 299.90m },
                new { File = "foto_individual_04.png", Name = "Foto Individual 04", Category = "Tênis", Gender = Gender.Unissex, Price = 299.90m },
                new { File = "foto_individual_05.png", Name = "Foto Individual 05", Category = "Tênis", Gender = Gender.Unissex, Price = 299.90m },
                new { File = "foto_individual_06.png", Name = "Foto Individual 06", Category = "Tênis", Gender = Gender.Unissex, Price = 299.90m },
                new { File = "foto_individual_07.png", Name = "Foto Individual 07", Category = "Tênis", Gender = Gender.Unissex, Price = 299.90m },
                new { File = "foto_meio_9x16_corpo.png", Name = "Foto Meio 9x16 Corpo", Category = "Tênis", Gender = Gender.Unissex, Price = 299.90m },
                new { File = "garrafa_centro_9x16.png", Name = "Garrafa Centro 9x16", Category = "Acessórios", Gender = Gender.Unissex, Price = 49.90m },
                new { File = "garrafa_direita_9x16.png", Name = "Garrafa Direita 9x16", Category = "Acessórios", Gender = Gender.Unissex, Price = 49.90m },
                new { File = "garrafa_esquerda_9x16.png", Name = "Garrafa Esquerda 9x16", Category = "Acessórios", Gender = Gender.Unissex, Price = 49.90m },
                new { File = "garrafa_ferrari_9x16.png", Name = "Garrafa Ferrari 9x16", Category = "Acessórios", Gender = Gender.Unissex, Price = 49.90m },
                new { File = "garrafa_luffy_9x16.png", Name = "Garrafa Luffy 9x16", Category = "Acessórios", Gender = Gender.Unissex, Price = 49.90m },
                new { File = "garrafa_mercedes_9x16.png", Name = "Garrafa Mercedes 9x16", Category = "Acessórios", Gender = Gender.Unissex, Price = 49.90m },
                new { File = "garrafa_noelle_9x16.png", Name = "Garrafa Noelle 9x16", Category = "Acessórios", Gender = Gender.Unissex, Price = 49.90m },
                new { File = "garrafa_red_bull_racing_9x16.png", Name = "Garrafa Red Bull Racing 9x16", Category = "Acessórios", Gender = Gender.Unissex, Price = 49.90m },
                new { File = "garrafa_sanji_9x16.png", Name = "Garrafa Sanji 9x16", Category = "Acessórios", Gender = Gender.Unissex, Price = 49.90m },
                new { File = "garrafa_sem_texto_alca_02.png", Name = "Garrafa Sem Texto Alca 02", Category = "Acessórios", Gender = Gender.Unissex, Price = 49.90m },
                new { File = "garrafa_yuno_9x16.png", Name = "Garrafa Yuno 9x16", Category = "Acessórios", Gender = Gender.Unissex, Price = 49.90m },
                new { File = "garrafa_zoro_9x16.png", Name = "Garrafa Zoro 9x16", Category = "Acessórios", Gender = Gender.Unissex, Price = 49.90m },
                new { File = "homem_costas_mercedes_betafit_mi (1).png", Name = "Homem Costas Mercedes Betafit Mi", Category = "Camisetas", Gender = Gender.Masculino, Price = 99.90m },
                new { File = "homem_costas_sem_protecao_9x16.j (1).png", Name = "Homem Costas Sem Protecao 9x16.j", Category = "Camisetas", Gender = Gender.Masculino, Price = 99.90m },
                new { File = "homem_ferrari_costas_academia_9x (1).png", Name = "Homem Ferrari Costas Academia 9x", Category = "Camisetas", Gender = Gender.Masculino, Price = 99.90m },
                new { File = "homem_ferrari_frente_academia_9x (1).png", Name = "Homem Ferrari Frente Academia 9x", Category = "Camisetas", Gender = Gender.Masculino, Price = 99.90m },
                new { File = "homem_frente_mercedes_betafit_9x (1).png", Name = "Homem Frente Mercedes Betafit 9x", Category = "Camisetas", Gender = Gender.Masculino, Price = 99.90m },
                new { File = "homem_sem_bracos_cruzados_9x16.j (1).png", Name = "Homem Sem Bracos Cruzados 9x16.j", Category = "Camisetas", Gender = Gender.Masculino, Price = 99.90m },
                new { File = "mulher_costas_betafit_mais_trein (1).png", Name = "Mulher Costas Betafit Mais Trein", Category = "Camisetas", Gender = Gender.Feminino, Price = 99.90m },
                new { File = "mulher_costas_mais_treinada_v2_9 (1).png", Name = "Mulher Costas Mais Treinada V2 9", Category = "Camisetas", Gender = Gender.Feminino, Price = 99.90m },
                new { File = "mulher_ferrari_costas_academia_9 (1).png", Name = "Mulher Ferrari Costas Academia 9", Category = "Camisetas", Gender = Gender.Feminino, Price = 99.90m },
                new { File = "mulher_ferrari_frente_academia_9 (1).png", Name = "Mulher Ferrari Frente Academia 9", Category = "Camisetas", Gender = Gender.Feminino, Price = 99.90m },
                new { File = "mulher_frente_9x16.jpg - Crop 9_ (1).png", Name = "Mulher Frente 9x16.jpg Crop 9", Category = "Camisetas", Gender = Gender.Feminino, Price = 99.90m },
                new { File = "mulher_separada_9x16.jpg - Crop (1).png", Name = "Mulher Separada 9x16.jpg Crop", Category = "Camisetas", Gender = Gender.Feminino, Price = 99.90m },
                new { File = "shaker_betafit_9x16.png", Name = "Shaker Betafit 9x16", Category = "Acessórios", Gender = Gender.Unissex, Price = 49.90m },
            };

            foreach (var asset in catalogAssets)
            {
                var imageUrl = $"/images/products/{asset.File}";
                if (await context.Products.AnyAsync(p => p.ImageUrl == imageUrl)) continue;

                var categoryEntity = await context.Categories.FirstAsync(c => c.Name == asset.Category);
                await context.Products.AddAsync(new Product
                {
                    Name = asset.Name,
                    Description = $"{asset.Name} da coleção Beta Fit.",
                    Price = asset.Price,
                    ImageUrl = imageUrl,
                    ImageUrlsJson = JsonSerializer.Serialize(new[] { imageUrl }),
                    Gender = asset.Gender,
                    CategoryId = categoryEntity.Id,
                    IsFeatured = false,
                    IsActive = true,
                    CreatedAt = DateTime.Now
                });
            }
            await context.SaveChangesAsync();

            // Corrige dados de demonstração antigos: acessórios não ganham tamanho inventado e tênis usam numeração.
            var shoeCategory = await context.Categories.FirstOrDefaultAsync(c => c.Name == "Tênis");
            if (shoeCategory is not null)
            {
                var shoeProducts = await context.Products.Where(p => p.CategoryId == shoeCategory.Id).ToListAsync();
                foreach (var shoe in shoeProducts) shoe.AvailableSizesJson = JsonSerializer.Serialize(Enumerable.Range(35, 8).Select(x => x.ToString()).ToList());
            }
            var noSizeCategories = await context.Categories.Where(c => c.Name == "Acessórios").Select(c => c.Id).ToListAsync();
            var accessories = await context.Products.Where(p => noSizeCategories.Contains(p.CategoryId)).ToListAsync();
            foreach (var item in accessories) item.AvailableSizesJson = "[]";
            await context.SaveChangesAsync();

            var clothingIds = await context.Categories.Where(c => c.Name == "Camisetas" || c.Name == "Leggings" || c.Name == "Moletons" || c.Name == "Shorts").Select(c => c.Id).ToListAsync();
            foreach (var p in await context.Products.Where(p => clothingIds.Contains(p.CategoryId)).ToListAsync())
                if (string.IsNullOrWhiteSpace(p.AvailableSizesJson) || p.AvailableSizesJson == "[]") p.AvailableSizesJson = "[\"P\",\"M\",\"G\",\"GG\",\"XG\"]";
            await context.SaveChangesAsync();
            // Garante que os produtos antigos também tenham linhas na galeria persistente.
            foreach (var product in await context.Products.Include(p => p.Images).ToListAsync())
            {
                if (!product.Images.Any())
                {
                    List<string> urls = new();
                    try { urls = JsonSerializer.Deserialize<List<string>>(product.ImageUrlsJson ?? "[]") ?? new(); } catch { }
                    if (!urls.Any() && !string.IsNullOrWhiteSpace(product.ImageUrl)) urls.Add(product.ImageUrl);
                    product.Images = urls.Where(x => !string.IsNullOrWhiteSpace(x)).Distinct(StringComparer.OrdinalIgnoreCase).Select((url, i) => new ProductImage { Url = url, SortOrder = i, IsPrimary = i == 0 }).ToList();
                }
                List<string> currentColors = new();
                try { currentColors = JsonSerializer.Deserialize<List<string>>(product.AvailableColorsJson ?? "[]") ?? new(); } catch { }
                if (!currentColors.Any())
                {
                    var name = product.Name.ToLowerInvariant();
                    currentColors = name.Contains("branco") ? new() { "Branco" }
                        : name.Contains("preto") || name.Contains("all black") ? new() { "Preto" }
                        : name.Contains("cinza") || name.Contains("grafite") ? new() { "Cinza" }
                        : name.Contains("verde") || name.Contains("neon") ? new() { "Verde neon" }
                        : new() { "Preto", "Branco" };
                    product.AvailableColorsJson = JsonSerializer.Serialize(currentColors);
                    if (product.Name.StartsWith("Boné Beta Fit", StringComparison.Ordinal))
                    {
                        var gallery = new Dictionary<string, List<string>> { { "Preto", new() { "/images/products/betafit_bone_preto.jpg" } }, { "Branco", new() { "/images/products/betafit_bone_branco.jpg" } }, { "Cinza", new() { "/images/products/betafit_bone_grafite.jpg" } }, { "Verde neon", new() { "/images/products/betafit_bone_verde_neon.jpg" } } };
                        product.ColorGalleriesJson = JsonSerializer.Serialize(gallery); product.ColorImageUrlsJson = JsonSerializer.Serialize(gallery.ToDictionary(x => x.Key, x => x.Value[0])); product.AvailableColorsJson = JsonSerializer.Serialize(gallery.Keys);
                    }
                }
            }
            await context.SaveChangesAsync();

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

            if (!await roleManager.RoleExistsAsync("Gerente"))
            {
                await roleManager.CreateAsync(new IdentityRole("Gerente"));
            }

            if (!await roleManager.RoleExistsAsync("Funcionario"))
            {
                await roleManager.CreateAsync(new IdentityRole("Funcionario"));
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
                    await userManager.AddClaimsAsync(adminUser, new[] { new Claim("FullName", "Administrador BetaFit"), new Claim("BirthDate", "1990-01-01") });
                }
            }

            if (adminUser is not null)
            {
                var adminClaims = await userManager.GetClaimsAsync(adminUser);
                if (!adminClaims.Any(c => c.Type == "FullName")) await userManager.AddClaimAsync(adminUser, new Claim("FullName", "Administrador BetaFit"));
                if (!adminClaims.Any(c => c.Type == "BirthDate")) await userManager.AddClaimAsync(adminUser, new Claim("BirthDate", "1990-01-01"));
            }
        }
    }
}
