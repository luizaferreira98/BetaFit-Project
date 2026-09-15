// =============================================================================
// BetaFit.Desktop - Helpers/ImagemProdutoHelper.cs
// =============================================================================
// Carrega thumbnails de produto para exibir dentro de um DataGridView.
//
// Esse código nasceu dentro de ProdutosUserControl (coluna colFoto). Como o
// Dashboard passou a mostrar a foto dos itens mais pedidos, ele foi extraído
// para cá em vez de ser duplicado — assim o cache de imagens é COMPARTILHADO
// entre as telas: uma foto baixada na lista de produtos já vem pronta quando
// o Dashboard precisa dela.
//
// Detalhe importante: as imagens ficam hospedadas no BetaFit.UI (conteúdo
// estático em wwwroot), não na API. Por isso a URL relativa que vem do backend
// ("/images/products/x.png") é resolvida contra AppConfig.UiBaseUrl.
// =============================================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;

namespace BetaFit.Desktop.Helpers
{
    public static class ImagemProdutoHelper
    {
        // Timeout curto: se a UI estiver fora do ar, o grid não pode travar
        // esperando — ele simplesmente mostra o placeholder.
        private static readonly HttpClient _http = new() { Timeout = TimeSpan.FromSeconds(5) };

        // Cache por URL absoluta. Estático de propósito: sobrevive à troca de
        // telas (o UserControl é recriado a cada navegação no MainForm).
        private static readonly Dictionary<string, Image> _cache = new();

        private static readonly Image _placeholder = CriarPlaceholder();

        /// <summary>
        /// Imagem "sem foto" usada enquanto o download não terminou e também
        /// quando ele falha. Use como <c>DefaultCellStyle.NullValue</c> da coluna.
        /// </summary>
        public static Image Placeholder => _placeholder;

        /// <summary>
        /// Baixa (ou devolve do cache) a imagem de um produto.
        /// Nunca lança: em qualquer falha devolve o <see cref="Placeholder"/>.
        /// </summary>
        /// <param name="imageUrl">
        /// URL absoluta ou relativa ao BetaFit.UI (ex.: "/images/products/x.png").
        /// </param>
        public static async Task<Image> ObterAsync(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return _placeholder;

            var urlAbsoluta = ResolverUrlAbsoluta(imageUrl);
            if (urlAbsoluta == null)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[Foto] UiBaseUrl vazio (AppConfig.UiBaseUrl) — BetaFit.UI parece não estar rodando. " +
                    $"ImageUrl recebida: '{imageUrl}'");
                return _placeholder;
            }

            if (_cache.TryGetValue(urlAbsoluta, out var cacheada))
                return cacheada;

            try
            {
                var bytes = await _http.GetByteArrayAsync(urlAbsoluta);
                using var ms = new MemoryStream(bytes);
                var imagem = Image.FromStream(ms);
                _cache[urlAbsoluta] = imagem;
                return imagem;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"[Foto] Falha ao baixar '{urlAbsoluta}': {ex.Message}");
                return _placeholder;
            }
        }

        /// <summary>
        /// Converte uma URL relativa do backend em absoluta usando a base do UI.
        /// Devolve null quando a base não pôde ser resolvida.
        /// </summary>
        public static string? ResolverUrlAbsoluta(string imageUrl)
        {
            if (Uri.TryCreate(imageUrl, UriKind.Absolute, out _))
                return imageUrl;

            var uiBaseUrl = AppConfig.UiBaseUrl;
            if (string.IsNullOrWhiteSpace(uiBaseUrl))
                return null;

            return $"{uiBaseUrl.TrimEnd('/')}/{imageUrl.TrimStart('/')}";
        }

        /// <summary>
        /// Limpa o cache de imagens. Útil se as fotos forem trocadas na web
        /// enquanto o Desktop está aberto.
        /// </summary>
        public static void LimparCache() => _cache.Clear();

        // Ícone "sem foto" desenhado via GDI+ (sol + montanha, estilo clássico de
        // imagem quebrada). Não depende de fonte de emoji, então sempre renderiza,
        // mesmo se a BetaFit.UI estiver offline e a foto real nunca chegar a baixar.
        private static Image CriarPlaceholder()
        {
            var bmp = new Bitmap(48, 48);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.FromArgb(30, 30, 30)); // combina com Admin.FundoLinhaPar/Impar

            using var caneta = new Pen(Color.FromArgb(90, 90, 90), 1.5f);
            g.DrawRectangle(caneta, 6, 6, 35, 35);

            using (var pincelSol = new SolidBrush(Color.FromArgb(90, 90, 90)))
                g.FillEllipse(pincelSol, 13, 13, 7, 7);

            using var montanha = new GraphicsPath();
            montanha.AddPolygon(new[]
            {
                new PointF(9, 37),
                new PointF(19, 21),
                new PointF(26, 29),
                new PointF(33, 17),
                new PointF(42, 37),
            });
            using (var pincelMontanha = new SolidBrush(Color.FromArgb(70, 70, 70)))
                g.FillPath(pincelMontanha, montanha);

            return bmp;
        }
    }
}