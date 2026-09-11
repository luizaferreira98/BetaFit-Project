using BetaFit.Desktop.DTOs;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetaFit.Desktop.Forms
{
    /// <summary>
    /// Formulário de criação/edição de Produto.
    /// Retorna CreateProductDto (novo) ou UpdateProductDto (edição).
    /// </summary>
    public partial class ProductFormDialog : Form
    {
        // =====================================================================
        // PROPRIEDADES DE SAÍDA
        // =====================================================================

        /// <summary>DTO preenchido quando no modo de criação (OK)</summary>
        public CreateProductDto? ProductDto { get; private set; }

        /// <summary>DTO preenchido quando no modo de edição (OK)</summary>
        public UpdateProductDto? UpdateDto { get; private set; }


        // =====================================================================
        // CAMPOS PRIVADOS
        // =====================================================================
        private List<CategoriaResponseDto> _categorias = new();
        private ProductResponseDto? _productExistente;


        // =====================================================================
        // CONSTRUTORES
        // =====================================================================
        /// <summary>
        /// Construtor padrão sem parâmetros — necessário para o Designer.
        /// Use o construtor com parâmetros em produção.
        /// </summary>
        public ProductFormDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Construtor de produção com categorias e produto opcional.
        /// </summary>
        /// <param name="categorias">Lista de categorias para o ComboBox</param>
        /// <param name="product">null para criação, produto existente para edição</param>
        public ProductFormDialog(List<CategoriaResponseDto> categorias, ProductResponseDto? product)
        {
            _categorias = categorias;
            _productExistente = product;
            InitializeComponent();
        }


        // =====================================================================
        // EVENTO LOAD
        // =====================================================================
        private void ProductFormDialog_Load(object sender, EventArgs e)
        {
            // Guard: não executa em modo de Design
            if (DesignMode) return;

            // 1. Configura título baseado no modo (criação/edição)
            this.Text = _productExistente == null ? "Novo Produto" : "Editar Produto";
            lblTituloFromProduto.Text = _productExistente == null ? "➕ Novo Produto" : "✏️ Editar Produto";

            // 2. Popula o ComboBox de Categorias
            cboCategoriaProduto.Items.Clear();
            cboCategoriaProduto.Items.Add("Selecione uma categoria...");
            foreach (var cat in _categorias)
            {
                cboCategoriaProduto.Items.Add(cat.Name);
            }
            cboCategoriaProduto.SelectedIndex = 0;

            // 3. Popula o ComboBox de Gêneros usando a Opção 1 (List<string>)
            List<string> generos = new List<string>
            {
                "Masculino",
                "Feminino",
                "Unissex"
            };

            cboGenero.Items.Clear();
            cboGenero.Items.Add("Selecione um gênero...");
            foreach (var genero in generos)
            {
                cboGenero.Items.Add(genero);
            }
            cboGenero.SelectedIndex = 0;

            // 4. Preenche os campos se estiver no modo de EDIÇÃO
            if (_productExistente != null)
            {
                // Seleciona a Categoria do produto existente
                var cat = _categorias.FirstOrDefault(c => c.Id == _productExistente.CategoryId);
                if (cat != null)
                {
                    cboCategoriaProduto.SelectedItem = cat.Name;
                }

                // Seleciona o Gênero do produto existente
                // Se estiver no modo de EDIÇÃO
                if (_productExistente != null)
                {
                    // Converte o Enum Gender para texto
                    string generoTexto = _productExistente.Gender.ToString();

                    if (cboGenero.Items.Contains(generoTexto))
                    {
                        cboGenero.SelectedItem = generoTexto;
                    }
                }
            }

            //Preenche campos se estiver no modo edição
            PreencherCampos();
        }

        // =====================================================================
        // PREENCHIMENTO (MODO EDIÇÃO)
        // =====================================================================

        private void PreencherCampos()
        {
            if (_productExistente == null) return;

            txtNomeProduto.Text = _productExistente.Name;
            txtDescricaoProduto.Text = _productExistente.Description;
            txtPrecoProduto.Text = _productExistente.Price.ToString();
            txtUrlImagemProduto.Text = _productExistente.ImageUrl;
            swAtivo.Checked = _productExistente.IsFeatured;

            var idx = _categorias.FindIndex(c => c.Id == _productExistente.CategoryId);
            if (idx >= 0) cboCategoriaProduto.SelectedIndex = idx + 1;

        }

        // =====================================================================
        // SALVAR
        // =====================================================================
        private void btnSalvarProduto_Click(object sender, EventArgs e)
        {
            // 1. Validações de Entrada
            if (string.IsNullOrWhiteSpace(txtNomeProduto.Text))
            {
                BetaFitMessageBox.Aviso(this, "Informe o nome do produto.", "Validação");
                return;
            }

            if (!decimal.TryParse(txtPrecoProduto.Text, out decimal preco) || preco <= 0)
            {
                BetaFitMessageBox.Aviso(this, "Informe um preço válido maior que zero.", "Validação");
                return;
            }

            if (cboCategoriaProduto.SelectedIndex <= 0)
            {
                BetaFitMessageBox.Aviso(this, "Selecione uma categoria.", "Validação");
                return;
            }

            if (cboGenero.SelectedIndex <= 0)
            {
                BetaFitMessageBox.Aviso(this, "Selecione um gênero.", "Validação");
                return;
            }

            // 2. Extração de Dados
            var categoriaIdx = cboCategoriaProduto.SelectedIndex - 1;
            var categoriaId = _categorias[categoriaIdx].Id;

            string generoTexto = cboGenero?.SelectedItem?.ToString()!;
            Enum.TryParse<BetaFit.Domain.Enums.Gender>(generoTexto, out var generoEnum);

            // 3. Montagem do DTO
            if (_productExistente == null)
            {
                ProductDto = new CreateProductDto
                {
                    Name = txtNomeProduto.Text.Trim(),
                    Description = txtDescricaoProduto.Text.Trim(),
                    Price = preco,
                    ImageUrl = txtUrlImagemProduto.Text.Trim(),
                    CategoryId = categoriaId,
                    Gender = generoEnum,
                    IsFeatured = swAtivo.Checked
                };
            }
            else
            {
                UpdateDto = new UpdateProductDto
                {
                    Name = txtNomeProduto.Text.Trim(),
                    Description = txtDescricaoProduto.Text.Trim(),
                    Price = preco,
                    ImageUrl = txtUrlImagemProduto.Text.Trim(),
                    CategoryId = categoriaId,
                    Gender = generoEnum,
                    IsFeatured = swAtivo.Checked
                };
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }


        // =====================================================================
        // FECHAR O FORMULÁRIO (CANCELAR)
        // =====================================================================
        private void btnCancelarProduto_Click(object sender, EventArgs e)
        {
            this.Close();
        }
        // =====================================================================
        // FECHAR O FORMULÁRIO (X)
        // =====================================================================
        private void btnFecharNovoProduto_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}