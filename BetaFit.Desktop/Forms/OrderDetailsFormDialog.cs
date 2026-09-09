// =============================================================================
// BetaFit.Desktop - Forms/OrderDetailsDialog.cs
// =============================================================================
// Popup de detalhes do pedido (substitui o antigo MessageBox.Show usado no
// duplo clique do gridPedidos em PedidosUserControl).
//
// Segue a identidade visual BetaFit (BetaFitTheme): preto + lima, bordas
// retas, tipografia forte em caixa alta. Construído 100% em código (sem
// Designer.cs) para ficar em um único arquivo, fácil de plugar no projeto.
//
// Uso (em PedidosUserControl.cs, no lugar do MessageBox):
//   using var dialog = new OrderDetailsDialog(pedido);
//   dialog.ShowDialog(this.FindForm());
// =============================================================================

using BetaFit.Desktop.DTOs;
using BetaFit.Desktop.Helpers;
using BetaFit.Desktop.Themes;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetaFit.Desktop.Forms
{
    public partial class OrderDetailsFormDialog : Form
    {
        private readonly OrderResponseDto _pedido;

        //=================================================
        // FOTO DO ITEM (mesmo padrão de cache/placeholder usado em
        // PedidosUserControl — duplicado aqui de propósito, porque este
        // arquivo foi feito pra ser autocontido/fácil de plugar).
        //=================================================
        private static readonly HttpClient _httpImagens = new() { Timeout = TimeSpan.FromSeconds(5) };
        private static readonly Dictionary<string, Image> _cacheImagens = new();
        private static readonly Image _imagemPlaceholder = CriarPlaceholderFoto();

        // Construtor "de design" — não deve ser usado em produção, mas
        // evita quebrar caso algum designer tente instanciar sem parâmetros.
        public OrderDetailsFormDialog() : this(new OrderResponseDto()) { }

        public OrderDetailsFormDialog(OrderResponseDto pedido)
        {
            _pedido = pedido;
            InitializeComponent();
            MontarConteudo();
        }

        //=================================================
        // MONTA TODO O CONTEÚDO DO POPUP A PARTIR DO PEDIDO
        //=================================================
        private void MontarConteudo()
        {
            lblTitulo.Text = $"PEDIDO #{_pedido.Id}";
            lblCliente.Text = $"Cliente: {_pedido.UserName}";
            lblData.Text = _pedido.CreatedAt.ToString("dd/MM/yyyy 'às' HH:mm");

            // Badge de status reaproveitando as mesmas cores do grid de Pedidos
            var (fundo, texto) = BetaFitTheme.CorStatusPedido(_pedido.Status);
            lblStatusBadge.Text = FormatarStatus(_pedido.Status);
            lblStatusBadge.ForeColor = texto;
            pnlStatusBadge.BackColor = fundo;
            pnlStatusBadge.Width = lblStatusBadge.PreferredSize.Width + 16;

            pnlItens.Controls.Clear();

            if (_pedido.Items == null || _pedido.Items.Count == 0)
            {
                var lblVazio = new Guna2HtmlLabel
                {
                    Text = "Este pedido não possui itens.",
                    ForeColor = BetaFitTheme.TextoMuted,
                    Font = BetaFitTheme.FonteNormal,
                    AutoSize = false,
                    Width = pnlItens.Width - 24,
                    Height = 40,
                    Margin = new Padding(4, 12, 4, 4),
                };
                pnlItens.Controls.Add(lblVazio);
            }
            else
            {
                foreach (var item in _pedido.Items)
                {
                    pnlItens.Controls.Add(CriarLinhaItem(item));
                }
            }

            lblQndItens.Text = $"{_pedido.Items?.Sum(i => i.Quantity) ?? 0} ITEM(NS)";
            lblTotalValor.Text = _pedido.Total.ToString("C");
        }

        //=================================================
        // CRIA UMA "LINHA CARD" PARA UM ITEM DO PEDIDO
        //=================================================
        private Guna2Panel CriarLinhaItem(OrderItemResponseDto item)
        {
            var linha = new Guna2Panel
            {
                Width = pnlItens.Width - 24,
                Height = 52,
                BackColor = BetaFitTheme.Superficie,
                BorderRadius = 2,
                BorderThickness = 1,
                BorderColor = BetaFitTheme.Linha,
                Margin = new Padding(0, 0, 0, 8),
            };

            var pctFoto = new PictureBox
            {
                Size = new Size(40, 40),
                Location = new Point(8, 6),
                SizeMode = PictureBoxSizeMode.Zoom,
                Image = _imagemPlaceholder,
                BackColor = Color.Transparent,
            };
            _ = CarregarFotoItemAsync(pctFoto, item.ImageUrl);

            var lblQtd = new Guna2HtmlLabel
            {
                Text = $"{item.Quantity}x",
                Font = BetaFitTheme.FonteSubtitulo,
                ForeColor = BetaFitTheme.LimaEscuro,
                Location = new Point(56, 8),
                AutoSize = true,
            };

            var lblNome = new Guna2HtmlLabel
            {
                Text = item.ProductName,
                Font = BetaFitTheme.FonteNormal,
                ForeColor = BetaFitTheme.Tinta,
                Location = new Point(100, 4),
                AutoSize = false,
                Size = new Size(linha.Width - 244, 18),
            };

            var lblUnit = new Guna2HtmlLabel
            {
                Text = $"{item.UnitPrice:C} / un.",
                Font = BetaFitTheme.FontePequena,
                ForeColor = BetaFitTheme.TextoMuted,
                Location = new Point(100, 24),
                AutoSize = true,
            };

            var lblSubtotal = new Guna2HtmlLabel
            {
                Text = item.Subtotal.ToString("C"),
                Font = BetaFitTheme.FonteSubtitulo,
                ForeColor = BetaFitTheme.Tinta,
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
            };
            lblSubtotal.Location = new Point(linha.Width - lblSubtotal.PreferredSize.Width - 12, 16);

            linha.Controls.Add(pctFoto);
            linha.Controls.Add(lblQtd);
            linha.Controls.Add(lblNome);
            linha.Controls.Add(lblUnit);
            linha.Controls.Add(lblSubtotal);

            return linha;
        }

        //=================================================
        // BAIXA (OU PEGA DO CACHE) A FOTO DE UM ITEM E APLICA NO PICTUREBOX
        //=================================================
        private static async Task CarregarFotoItemAsync(PictureBox pct, string? imageUrl)
        {
            var imagem = await ObterImagemItemAsync(imageUrl);
            if (pct.IsDisposed) return;
            pct.Image = imagem;
        }

        private static async Task<Image> ObterImagemItemAsync(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return _imagemPlaceholder;

            var urlAbsoluta = ResolverUrlAbsolutaDaImagem(imageUrl);
            if (urlAbsoluta == null) return _imagemPlaceholder;

            if (_cacheImagens.TryGetValue(urlAbsoluta, out var cacheada))
                return cacheada;

            try
            {
                var bytes = await _httpImagens.GetByteArrayAsync(urlAbsoluta);
                using var ms = new System.IO.MemoryStream(bytes);
                var imagem = Image.FromStream(ms);
                _cacheImagens[urlAbsoluta] = imagem;
                return imagem;
            }
            catch
            {
                return _imagemPlaceholder;
            }
        }

        private static string? ResolverUrlAbsolutaDaImagem(string imageUrl)
        {
            if (Uri.TryCreate(imageUrl, UriKind.Absolute, out _))
                return imageUrl;

            var uiBaseUrl = AppConfig.UiBaseUrl;
            if (string.IsNullOrWhiteSpace(uiBaseUrl))
                return null;

            return $"{uiBaseUrl.TrimEnd('/')}/{imageUrl.TrimStart('/')}";
        }

        private static Image CriarPlaceholderFoto()
        {
            var bmp = new Bitmap(40, 40);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.FromArgb(235, 235, 231));
            using var caneta = new Pen(Color.FromArgb(190, 190, 186), 1.3f);
            g.DrawRectangle(caneta, 5, 5, 30, 30);
            return bmp;
        }

        //=================================================
        // "EmPreparacao" -> "EM PREPARAÇÃO" (mais legível no badge)
        //=================================================
        private static string FormatarStatus(string status) => status switch
        {
            "Pendente" => "PENDENTE",
            "EmPreparacao" => "EM PREPARAÇÃO",
            "Pronto" => "PRONTO",
            "Entregue" => "ENTREGUE",
            "Cancelado" => "CANCELADO",
            _ => status.ToUpperInvariant()
        };

        //=================================================
        // FECHAR
        //=================================================
        private void btnFechar_Click(object? sender, EventArgs e) => Close();


    }
}