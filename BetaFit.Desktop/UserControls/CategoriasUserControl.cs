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
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetaFit.Desktop.UserControls
{
    public partial class CategoriasUserControl : UserControl
    {

        //=================================================
        // SERVIÇOS (Inicilizados no Load)
        //=================================================
        private CategoriesApiService _categoriesService = null;

        //=================================================
        // DADOS
        //=================================================
        private List<CategoriaResponseDto> _categorias = new();

        //=================================================
        // CONSTRUTOR
        //=================================================
        public CategoriasUserControl()
        {
            InitializeComponent();
        }

        //=================================================
        // LOAD CATEGORIES
        //=================================================
        private async void CategoriesUserControl_Load(object sender, EventArgs e)
        {
            //Guard: não executa em tempo de Design
            if (DesignMode) return;

            //Inicializa serviços
            _categoriesService = new CategoriesApiService();

            //Aplica o tema no DataGridView
            //BetaFitThemes.AplicarEstiloGrid(gridGames); nao sei se tem ou nao mais vou deixar aqui!!!

            //Configurar permissões
            ConfigurarPermissoes();

            // Seleciona a linha completa ao clicar em qualquer célula
            gridCategorias.SelectionMode = DataGridViewSelectionMode.FullRowSelect;

            // Impede que o usuário selecione múltiplas linhas de uma vez
            gridCategorias.MultiSelect = false;

            //Reservado para CarregarDados
            await CarregarDadosAsync();
        }

        //=================================================
        // CONFIGURAÇÕES DE PERMISSÕES
        //=================================================
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
            gridCategorias.Rows.Clear();

            try
            {
                var tarefaCategorias = _categoriesService.GetAllAsync();
                await Task.WhenAll(tarefaCategorias);

                _categorias = tarefaCategorias.Result;

                PopularGrid(_categorias);

            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao carregar games: {ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        //=================================================
        // POPULAR GRID
        //=================================================
        private void PopularGrid(List<CategoriaResponseDto> categorias)
        {
            gridCategorias.Rows.Clear();
            foreach (var c in categorias)
            {
                gridCategorias.Rows.Add(
                    c.Id,
                    c.Name,
                    c.IsActive,
                    c.ProductCount,
                    c.CreatedAt.ToString("dd/MM/yyyy HH:mm"));
            }
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
                    MessageBox.Show("✅ Categoria criada com sucesso!",
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

        // =====================================================================
        // BTN DE EDITAR CATEGORIA
        // =====================================================================
        private async void btnEditarCategoria_Click(object sender, EventArgs e)
        {
            var categoria = ObterCategoriaSelecionada();
            if (categoria == null)
            {
                MessageBox.Show("Selecione uma categoria para editar.",
                    "Aviso",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            using var form = new CategoriesFormDialog(categoria);
            if (form.ShowDialog() == DialogResult.OK && form.UpdateDto != null)
            {
                var (success, _, error) = await _categoriesService.UpdateAsync(categoria.Id, form.UpdateDto);
                if (success)
                {
                    MessageBox.Show("✅ Categoria atualizada com sucesso!",
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

        // =====================================================================
        // OBTER CATEGORIA SELECIONADA
        // =====================================================================
        private CategoriaResponseDto? ObterCategoriaSelecionada()
        {
            if (gridCategorias.SelectedRows.Count == 0) return null;

            var row = gridCategorias.SelectedRows[0];
            if (row.Cells["colId"].Value == null) return null;

            var id = Convert.ToInt32(row.Cells["colId"].Value);

            // Busca na lista de categorias pelo ID selecionado na grid
            return _categorias.FirstOrDefault(c => c.Id == id);
        }

        // =====================================================================
        // EXCLUIR CATEGORIA
        // =====================================================================
        private async void btnExcluirCategoria_Click(object sender, EventArgs e)
        {
            var category = ObterCategoriaSelecionada();
            if (category == null)
            {
                MessageBox.Show("Selecione um categoria para excluir.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var conf = MessageBox.Show(
                $"Tem certeza que deseja excluir essa categoria:\n\"{category.Name}\"?",
                "Confirmar Exclusão",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (conf != DialogResult.Yes) return;

            var (success, error) = await _categoriesService.DeleteAsync(category.Id);
            if (success)
            {
                MessageBox.Show("✅ Categoria excluída com sucesso!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                await CarregarDadosAsync();
            }
            else
            {
                MessageBox.Show($"❌ {error}", "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // =====================================================================
        // ATUALIZAR BTN
        // =====================================================================
        private async void btnAtualizarProdutos_Click(object sender, EventArgs e) => await CarregarDadosAsync();
    }
}
