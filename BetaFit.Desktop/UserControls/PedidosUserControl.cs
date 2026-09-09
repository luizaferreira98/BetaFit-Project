using BetaFit.Desktop.DTOs;
using BetaFit.Desktop.Forms;
using BetaFit.Desktop.Helpers;
using BetaFit.Desktop.Services;
using BetaFit.Desktop.Themes;
using Guna.UI2.WinForms;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetaFit.Desktop.UserControls
{
    public partial class PedidosUserControl : UserControl
    {
        //=================================================
        // SERVIÇOS (Inicializados no Load)
        //=================================================
        private OrdersApiService _ordersApiService = null!;

        //=================================================
        // DADOS
        //=================================================
        private List<OrderResponseDto> _todosPedidos = new();
        private List<OrderResponseDto> _pedidosFiltrados = new(); // resultado da busca (ou todos, se vazio)

        // Precisa bater exatamente com o enum OrderStatus do BetaFit.Domain,
        // pois a API faz Enum.TryParse<OrderStatus>(status, true, ...).
        // Os RÓTULOS exibidos (Aguardando/Em separação/...) vêm de
        // BetaFitTheme.RotuloStatusPedido — não mexemos no enum em si.
        private static readonly string[] StatusDisponiveis =
        {
            "Pendente", "EmPreparacao", "Pronto", "Entregue", "Cancelado"
        };

        // Item do cboStatusPedido: guarda o valor REAL do enum (EnumValue,
        // o que vai pra API) separado do RÓTULO traduzido (o que aparece pro
        // usuário). O ComboBox chama ToString() pra decidir o que mostrar,
        // então só sobrescrever ToString() já resolve — sem precisar mexer
        // em DisplayMember/ValueMember do Guna2ComboBox.
        private readonly struct StatusItem
        {
            public string EnumValue { get; }
            public string Rotulo { get; }
            public StatusItem(string enumValue, string rotulo)
            {
                EnumValue = enumValue;
                Rotulo = rotulo;
            }
            public override string ToString() => Rotulo;
        }

        //=================================================
        // PAGINAÇÃO
        //=================================================
        private int _paginaAtual = 1;
        private const int TamanhoPagina = 5;

        //=================================================
        // MENU DE AÇÕES (coluna "⋮")
        //=================================================
        private readonly ContextMenuStrip _menuAcoesPedido = new();

        //=================================================
        // MINIATURAS DOS ITENS (coluna colItens) — cache por PEDIDO (não por
        // linha/página), pra não rebaixar a mesma imagem toda vez que o
        // usuário troca de página ou atualiza a busca.
        //=================================================
        private static readonly HttpClient _httpImagens = new() { Timeout = TimeSpan.FromSeconds(5) };
        private static readonly Dictionary<string, Image> _cacheImagens = new();
        private static readonly Image _imagemPlaceholder = CriarPlaceholderFoto();
        private readonly Dictionary<int, List<Image>> _imagensItensPorPedido = new();

        //=================================================
        // CONSTRUTOR
        //=================================================
        public PedidosUserControl()
        {
            InitializeComponent();
            ConfigurarMenuAcoes();
        }

        private void ConfigurarMenuAcoes()
        {
            _menuAcoesPedido.Items.Add("👁  Ver detalhes", null, (s, e) => AbrirDetalhesPedidoSelecionado());
            _menuAcoesPedido.Items.Add("🔃  Selecionar p/ status", null, (s, e) => { /* seleção já preenche o combo via gridPedidos_SelectionChanged */ });
        }

        //=================================================
        // LOAD DO USER CONTROL
        //=================================================
        private async void PedidosUserControl_Load(object sender, EventArgs e)
        {
            // Guard: não executa em tempo de Design
            if (DesignMode) return;

            _ordersApiService = new OrdersApiService();

            // Tema escuro (mesmo usado em Categorias/Produtos)
            BetaFitTheme.AplicarEstiloGridEscuro(gridPedidos);

            ConfigurarColunaAcoes();
            ConfigurarColunaData();

            // Ligados aqui (em vez de no Designer) pra não precisar mexer nas
            // propriedades de evento do grid na aba Properties.
            gridPedidos.CellClick += gridPedidos_CellClick;
            gridPedidos.CellPainting += gridPedidos_CellPainting;

            cboStatusPedido.Items.Clear();
            cboStatusPedido.Items.AddRange(
                StatusDisponiveis
                    .Select(s => (object)new StatusItem(s, BetaFitTheme.RotuloStatusPedido(s)))
                    .ToArray());

            // Ícone do estado vazio: desenhado via GDI+ em vez de emoji, porque
            // "🛒" em fonte grande (36pt) num Guna2HtmlLabel não renderiza
            // direito (vira duas caixinhas vazias / "tofu boxes") e o AutoSize
            // do label incha o layout. Desenhando na mão isso nunca acontece.
            pctIconeVazio.Image = CriarIconeCarrinhoVazio();
            pctIconeVazio.SizeMode = PictureBoxSizeMode.CenterImage;

            await CarregarDadosAsync();
        }

        // Ícone de carrinho vazio (contorno cinza + "brilho" lima em cima),
        // desenhado via GDI+ — sem depender de nenhuma fonte de emoji.
        private static Image CriarIconeCarrinhoVazio()
        {
            var bmp = new Bitmap(64, 64);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);

            using var canetaCarrinho = new Pen(Color.FromArgb(130, 130, 130), 2.5f)
            {
                LineJoin = LineJoin.Round,
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };

            // Corpo (trapézio) do carrinho
            PointF pTopoEsq = new(16, 20), pTopoDir = new(50, 20),
                   pBaseDir = new(44, 40), pBaseEsq = new(20, 40);
            g.DrawLine(canetaCarrinho, pTopoEsq, pTopoDir);
            g.DrawLine(canetaCarrinho, pTopoDir, pBaseDir);
            g.DrawLine(canetaCarrinho, pBaseDir, pBaseEsq);
            g.DrawLine(canetaCarrinho, pBaseEsq, pTopoEsq);

            // Haste + alça
            g.DrawLine(canetaCarrinho, 6, 12, 16, 20);
            g.DrawLine(canetaCarrinho, 6, 12, 13, 12);

            // Rodas
            using var brushRoda = new SolidBrush(Color.FromArgb(130, 130, 130));
            g.FillEllipse(brushRoda, 23, 44, 8, 8);
            g.FillEllipse(brushRoda, 38, 44, 8, 8);

            // "Brilho" lima em cima, referência ao mockup
            using var canetaLima = new Pen(BetaFitTheme.Admin.Lima, 2.5f)
            {
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };
            g.DrawLine(canetaLima, 32, 0, 32, 8);
            g.DrawLine(canetaLima, 42, 3, 46, 9);
            g.DrawLine(canetaLima, 22, 3, 18, 9);

            return bmp;
        }

        private void ConfigurarColunaAcoes()
        {
            if (!gridPedidos.Columns.Contains("colAcoes")) return;
            var colAcoes = (DataGridViewButtonColumn)gridPedidos.Columns["colAcoes"];
            colAcoes.Text = "⋮";
            colAcoes.FlatStyle = FlatStyle.Flat;
        }

        // Faz a data quebrar em 2 linhas (dd/MM/yyyy + HH:mm), como no mockup
        private void ConfigurarColunaData()
        {
            if (!gridPedidos.Columns.Contains("colData")) return;
            gridPedidos.Columns["colData"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        }

        //=================================================
        // CARREGAR DADOS (busca os pedidos salvos no banco via API)
        //=================================================
        private async Task CarregarDadosAsync()
        {
            try
            {
                _todosPedidos = await _ordersApiService.GetAllAsync();
                _pedidosFiltrados = _todosPedidos.OrderByDescending(p => p.CreatedAt).ToList();
                _paginaAtual = 1;
                PopularGrid();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar pedidos: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //=================================================
        // POPULAR GRID (já considerando página atual) + alternar estado vazio
        //=================================================
        private void PopularGrid()
        {
            gridPedidos.Rows.Clear();

            int inicio = (_paginaAtual - 1) * TamanhoPagina;
            var pagina = _pedidosFiltrados.Skip(inicio).Take(TamanhoPagina).ToList();

            foreach (var pedido in pagina)
            {
                string dataFormatada = $"{pedido.CreatedAt:dd/MM/yyyy}\n{pedido.CreatedAt:HH:mm}";

                gridPedidos.Rows.Add(
                    pedido.Id,          // colId
                    pedido.UserName,    // colCliente (pintado como avatar + nome)
                    dataFormatada,      // colData
                    pedido.Total.ToString("C"), // colTotal
                    pedido.Items.Count, // colItens (pintado como miniaturas empilhadas)
                    pedido.Status,      // colStatus (pintado como bolinha + rótulo)
                    "⋮"                 // colAcoes
                );

                _ = CarregarThumbsPedidoAsync(pedido);
            }

            AtualizarRodapePaginacao();
            AtualizarEstadoVazio();
        }

        //=================================================
        // ALTERNA ENTRE O GRID (pnlTabela) E O ESTADO VAZIO (pnlPedidosVazio)
        //=================================================
        private void AtualizarEstadoVazio()
        {
            bool semResultados = _pedidosFiltrados.Count == 0;
            pnlTabela.Visible = !semResultados;
            pnlPedidosVazio.Visible = semResultados;
        }

        //=================================================
        // MINIATURAS DOS ITENS — baixa (ou pega do cache) até 2 fotos por pedido
        //=================================================
        private async Task CarregarThumbsPedidoAsync(OrderResponseDto pedido)
        {
            if (_imagensItensPorPedido.ContainsKey(pedido.Id))
            {
                InvalidarLinhaDoPedido(pedido.Id);
                return;
            }

            var imagens = new List<Image>();
            foreach (var item in pedido.Items.Take(2))
            {
                imagens.Add(await ObterImagemItemAsync(item.ImageUrl));
            }

            _imagensItensPorPedido[pedido.Id] = imagens;
            InvalidarLinhaDoPedido(pedido.Id);
        }

        // Se a página mudou antes do download terminar, a linha pode não
        // existir mais na tela — nesse caso não tem problema, o cache já
        // está preenchido e a pintura correta acontece na próxima PopularGrid.
        private void InvalidarLinhaDoPedido(int pedidoId)
        {
            if (gridPedidos.IsDisposed) return;

            foreach (DataGridViewRow row in gridPedidos.Rows)
            {
                if (row.Cells["colId"].Value == null) continue;
                if (Convert.ToInt32(row.Cells["colId"].Value) == pedidoId)
                {
                    gridPedidos.InvalidateCell(gridPedidos.Columns["colItens"].Index, row.Index);
                    return;
                }
            }
        }

        private async Task<Image> ObterImagemItemAsync(string? imageUrl)
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

        private string? ResolverUrlAbsolutaDaImagem(string imageUrl)
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
            var bmp = new Bitmap(32, 32);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.FromArgb(35, 35, 35));
            using var caneta = new Pen(Color.FromArgb(90, 90, 90), 1.3f);
            g.DrawRectangle(caneta, 4, 4, 24, 24);
            return bmp;
        }

        //=================================================
        // PAGINAÇÃO — rodapé, ‹ ›, e os botões numerados (1 2 3 ... N)
        //=================================================
        private void AtualizarRodapePaginacao()
        {
            int total = _pedidosFiltrados.Count;
            int inicio = (_paginaAtual - 1) * TamanhoPagina;
            int de = total == 0 ? 0 : inicio + 1;
            int ate = Math.Min(inicio + TamanhoPagina, total);

            lblResumoPag.Text = $"Exibindo {de} a {ate} de {total} pedidos";

            int totalPaginas = Math.Max(1, (int)Math.Ceiling(total / (double)TamanhoPagina));
            btnPaginaAnterior.Enabled = _paginaAtual > 1;
            btnProximaPagina.Enabled = _paginaAtual < totalPaginas;

            AtualizarBotoesNumerados(totalPaginas);
            ReposicionarBotoesPaginacao();
        }

        // Usa os 4 botões fixos do Designer (btnPagina1/2/3 + btnUltimaPagina)
        // como "janela" deslizante em torno da página atual, com "..." quando
        // necessário. Ex.: totalPaginas=7, paginaAtual=1 -> [1][2][3] ... [7]
        private void AtualizarBotoesNumerados(int totalPaginas)
        {
            var janela = new List<int>();

            if (totalPaginas <= 3)
            {
                for (int i = 1; i <= totalPaginas; i++) janela.Add(i);
            }
            else
            {
                int inicioJanela = Math.Max(1, Math.Min(_paginaAtual - 1, totalPaginas - 2));
                for (int i = 0; i < 3; i++) janela.Add(inicioJanela + i);
            }

            ConfigurarBotaoPagina(btnPagina1, janela.Count > 0 ? janela[0] : null);
            ConfigurarBotaoPagina(btnPagina2, janela.Count > 1 ? janela[1] : null);
            ConfigurarBotaoPagina(btnPagina3, janela.Count > 2 ? janela[2] : null);

            int ultimoDaJanela = janela.Count > 0 ? janela[^1] : 0;
            bool mostrarUltima = totalPaginas > ultimoDaJanela;
            bool mostrarReticencias = totalPaginas > ultimoDaJanela + 1;

            lblRetic.Visible = mostrarReticencias;
            ConfigurarBotaoPagina(btnUltimaPagina, mostrarUltima ? totalPaginas : null);
        }

        private void ConfigurarBotaoPagina(Guna2Button btn, int? pagina)
        {
            if (pagina == null)
            {
                btn.Visible = false;
                return;
            }

            btn.Visible = true;
            btn.Enabled = true; // no Designer eles ficam Enabled=false; com Enabled=false
                                // o Guna2Button ignora FillColor/BorderColor/ForeColor e
                                // sempre desenha com as cores de DisabledState.
            btn.Text = pagina.ToString();
            btn.Tag = pagina;

            bool ativa = pagina == _paginaAtual;
            btn.FillColor = ativa ? BetaFitTheme.Admin.Lima : Color.Transparent;
            btn.ForeColor = ativa ? Color.Black : Color.White;
            btn.BorderColor = ativa ? BetaFitTheme.Admin.Lima : BetaFitTheme.Admin.Borda;
        }

        // Reposiciona da direita pra esquerda considerando só quem está
        // visível (evita "buraco" no lugar de um botão/reticências escondido)
        private void ReposicionarBotoesPaginacao()
        {
            var elementos = new List<Control>();
            if (btnProximaPagina.Visible) elementos.Add(btnProximaPagina);
            if (btnUltimaPagina.Visible) elementos.Add(btnUltimaPagina);
            if (lblRetic.Visible) elementos.Add(lblRetic);
            if (btnPagina3.Visible) elementos.Add(btnPagina3);
            if (btnPagina2.Visible) elementos.Add(btnPagina2);
            if (btnPagina1.Visible) elementos.Add(btnPagina1);
            elementos.Add(btnPaginaAnterior);

            const int margemDireita = 20;
            const int espaco = 6;
            int borda = pnlPaginacao.Width - margemDireita;

            foreach (var ctrl in elementos)
            {
                ctrl.Left = borda - ctrl.Width;
                borda = ctrl.Left - espaco;
            }
        }

        private void btnPaginaAnterior_Click(object sender, EventArgs e)
        {
            if (_paginaAtual <= 1) return;
            _paginaAtual--;
            PopularGrid();
        }

        private void btnProximaPagina_Click(object sender, EventArgs e)
        {
            int totalPaginas = Math.Max(1, (int)Math.Ceiling(_pedidosFiltrados.Count / (double)TamanhoPagina));
            if (_paginaAtual >= totalPaginas) return;
            _paginaAtual++;
            PopularGrid();
        }

        // Mesmo método nos 4 botões numerados (btnPagina1, btnPagina2, btnPagina3, btnUltimaPagina)
        private void btnPagina_Click(object sender, EventArgs e)
        {
            if (sender is Control c && c.Tag is int pagina)
            {
                _paginaAtual = pagina;
                PopularGrid();
            }
        }

        //=================================================
        // PINTURA CUSTOMIZADA DAS CÉLULAS
        //=================================================
        private void gridPedidos_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
            var nomeColuna = gridPedidos.Columns[e.ColumnIndex].Name;

            switch (nomeColuna)
            {
                case "colCliente":
                    PintarCelulaCliente(e);
                    break;
                case "colItens":
                    PintarCelulaItens(e);
                    break;
                case "colStatus":
                    PintarCelulaStatus(e);
                    break;
            }
        }

        // Avatar (bolinha com iniciais) + nome do cliente
        private void PintarCelulaCliente(DataGridViewCellPaintingEventArgs e)
        {
            e.PaintBackground(e.CellBounds, true);
            var nome = e.Value?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(nome)) { e.Handled = true; return; }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            const int diametro = 28;
            int x = e.CellBounds.Left + 8;
            int y = e.CellBounds.Top + (e.CellBounds.Height - diametro) / 2;

            using (var brush = new SolidBrush(BetaFitTheme.CorAvatar(nome)))
                e.Graphics.FillEllipse(brush, x, y, diametro, diametro);

            string iniciais = ObterIniciais(nome);
            using var fonteIniciais = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            var tamIniciais = e.Graphics.MeasureString(iniciais, fonteIniciais);
            using (var brushTexto = new SolidBrush(Color.White))
                e.Graphics.DrawString(iniciais, fonteIniciais, brushTexto,
                    x + (diametro - tamIniciais.Width) / 2, y + (diametro - tamIniciais.Height) / 2);

            using var fonteNome = new Font("Segoe UI", 9F);
            var tamNome = e.Graphics.MeasureString(nome, fonteNome);
            using (var brushNome = new SolidBrush(BetaFitTheme.Admin.TextoPrincipal))
                e.Graphics.DrawString(nome, fonteNome, brushNome,
                    x + diametro + 10, e.CellBounds.Top + (e.CellBounds.Height - tamNome.Height) / 2);

            e.Handled = true;
        }

        private static string ObterIniciais(string nomeCompleto)
        {
            var partes = nomeCompleto.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length == 0) return "?";
            if (partes.Length == 1) return partes[0][..1].ToUpper();
            return $"{partes[0][0]}{partes[^1][0]}".ToUpper();
        }

        // Miniaturas empilhadas dos itens do pedido + "+N" se tiver mais de 2
        private void PintarCelulaItens(DataGridViewCellPaintingEventArgs e)
        {
            e.PaintBackground(e.CellBounds, true);

            var pedidoId = Convert.ToInt32(gridPedidos.Rows[e.RowIndex].Cells["colId"].Value);
            int totalItens = e.Value != null ? Convert.ToInt32(e.Value) : 0;

            const int lado = 28;
            const int sobreposicao = 8;
            int x = e.CellBounds.Left + 8;
            int y = e.CellBounds.Top + (e.CellBounds.Height - lado) / 2;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            var imagens = _imagensItensPorPedido.TryGetValue(pedidoId, out var lista)
                ? lista
                : new List<Image> { _imagemPlaceholder };

            int qtdMostrada = Math.Min(2, Math.Max(1, imagens.Count));
            for (int i = 0; i < qtdMostrada; i++)
            {
                var img = i < imagens.Count ? imagens[i] : _imagemPlaceholder;
                var destino = new Rectangle(x + i * (lado - sobreposicao), y, lado, lado);

                using (var path = RetanguloArredondado(destino, 6))
                {
                    e.Graphics.SetClip(path, System.Drawing.Drawing2D.CombineMode.Replace);
                    e.Graphics.DrawImage(img, destino);
                    e.Graphics.ResetClip();

                    using var caneta = new Pen(BetaFitTheme.Admin.FundoLinhaPar, 2f);
                    e.Graphics.DrawPath(caneta, path);
                }
            }

            if (totalItens > qtdMostrada)
            {
                string rotulo = $"+{totalItens - qtdMostrada}";
                using var fonte = new Font("Segoe UI", 8F, FontStyle.Bold);
                var tam = e.Graphics.MeasureString(rotulo, fonte);

                int xBadge = x + qtdMostrada * (lado - sobreposicao) + 4;
                var destinoBadge = new Rectangle(xBadge, y, lado, lado);

                using (var path = RetanguloArredondado(destinoBadge, 6))
                using (var brush = new SolidBrush(BetaFitTheme.Admin.FundoLinhaImpar))
                    e.Graphics.FillPath(brush, path);

                using var brushTexto = new SolidBrush(BetaFitTheme.Admin.TextoMuted);
                e.Graphics.DrawString(rotulo, fonte, brushTexto,
                    xBadge + (lado - tam.Width) / 2, y + (lado - tam.Height) / 2);
            }

            e.Handled = true;
        }

        // Badge preenchido (pílula) com o rótulo traduzido do status
        private void PintarCelulaStatus(DataGridViewCellPaintingEventArgs e)
        {
            e.PaintBackground(e.CellBounds, true);
            var status = e.Value?.ToString() ?? "";
            if (string.IsNullOrEmpty(status)) { e.Handled = true; return; }

            var (fundo, texto) = BetaFitTheme.CorBadgeStatusPedidoAdmin(status);
            var rotulo = BetaFitTheme.RotuloStatusPedido(status);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var fonte = new Font("Segoe UI", 9F, FontStyle.Bold);
            var tam = e.Graphics.MeasureString(rotulo, fonte);

            int largura = (int)tam.Width + 24;
            int altura = 26;
            int x = e.CellBounds.Left + 8;
            int y = e.CellBounds.Top + (e.CellBounds.Height - altura) / 2;

            using (var path = RetanguloArredondado(new Rectangle(x, y, largura, altura), altura / 2))
            using (var brush = new SolidBrush(fundo))
                e.Graphics.FillPath(brush, path);

            using (var brushTexto = new SolidBrush(texto))
                e.Graphics.DrawString(rotulo, fonte, brushTexto,
                    x + (largura - tam.Width) / 2, y + (altura - tam.Height) / 2);

            e.Handled = true;
        }

        private static GraphicsPath RetanguloArredondado(Rectangle bounds, int raio)
        {
            var path = new GraphicsPath();
            int d = raio * 2;
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        //=================================================
        // CLIQUE NA CÉLULA (coluna de ações "⋮")
        //=================================================
        private void gridPedidos_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (gridPedidos.Columns[e.ColumnIndex].Name != "colAcoes") return;

            gridPedidos.ClearSelection();
            gridPedidos.Rows[e.RowIndex].Selected = true;
            gridPedidos.CurrentCell = gridPedidos.Rows[e.RowIndex].Cells[e.ColumnIndex];

            var cellRect = gridPedidos.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
            var pontoTela = gridPedidos.PointToScreen(new Point(cellRect.Left, cellRect.Bottom));
            _menuAcoesPedido.Show(pontoTela);
        }

        //=================================================
        // OBTER PEDIDO SELECIONADO
        //=================================================
        private OrderResponseDto? ObterPedidoSelecionado()
        {
            if (gridPedidos.SelectedRows.Count == 0) return null;
            var row = gridPedidos.SelectedRows[0];
            var id = Convert.ToInt32(row.Cells["colId"].Value);
            return _todosPedidos.FirstOrDefault(p => p.Id == id);
        }

        //=================================================
        // AO SELECIONAR UM PEDIDO, PRÉ-SELECIONA O STATUS ATUAL NO COMBO
        //=================================================
        private void gridPedidos_SelectionChanged(object sender, EventArgs e)
        {
            var pedido = ObterPedidoSelecionado();
            if (pedido == null) return;

            foreach (StatusItem item in cboStatusPedido.Items)
            {
                if (item.EnumValue == pedido.Status)
                {
                    cboStatusPedido.SelectedItem = item;
                    break;
                }
            }
        }

        //=================================================
        // DUPLO CLIQUE NA LINHA -> ABRE O POPUP DE DETALHES DO PEDIDO
        //=================================================
        private void gridPedidos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            AbrirDetalhesPedidoSelecionado();
        }

        private void AbrirDetalhesPedidoSelecionado()
        {
            var pedido = ObterPedidoSelecionado();
            if (pedido == null) return;

            using var dialog = new OrderDetailsFormDialog(pedido);
            dialog.ShowDialog(this.FindForm());
        }

        //=================================================
        // BUSCA (POR CLIENTE OU ID)
        //=================================================
        private void FiltrarPedidos()
        {
            var termo = txtPesquisaPedido.Text.Trim().ToLower();

            _pedidosFiltrados = string.IsNullOrEmpty(termo)
                ? _todosPedidos.OrderByDescending(p => p.CreatedAt).ToList()
                : _todosPedidos
                    .Where(p => p.UserName.Contains(termo, StringComparison.OrdinalIgnoreCase)
                             || p.Id.ToString().Contains(termo))
                    .OrderByDescending(p => p.CreatedAt)
                    .ToList();

            _paginaAtual = 1;
            PopularGrid();
        }

        private void txtPesquisaPedido_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                FiltrarPedidos();
            }
        }

        //=================================================
        // ESTADO VAZIO -> BOTÃO "IR PARA PRODUTOS"
        //=================================================
        private void btnIrParaProdutos_Click(object sender, EventArgs e)
        {
            (this.FindForm() as MainForm)?.NavegarParaProdutos();
        }

        //=================================================
        // ATUALIZAR PEDIDOS (BOTÃO) - RECARREGA DADOS DA API
        //=================================================
        private async void btnAtualizarPedidos_Click(object sender, EventArgs e)
            => await CarregarDadosAsync();

        //=================================================
        // ATUALIZAR STATUS DO PEDIDO SELECIONADO
        //=================================================
        private async void btnAtualizarStatusPedido_Click(object sender, EventArgs e)
        {
            var pedido = ObterPedidoSelecionado();
            if (pedido == null)
            {
                MessageBox.Show("Selecione um pedido na lista.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboStatusPedido.SelectedItem is not StatusItem statusSelecionado)
            {
                MessageBox.Show("Selecione o novo status.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var novoStatus = statusSelecionado.EnumValue;

            var (success, error) = await _ordersApiService.UpdateStatusAsync(pedido.Id, novoStatus);
            if (success)
            {
                MessageBox.Show("✅ Status atualizado com sucesso!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                await CarregarDadosAsync();
            }
            else
            {
                MessageBox.Show($"❌ {error}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}