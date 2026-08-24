// =============================================================================
// BetaFit.API - Program.cs
// =============================================================================
//  CONCEITO IMPORTANTE: Program.cs
// Este é o PONTO DE ENTRADA da aplicação API.
// Aqui configuramos todos os serviços (DI), middlewares e a pipeline HTTP.
//
// O que é configurado aqui:
// 1. Entity Framework Core (conexão com banco de dados)
// 2. ASP.NET Core Identity (autenticação via Cookie — SEM JWT)
// 3. Dependency Injection (repositórios e serviços)
// 4. Swagger (documentação da API)
// 5. CORS (permissões de acesso cross-origin)
// =============================================================================

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using BetaFit.Application.Interfaces;
using BetaFit.Application.Services;
using BetaFit.Domain.Interfaces;
using BetaFit.Infraestructure.Context;
using BetaFit.Infraestructure.Identity;
using BetaFit.Infraestructure.Repositories;

var builder = WebApplication.CreateBuilder(args);

// =====================================================================
// 1. ENTITY FRAMEWORK CORE — Configuração do banco de dados
// =====================================================================
//  CONCEITO: AddDbContext registra o DbContext no container de DI.
// UseSqlServer configura o Entity Framework para usar o SQL Server.
// A connection string é lida do arquivo appsettings.json.
// =====================================================================
builder.Services.AddDbContext<BetaFitDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// =====================================================================
// 2. ASP.NET CORE IDENTITY — Autenticação e Autorização
// =====================================================================
//  CONCEITO: Identity é o sistema de autenticação do ASP.NET Core.
// Ele gerencia: usuários, senhas, roles, claims, login, logout, etc.
// AddIdentity registra os serviços do Identity no container de DI.
// AddEntityFrameworkStores conecta o Identity ao banco via EF Core.
// NÃO há JWT aqui — a autenticação é feita via Cookie.
// =====================================================================
builder.Services.AddIdentity<IdentityUser, IdentityRole>(options =>
{
    // Configurações de senha (simplificadas para ensino)
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 6;
})
.AddEntityFrameworkStores<BetaFitDbContext>()
.AddDefaultTokenProviders();

// Configuração de Cookie Authentication para a API
builder.Services.ConfigureApplicationCookie(options =>
{
    options.Events.OnRedirectToLogin = context =>
    {
        context.Response.StatusCode = 401;
        return Task.CompletedTask;
    };
    options.Events.OnRedirectToAccessDenied = context =>
    {
        context.Response.StatusCode = 403;
        return Task.CompletedTask;
    };
});

// =====================================================================
// 3. DEPENDENCY INJECTION — Registro de Repositórios e Serviços
// =====================================================================
//  CONCEITO: Dependency Injection (DI)
// AddScoped registra um serviço com ciclo de vida "por requisição".
// Isso significa que uma nova instância é criada para cada requisição HTTP.
// =====================================================================
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();
builder.Services.AddScoped<IUsuariosService, UsuariosService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();

// =====================================================================
// 4. CONTROLLERS
// =====================================================================
builder.Services.AddControllers();

// =====================================================================
// 5. SWAGGER — Documentação automática da API
// =====================================================================
//  CONCEITO: Swagger gera automaticamente uma interface visual
// para testar os endpoints da API no navegador.
// Acesse: https://localhost:PORTA/swagger
// =====================================================================
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "BetaFit API",
        Version = "v1",
        Description = "API REST do sistema BetaFit — Catálogo institucional de roupas"
    });
});

// =====================================================================
// 6. CORS — Permite requisições de outras origens (UI e Desktop)
// =====================================================================
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// =====================================================================
// PIPELINE DE MIDDLEWARES
// =====================================================================
//  CONCEITO: Middlewares são executados em sequência para cada requisição.
// A ordem importa! Cada middleware processa a requisição e passa adiante.
// =====================================================================

if (app.Environment.IsDevelopment())
{
    // Swagger só é habilitado em ambiente de desenvolvimento
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

//  IMPORTANTE: UseAuthentication ANTES de UseAuthorization
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// =====================================================================
// SEED DATA — Popula o banco com dados iniciais
// =====================================================================
//  CONCEITO: O seed é executado na inicialização da aplicação.
// Ele cria categorias, produtos de exemplo e o usuário admin.
// =====================================================================
await SeedData.SeedAsync(app.Services);

app.Run();