// =============================================================================
// BetaFit.UI - Program.cs
// =============================================================================
//  CONCEITO IMPORTANTE: BetaFit.UI é um projeto ASP.NET Core MVC
// (Controllers + Views), assim como o SenacGames.UI. Ela NUNCA acessa o
// banco de dados diretamente: consome exclusivamente a BetaFit.API,
// exatamente como o front-end original (BetaFit-master) fazia.
// =============================================================================

using BetaFit.Application.Interfaces;
using BetaFit.UI.Helpers;
using BetaFit.UI.Services;
using Microsoft.AspNetCore.Authentication.Cookies;

var builder = WebApplication.CreateBuilder(args);

// =====================================================================
// AUTENTICAÇÃO MVC (Cookie local da UI)
// =====================================================================
//  CONCEITO: A BetaFit.API autentica via Identity + Cookie (sem JWT).
// A UI mantém o SEU PRÓPRIO cookie de sessão (Claims do usuário logado)
// e guarda, dentro dele, o cookie devolvido pela API para repassar nas
// próximas chamadas (ver Helpers/ApiCookieHandler.cs).
// =====================================================================
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

// =====================================================================
// CARRINHO (Session) — demonstrativo, sem checkout real
// =====================================================================
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(4);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// =====================================================================
// HTTP CLIENTS & SERVIÇOS DA API
// =====================================================================
// Handler que repassa o cookie de autenticação da API nas requisições
builder.Services.AddTransient<ApiCookieHandler>();

// Resolve a URL da BetaFit.API dinamicamente (launchSettings ou appsettings)
var apiBaseUrl = AppConfig.ApiBaseUrl;

// Cliente usado apenas para Login/Register (ainda não existe cookie a repassar)
builder.Services.AddHttpClient("ApiClientAuth", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
});

// Cliente padrão para os demais serviços (repassa o cookie de autenticação)
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
})
.AddHttpMessageHandler<ApiCookieHandler>();

// A UI implementa os MESMOS contratos (interfaces) definidos na Application,
// só que "do outro lado da rede", consumindo a BetaFit.API via HTTP.
builder.Services.AddScoped<IProductService>(sp =>
    new HttpProductService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient")));

builder.Services.AddScoped<ICategoryService>(sp =>
    new HttpCategoryService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient")));

builder.Services.AddScoped<IDashboardService>(sp =>
    new HttpDashboardService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient")));

// =====================================================================
// MVC
// =====================================================================
builder.Services.AddControllersWithViews();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseSession();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Não há SeedData aqui: o banco de dados pertence exclusivamente à BetaFit.API.

app.Run();
