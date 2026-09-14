// =============================================================================
// BetaFit.Infraestructure - Seed Data (Dados Iniciais)
// =============================================================================
// Seed Data são os dados iniciais utilizados para popular o banco.
// O método abaixo é idempotente: pode ser executado várias vezes sem duplicar
// produtos ou categorias.
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
        /// Popula categorias, catálogo de produtos, roles e usuário administrador.
        /// </summary>
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var context = scope.ServiceProvider.GetRequiredService<BetaFitDbContext>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

            // Aplica migrations pendentes automaticamente.
            await context.Database.MigrateAsync();

            // =====================================================================
            // 1. SEED DE CATEGORIAS
            // =====================================================================

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

            // =====================================================================
            // 2. SEED / ATUALIZAÇÃO DO CATÁLOGO DE PRODUTOS
            // =====================================================================
            // A lista abaixo reúne os produtos presentes nos arquivos enviados:
            // roupas, tênis, bonés, shakers, galões e garrafas temáticas.
            // =====================================================================

            var products = new List<Product>
            {
                // ---------------- TÊNIS ----------------
                new Product
                {
                    Name = "Tênis Urban Low White",
                    Description = "Tênis esportivo branco, leve e versátil para academia, caminhadas e treinos do dia a dia.",
                    Price = 279.90m,
                    ImageUrl = "/images/products/foto_individual_04.png",
                    ImageUrlsJson = "[\"/images/products/foto_individual_04.png\"]",
                    Gender = Gender.Feminino,
                    CategoryId = tenis.Id,
                    IsFeatured = true,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Tênis Runner Flex Black",
                    Description = "Tênis esportivo preto de perfil baixo, com visual minimalista e solado emborrachado para uso diário.",
                    Price = 289.90m,
                    ImageUrl = "/images/products/foto_individual_05.png",
                    ImageUrlsJson = "[\"/images/products/foto_individual_05.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = tenis.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Tênis Runner Panther Black",
                    Description = "Tênis preto para treino, com design discreto e estrutura leve para academia e atividades esportivas.",
                    Price = 299.90m,
                    ImageUrl = "/images/products/foto_individual_06.png",
                    ImageUrlsJson = "[\"/images/products/foto_individual_06.png\"]",
                    Gender = Gender.Masculino,
                    CategoryId = tenis.Id,
                    IsFeatured = true,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Tênis Runner Panther White",
                    Description = "Tênis branco esportivo com visual clean e solado confortável para corrida e treinos leves.",
                    Price = 279.90m,
                    ImageUrl = "/images/products/foto_individual_07.png",
                    ImageUrlsJson = "[\"/images/products/foto_individual_07.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = tenis.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Tênis Classic White",
                    Description = "Tênis branco com visual casual e esportivo para academia e uso diário.",
                    Price = 269.90m,
                    ImageUrl = "/images/products/foto_individual_01.png - Crop (1).png",
                    ImageUrlsJson = "[\"/images/products/foto_individual_01.png - Crop (1).png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = tenis.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },


                // ---------------- CAMISETAS / REGATAS ----------------
                new Product
                {
                    Name = "Regata Dry Performance Preta",
                    Description = "Regata masculina preta em tecido leve e respirável, com textura esportiva para treinos intensos.",
                    Price = 79.90m,
                    ImageUrl = "/images/products/betafit_regata_preta_textura.png",
                    ImageUrlsJson = "[\"/images/products/betafit_regata_preta_textura.png\"]",
                    Gender = Gender.Masculino,
                    CategoryId = camisetas.Id,
                    IsFeatured = true,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Regata Dry Performance Cinza",
                    Description = "Regata masculina cinza mescla com identidade Beta Fit e tecido leve para academia e corrida.",
                    Price = 79.90m,
                    ImageUrl = "/images/products/betafit_regata_cinza_frente_urso.png",
                    ImageUrlsJson = "[\"/images/products/betafit_regata_cinza_frente_urso.png\",\"/images/products/betafit_regata_cinza_textura.png\"]",
                    Gender = Gender.Masculino,
                    CategoryId = camisetas.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Camiseta Oversized Move Forward",
                    Description = "Camiseta oversized preta com arte neon da linha Progression, modelagem ampla e estética street fitness.",
                    Price = 119.90m,
                    ImageUrl = "/images/products/betafit_progression_line_clean.png",
                    ImageUrlsJson = "[\"/images/products/betafit_progression_line_clean.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = camisetas.Id,
                    IsFeatured = true,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Camiseta Oversized Core Discipline",
                    Description = "Camiseta oversized preta com identidade visual Core Discipline, feita para treinos e uso casual.",
                    Price = 119.90m,
                    ImageUrl = "/images/products/betafit_core_discipline_clean.png",
                    ImageUrlsJson = "[\"/images/products/betafit_core_discipline_clean.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = camisetas.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Camiseta Oversized Power Elements",
                    Description = "Camiseta oversized branca com estampa Power Elements, tecido encorpado e caimento solto.",
                    Price = 119.90m,
                    ImageUrl = "/images/products/betafit_power_elements_robusto.png",
                    ImageUrlsJson = "[\"/images/products/betafit_power_elements_robusto.png\"]",
                    Gender = Gender.Masculino,
                    CategoryId = camisetas.Id,
                    IsFeatured = true,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Camiseta Asta",
                    Description = "Camiseta oversized de coleção anime com estampas frontal e traseira inspiradas em Asta.",
                    Price = 129.90m,
                    ImageUrl = "/images/products/Camisa Asta Frente.png",
                    ImageUrlsJson = "[\"/images/products/Camisa Asta Frente.png\",\"/images/products/Camisa Asta Costa.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = camisetas.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Camiseta Nami",
                    Description = "Camiseta oversized de coleção anime com estampas frontal e traseira inspiradas em Nami.",
                    Price = 129.90m,
                    ImageUrl = "/images/products/Camisa Nami Frente.png",
                    ImageUrlsJson = "[\"/images/products/Camisa Nami Frente.png\",\"/images/products/Camisa Nami.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = camisetas.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Camiseta Satoro Gojo",
                    Description = "Camiseta oversized de coleção anime com estampas frontal e traseira inspiradas em Satoro Gojo.",
                    Price = 129.90m,
                    ImageUrl = "/images/products/Camisa Satoro Gojo Frente.png",
                    ImageUrlsJson = "[\"/images/products/Camisa Satoro Gojo Frente.png\",\"/images/products/Camisa Satoro Gojo.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = camisetas.Id,
                    IsFeatured = true,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Camiseta Zoro",
                    Description = "Camiseta oversized de coleção anime com estampas frontal e traseira inspiradas em Zoro.",
                    Price = 129.90m,
                    ImageUrl = "/images/products/Camisa Zoro Frente.png",
                    ImageUrlsJson = "[\"/images/products/Camisa Zoro Frente.png\",\"/images/products/Camisa Zoro Costa.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = camisetas.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Camiseta Beta Fit Mercedes Masculina",
                    Description = "Camiseta fitness masculina com identidade esportiva inspirada em Mercedes, com frente e costas.",
                    Price = 129.90m,
                    ImageUrl = "/images/products/homem_frente_mercedes_betafit_9x (1).png",
                    ImageUrlsJson = "[\"/images/products/homem_frente_mercedes_betafit_9x (1).png\",\"/images/products/homem_costas_mercedes_betafit_mi (1).png\"]",
                    Gender = Gender.Masculino,
                    CategoryId = camisetas.Id,
                    IsFeatured = true,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Camiseta Beta Fit Ferrari Masculina",
                    Description = "Camiseta fitness masculina com identidade esportiva inspirada em Ferrari, com frente e costas.",
                    Price = 129.90m,
                    ImageUrl = "/images/products/homem_ferrari_frente_academia_9x (1).png",
                    ImageUrlsJson = "[\"/images/products/homem_ferrari_frente_academia_9x (1).png\",\"/images/products/homem_ferrari_costas_academia_9x (1).png\"]",
                    Gender = Gender.Masculino,
                    CategoryId = camisetas.Id,
                    IsFeatured = true,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Camiseta Beta Fit Ferrari Feminina",
                    Description = "Camiseta fitness feminina com identidade esportiva inspirada em Ferrari, com frente e costas.",
                    Price = 129.90m,
                    ImageUrl = "/images/products/mulher_ferrari_frente_academia_9 (1).png",
                    ImageUrlsJson = "[\"/images/products/mulher_ferrari_frente_academia_9 (1).png\",\"/images/products/mulher_ferrari_costas_academia_9 (1).png\"]",
                    Gender = Gender.Feminino,
                    CategoryId = camisetas.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Camiseta Beta Fit Feminina Mais Treinada",
                    Description = "Camiseta fitness feminina com modelagem esportiva e identidade Beta Fit.",
                    Price = 119.90m,
                    ImageUrl = "/images/products/mulher_frente_9x16.jpg - Crop 9 (1).png",
                    ImageUrlsJson = "[\"/images/products/mulher_frente_9x16.jpg - Crop 9 (1).png\",\"/images/products/mulher_costas_mais_treinada_v2_9 (1).png\"]",
                    Gender = Gender.Feminino,
                    CategoryId = camisetas.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Camiseta Beta Fit Feminina Treino",
                    Description = "Camiseta fitness feminina com estampa Beta Fit e visual voltado para treino.",
                    Price = 119.90m,
                    ImageUrl = "/images/products/mulher_costas_betafit_mais_trein (1).png",
                    ImageUrlsJson = "[\"/images/products/mulher_costas_betafit_mais_trein (1).png\"]",
                    Gender = Gender.Feminino,
                    CategoryId = camisetas.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Camiseta Fitness Feminina Separada",
                    Description = "Camiseta fitness feminina apresentada em composição individual para catálogo.",
                    Price = 119.90m,
                    ImageUrl = "/images/products/mulher_separada_9x16.jpg - Crop (1).png",
                    ImageUrlsJson = "[\"/images/products/mulher_separada_9x16.jpg - Crop (1).png\"]",
                    Gender = Gender.Feminino,
                    CategoryId = camisetas.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Camiseta Fitness Masculina Sem Braços Cruzados",
                    Description = "Camiseta fitness masculina apresentada em modelo para catálogo.",
                    Price = 119.90m,
                    ImageUrl = "/images/products/homem_sem_bracos_cruzados_9x16.jpg (1).png",
                    ImageUrlsJson = "[\"/images/products/homem_sem_bracos_cruzados_9x16.jpg (1).png\"]",
                    Gender = Gender.Masculino,
                    CategoryId = camisetas.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Camiseta Fitness Masculina Costas",
                    Description = "Camiseta fitness masculina apresentada pela parte traseira para catálogo.",
                    Price = 119.90m,
                    ImageUrl = "/images/products/homem_costas_sem_protecao_9x16.jpg (1).png",
                    ImageUrlsJson = "[\"/images/products/homem_costas_sem_protecao_9x16.jpg (1).png\"]",
                    Gender = Gender.Masculino,
                    CategoryId = camisetas.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Camiseta Fitness Modelo Direita",
                    Description = "Camiseta fitness apresentada em modelo, ideal para composição de catálogo.",
                    Price = 119.90m,
                    ImageUrl = "/images/products/foto_direita_9x16_corpo.jpg - Cr (1).png",
                    ImageUrlsJson = "[\"/images/products/foto_direita_9x16_corpo.jpg - Cr (1).png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = camisetas.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Camiseta Fitness Modelo Centro",
                    Description = "Camiseta fitness apresentada em modelo para catálogo.",
                    Price = 119.90m,
                    ImageUrl = "/images/products/foto_meio_9x16_corpo.png",
                    ImageUrlsJson = "[\"/images/products/foto_meio_9x16_corpo.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = camisetas.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Camiseta Fitness Dois Modelos",
                    Description = "Camiseta fitness apresentada em composição com dois modelos.",
                    Price = 119.90m,
                    ImageUrl = "/images/products/composicao_dois_modelos_costas_c (1).png",
                    ImageUrlsJson = "[\"/images/products/composicao_dois_modelos_costas_c (1).png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = camisetas.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Camiseta Fitness Premium Gemini 01",
                    Description = "Camiseta fitness apresentada em modelo, com visual premium para treino e uso casual.",
                    Price = 119.90m,
                    ImageUrl = "/images/products/Gemini_Generated_Image_6bmtea66bmtea66bmt (1).jpg",
                    ImageUrlsJson = "[\"/images/products/Gemini_Generated_Image_6bmtea66bmtea66bmt (1).jpg\"]",
                    Gender = Gender.Unissex,
                    CategoryId = camisetas.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Camiseta Fitness Premium Gemini 02",
                    Description = "Camiseta fitness apresentada em modelo, com visual premium para treino e uso casual.",
                    Price = 119.90m,
                    ImageUrl = "/images/products/Gemini_Generated_Image_p2umigp2umigp2um (1).jpg",
                    ImageUrlsJson = "[\"/images/products/Gemini_Generated_Image_p2umigp2umigp2um (1).jpg\"]",
                    Gender = Gender.Unissex,
                    CategoryId = camisetas.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },


                // ---------------- LEGGINGS / CALÇAS DE TREINO ----------------
                new Product
                {
                    Name = "Calça Jogger Essential Cinza",
                    Description = "Calça jogger masculina cinza mescla, com ajuste na cintura e punhos elásticos para pré e pós-treino.",
                    Price = 139.90m,
                    ImageUrl = "/images/products/betafit_calca_cinza_com_camiseta.png",
                    ImageUrlsJson = "[\"/images/products/betafit_calca_cinza_com_camiseta.png\"]",
                    Gender = Gender.Masculino,
                    CategoryId = leggings.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Legging Performance Grafite",
                    Description = "Legging feminina grafite com visual de alta performance e identidade Beta Fit.",
                    Price = 149.90m,
                    ImageUrl = "/images/products/betafit_calca_feminina_grafite.png",
                    ImageUrlsJson = "[\"/images/products/betafit_calca_feminina_grafite.png\"]",
                    Gender = Gender.Feminino,
                    CategoryId = leggings.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Legging Cargo Neon",
                    Description = "Legging feminina verde neon com visual marcante para treinos funcionais e academia.",
                    Price = 149.90m,
                    ImageUrl = "/images/products/betafit_calca_feminina_neon.png",
                    ImageUrlsJson = "[\"/images/products/betafit_calca_feminina_neon.png\"]",
                    Gender = Gender.Feminino,
                    CategoryId = leggings.Id,
                    IsFeatured = true,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Calça Fitness Feminina Modelo",
                    Description = "Calça fitness feminina apresentada em modelo para catálogo.",
                    Price = 149.90m,
                    ImageUrl = "/images/products/mulher_costas_mais_treinada_v2_9 (1).png",
                    ImageUrlsJson = "[\"/images/products/mulher_costas_mais_treinada_v2_9 (1).png\"]",
                    Gender = Gender.Feminino,
                    CategoryId = leggings.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },


                // ---------------- SHORTS ----------------
                new Product
                {
                    Name = "Short Feminino Performance",
                    Description = "Short feminino para academia, com modelagem esportiva e visual clean.",
                    Price = 99.90m,
                    ImageUrl = "/images/products/betafit_short_feminino_9x16.png.png",
                    ImageUrlsJson = "[\"/images/products/betafit_short_feminino_9x16.png.png\"]",
                    Gender = Gender.Feminino,
                    CategoryId = shorts.Id,
                    IsFeatured = true,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Short Feminino Performance Branco",
                    Description = "Short feminino branco com visual leve e esportivo para treino e corrida.",
                    Price = 99.90m,
                    ImageUrl = "/images/products/betafit_short_feminino_branco.png",
                    ImageUrlsJson = "[\"/images/products/betafit_short_feminino_branco.png\"]",
                    Gender = Gender.Feminino,
                    CategoryId = shorts.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Short Masculino Performance",
                    Description = "Short masculino esportivo de corte confortável para academia e corrida.",
                    Price = 99.90m,
                    ImageUrl = "/images/products/betafit_short_masculino_9x16.png",
                    ImageUrlsJson = "[\"/images/products/betafit_short_masculino_9x16.png\"]",
                    Gender = Gender.Masculino,
                    CategoryId = shorts.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Short Masculino Performance Branco",
                    Description = "Short masculino branco, leve e versátil para treino e atividades esportivas.",
                    Price = 99.90m,
                    ImageUrl = "/images/products/betafit_short_masculino_branco.png",
                    ImageUrlsJson = "[\"/images/products/betafit_short_masculino_branco.png\"]",
                    Gender = Gender.Masculino,
                    CategoryId = shorts.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },


                // ---------------- MOLETONS ----------------
                new Product
                {
                    Name = "Moletom Canguru Beta Fit",
                    Description = "Moletom masculino preto com capuz e visual esportivo, ideal para aquecimento e dias frios.",
                    Price = 199.90m,
                    ImageUrl = "/images/products/betafit_moletom_masculino.png",
                    ImageUrlsJson = "[\"/images/products/betafit_moletom_masculino.png\"]",
                    Gender = Gender.Masculino,
                    CategoryId = moletons.Id,
                    IsFeatured = true,
                    CreatedAt = DateTime.Now
                },


                // ---------------- ACESSÓRIOS ----------------
                new Product
                {
                    Name = "Boné Beta Fit Branco",
                    Description = "Boné branco com logo Beta Fit e aba curva, ideal para treino e uso casual.",
                    Price = 59.90m,
                    ImageUrl = "/images/products/betafit_bone_branco.png",
                    ImageUrlsJson = "[\"/images/products/betafit_bone_branco.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = acessorios.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Boné Beta Fit Grafite",
                    Description = "Boné grafite com logo Beta Fit, visual discreto e esportivo.",
                    Price = 59.90m,
                    ImageUrl = "/images/products/betafit_bone_grafite.png",
                    ImageUrlsJson = "[\"/images/products/betafit_bone_grafite.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = acessorios.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Boné Beta Fit Preto",
                    Description = "Boné preto com logo Beta Fit tom sobre tom, clássico da linha de acessórios.",
                    Price = 59.90m,
                    ImageUrl = "/images/products/betafit_bone_preto.png",
                    ImageUrlsJson = "[\"/images/products/betafit_bone_preto.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = acessorios.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Boné Beta Fit Verde Neon",
                    Description = "Boné verde neon com logo Beta Fit em preto, peça de destaque da coleção.",
                    Price = 64.90m,
                    ImageUrl = "/images/products/betafit_bone_verde_neon.png",
                    ImageUrlsJson = "[\"/images/products/betafit_bone_verde_neon.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = acessorios.Id,
                    IsFeatured = true,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Shaker Beta Fit Branco 700ml",
                    Description = "Coqueteleira branca Beta Fit para pré e pós-treino, com visual clean.",
                    Price = 39.90m,
                    ImageUrl = "/images/products/betafit_shaker_branco_urso_9x16.png",
                    ImageUrlsJson = "[\"/images/products/betafit_shaker_branco_urso_9x16.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = acessorios.Id,
                    IsFeatured = true,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Shaker Beta Fit Preto 700ml",
                    Description = "Coqueteleira preta Beta Fit com identidade visual da marca para rotina de treino.",
                    Price = 39.90m,
                    ImageUrl = "/images/products/betafit_shaker_preto_beta_9x16.png",
                    ImageUrlsJson = "[\"/images/products/betafit_shaker_preto_beta_9x16.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = acessorios.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Shaker Beta Fit 700ml",
                    Description = "Coqueteleira Beta Fit com acabamento esportivo para academia.",
                    Price = 39.90m,
                    ImageUrl = "/images/products/shaker_betafit_9x16.png",
                    ImageUrlsJson = "[\"/images/products/shaker_betafit_9x16.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = acessorios.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Galão Beta Fit Grafite",
                    Description = "Galão esportivo grafite com identidade Beta Fit e design robusto para hidratação durante o treino.",
                    Price = 69.90m,
                    ImageUrl = "/images/products/betafit_galao_grafite_urso_9x16.png",
                    ImageUrlsJson = "[\"/images/products/betafit_galao_grafite_urso_9x16.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = acessorios.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Galão Beta Fit Preto",
                    Description = "Galão preto Beta Fit de alta capacidade para acompanhar treinos e rotina esportiva.",
                    Price = 69.90m,
                    ImageUrl = "/images/products/betafit_galao_preto_beta_9x16.png",
                    ImageUrlsJson = "[\"/images/products/betafit_galao_preto_beta_9x16.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = acessorios.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Garrafa Beta Fit Centro",
                    Description = "Garrafa esportiva com identidade visual Beta Fit e acabamento escuro.",
                    Price = 49.90m,
                    ImageUrl = "/images/products/garrafa_centro_9x16.png",
                    ImageUrlsJson = "[\"/images/products/garrafa_centro_9x16.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = acessorios.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Garrafa Beta Fit Direita",
                    Description = "Garrafa esportiva da coleção Beta Fit com acabamento premium em preto.",
                    Price = 49.90m,
                    ImageUrl = "/images/products/garrafa_direita_9x16.png",
                    ImageUrlsJson = "[\"/images/products/garrafa_direita_9x16.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = acessorios.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Garrafa Beta Fit Esquerda",
                    Description = "Garrafa esportiva Beta Fit com design escuro e visual premium.",
                    Price = 49.90m,
                    ImageUrl = "/images/products/garrafa_esquerda_9x16.png",
                    ImageUrlsJson = "[\"/images/products/garrafa_esquerda_9x16.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = acessorios.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Garrafa Ferrari",
                    Description = "Garrafa temática Ferrari da coleção especial Beta Fit.",
                    Price = 79.90m,
                    ImageUrl = "/images/products/garrafa_ferrari_9x16.png",
                    ImageUrlsJson = "[\"/images/products/garrafa_ferrari_9x16.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = acessorios.Id,
                    IsFeatured = true,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Garrafa Luffy",
                    Description = "Garrafa temática inspirada em Luffy, com arte exclusiva.",
                    Price = 69.90m,
                    ImageUrl = "/images/products/garrafa_luffy_9x16.png",
                    ImageUrlsJson = "[\"/images/products/garrafa_luffy_9x16.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = acessorios.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Garrafa Mercedes",
                    Description = "Garrafa temática Mercedes com acabamento esportivo.",
                    Price = 79.90m,
                    ImageUrl = "/images/products/garrafa_mercedes_9x16.png",
                    ImageUrlsJson = "[\"/images/products/garrafa_mercedes_9x16.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = acessorios.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Garrafa Noelle",
                    Description = "Garrafa temática inspirada em Noelle, com arte exclusiva da coleção.",
                    Price = 69.90m,
                    ImageUrl = "/images/products/garrafa_noelle_9x16.png",
                    ImageUrlsJson = "[\"/images/products/garrafa_noelle_9x16.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = acessorios.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Garrafa Red Bull Racing",
                    Description = "Garrafa temática Red Bull Racing com visual de automobilismo.",
                    Price = 79.90m,
                    ImageUrl = "/images/products/garrafa_red_bull_racing_9x16.png",
                    ImageUrlsJson = "[\"/images/products/garrafa_red_bull_racing_9x16.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = acessorios.Id,
                    IsFeatured = true,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Garrafa Sanji",
                    Description = "Garrafa temática inspirada em Sanji, com arte exclusiva da coleção.",
                    Price = 69.90m,
                    ImageUrl = "/images/products/garrafa_sanji_9x16.png",
                    ImageUrlsJson = "[\"/images/products/garrafa_sanji_9x16.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = acessorios.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Garrafa Sem Texto com Alça",
                    Description = "Garrafa esportiva com alça e acabamento preto, sem texto externo.",
                    Price = 59.90m,
                    ImageUrl = "/images/products/garrafa_sem_texto_alca_02.png",
                    ImageUrlsJson = "[\"/images/products/garrafa_sem_texto_alca_02.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = acessorios.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Garrafa Yuno",
                    Description = "Garrafa temática inspirada em Yuno, com arte exclusiva da coleção.",
                    Price = 69.90m,
                    ImageUrl = "/images/products/garrafa_yuno_9x16.png",
                    ImageUrlsJson = "[\"/images/products/garrafa_yuno_9x16.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = acessorios.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                },

                new Product
                {
                    Name = "Garrafa Zoro",
                    Description = "Garrafa temática inspirada em Zoro, com arte exclusiva da coleção.",
                    Price = 69.90m,
                    ImageUrl = "/images/products/garrafa_zoro_9x16.png",
                    ImageUrlsJson = "[\"/images/products/garrafa_zoro_9x16.png\"]",
                    Gender = Gender.Unissex,
                    CategoryId = acessorios.Id,
                    IsFeatured = false,
                    CreatedAt = DateTime.Now
                }
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

            // =====================================================================
            // 2.1. TAMANHOS E CORES
            // =====================================================================

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

            // Define cores básicas quando o produto ainda não possui cores.
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

            // Cores e galerias específicas dos bonés.
            var capProducts = await context.Products
                .Where(p => p.Name.StartsWith("Boné Beta Fit"))
                .ToListAsync();

            var capGallery = new Dictionary<string, List<string>>
            {
                ["Branco"] = new() { "/images/products/betafit_bone_branco.png" },
                ["Cinza"] = new() { "/images/products/betafit_bone_grafite.png" },
                ["Preto"] = new() { "/images/products/betafit_bone_preto.png" },
                ["Verde neon"] = new() { "/images/products/betafit_bone_verde_neon.png" }
            };

            foreach (var cap in capProducts)
            {
                cap.AvailableColorsJson = JsonSerializer.Serialize(capGallery.Keys);
                cap.ColorGalleriesJson = JsonSerializer.Serialize(capGallery);
                cap.ColorImageUrlsJson = JsonSerializer.Serialize(
                    capGallery.ToDictionary(x => x.Key, x => x.Value[0]));
            }

            await context.SaveChangesAsync();

            // =====================================================================
            // 2.2. GALERIA PERSISTENTE
            // =====================================================================
            // Garante que produtos sem registros em ProductImage tenham sua
            // imagem principal cadastrada na galeria.
            // =====================================================================

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

            // =====================================================================
            // 3. SEED DE ROLES
            // =====================================================================

            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            if (!await roleManager.RoleExistsAsync("Usuario"))
            {
                await roleManager.CreateAsync(new IdentityRole("Usuario"));
            }

            if (!await roleManager.RoleExistsAsync("Funcionario"))
            {
                await roleManager.CreateAsync(new IdentityRole("Funcionario"));
            }

            // =====================================================================
            // 4. SEED DO USUÁRIO ADMINISTRADOR
            // =====================================================================

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

                    await userManager.AddClaimsAsync(
                        adminUser,
                        new[]
                        {
                            new Claim("FullName", "Administrador BetaFit"),
                            new Claim("BirthDate", "1990-01-01")
                        });
                }
            }

            if (adminUser is not null)
            {
                var adminClaims = await userManager.GetClaimsAsync(adminUser);

                if (!adminClaims.Any(c => c.Type == "FullName"))
                {
                    await userManager.AddClaimAsync(
                        adminUser,
                        new Claim("FullName", "Administrador BetaFit"));
                }

                if (!adminClaims.Any(c => c.Type == "BirthDate"))
                {
                    await userManager.AddClaimAsync(
                        adminUser,
                        new Claim("BirthDate", "1990-01-01"));
                }
            }
        }
    }
}
