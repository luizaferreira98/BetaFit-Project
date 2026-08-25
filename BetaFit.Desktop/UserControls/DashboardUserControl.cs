using BetaFit.Desktop.DTOs;
using BetaFit.Desktop.Helpers;
using BetaFit.Desktop.Services;
using BetaFit.Desktop.Themes;
using System.Drawing.Drawing2D;

namespace BetaFit.Desktop.UserControls
{
    /// <summary>
    /// Dashboard administrativo do Desktop.
    /// Somente visualização: métricas e produtos recentes.
    /// Novo, editar e excluir ficam em UserControls próprias.
    /// </summary>
    public partial class DashboardUserControl : UserControl
    {
        private readonly DashboardApiService _dashboardService = new();
        private bool _carregando;

        public DashboardUserControl()
        {
            InitializeComponent();
            BackColor = BetaFitTheme.Superficie;
            DoubleBuffered = true;
            pnlHero.Resize += PnlHero_Resize;
            PnlHero_Resize(pnlHero, EventArgs.Empty);
        }


        private void PnlHero_Resize(object? sender, EventArgs e)
        {
            if (pnlHero == null || pnlHeroMarca == null) return;
            pnlHeroMarca.Left = pnlHero.ClientSize.Width - pnlHeroMarca.Width - 30;
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (DesignMode) return;
            await CarregarDashboardAsync();
        }

        public async Task CarregarDashboardAsync()
        {
            if (_carregando || IsDisposed) return;
            _carregando = true;
            MostrarEstado("CARREGANDO DASHBOARD...", false);

            try
            {
                var dashboard = await _dashboardService.GetSummaryAsync();
                if (dashboard is null)
                {
                    MostrarEstado("NÃO FOI POSSÍVEL CARREGAR OS DADOS.", true);
                    return;
                }

                AtualizarMetricas(dashboard);
                PreencherProdutos(dashboard.RecentProducts);
                pnlEstado.Visible = false;
                pnlConteudo.Visible = true;
            }
            catch (Exception ex)
            {
                pnlConteudo.Visible = false;
                MostrarEstado($"NÃO FOI POSSÍVEL CARREGAR O DASHBOARD.\r\n{MensagemErroAmigavel(ex)}", true);
            }
            finally
            {
                _carregando = false;
            }
        }

        private void AtualizarMetricas(DashboardResponseDto dashboard)
        {
            lblProdutosValor.Text = dashboard.TotalProducts.ToString("00");
            lblCategoriasValor.Text = dashboard.TotalCategories.ToString("00");
            lblDestaqueValor.Text = dashboard.FeaturedProducts.ToString("00");
            lblAtivosValor.Text = dashboard.ActiveProducts.ToString("00");
            //lblListaQuantidade.Text = $"{dashboard.RecentProducts?.Count ?? 0:00} ITENS";
        }

        private void PreencherProdutos(IEnumerable<DashboardProductDto> produtos)
        {
            pnlProdutos.SuspendLayout();
            pnlProdutos.Controls.Clear();

            var lista = produtos?.Take(5).ToList() ?? new List<DashboardProductDto>();
            if (lista.Count == 0)
            {
                pnlProdutos.Controls.Add(CriarVazio());
                pnlProdutos.ResumeLayout();
                return;
            }

            // Adiciona de trás para frente para manter a ordem visual no FlowLayoutPanel.
            foreach (var produto in lista.AsEnumerable().Reverse())
                pnlProdutos.Controls.Add(CriarLinhaProduto(produto));

            pnlProdutos.ResumeLayout();
        }

        private Control CriarVazio()
        {
            var painel = new Panel { Dock = DockStyle.Fill, BackColor = BetaFitTheme.Branco };
            var titulo = new Label
            {
                Text = "NENHUM PRODUTO CADASTRADO AINDA",
                Dock = DockStyle.Top,
                Height = 32,
                Padding = new Padding(20, 20, 20, 0),
                Font = BetaFitTheme.FonteRotulo,
                ForeColor = BetaFitTheme.Tinta
            };
            var texto = new Label
            {
                Text = "Os produtos cadastrados aparecerão aqui.",
                Dock = DockStyle.Top,
                Height = 50,
                Padding = new Padding(20, 5, 20, 0),
                Font = BetaFitTheme.FonteMedia,
                ForeColor = BetaFitTheme.TextoMuted
            };
            painel.Controls.Add(texto);
            painel.Controls.Add(titulo);
            return painel;
        }

        private Control CriarLinhaProduto(DashboardProductDto produto)
        {
            var linha = new Panel
            {
                Height = 92,
                Dock = DockStyle.Top,
                BackColor = BetaFitTheme.Branco,
                Margin = new Padding(0),
                Padding = new Padding(14, 10, 14, 10)
            };
            linha.Paint += (_, e) =>
            {
                using var pen = new Pen(BetaFitTheme.Linha);
                e.Graphics.DrawLine(pen, 0, linha.Height - 1, linha.Width, linha.Height - 1);
            };

            var imagem = CriarImagemProduto(produto);
            imagem.Location = new Point(14, 10);
            imagem.Anchor = AnchorStyles.Top | AnchorStyles.Left;

            var lblCategoria = new Label
            {
                AutoSize = true,
                Location = new Point(96, 10),
                Font = BetaFitTheme.FontePequena,
                ForeColor = BetaFitTheme.TextoMuted,
                Text = string.IsNullOrWhiteSpace(produto.CategoryName)
                    ? produto.Gender.ToString().ToUpperInvariant()
                    : $"{produto.CategoryName.ToUpperInvariant()}  /  {produto.Gender.ToString().ToUpperInvariant()}"
            };

            var lblNome = new Label
            {
                AutoEllipsis = true,
                Location = new Point(96, 30),
                Size = new Size(390, 23),
                Font = BetaFitTheme.FonteSubtitulo,
                ForeColor = BetaFitTheme.Tinta,
                Text = produto.Name
            };

            var lblData = new Label
            {
                AutoSize = true,
                Location = new Point(96, 56),
                Font = BetaFitTheme.FontePequena,
                ForeColor = BetaFitTheme.TextoMuted,
                Text = produto.CreatedAt == default ? "CADASTRO RECENTE" : $"CADASTRADO EM {produto.CreatedAt:dd/MM/yyyy}"
            };

            var lblPreco = new Label
            {
                AutoSize = true,
                Font = BetaFitTheme.FonteSubtitulo,
                ForeColor = BetaFitTheme.Tinta,
                Text = produto.Price.ToString("C2"),
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            var lblStatus = new Label
            {
                AutoSize = true,
                Font = BetaFitTheme.FontePequena,
                ForeColor = produto.IsActive ? BetaFitTheme.SucessoTexto : BetaFitTheme.TextoMuted,
                Text = produto.IsActive ? "ATIVO" : "INATIVO",
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            var lblDestaque = new Label
            {
                AutoSize = true,
                Font = BetaFitTheme.FontePequena,
                ForeColor = produto.IsFeatured ? BetaFitTheme.LimaEscuro : BetaFitTheme.TextoMuted,
                Text = produto.IsFeatured ? "EM DESTAQUE" : "",
                Anchor = AnchorStyles.Top | AnchorStyles.Right
            };

            linha.Resize += (_, _) =>
            {
                lblPreco.Left = linha.ClientSize.Width - lblPreco.Width - 18;
                lblStatus.Left = linha.ClientSize.Width - lblStatus.Width - 18;
                lblDestaque.Left = linha.ClientSize.Width - lblDestaque.Width - 18;
            };

            linha.Controls.Add(lblCategoria);
            linha.Controls.Add(lblNome);
            linha.Controls.Add(lblData);
            linha.Controls.Add(lblPreco);
            linha.Controls.Add(lblStatus);
            linha.Controls.Add(lblDestaque);
            linha.Controls.Add(imagem);
            linha.PerformLayout();

            lblPreco.Left = linha.ClientSize.Width - lblPreco.Width - 18;
            lblStatus.Left = linha.ClientSize.Width - lblStatus.Width - 18;
            lblDestaque.Left = linha.ClientSize.Width - lblDestaque.Width - 18;
            return linha;
        }

        private Control CriarImagemProduto(DashboardProductDto produto)
        {
            var painel = new Panel
            {
                Size = new Size(68, 70),
                BackColor = BetaFitTheme.SuperficieAlt
            };

            painel.Paint += (_, e) =>
            {
                using var brush = new SolidBrush(Color.FromArgb(35, 35, 35));
                e.Graphics.FillRectangle(brush, 0, 0, 4, painel.Height);
            };

            var placeholder = new Label
            {
                Dock = DockStyle.Fill,
                Text = "BF",
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font(BetaFitTheme.FonteBase, 17f, FontStyle.Bold),
                ForeColor = Color.FromArgb(145, 145, 140),
                BackColor = BetaFitTheme.SuperficieAlt
            };
            painel.Controls.Add(placeholder);

            if (!string.IsNullOrWhiteSpace(produto.ImageUrl))
                _ = CarregarImagemAsync(painel, placeholder, produto.ImageUrl);

            return painel;
        }

        private async Task CarregarImagemAsync(Panel painel, Label placeholder, string imageUrl)
        {
            try
            {
                var baseUrl = AppConfig.ApiBaseUrl?.TrimEnd('/');
                if (string.IsNullOrWhiteSpace(baseUrl)) return;

                var uri = Uri.TryCreate(imageUrl, UriKind.Absolute, out var absolute)
                    ? absolute
                    : new Uri(baseUrl + "/" + imageUrl.TrimStart('/'));

                using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(5) };
                using var stream = await client.GetStreamAsync(uri);
                using var original = Image.FromStream(stream);
                var bitmap = new Bitmap(original);

                if (IsDisposed || painel.IsDisposed) { bitmap.Dispose(); return; }

                if (painel.InvokeRequired)
                    painel.BeginInvoke(new Action(() => AplicarImagem(painel, placeholder, bitmap)));
                else
                    AplicarImagem(painel, placeholder, bitmap);
            }
            catch { }
        }

        private static void AplicarImagem(Panel painel, Label placeholder, Image imagem)
        {
            if (painel.IsDisposed) { imagem.Dispose(); return; }

            var picture = new PictureBox
            {
                Dock = DockStyle.Fill,
                Image = imagem,
                SizeMode = PictureBoxSizeMode.Zoom,
                BackColor = BetaFitTheme.SuperficieAlt
            };
            painel.Controls.Clear();
            painel.Controls.Add(picture);
            placeholder.Dispose();
        }

        private void MostrarEstado(string mensagem, bool erro)
        {
            pnlConteudo.Visible = false;
            pnlEstado.Visible = true;
            lblEstado.Text = mensagem;
            lblEstado.ForeColor = erro ? BetaFitTheme.PerigoTexto : BetaFitTheme.TextoMuted;
        }

        private static string MensagemErroAmigavel(Exception ex)
        {
            if (ex is HttpRequestException)
                return "VERIFIQUE SE A API BETAFIT ESTÁ EM EXECUÇÃO.";
            return ex.Message.ToUpperInvariant();
        }
    }
}
