// =============================================================================
// BetaFit.Infraestructure - DbContext
// =============================================================================
//  CONCEITO IMPORTANTE: DbContext (Entity Framework Core)
// O DbContext é a classe PRINCIPAL do Entity Framework Core.
// Ele representa uma "sessão" com o banco de dados e permite:
// - Consultar dados (SELECT)
// - Inserir dados (INSERT)
// - Atualizar dados (UPDATE)
// - Deletar dados (DELETE)
//
// Ele herda de IdentityDbContext porque também gerencia as tabelas
// do ASP.NET Core Identity (usuários, roles, claims, etc.), exatamente
// como o SenacGamesDbContext. Não há JWT nem entidade de usuário própria:
// a autenticação inteira fica por conta do Identity.
// =============================================================================

using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BetaFit.Domain.Entities;
using BetaFit.Infraestructure.Configurations;

namespace BetaFit.Infraestructure.Context
{
    /// <summary>
    /// Contexto do banco de dados do BetaFit.
    /// Herda de IdentityDbContext para incluir as tabelas do Identity
    /// (AspNetUsers, AspNetRoles, etc.).
    /// </summary>
    public class BetaFitDbContext : IdentityDbContext
    {
        // =====================================================================
        //  CONCEITO: Construtor com DbContextOptions
        // O ASP.NET Core injeta as opções de configuração (connection string,
        // provider, etc.) automaticamente via Dependency Injection.
        // =====================================================================
        public BetaFitDbContext(DbContextOptions<BetaFitDbContext> options)
            : base(options)
        {
        }

        // =====================================================================
        // DbSets — Representam as tabelas no banco de dados
        // =====================================================================
        //  CONCEITO: DbSet<T>
        // Cada DbSet<T> representa uma tabela no banco de dados.
        // O Entity Framework cria automaticamente as tabelas correspondentes.
        // =====================================================================

        /// <summary>
        /// Tabela de Produtos (roupas, calçados, acessórios) no banco de dados.
        /// </summary>
        public DbSet<Product> Products { get; set; }

        /// <summary>
        /// Tabela de Categorias no banco de dados.
        /// </summary>
        public DbSet<Category> Categories { get; set; }

        /// <summary>
        /// Tabela de Pedidos no banco de dados.
        /// </summary>
        public DbSet<Order> Orders { get; set; }

        /// <summary>
        /// Tabela de Itens de Pedido no banco de dados.
        /// </summary>
        public DbSet<OrderItem> OrderItems { get; set; }

        // =====================================================================
        //  CONCEITO: OnModelCreating (Fluent API)
        // Este método permite configurar o modelo do banco de dados usando
        // a Fluent API do Entity Framework Core.
        // Aqui aplicamos as configurações definidas em classes separadas.
        // =====================================================================
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // IMPORTANTE: Sempre chamar base.OnModelCreating() quando herdar
            // de IdentityDbContext, para que as tabelas do Identity sejam criadas.
            base.OnModelCreating(modelBuilder);

            // Aplica as configurações de cada entidade (definidas em classes separadas)
            modelBuilder.ApplyConfiguration(new ProductConfiguration());
            modelBuilder.ApplyConfiguration(new CategoryConfiguration());
            modelBuilder.ApplyConfiguration(new OrderConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemConfiguration());
        }
    }
}