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
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

var builder = WebApplication.CreateBuilder(args);

// =====================================================================
// LOCALIZAÇÃO (deve ser configurada ANTES de adicionar serviços de rota)
// =====================================================================
builder.Services.Configure<RequestLocalizationOptions>(options =>
{
    var supportedCultures = new[] { "pt-BR" };
    options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("pt-BR");
    options.SupportedCultures = supportedCultures.Select(c => new System.Globalization.CultureInfo(c)).ToList();
    options.SupportedUICultures = supportedCultures.Select(c => new System.Globalization.CultureInfo(c)).ToList();
});

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
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
        options.Cookie.HttpOnly = true;
        options.Cookie.SameSite = SameSiteMode.Lax;
        options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
            ? CookieSecurePolicy.SameAsRequest
            : CookieSecurePolicy.Always;
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

// =====================================================================
// Infraestrutura de sessão MVC (não utilizada para persistência de carrinho/favoritos)
// =====================================================================
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromHours(4);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.Cookie.SameSite = SameSiteMode.Lax;
    options.Cookie.SecurePolicy = builder.Environment.IsDevelopment()
        ? CookieSecurePolicy.SameAsRequest
        : CookieSecurePolicy.Always;
});

// =====================================================================
// HTTP CLIENTS & SERVIÇOS DA API
// =====================================================================

// Inicializar AppConfig ANTES de registrar serviços HTTP
// Isso resolve a URL da API uma única vez na inicialização
AppConfig.Initialize(builder.Configuration, builder.Environment);

// Handler que repassa o cookie de autenticação da API nas requisições
builder.Services.AddTransient<ApiCookieHandler>();

// Cliente usado apenas para Login/Register (ainda não existe cookie a repassar)
builder.Services.AddHttpClient("ApiClientAuth", client =>
{
    client.BaseAddress = new Uri(AppConfig.ApiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { UseCookies = false });

// Cliente padrão para os demais serviços (repassa o cookie de autenticação)
builder.Services.AddHttpClient("ApiClient", client =>
{
    client.BaseAddress = new Uri(AppConfig.ApiBaseUrl);
    client.Timeout = TimeSpan.FromSeconds(30);
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler { UseCookies = false })
.AddHttpMessageHandler<ApiCookieHandler>();

// A UI implementa os MESMOS contratos (interfaces) definidos na Application,
// só que "do outro lado da rede", consumindo a BetaFit.API via HTTP.
builder.Services.AddScoped<IProductService>(sp =>
    new HttpProductService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient")));

builder.Services.AddScoped<ICategoryService>(sp =>
    new HttpCategoryService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient")));

builder.Services.AddScoped<IDashboardService>(sp =>
    new HttpDashboardService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient")));

builder.Services.AddScoped<IOrderService>(sp =>
    new HttpOrderService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient")));

builder.Services.AddScoped<HttpReviewService>(sp =>
    new HttpReviewService(
        sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient"),
        sp.GetRequiredService<ILogger<HttpReviewService>>()
    ));

builder.Services.AddScoped<HttpCartService>(sp =>
    new HttpCartService(
        sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient"),
        sp.GetRequiredService<IHttpContextAccessor>(),
        sp.GetRequiredService<IProductService>()
    ));

builder.Services.AddScoped<HttpFavoriteService>(sp =>
    new HttpFavoriteService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient")));

builder.Services.AddScoped<HttpPaymentService>(sp =>
    new HttpPaymentService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient")));

builder.Services.AddScoped<HttpProfileService>(sp =>
    new HttpProfileService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient")));

builder.Services.AddScoped<HttpSiteSettingsService>(sp =>
    new HttpSiteSettingsService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient")));

builder.Services.AddScoped<HttpNotificationService>(sp =>
    new HttpNotificationService(sp.GetRequiredService<IHttpClientFactory>().CreateClient("ApiClient")));

// =====================================================================
// MVC
// =====================================================================
builder.Services.AddControllersWithViews();

var app = builder.Build();

// =====================================================================
// MIDDLEWARE PIPELINE (em ordem correta)
// =====================================================================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// ⭐ Localização deve estar CEDO na pipeline
var requestLocalizationOptions = app.Services.GetRequiredService<IOptions<RequestLocalizationOptions>>().Value;
app.UseRequestLocalization(requestLocalizationOptions);

app.UseRouting();

// ⭐ Sessão DEPOIS de Routing
app.UseSession();

// ⭐ Autenticação DEPOIS de Sessão
app.UseAuthentication();

// ⭐ Autorização DEPOIS de Autenticação
app.UseAuthorization();

// ⭐ Roteamento de endpoints por ÚLTIMO
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

// Não há SeedData aqui: o banco de dados pertence exclusivamente à BetaFit.API.

app.Run();