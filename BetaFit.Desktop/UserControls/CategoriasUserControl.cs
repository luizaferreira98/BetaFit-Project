using BetaFit.Desktop.DTOs;
using BetaFit.Desktop.Forms;
using BetaFit.Desktop.Helpers;
using BetaFit.Desktop.Services;
using BetaFit.Desktop.Themes;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetaFit.Desktop.UserControls
{
    public partial class CategoriasUserControl : UserControl
    {
        //=================================================
        // SERVIÇOS (Inicializados no Load)
        //=================================================
        private CategoriesApiService _categoriesService = null!;

        //=================================================
        // DADOS
        //=================================================
        private List<CategoriaResponseDto> _categorias = new();

        //=================================================
        // PAGINAÇÃO
        //=================================================
        private int _paginaAtual = 1;
        private const int TamanhoPagina = 10;

        //=================================================
        // MENU DE AÇÕES (coluna "⋮")
        //=================================================
        private readonly ContextMenuStrip _menuAcoesCategoria = new();

        //=================================================
        // ÍCONE + COR POR CATEGORIA (aproximação visual do mockup;
        // categorias sem mapeamento caem no ícone padrão 🏷)
        //=================================================
        private static readonly Dictionary<string, string> _iconePorCategoria =
            new(StringComparer.OrdinalIgnoreCase)
            {
                ["Acessórios"] = "🧢",
                ["Camisetas"] = "👕",
                ["Leggings"] = "👖",
                ["Moletons"] = "🧥",
                ["Shorts"] = "🩳",
                ["Tênis"] = "👟",
            };

        //=================================================
        // CONSTRUTOR
        //=================================================
        public CategoriasUserControl()
        {
            InitializeComponent();
            ConfigurarMenuAcoes();
        }

        private void ConfigurarMenuAcoes()
        {
            _menuAcoesCategoria.Items.Add("🖊  Editar", null, (s, e) => btnEditarCategoria_Click(s!, e));
            _menuAcoesCategoria.Items.Add("🗑  Excluir", null, (s, e) => btnExcluirCategoria_Click(s!, e));
        }

        //=================================================
        // LOAD
        //=================================================
        private async void CategoriesUserControl_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;

            _categoriesService = new CategoriesApiService();

            // Tema escuro (substitui o AplicarEstiloGrid claro original)
            BetaFitTheme.AplicarEstiloGridEscuro(gridCategorias);

            ConfigurarPermissoes();

            gridCategorias.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridCategorias.MultiSelect = false;

            await CarregarDadosAsync();
        }

        private void ConfigurarPermissoes()
        {
            bool isAdmin = SessionManager.Instance.IsAdmin;
            btnNovaCategoria.Visible = isAdmin;
            btnEditarCategoria.Visible = isAdmin;
            btnExcluirCategoria.Visible = isAdmin;
        }

        //=================================================
        // CARREGAR DADOS
        //=================================================
        private async Task CarregarDadosAsync()
        {
            try
            {
                _categorias = await _categoriesService.GetAllAsync();
                _paginaAtual = 1;
                PopularGrid();
            }
            catch (Exception ex)
            {
                BetaFitMessageBox.Erro(this, $"Erro ao carregar categorias: {ex.Message}");
            }
        }

        //=================================================
        // POPULAR GRID (já considerando a página atual)
        //=================================================
        private void PopularGrid()
        {
            gridCategorias.Rows.Clear();

            int inicio = (_paginaAtual - 1) * TamanhoPagina;
            var pagina = _categorias.Skip(inicio).Take(TamanhoPagina).ToList();

            foreach (var c in pagina)
            {
                gridCategorias.Rows.Add(
                    c.Id,
                    c.Name,
                    $"📦  {c.ProductCount}",
                    c.IsActive,
                    $"📅  {c.CreatedAt:dd/MM/yyyy HH:mm}",
                    "⋮");
            }

            AtualizarRodapePaginacao(inicio, pagina.Count);
        }

        //=================================================
        // PAGINAÇÃO — rodapé e botões ‹ ›
        //=================================================
        private void AtualizarRodapePaginacao(int inicio, int quantidadeNaPagina)
        {
            int total = _categorias.Count;
            int de = total == 0 ? 0 : inicio + 1;
            int ate = inicio + quantidadeNaPagina;

            lblResumoPaginacao.Text = $"Exibindo {de} a {ate} de {total} categorias";
            btnPaginaAtual.Text = _paginaAtual.ToString();

            int totalPaginas = Math.Max(1, (int)Math.Ceiling(total / (double)TamanhoPagina));
            btnPaginaAnterior.Enabled = _paginaAtual > 1;
            btnProximaPagina.Enabled = _paginaAtual < totalPaginas;
        }

        private void btnPaginaAnterior_Click(object sender, EventArgs e)
        {
            if (_paginaAtual <= 1) return;
            _paginaAtual--;
            PopularGrid();
        }

        private void btnProximaPagina_Click(object sender, EventArgs e)
        {
            int totalPaginas = Math.Max(1, (int)Math.Ceiling(_categorias.Count / (double)TamanhoPagina));
            if (_paginaAtual >= totalPaginas) return;
            _paginaAtual++;
            PopularGrid();
        }

        //=================================================
        // PINTURA CUSTOMIZADA DAS CÉLULAS
        //=================================================
        private void gridCategorias_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;

            var nomeColuna = gridCategorias.Columns[e.ColumnIndex].Name;
            switch (nomeColuna)
            {
                case "colId":
                    PintarCelulaId(e);
                    break;
                case "colNome":
                    PintarCelulaCategoria(e);
                    break;
                case "colIsActive":
                    PintarCelulaStatus(e);
                    break;
            }
        }

        private void PintarCelulaId(DataGridViewCellPaintingEventArgs e)
        {
            e.PaintBackground(e.CellBounds, true);
            var texto = e.Value?.ToString() ?? "";

            using var fonte = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            var tam = e.Graphics.MeasureString(texto, fonte);

            int lado = 26;
            int x = e.CellBounds.Left + 16;
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

        private void PintarCelulaCategoria(DataGridViewCellPaintingEventArgs e)
        {
            e.PaintBackground(e.CellBounds, true);
            var nome = e.Value?.ToString() ?? "";
            var emoji = _iconePorCategoria.TryGetValue(nome, out var ic) ? ic : "🏷";

            const int diametro = 30;
            int x = e.CellBounds.Left + 12;
            int y = e.CellBounds.Top + (e.CellBounds.Height - diametro) / 2;

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (var brush = new SolidBrush(BetaFitTheme.Admin.Lima))
                e.Graphics.FillEllipse(brush, x, y, diametro, diametro);

            using (var fonteEmoji = new Font("Segoe UI Emoji", 11F))
            {
                var tamEmoji = e.Graphics.MeasureString(emoji, fonteEmoji);
                e.Graphics.DrawString(emoji, fonteEmoji, Brushes.Black,
                    x + (diametro - tamEmoji.Width) / 2, y + (diametro - tamEmoji.Height) / 2);
            }

            using var fonteNome = new Font("Segoe UI", 10F, FontStyle.Bold);
            e.Graphics.DrawString(nome, fonteNome, Brushes.White,
                x + diametro + 10, e.CellBounds.Top + (e.CellBounds.Height - fonteNome.Height) / 2);

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
        // CLIQUE NA COLUNA DE AÇÕES ("⋮")
        //=================================================
        private void gridCategorias_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;
            if (gridCategorias.Columns[e.ColumnIndex].Name != "colAcoes") return;

            gridCategorias.ClearSelection();
            gridCategorias.Rows[e.RowIndex].Selected = true;
            gridCategorias.CurrentCell = gridCategorias.Rows[e.RowIndex].Cells[e.ColumnIndex];

            var cellRect = gridCategorias.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
            var pontoTela = gridCategorias.PointToScreen(new Point(cellRect.Left, cellRect.Bottom));
            _menuAcoesCategoria.Show(pontoTela);
        }

        //=================================================
        // BTN DE NOVA CATEGORIA
        //=================================================
        private async void btnNovaCategoria_Click(object sender, EventArgs e)
        {
            using var form = new CategoriesFormDialog(null);
            if (form.ShowDialog() == DialogResult.OK && form.CategoryDto != null)
            {
                var (success, _, error) = await _categoriesService.CreateAsync(form.CategoryDto);
                if (success)
                {
                    BetaFitMessageBox.Sucesso(this, "Categoria criada com sucesso!");
                    await CarregarDadosAsync();
                }
                else
                {
                    BetaFitMessageBox.Erro(this, error);
                }
            }
        }

        //=================================================
        // BTN DE EDITAR CATEGORIA
        //=================================================
        private async void btnEditarCategoria_Click(object sender, EventArgs e)
        {
            var categoria = ObterCategoriaSelecionada();
            if (categoria == null)
            {
                BetaFitMessageBox.Aviso(this, "Selecione uma categoria para editar.");
                return;
            }

            using var form = new CategoriesFormDialog(categoria);
            if (form.ShowDialog() == DialogResult.OK && form.UpdateDto != null)
            {
                var (success, _, error) = await _categoriesService.UpdateAsync(categoria.Id, form.UpdateDto);
                if (success)
                {
                    BetaFitMessageBox.Sucesso(this, "Categoria atualizada com sucesso!");
                    await CarregarDadosAsync();
                }
                else
                {
                    BetaFitMessageBox.Erro(this, error);
                }
            }
        }

        //=================================================
        // OBTER CATEGORIA SELECIONADA
        //=================================================
        private CategoriaResponseDto? ObterCategoriaSelecionada()
        {
            if (gridCategorias.SelectedRows.Count == 0) return null;

            var row = gridCategorias.SelectedRows[0];
            if (row.Cells["colId"].Value == null) return null;

            var id = Convert.ToInt32(row.Cells["colId"].Value);
            return _categorias.FirstOrDefault(c => c.Id == id);
        }

        //=================================================
        // EXCLUIR CATEGORIA
        //=================================================
        private async void btnExcluirCategoria_Click(object sender, EventArgs e)
        {
            var category = ObterCategoriaSelecionada();
            if (category == null)
            {
                BetaFitMessageBox.Aviso(this, "Selecione uma categoria para excluir.");
                return;
            }

            bool conf = BetaFitMessageBox.Confirmar(
                this,
                $"Tem certeza que deseja excluir essa categoria:\n\"{category.Name}\"?",
                "Confirmar Exclusão");

            if (!conf) return;

            var (success, error) = await _categoriesService.DeleteAsync(category.Id);
            if (success)
            {
                BetaFitMessageBox.Sucesso(this, "Categoria excluída com sucesso!");
                await CarregarDadosAsync();
            }
            else
            {
                BetaFitMessageBox.Erro(this, error);
            }
        }

        //=================================================
        // ATUALIZAR BTN
        //=================================================
        private async void btnAtualizarProdutos_Click(object sender, EventArgs e) => await CarregarDadosAsync();
    }
}