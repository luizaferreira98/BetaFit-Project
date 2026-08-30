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
    public partial class ProdutosUserControl : UserControl
    {
        //Usar esse sistema de anotacoes no codigo inteiro para ficar legivel e organizado, principalmente em arquivos grandes.
        //=================================================
        // SERVIÇOS (Inicilizados no Load)
        //=================================================
        private ProductsApiService _productsApiService = null;
        private CategoriesApiService _categoriesApiService = null!; // Se nao tiver nenhuma categoria ele avisa

        //=================================================
        // DADOS (LISTAS DAS PARADAS)
        //=================================================
        private List<ProductResponseDto> _todosProdutos = new();
        private List<CategoriaResponseDto> _categorias = new();

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
            //BetaFitThemes.AplicarEstiloGrid(gridGames); nao sei se tem ou nao mais vou deixar aqui!!!

            ConfigurarPermissoes();
            await CarregarDadosAsync();
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
                //Preenche o DataGridView com os produtos
                foreach (var produto in _todosProdutos)
                {
                    var categoria = _categorias.FirstOrDefault(c => c.Id == produto.CategoryId);
                    string nomeCategoria = categoria != null ? categoria.Name : "Categoria não encontrada";
                    gridProdutos.Rows.Add(produto.Id, produto.Name, nomeCategoria, produto.Price.ToString("C"), produto.Gender, produto.IsFeatured, produto.CreatedAt);
                }

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

                gridProdutos.Rows.Add(
                    p.Id,
                    p.Name,
                    nomeCategoria,
                    p.Price.ToString("C"),
                    p.Gender,
                    p.IsFeatured ? "Ativo" : "Inativo",
                    p.CreatedAt.ToString("dd/MM/yyyy HH:mm")
                );
            }
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
