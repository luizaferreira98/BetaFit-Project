using BetaFit.Desktop.DTOs;
using BetaFit.Desktop.Forms;
using BetaFit.Desktop.Helpers;
using BetaFit.Desktop.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetaFit.Desktop.UserControls
{
    public partial class ProdutosUserControl : UserControl
    {
        //Usar esse sistema de anotacoes no codigo inteiro para ficar legivel e organizado, principalmente em arquivos grandes.
        //=================================================
        // SERVIÇOS (Inicilizados no Load)
        //=================================================
        private ProductsApiService _productsApiService = null!;
        private CategoriesApiService _categoriesApiService = null!; // Se nao tiver nenhuma categoria ele avisa

        //=================================================
        // DADOS (LISTAS DAS PARADAS)
        //=================================================
        private List<ProductResponseDto> _todosProdutos = new();
        private List<CategoriaResponseDto> _categorias = new();

        //=================================================
        // FOTO NO GRID (thumbnail da coluna colFoto)
        //=================================================
        // Cliente HTTP dedicado só pra baixar as imagens (timeout curto:
        // se a URL estiver quebrada/lenta não pode travar a UI)
        private static readonly HttpClient _httpImagens = new() { Timeout = TimeSpan.FromSeconds(5) };

        // Cache em memória: evita rebaixar a mesma imagem toda vez que o grid recarrega
        private static readonly Dictionary<string, Image> _cacheImagens = new();

        // Placeholder exibido quando o produto não tem ImageUrl ou o download falha
        private static readonly Image _imagemPlaceholder = CriarPlaceholder();

        //=================================================
        // CONSTRUTOR
        //=================================================
        public ProdutosUserControl()
        {
            InitializeComponent();
        }

        //=================================================
        // LOAD DO USER CONTROL
        //=================================================
        private async void ProductUserControl_Load(object sender, EventArgs e)
        {
            //Guard: não executa em tempo de Design
            if (DesignMode) return;

            //Inicializa os serviços
            _productsApiService = new ProductsApiService();
            _categoriesApiService = new CategoriesApiService();

            //Aplica o tema no DataGridView
            BetaFit.Desktop.Themes.BetaFitTheme.AplicarEstiloGrid(gridProdutos);

            ConfigurarFotoNoGrid();
            ConfigurarPermissoes();
            await CarregarDadosAsync();
        }

        //=================================================
        // CONFIGURA A COLUNA DE FOTO NO GRID (thumbnail do produto)
        //=================================================
        private void ConfigurarFotoNoGrid()
        {
            // Evita duplicar a coluna se o Load rodar mais de uma vez
            if (gridProdutos.Columns.Contains("colFoto")) return;

            var colFoto = new DataGridViewImageColumn
            {
                Name = "colFoto",
                HeaderText = "",
                Width = 56,
                MinimumWidth = 56,
                ImageLayout = DataGridViewImageCellLayout.Zoom,
                Resizable = DataGridViewTriState.False,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                DefaultCellStyle = new DataGridViewCellStyle
                {
                    NullValue = _imagemPlaceholder,
                    Padding = new Padding(2)
                }
            };

            gridProdutos.Columns.Insert(0, colFoto);
            gridProdutos.RowTemplate.Height = 60;
            gridProdutos.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.None;
        }

        //=================================================
        // BAIXA (OU PEGA DO CACHE) A IMAGEM DE UM PRODUTO
        //=================================================
        private async Task<Image> ObterImagemProdutoAsync(string? imageUrl)
        {
            if (string.IsNullOrWhiteSpace(imageUrl))
                return _imagemPlaceholder;

            // Monta a URL absoluta antes de baixar. O banco guarda caminhos
            // relativos (ex: "/images/products/xxx.jpg"), que só existem
            // servidos pelo BetaFit.UI — não pela BetaFit.API.
            var urlAbsoluta = ResolverUrlAbsolutaDaImagem(imageUrl);
            if (urlAbsoluta == null)
                return _imagemPlaceholder;

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
                // URL quebrada, 404, timeout, arquivo não é imagem, etc.
                return _imagemPlaceholder;
            }
        }

        //=================================================
        // RESOLVE O CAMINHO DA IMAGEM PARA UMA URL ABSOLUTA
        //=================================================
        // Se já vier absoluta (http/https), usa como está.
        // Se vier relativa ("/images/products/xxx.jpg"), monta usando o
        // UiBaseUrl (BetaFit.UI é quem serve os arquivos estáticos de imagem).
        private string? ResolverUrlAbsolutaDaImagem(string imageUrl)
        {
            if (Uri.TryCreate(imageUrl, UriKind.Absolute, out _))
                return imageUrl;

            var uiBaseUrl = AppConfig.UiBaseUrl;
            if (string.IsNullOrWhiteSpace(uiBaseUrl))
                return null; // BetaFit.UI não localizado — sem como resolver o caminho relativo

            return $"{uiBaseUrl.TrimEnd('/')}/{imageUrl.TrimStart('/')}";
        }

        //=================================================
        // PLACEHOLDER (usado quando não há foto ou o download falha)
        //=================================================
        private static Image CriarPlaceholder()
        {
            var bmp = new Bitmap(48, 48);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            g.Clear(Color.FromArgb(238, 238, 234)); // bf-surface-2
            using var caneta = new Pen(Color.FromArgb(222, 222, 217), 1.5f); // bf-line
            g.DrawRectangle(caneta, 4, 4, 39, 39);
            using var fonte = new Font("Segoe UI", 18f, FontStyle.Bold);
            using var pincel = new SolidBrush(Color.FromArgb(111, 112, 108)); // bf-muted
            var texto = "📦";
            var tam = g.MeasureString(texto, fonte);
            g.DrawString(texto, fonte, pincel, (48 - tam.Width) / 2, (48 - tam.Height) / 2);
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
            gridProdutos.Rows.Clear();

            try
            {
                //Carrega categorias
                _categorias = await _categoriesApiService.GetAllAsync();
                //Carrega produtos
                _todosProdutos = await _productsApiService.GetAllAsync();
                //Preenche o DataGridView com os produtos (via PopularGrid, já com foto)
                PopularGrid(_todosProdutos);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados: {ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        //=================================================
        // POPULAR GRID COM OS PRODUTOS
        //=================================================
        private void PopularGrid(List<ProductResponseDto> produtos)
        {
            gridProdutos.Rows.Clear();
            foreach (var p in produtos)
            {
                var categoria = _categorias.FirstOrDefault(c => c.Id == p.CategoryId);
                string nomeCategoria = categoria?.Name ?? "Sem Categoria";

                // colFoto entra vazia (placeholder) e é preenchida em segundo plano
                // assim que a imagem termina de baixar — não trava o preenchimento do grid
                int idxLinha = gridProdutos.Rows.Add(
                    _imagemPlaceholder,
                    p.Id,
                    p.Name,
                    nomeCategoria,
                    p.Price.ToString("C"),
                    p.Gender,
                    p.IsActive ? "Ativo" : "Inativo",
                    p.CreatedAt.ToString("dd/MM/yyyy HH:mm")
                );

                _ = CarregarFotoLinhaAsync(idxLinha, p.ImageUrl);
            }
        }

        //=================================================
        // CARREGA A FOTO DE UMA LINHA ESPECÍFICA (EM SEGUNDO PLANO)
        //=================================================
        private async Task CarregarFotoLinhaAsync(int indiceLinha, string? imageUrl)
        {
            var imagem = await ObterImagemProdutoAsync(imageUrl);

            // A linha pode ter sido removida (filtro/refresh) enquanto a imagem baixava
            if (indiceLinha < 0 || indiceLinha >= gridProdutos.Rows.Count) return;
            if (gridProdutos.IsDisposed) return;

            gridProdutos.Rows[indiceLinha].Cells["colFoto"].Value = imagem;
        }

        //=================================================
        // FILTRO DE PRODUTOS (PESQUISA)
        //=================================================
        private void FiltrarProdutos()
        {
            var termo = txtPesquisa.Text.Trim().ToLower();
            if (string.IsNullOrEmpty(termo))
            {
                PopularGrid(_todosProdutos);
                return;
            }

            var filtrados = _todosProdutos
                .Where(g => g.Name.Contains(termo, StringComparison.OrdinalIgnoreCase)
                || g.CategoryName.Contains(termo, StringComparison.OrdinalIgnoreCase))
                .ToList();

            PopularGrid(filtrados);
        }

        //=================================================
        // TEXT BOX PESQUISA (ENTER PARA FILTRAR)
        //=================================================
        private void txtPesquisa_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true; // Evita o som de "bip" do Windows ao pressionar Enter
                FiltrarProdutos();
            }
        }

        //=================================================
        // ADICIONAR NOVO PRODUTO (ABRE FORMULARIO)
        //=================================================
        private async void btnNovoProduto_Click(object sender, EventArgs e)
        {
            using var form = new ProductFormDialog(_categorias, null);
            if (form.ShowDialog() == DialogResult.OK && form.ProductDto != null)
            {
                var (success, _, error) = await _productsApiService.CreateAsync(form.ProductDto);
                if (success)
                {
                    MessageBox.Show("✅ Produto criado com sucesso!",
                        "Sucesso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    await CarregarDadosAsync();
                }
                else
                {
                    MessageBox.Show($"❌ {error}",
                      "Erro",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
                }
            }
        }

        //=================================================
        // EDITAR PRODUTO (ABRE FORMULARIO COM DADOS EXISTENTES)
        //=================================================
        private async void btnEditarProduto_Click(object sender, EventArgs e)
        {
            var produto = ObterProdutoSelecionado();
            if (produto == null)
            {
                MessageBox.Show($"Selecione um game para editar.",
                      "Aviso",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Warning);
                return;
            }

            using var form = new ProductFormDialog(_categorias, produto);
            if (form.ShowDialog() == DialogResult.OK && form.UpdateDto != null)
            {
                var (success, _, error) = await _productsApiService.UpdateAsync(produto.Id, form.UpdateDto);
                if (success)
                {
                    MessageBox.Show("✅ Produto atualizado com sucesso!",
                        "Sucesso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    await CarregarDadosAsync();
                }
                else
                {
                    MessageBox.Show($"❌ {error}",
                      "Erro",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
                }
            }
        }

        //=================================================
        // OBTER PRODUTO SELECIONADO (RETORNA O OBJETO SELECIONADO NO GRID)
        //=================================================
        private ProductResponseDto? ObterProdutoSelecionado()
        {
            if (gridProdutos.SelectedRows.Count == 0) return null;
            var row = gridProdutos.SelectedRows[0];
            var id = Convert.ToInt32(row.Cells["colId"].Value);
            return _todosProdutos.FirstOrDefault(p => p.Id == id);
        }


        //=================================================
        // EXCLUIR PRODUTO (CONFIRMAÇÃO E CHAMADA AO SERVIÇO)
        //=================================================
        private async void btnExcluirProduto_Click(object sender, EventArgs e)
        {
            var produto = ObterProdutoSelecionado();
            if (produto == null)
            {
                MessageBox.Show("Selecione um produto para excluir.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var conf = MessageBox.Show(
                $"Tem certeza que deseja excluir o produto:\n\"{produto.Name}\"?",
                "Confirmar Exclusão",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (conf != DialogResult.Yes) return;

            var (success, error) = await _productsApiService.DeleteAsync(produto.Id);
            if (success)
            {
                MessageBox.Show("✅ Produto excluído com sucesso!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                await CarregarDadosAsync();
            }
            else
            {
                MessageBox.Show($"❌ {error}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //=================================================
        // ATUALIZAR PRODUTOS (BOTÃO) - RECARREGA DADOS DO SERVIDOR
        //=================================================
        private async void btnAtualizarProdutos_Click(object sender, EventArgs e) => await CarregarDadosAsync();
    }
}