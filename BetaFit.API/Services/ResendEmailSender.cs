using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace BetaFit.API.Services;

public interface IEmailSender
{
    Task SendAsync(string to, string subject, string html, CancellationToken cancellationToken = default);
}

public sealed class ResendEmailSender : IEmailSender
{
    private readonly HttpClient _http;
    private readonly IConfiguration _configuration;
    private readonly ILogger<ResendEmailSender> _logger;
    private readonly IWebHostEnvironment _environment;

    public ResendEmailSender(HttpClient http, IConfiguration configuration, ILogger<ResendEmailSender> logger, IWebHostEnvironment environment)
    { _http = http; _configuration = configuration; _logger = logger; _environment = environment; }

    public async Task SendAsync(string to, string subject, string html, CancellationToken cancellationToken = default)
    {
        var key = _configuration["Resend:ApiKey"];
        if (string.IsNullOrWhiteSpace(key)) key = Environment.GetEnvironmentVariable("RESEND_API_KEY");
        var from = _configuration["Resend:From"] ?? Environment.GetEnvironmentVariable("RESEND_FROM") ?? "Beta Fit <onboarding@resend.dev>";
        if (_environment.IsDevelopment() && _configuration["Email:Mode"] == "Outbox")
        {
            if (_environment.IsDevelopment())
            {
                _logger.LogWarning("RESEND DEVELOPMENT OUTBOX | To={Email} | Subject={Subject} | Html={Html}", to, subject, html);
                return;
            }
            throw new InvalidOperationException("O envio de e-mail está temporariamente indisponível.");
        }
        if (string.IsNullOrWhiteSpace(key)) throw new InvalidOperationException("Envio de e-mail não configurado. A alteração não foi realizada. Configure a chave Resend e um remetente autorizado na API.");
        using var request = new HttpRequestMessage(HttpMethod.Post, "emails");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", key);
        request.Content = JsonContent.Create(new { from, to = new[] { to }, subject, html });
        var response = await _http.SendAsync(request, cancellationToken);
        if (!response.IsSuccessStatusCode)
        {
            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            _logger.LogError("Resend retornou {Status}: {Body}", response.StatusCode, body);
            throw new InvalidOperationException("O envio de e-mail está temporariamente indisponível.");
        }
    }
}
