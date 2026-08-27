// =============================================================================
// BetaFit.Desktop - UserControls/CategoriesUserControl.cs
// =============================================================================
//  CONCEITO: UserControl de CRUD de Categorias
//
// Permite gerenciar categorias de produtos:
//   GET    /api/categories         Listar
//   POST   /api/categories         Criar (Admin)
//   PUT    /api/categories/{id}    Editar (Admin)
//   DELETE /api/categories/{id}    Excluir (Admin)
//
// Todo o layout (grid, painel de formulário, botões) é definido em
// CategoriesUserControl.Designer.cs. Esta classe cuida apenas dos dados.
// =============================================================================

using BetaFit.Desktop.DTOs;
using BetaFit.Desktop.Services;
using BetaFit.Desktop.Themes;

namespace BetaFit.Desktop.UserControls
{
    public partial class CategoriesUserControl : UserControl
    {
        private readonly CategoriesApiService _categoriesService = new();
        private List<CategoriaResponseDto> _categories = new();

        // Estado do formulário: null = criação, valor = edição do Id correspondente
        private int? _editingId;

        public CategoriesUserControl()
        {
            InitializeComponent();
        }

        private async void CategoriesUserControl_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;

            pnlTop.BackColor = BetaFitTheme.PretoPrimario;
            lblTitle.ForeColor = Color.White;
            lblSubtitle.ForeColor = BetaFitTheme.TextoMuted;
            BackColor = BetaFitTheme.Superficie;
            pnlToolbar.BackColor = BetaFitTheme.Superficie;
            pnlForm.BackColor = BetaFitTheme.Branco;

            BetaFitButtons.EstilizarEscuro(btnNew);
            BetaFitButtons.EstilizarFantasma(btnEdit);
            BetaFitButtons.EstilizarPerigo(btnDelete);
            BetaFitButtons.EstilizarFantasma(btnRefresh);
            BetaFitButtons.EstilizarPrimario(btnSave);
            BetaFitButtons.EstilizarFantasma(btnCancel);
            BetaFitInputs.EstilizarSimples(txtName);
            BetaFitInputs.EstilizarRotulo(lblName);

            BetaFitTheme.AplicarEstiloGrid(gridCategories);

            await CarregarDadosAsync();
        }

        // =====================================================================
        // DADOS
        // =====================================================================

        private async Task CarregarDadosAsync()
        {
            gridCategories.Rows.Clear();
            try
            {
                _categories = await _categoriesService.GetAllAsync();
                foreach (var categoria in _categories)
                    gridCategories.Rows.Add(categoria.Id, categoria.Name, categoria.GameCount);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar categorias: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // =====================================================================
        // FORMULÁRIO
        // =====================================================================

        private void MostrarFormulario(CategoriaResponseDto? categoria)
        {
            _editingId = categoria?.Id;
            txtName.Text = categoria?.Name ?? string.Empty;
            lblFormTitle.Text = categoria == null ? "Nova Categoria" : "Editar Categoria";
            pnlForm.Visible = true;
            txtName.Focus();
        }

        private void OcultarFormulario()
        {
            pnlForm.Visible = false;
            _editingId = null;
            txtName.Clear();
        }

        // =====================================================================
        // EVENTOS DOS BOTÕES
        // =====================================================================

        private void btnNew_Click(object sender, EventArgs e) => MostrarFormulario(null);

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var categoria = ObterCategoriaSelecionada();
            if (categoria == null)
            {
                MessageBox.Show("Selecione uma categoria para editar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MostrarFormulario(categoria);
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            var categoria = ObterCategoriaSelecionada();
            if (categoria == null)
            {
                MessageBox.Show("Selecione uma categoria para excluir.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (categoria.GameCount > 0)
            {
                MessageBox.Show(
                    $"A categoria \"{categoria.Name}\" possui {categoria.GameCount} produto(s) vinculado(s).\nRemova os produtos antes de excluir.",
                    "Não é possível excluir", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmacao = MessageBox.Show($"Excluir a categoria \"{categoria.Name}\"?",
                "Confirmar Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirmacao != DialogResult.Yes) return;

            var (success, error) = await _categoriesService.DeleteAsync(categoria.Id);
            if (success)
            {
                MessageBox.Show("Categoria excluída com sucesso!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                await CarregarDadosAsync();
            }
            else
            {
                MessageBox.Show(error, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e) => await CarregarDadosAsync();

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Informe o nome da categoria.", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool success;
            string error;

            if (_editingId is null)
            {
                var dto = new CreateCategoriaDto { Name = txtName.Text.Trim() };
                var result = await _categoriesService.CreateAsync(dto);
                success = result.Success;
                error = result.ErrorMessage;
            }
            else
            {
                var dto = new UpdateCategoriaDto { Name = txtName.Text.Trim() };
                var result = await _categoriesService.UpdateAsync(_editingId.Value, dto);
                success = result.Success;
                error = result.ErrorMessage;
            }

            if (success)
            {
                MessageBox.Show("Categoria salva com sucesso!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                OcultarFormulario();
                await CarregarDadosAsync();
            }
            else
            {
                MessageBox.Show(error, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btnCancel_Click(object sender, EventArgs e) => OcultarFormulario();

        // =====================================================================
        // AUXILIARES
        // =====================================================================

        private CategoriaResponseDto? ObterCategoriaSelecionada()
        {
            if (gridCategories.SelectedRows.Count == 0) return null;
            var id = Convert.ToInt32(gridCategories.SelectedRows[0].Cells[colId.Name].Value);
            return _categories.FirstOrDefault(c => c.Id == id);
        }
    }
}
