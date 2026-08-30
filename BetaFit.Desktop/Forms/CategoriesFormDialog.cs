using BetaFit.Application.DTOs;
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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace BetaFit.Desktop.Forms
{
    /// <summary>
    /// Formulário de criação/edição de Categoria.
    /// Retorna CreateCategoryDto (novo) ou UpdateCategoryDto (edição).
    /// </summary>
    public partial class CategoriesFormDialog : Form
    {
        // =====================================================================
        // PROPRIEDADES DE SAÍDA
        // =====================================================================

        /// <summary>DTO preenchido quando no modo de criação (OK)</summary>
        public CreateCategoriaDto? CategoryDto { get; private set; }

        /// <summary>DTO preenchido quando no modo de edição (OK)</summary>
        public UpdateCategoriaDto? UpdateDto { get; private set; }

        // =====================================================================
        // CAMPOS PRIVADOS
        // =====================================================================
        private CategoriaResponseDto? _categoriaExistente;


        // =====================================================================
        // CONSTRUTORES
        // =====================================================================

        /// <summary>
        /// Construtor padrão sem parâmetros — necessário para o Designer.
        /// Use o construtor com parâmetros em produção.
        /// </summary>
        public CategoriesFormDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Construtor de produção com categorias opcional.
        /// </summary>
        /// <param name="categorias">Lista de categorias para o ComboBox</param>
        /// <param name="game">null para criação, game existente para edição</param>
        public CategoriesFormDialog(CategoriaResponseDto? categoria)
        {
            _categoriaExistente = categoria;
            InitializeComponent();
        }

        // =====================================================================
        // EVENTO LOAD
        // =====================================================================
        private void CategoriesFormDialog_Load(object sender, EventArgs e)
        {
            //Guard
            if (DesignMode) return;

            // Configura título baseado no modo (criação/edição)
            this.Text = _categoriaExistente == null ? "Novo Game" : "Editar Game";
            lblTituloNovaCategoria.Text = _categoriaExistente == null ? "➕ Novo Game" : "✏️ Editar Game";

            //Preenche campos se estiver no modo edição
            PreencherCampos();
        }

        // =====================================================================
        // PREENCHIMENTO (MODO EDIÇÃO)
        // =====================================================================
        private void PreencherCampos()
        {
            if (_categoriaExistente == null) return;

            txtNomeCategoria.Text = _categoriaExistente.Name;
        }

        // =====================================================================
        // SALVAR
        // =====================================================================
        private void btnSalvarCategoria_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNomeCategoria.Text))
            {
                MessageBox.Show(
                    "Informe o nome da categoria.",
                    "Validação",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            if (_categoriaExistente == null)
            {
                CategoryDto = new CreateCategoriaDto
                {
                    Name = txtNomeCategoria.Text.Trim(),
                };
            }
            else
            {
                UpdateDto = new UpdateCategoriaDto
                {
                    Name = txtNomeCategoria.Text.Trim(),
                };
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        // =====================================================================
        // BTN FECHAR /CANCELAR
        // =====================================================================
        private void btnCancelarCategoria_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // =====================================================================
        // BTN FECHAR (X) - MESMO COMPORTAMENTO DO CANCELAR
        // =====================================================================
        private void btnFecharNovaCategoria_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
