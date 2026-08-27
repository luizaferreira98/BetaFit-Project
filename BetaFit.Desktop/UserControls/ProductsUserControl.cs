// =============================================================================
// BetaFit.Desktop - UserControls/ProductsUserControl.cs
// =============================================================================
//  CONCEITO: UserControl de CRUD de Produtos
//
// Permite gerenciar o catálogo de produtos da loja:
//   GET    /api/products         Listar
//   POST   /api/products         Criar (Admin)
//   PUT    /api/products/{id}    Editar (Admin)
//   DELETE /api/products/{id}    Excluir (Admin)
//
// Todo o layout (grid, painel de formulário, botões) é definido em
// ProductsUserControl.Designer.cs. Esta classe cuida apenas dos dados.
// =============================================================================

using BetaFit.Desktop.DTOs;
using BetaFit.Desktop.Services;
using BetaFit.Desktop.Themes;
using BetaFit.Domain.Enums;

namespace BetaFit.Desktop.UserControls
{
    public partial class ProductsUserControl : UserControl
    {
        private readonly ProductsApiService _productsService = new();
        private readonly CategoriesApiService _categoriesService = new();

        private List<ProductResponseDto> _products = new();
        private List<CategoriaResponseDto> _categories = new();

        // Estado do formulário: null = criação, valor = edição do Id correspondente
        private int? _editingId;

        public ProductsUserControl()
        {
            InitializeComponent();
        }

        private async void ProductsUserControl_Load(object sender, EventArgs e)
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
            BetaFitInputs.EstilizarSimples(txtDescription);
            BetaFitInputs.EstilizarSimples(txtPrice);
            BetaFitInputs.EstilizarSimples(txtImageUrl);
            BetaFitInputs.EstilizarComboBox(cmbCategory);
            BetaFitInputs.EstilizarComboBox(cmbGender);
            BetaFitInputs.EstilizarRotulo(lblName);
            BetaFitInputs.EstilizarRotulo(lblDescription);
            BetaFitInputs.EstilizarRotulo(lblPrice);
            BetaFitInputs.EstilizarRotulo(lblImageUrl);
            BetaFitInputs.EstilizarRotulo(lblCategory);
            BetaFitInputs.EstilizarRotulo(lblGender);

            cmbGender.DataSource = Enum.GetValues(typeof(Gender));

            BetaFitTheme.AplicarEstiloGrid(gridProducts);

            await CarregarCategoriasAsync();
            await CarregarDadosAsync();
        }

        // =====================================================================
        // DADOS
        // =====================================================================

        private async Task CarregarCategoriasAsync()
        {
            try
            {
                _categories = await _categoriesService.GetAllAsync();
                cmbCategory.DataSource = _categories;
                cmbCategory.DisplayMember = nameof(CategoriaResponseDto.Name);
                cmbCategory.ValueMember = nameof(CategoriaResponseDto.Id);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar categorias: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async Task CarregarDadosAsync()
        {
            gridProducts.Rows.Clear();
            try
            {
                _products = await _productsService.GetAllAsync();
                foreach (var produto in _products)
                {
                    gridProducts.Rows.Add(
                        produto.Id,
                        produto.Name,
                        produto.CategoryName,
                        produto.Price.ToString("C2"),
                        produto.Gender.ToString(),
                        produto.IsFeatured ? "SIM" : "NÃO",
                        produto.IsActive ? "SIM" : "NÃO");
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar produtos: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        // =====================================================================
        // FORMULÁRIO
        // =====================================================================

        private void MostrarFormulario(ProductResponseDto? produto)
        {
            _editingId = produto?.Id;
            lblFormTitle.Text = produto == null ? "Novo Produto" : "Editar Produto";

            txtName.Text = produto?.Name ?? string.Empty;
            txtDescription.Text = produto?.Description ?? string.Empty;
            txtPrice.Text = produto?.Price.ToString("0.00") ?? string.Empty;
            txtImageUrl.Text = produto?.ImageUrl ?? string.Empty;
            cmbCategory.SelectedValue = produto?.CategoryId ?? (_categories.FirstOrDefault()?.Id ?? 0);
            cmbGender.SelectedItem = produto != null
                ? produto.Gender
                : Enum.GetValues(typeof(Gender)).Cast<Gender>().First();
            chkFeatured.Checked = produto?.IsFeatured ?? false;
            chkActive.Checked = produto?.IsActive ?? true;
            chkActive.Enabled = produto != null; // produto novo já nasce ativo

            pnlForm.Visible = true;
            txtName.Focus();
        }

        private void OcultarFormulario()
        {
            pnlForm.Visible = false;
            _editingId = null;
        }

        // =====================================================================
        // EVENTOS DOS BOTÕES
        // =====================================================================

        private void btnNew_Click(object sender, EventArgs e) => MostrarFormulario(null);

        private void btnEdit_Click(object sender, EventArgs e)
        {
            var produto = ObterProdutoSelecionado();
            if (produto == null)
            {
                MessageBox.Show("Selecione um produto para editar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            MostrarFormulario(produto);
        }

        private async void btnDelete_Click(object sender, EventArgs e)
        {
            var produto = ObterProdutoSelecionado();
            if (produto == null)
            {
                MessageBox.Show("Selecione um produto para excluir.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var confirmacao = MessageBox.Show($"Excluir o produto \"{produto.Name}\"?",
                "Confirmar Exclusão", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);
            if (confirmacao != DialogResult.Yes) return;

            var (success, error) = await _productsService.DeleteAsync(produto.Id);
            if (success)
            {
                MessageBox.Show("Produto excluído com sucesso!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                await CarregarDadosAsync();
            }
            else
            {
                MessageBox.Show(error, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e)
        {
            await CarregarCategoriasAsync();
            await CarregarDadosAsync();
        }

        private async void btnSave_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtName.Text))
            {
                MessageBox.Show("Informe o nome do produto.", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (!decimal.TryParse(txtPrice.Text, out var preco) || preco < 0)
            {
                MessageBox.Show("Informe um preço válido.", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbCategory.SelectedValue is not int categoryId)
            {
                MessageBox.Show("Selecione uma categoria.", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var gender = cmbGender.SelectedItem is Gender selectedGender
                ? selectedGender
                : Enum.GetValues(typeof(Gender)).Cast<Gender>().First();

            bool success;
            string error;

            if (_editingId is null)
            {
                var dto = new CreateProductDto
                {
                    Name = txtName.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    Price = preco,
                    ImageUrl = string.IsNullOrWhiteSpace(txtImageUrl.Text) ? null : txtImageUrl.Text.Trim(),
                    Gender = gender,
                    CategoryId = categoryId,
                    IsFeatured = chkFeatured.Checked
                };
                var result = await _productsService.CreateAsync(dto);
                success = result.Success;
                error = result.ErrorMessage;
            }
            else
            {
                var dto = new UpdateProductDto
                {
                    Name = txtName.Text.Trim(),
                    Description = txtDescription.Text.Trim(),
                    Price = preco,
                    ImageUrl = string.IsNullOrWhiteSpace(txtImageUrl.Text) ? null : txtImageUrl.Text.Trim(),
                    Gender = gender,
                    CategoryId = categoryId,
                    IsFeatured = chkFeatured.Checked,
                    IsActive = chkActive.Checked
                };
                var result = await _productsService.UpdateAsync(_editingId.Value, dto);
                success = result.Success;
                error = result.ErrorMessage;
            }

            if (success)
            {
                MessageBox.Show("Produto salvo com sucesso!", "Sucesso",
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

        private ProductResponseDto? ObterProdutoSelecionado()
        {
            if (gridProducts.SelectedRows.Count == 0) return null;
            var id = Convert.ToInt32(gridProducts.SelectedRows[0].Cells[colId.Name].Value);
            return _products.FirstOrDefault(p => p.Id == id);
        }
    }
}
