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
    public partial class ProdutosUserControl : UserControl
    {
        //=================================================
        // SERVIÇOS (Inicializados no Load)
        //=================================================
        private ProductsApiService _productsApiService = null!;
        private CategoriesApiService _categoriesApiService = null!;

        //=================================================
        // DADOS
        //=================================================
        private List<ProductResponseDto> _todosProdutos = new();
        private List<ProductResponseDto> _produtosFiltrados = new(); // resultado da busca (ou todos, se vazio)
        private List<CategoriaResponseDto> _categorias = new();

        //=================================================
        // PAGINAÇÃO
        //=================================================
        private int _paginaAtual = 1;
        private const int TamanhoPagina = 8;

        //=================================================
        // SELEÇÃO (checkbox da 1ª coluna) — persiste entre páginas
        //=================================================
        private readonly HashSet<int> _idsSelecionados = new();

        //=================================================
        // MENU DE AÇÕES (coluna "⋮")
        //=================================================
        private readonly ContextMenuStrip _menuAcoesProduto = new();

        //=================================================
        // FOTO NO GRID (thumbnail da coluna colFoto)
        //=================================================
        private static readonly HttpClient _httpImagens = new() { Timeout = TimeSpan.FromSeconds(5) };
        private static readonly Dictionary<string, Image> _cacheImagens = new();
        private static readonly Image _imagemPlaceholder = CriarPlaceholder();

        //=================================================
        // CONSTRUTOR
        //=================================================
        public ProdutosUserControl()
        {
            InitializeComponent();
            ConfigurarMenuAcoes();
        }

        private void ConfigurarMenuAcoes()
        {
            _menuAcoesProduto.Items.Add("🖊  Editar", null, (s, e) => btnEditarProduto_Click(s!, e));
            _menuAcoesProduto.Items.Add("🗑  Excluir", null, (s, e) => btnExcluirProduto_Click(s!, e));
        }

        //=================================================
        // LOAD DO USER CONTROL
        //=================================================
        private async void ProductUserControl_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;

            _productsApiService = new ProductsApiService();
            _categoriesApiService = new CategoriesApiService();

            // Tema escuro (mesmo usado em Categorias) — substitui o AplicarEstiloGrid claro original
            BetaFitTheme.AplicarEstiloGridEscuro(gridProdutos);

            ConfigurarColunaFoto();
            ConfigurarColunaAcoes();
            ConfigurarColunaData();
            ConfigurarPermissoes();

            gridProdutos.ColumnHeaderMouseClick += gridProdutos_ColumnHeaderMouseClick;

            await CarregarDadosAsync();
        }

        //=================================================
        // AJUSTA A COLUNA DE FOTO JÁ EXISTENTE NO DESIGNER
        // (NÃO insere coluna nova — colFoto já vem do Designer,
        // inserir de novo aqui duplicaria a coluna)
        //=================================================
        private void ConfigurarColunaFoto()
        {
            if (!gridProdutos.Columns.Contains("colFoto")) return;

            var colFoto = (DataGridViewImageColumn)gridProdutos.Columns["colFoto"];
            colFoto.ImageLayout = DataGridViewImageCellLayout.Zoom;
            colFoto.DefaultCellStyle.NullValue = _imagemPlaceholder;
            colFoto.DefaultCellStyle.Padding = new Padding(2);
        }

        //=================================================
        // GARANTE O TEXTO "⋮" NA COLUNA DE AÇÕES
        //=================================================
        private void ConfigurarColunaAcoes()
        {
            if (!gridProdutos.Columns.Contains("colAcoes")) return;
            var colAcoes = (DataGridViewButtonColumn)gridProdutos.Columns["colAcoes"];
            colAcoes.Text = "⋮";
            colAcoes.FlatStyle = FlatStyle.Flat;
        }

        //=================================================
        // FAZ A DATA QUEBRAR EM 2 LINHAS (dd/MM/yyyy + HH:mm), como no mockup
        //=================================================
        private void ConfigurarColunaData()
        {
            if (!gridProdutos.Columns.Contains("colData")) return;
            gridProdutos.Columns["colData"].DefaultCellStyle.WrapMode = DataGridViewTriState.True;
        }

        //=================================================
        // BAIXA (OU PEGA DO CACHE) A IMAGEM DE UM PRODUTO
        //=================================================
        private async Task<Image> ObterImagemProdutoAsync(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return _imagemPlaceholder;

            var urlAbsoluta = ResolverUrlAbsolutaDaImagem(imageUrl);
            if (urlAbsoluta == null)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[Produtos/Foto] UiBaseUrl vazio (AppConfig.UiBaseUrl) — BetaFit.UI parece não estar rodando. ImageUrl recebida: '{imageUrl}'");
                return _imagemPlaceholder;
            }

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
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(
                    $"[Produtos/Foto] Falha ao baixar '{urlAbsoluta}': {ex.Message}");
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

        // Ícone "sem foto" desenhado via GDI+ (sol + montanha, estilo clássico de
        // imagem quebrada). Não depende de fonte de emoji, então sempre renderiza,
        // mesmo se a BetaFit.UI estiver offline e a foto real nunca chegar a baixar.
        private static Image CriarPlaceholder()
        {
            var bmp = new Bitmap(48, 48);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.FromArgb(30, 30, 30)); // combina com o tema escuro (Admin.FundoLinhaPar/Impar)

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

        //=================================================
        // PERMISSOES APENAS DO ADMIN
        //=================================================
        private void ConfigurarPermissoes()
        {
            bool isAdmin = SessionManager.Instance.IsAdmin;
            btnNovoProduto.Visible = isAdmin;
            btnEditarProduto.Visible = isAdmin;
            btnExcluirProduto.Visible = isAdmin;
        }

        //=================================================
        // CARREGAR DADOS (Produtos e Categorias)
        //=================================================
        private async Task CarregarDadosAsync()
        {
            try
            {
                _categorias = await _categoriesApiService.GetAllAsync();
                _todosProdutos = await _productsApiService.GetAllAsync();
                _produtosFiltrados = _todosProdutos;
                _paginaAtual = 1;
                PopularGrid();
            }
            catch (Exception ex)
            {
                BetaFitMessageBox.Erro(this, $"Erro ao carregar dados: {ex.Message}");
            }
        }

        //=================================================
        // POPULAR GRID (já considerando a página atual)
        //=================================================
        private void PopularGrid()
        {
            gridProdutos.Rows.Clear();

            int inicio = (_paginaAtual - 1) * TamanhoPagina;
            var pagina = _produtosFiltrados.Skip(inicio).Take(TamanhoPagina).ToList();

            foreach (var p in pagina)
            {
                var categoria = _categorias.FirstOrDefault(c => c.Id == p.CategoryId);
                string nomeCategoria = categoria?.Name ?? "Sem Categoria";
                string dataFormatada = $"{p.CreatedAt:dd/MM/yyyy}\n{p.CreatedAt:HH:mm}";

                int idxLinha = gridProdutos.Rows.Add(
                    _idsSelecionados.Contains(p.Id), // colSelecionar
                    p.Id,                            // colId
                    _imagemPlaceholder,               // colFoto (preenchida em segundo plano)
                    p.Name,                           // colNome
                    nomeCategoria,                     // colCategoria
                    p.Price.ToString("C"),            // colPreco
                    p.Gender.ToString(),               // colGenero
                    p.IsActive,                        // colStatus (bool -> pintado como badge)
                    dataFormatada,                      // colData
                    "⋮"                                 // colAcoes
                );

                _ = CarregarFotoLinhaAsync(idxLinha, p.ImageUrl);
            }

            AtualizarRodapePaginacao();
        }

        //=================================================
        // CARREGA A FOTO DE UMA LINHA ESPECÍFICA (EM SEGUNDO PLANO)
        //=================================================
        private async Task CarregarFotoLinhaAsync(int indiceLinha, string? imageUrl)
        {
            var imagem = await ObterImagemProdutoAsync(imageUrl);

            if (indiceLinha < 0 || indiceLinha >= gridProdutos.Rows.Count) return;
            if (gridProdutos.IsDisposed) return;

            gridProdutos.Rows[indiceLinha].Cells["colFoto"].Value = imagem;
        }

        //=================================================
        // PAGINAÇÃO — rodapé, ‹ ›, e os botões numerados (1 2 3 ... 9)
        //=================================================
        private void AtualizarRodapePaginacao()
        {
            int total = _produtosFiltrados.Count;
            int inicio = (_paginaAtual - 1) * TamanhoPagina;
            int de = total == 0 ? 0 : inicio + 1;
            int ate = Math.Min(inicio + TamanhoPagina, total);

            lblResumoPaginacao.Text = $"Exibindo {de} a {ate} de {total} produtos";

            int totalPaginas = Math.Max(1, (int)Math.Ceiling(total / (double)TamanhoPagina));
            btnPaginaAnterior.Enabled = _paginaAtual > 1;
            btnProximaPagina.Enabled = _paginaAtual < totalPaginas;

            AtualizarBotoesNumerados(totalPaginas);
            ReposicionarBotoesPaginacao();
        }

        // Usa os 4 botões fixos do Designer (btnPagina1/2/3 + btnUltimaPagina) como
        // uma "janela" deslizante em torno da página atual, com "..." quando necessário.
        // Ex.: totalPaginas=9, paginaAtual=1  -> [1][2][3] ... [9]
        //      totalPaginas=9, paginaAtual=5  -> [4][5][6] ... [9]
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

            lblReticencias.Visible = mostrarReticencias;
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
            btn.Enabled = true; // no Designer eles ficaram Enabled=false (herdado do "esqueleto" fixo),
                                // e com Enabled=false o Guna2Button ignora FillColor/BorderColor/ForeColor
                                // e sempre desenha com as cores de DisabledState (por isso ficavam todos cinza)
            btn.Text = pagina.ToString();
            btn.Tag = pagina;

            bool ativa = pagina == _paginaAtual;
            btn.FillColor = ativa ? BetaFitTheme.Admin.Lima : Color.Transparent;
            btn.ForeColor = ativa ? Color.Black : Color.White;
            btn.BorderColor = ativa ? BetaFitTheme.Admin.Lima : BetaFitTheme.Admin.Borda;
        }

        // Reposiciona da direita pra esquerda considerando só quem está visível
        // (evita "buraco" no lugar de um botão/​reticências escondido)
        private void ReposicionarBotoesPaginacao()
        {
            var elementos = new List<Control>();
            if (btnProximaPagina.Visible) elementos.Add(btnProximaPagina);
            if (btnUltimaPagina.Visible) elementos.Add(btnUltimaPagina);
            if (lblReticencias.Visible) elementos.Add(lblReticencias);
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
            int totalPaginas = Math.Max(1, (int)Math.Ceiling(_produtosFiltrados.Count / (double)TamanhoPagina));
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
        private void gridProdutos_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex < 0) return;
            var nomeColuna = gridProdutos.Columns[e.ColumnIndex].Name;

            // Cabeçalho da coluna de seleção -> desenha o quadradinho "selecionar tudo"
            if (e.RowIndex == -1 && nomeColuna == "colSelecionar")
            {
                PintarCheckboxCabecalho(e);
                return;
            }

            if (e.RowIndex < 0) return;

            switch (nomeColuna)
            {
                case "colId":
                    PintarCelulaId(e);
                    break;
                case "colStatus":
                    PintarCelulaStatus(e);
                    break;
            }
        }

        private void PintarCheckboxCabecalho(DataGridViewCellPaintingEventArgs e)
        {
            e.PaintBackground(e.CellBounds, true);

            bool marcado = gridProdutos.Rows.Count > 0 && PaginaAtualTotalmenteSelecionada();

            const int lado = 16;
            int x = e.CellBounds.Left + (e.CellBounds.Width - lado) / 2;
            int y = e.CellBounds.Top + (e.CellBounds.Height - lado) / 2;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var caneta = new Pen(BetaFitTheme.Admin.TextoMuted, 1.5f);
            e.Graphics.DrawRectangle(caneta, x, y, lado, lado);

            if (marcado)
            {
                using var brush = new SolidBrush(BetaFitTheme.Admin.Lima);
                e.Graphics.FillRectangle(brush, x + 2, y + 2, lado - 4, lado - 4);
            }

            e.Handled = true;
        }

        private bool PaginaAtualTotalmenteSelecionada()
        {
            foreach (DataGridViewRow row in gridProdutos.Rows)
            {
                if (row.Cells["colId"].Value == null) continue;
                var id = Convert.ToInt32(row.Cells["colId"].Value);
                if (!_idsSelecionados.Contains(id)) return false;
            }
            return true;
        }

        private void PintarCelulaId(DataGridViewCellPaintingEventArgs e)
        {
            e.PaintBackground(e.CellBounds, true);
            var texto = e.Value?.ToString() ?? "";

            using var fonte = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            var tam = e.Graphics.MeasureString(texto, fonte);

            int lado = 26;
            int x = e.CellBounds.Left + 12;
            int y = e.CellBounds.Top + (e.CellBounds.Height - lado) / 2;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var path = RetanguloArredondado(new Rectangle(x, y, lado, lado), 6))
            using (var brush = new SolidBrush(BetaFitTheme.Admin.IdBadgeFundo))
                e.Graphics.FillPath(brush, path);

            using (var brushTexto = new SolidBrush(BetaFitTheme.Admin.Lima))
                e.Graphics.DrawString(texto, fonte, brushTexto,
                    x + (lado - tam.Width) / 2, y + (lado - tam.Height) / 2);

            e.Handled = true;
        }

        private void PintarCelulaStatus(DataGridViewCellPaintingEventArgs e)
        {
            e.PaintBackground(e.CellBounds, true);
            bool ativo = e.Value != null && Convert.ToBoolean(e.Value);

            var fundo = ativo ? BetaFitTheme.Admin.BadgeAtivoFundo : BetaFitTheme.Admin.BadgeInativoFundo;
            var texto = ativo ? BetaFitTheme.Admin.BadgeAtivoTexto : BetaFitTheme.Admin.BadgeInativoTexto;
            var rotulo = ativo ? "✓  Ativo" : "Inativo";

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
        // CLIQUE NO CABEÇALHO DA COLUNA DE SELEÇÃO (marcar/desmarcar todos da página)
        //=================================================
        private void gridProdutos_ColumnHeaderMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
        {
            if (gridProdutos.Columns[e.ColumnIndex].Name != "colSelecionar") return;

            bool marcarTodos = !PaginaAtualTotalmenteSelecionada();

            foreach (DataGridViewRow row in gridProdutos.Rows)
            {
                if (row.Cells["colId"].Value == null) continue;
                var id = Convert.ToInt32(row.Cells["colId"].Value);
                row.Cells["colSelecionar"].Value = marcarTodos;
                if (marcarTodos) _idsSelecionados.Add(id);
                else _idsSelecionados.Remove(id);
            }

            gridProdutos.InvalidateColumn(gridProdutos.Columns["colSelecionar"].Index);
        }

        //=================================================
        // CLIQUE NA CÉLULA (checkbox de seleção OU coluna de ações "⋮")
        //=================================================
        private void gridProdutos_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            var nomeColuna = gridProdutos.Columns[e.ColumnIndex].Name;

            if (nomeColuna == "colSelecionar")
            {
                var cell = gridProdutos.Rows[e.RowIndex].Cells[e.ColumnIndex];
                bool atual = cell.Value != null && Convert.ToBoolean(cell.Value);
                bool novo = !atual;
                cell.Value = novo;

                var id = Convert.ToInt32(gridProdutos.Rows[e.RowIndex].Cells["colId"].Value);
                if (novo) _idsSelecionados.Add(id);
                else _idsSelecionados.Remove(id);

                gridProdutos.InvalidateColumn(gridProdutos.Columns["colSelecionar"].Index); // repinta o header
                return;
            }

            if (nomeColuna != "colAcoes") return;

            gridProdutos.ClearSelection();
            gridProdutos.Rows[e.RowIndex].Selected = true;
            gridProdutos.CurrentCell = gridProdutos.Rows[e.RowIndex].Cells[e.ColumnIndex];

            var cellRect = gridProdutos.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
            var pontoTela = gridProdutos.PointToScreen(new Point(cellRect.Left, cellRect.Bottom));
            _menuAcoesProduto.Show(pontoTela);
        }

        //=================================================
        // FILTRO DE PRODUTOS (PESQUISA POR TÍTULO, CATEGORIA OU ID)
        //=================================================
        private void FiltrarProdutos()
        {
            var termo = txtPesquisa.Text.Trim().ToLower();

            _produtosFiltrados = string.IsNullOrEmpty(termo)
                ? _todosProdutos
                : _todosProdutos.Where(p =>
                        p.Name.Contains(termo, StringComparison.OrdinalIgnoreCase)
                        || p.CategoryName.Contains(termo, StringComparison.OrdinalIgnoreCase)
                        || p.Id.ToString().Contains(termo))
                    .ToList();

            _paginaAtual = 1;
            PopularGrid();
        }

        private void txtPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                FiltrarProdutos();
            }
        }

        //=================================================
        // ADICIONAR NOVO PRODUTO
        //=================================================
        private async void btnNovoProduto_Click(object sender, EventArgs e)
        {
            using var form = new ProductFormDialog(_categorias, null);
            if (form.ShowDialog() == DialogResult.OK && form.ProductDto != null)
            {
                var (success, _, error) = await _productsApiService.CreateAsync(form.ProductDto);
                if (success)
                {
                    BetaFitMessageBox.Sucesso(this, "Produto criado com sucesso!");
                    await CarregarDadosAsync();
                }
                else
                {
                    BetaFitMessageBox.Erro(this, error);
                }
            }
        }

        //=================================================
        // EDITAR PRODUTO
        //=================================================
        private async void btnEditarProduto_Click(object sender, EventArgs e)
        {
            var produto = ObterProdutoSelecionado();
            if (produto == null)
            {
                BetaFitMessageBox.Aviso(this, "Selecione um produto para editar.");
                return;
            }

            using var form = new ProductFormDialog(_categorias, produto);
            if (form.ShowDialog() == DialogResult.OK && form.UpdateDto != null)
            {
                var (success, _, error) = await _productsApiService.UpdateAsync(produto.Id, form.UpdateDto);
                if (success)
                {
                    BetaFitMessageBox.Sucesso(this, "Produto atualizado com sucesso!");
                    await CarregarDadosAsync();
                }
                else
                {
                    BetaFitMessageBox.Erro(this, error);
                }
            }
        }

        //=================================================
        // OBTER PRODUTO SELECIONADO
        //=================================================
        private ProductResponseDto? ObterProdutoSelecionado()
        {
            if (gridProdutos.SelectedRows.Count == 0) return null;
            var row = gridProdutos.SelectedRows[0];
            if (row.Cells["colId"].Value == null) return null;
            var id = Convert.ToInt32(row.Cells["colId"].Value);
            return _todosProdutos.FirstOrDefault(p => p.Id == id);
        }

        //=================================================
        // EXCLUIR PRODUTO
        //=================================================
        private async void btnExcluirProduto_Click(object sender, EventArgs e)
        {
            var produto = ObterProdutoSelecionado();
            if (produto == null)
            {
                BetaFitMessageBox.Aviso(this, "Selecione um produto para excluir.");
                return;
            }

            bool conf = BetaFitMessageBox.Confirmar(
                this,
                $"Tem certeza que deseja excluir o produto:\n\"{produto.Name}\"?",
                "Confirmar Exclusão");

            if (!conf) return;

            var (success, error) = await _productsApiService.DeleteAsync(produto.Id);
            if (success)
            {
                _idsSelecionados.Remove(produto.Id);
                BetaFitMessageBox.Sucesso(this, "Produto excluído com sucesso!");
                await CarregarDadosAsync();
            }
            else
            {
                BetaFitMessageBox.Erro(this, error);
            }
        }

        //=================================================
        // ATUALIZAR PRODUTOS
        //=================================================
        private async void btnAtualizarProdutos_Click(object sender, EventArgs e) => await CarregarDadosAsync();
    }
}