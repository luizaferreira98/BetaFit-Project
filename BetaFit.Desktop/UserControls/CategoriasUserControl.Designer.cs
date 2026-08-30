namespace BetaFit.Desktop.UserControls
{
    partial class CategoriasUserControl
    {
        /// <summary> 
        /// Variável de designer necessária.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Limpar os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se for necessário descartar os recursos gerenciados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Designer de Componentes

        /// <summary> 
        /// Método necessário para suporte ao Designer - não modifique 
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            btnAtualizarProdutos = new Guna.UI2.WinForms.Guna2Button();
            lblTituloCategoria = new Guna.UI2.WinForms.Guna2HtmlLabel();
            guna2HtmlLabel1 = new Guna.UI2.WinForms.Guna2HtmlLabel();
            pnlBotoesCategorias = new Guna.UI2.WinForms.Guna2Panel();
            btnExcluirCategoria = new Guna.UI2.WinForms.Guna2Button();
            btnEditarCategoria = new Guna.UI2.WinForms.Guna2Button();
            btnNovaCategoria = new Guna.UI2.WinForms.Guna2Button();
            gridCategorias = new Guna.UI2.WinForms.Guna2DataGridView();
            colId = new DataGridViewTextBoxColumn();
            colNome = new DataGridViewTextBoxColumn();
            Column1 = new DataGridViewTextBoxColumn();
            colIsActive = new DataGridViewTextBoxColumn();
            colCreatedAt = new DataGridViewTextBoxColumn();
            pnlBotoesCategorias.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridCategorias).BeginInit();
            SuspendLayout();
            // 
            // btnAtualizarProdutos
            // 
            btnAtualizarProdutos.BorderRadius = 5;
            btnAtualizarProdutos.Cursor = Cursors.Hand;
            btnAtualizarProdutos.CustomizableEdges = customizableEdges1;
            btnAtualizarProdutos.DisabledState.BorderColor = Color.DarkGray;
            btnAtualizarProdutos.DisabledState.CustomBorderColor = Color.DarkGray;
            btnAtualizarProdutos.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnAtualizarProdutos.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnAtualizarProdutos.FillColor = Color.GreenYellow;
            btnAtualizarProdutos.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAtualizarProdutos.ForeColor = Color.Black;
            btnAtualizarProdutos.Location = new Point(516, 16);
            btnAtualizarProdutos.Name = "btnAtualizarProdutos";
            btnAtualizarProdutos.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnAtualizarProdutos.Size = new Size(148, 45);
            btnAtualizarProdutos.TabIndex = 6;
            btnAtualizarProdutos.Text = "🔃 ATUALIZAR";
            btnAtualizarProdutos.Click += btnAtualizarProdutos_Click;
            // 
            // lblTituloCategoria
            // 
            lblTituloCategoria.BackColor = Color.Transparent;
            lblTituloCategoria.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTituloCategoria.ForeColor = SystemColors.ControlLightLight;
            lblTituloCategoria.Location = new Point(16, 16);
            lblTituloCategoria.Name = "lblTituloCategoria";
            lblTituloCategoria.Size = new Size(151, 34);
            lblTituloCategoria.TabIndex = 7;
            lblTituloCategoria.Text = "CATEGORIAS";
            // 
            // guna2HtmlLabel1
            // 
            guna2HtmlLabel1.BackColor = Color.Transparent;
            guna2HtmlLabel1.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            guna2HtmlLabel1.ForeColor = SystemColors.ActiveBorder;
            guna2HtmlLabel1.Location = new Point(16, 49);
            guna2HtmlLabel1.Name = "guna2HtmlLabel1";
            guna2HtmlLabel1.Size = new Size(182, 19);
            guna2HtmlLabel1.TabIndex = 8;
            guna2HtmlLabel1.Text = "Organize os produtos da loja";
            // 
            // pnlBotoesCategorias
            // 
            pnlBotoesCategorias.Controls.Add(btnExcluirCategoria);
            pnlBotoesCategorias.Controls.Add(btnEditarCategoria);
            pnlBotoesCategorias.Controls.Add(btnNovaCategoria);
            pnlBotoesCategorias.CustomizableEdges = customizableEdges9;
            pnlBotoesCategorias.Location = new Point(16, 74);
            pnlBotoesCategorias.Name = "pnlBotoesCategorias";
            pnlBotoesCategorias.ShadowDecoration.CustomizableEdges = customizableEdges10;
            pnlBotoesCategorias.Size = new Size(648, 71);
            pnlBotoesCategorias.TabIndex = 9;
            // 
            // btnExcluirCategoria
            // 
            btnExcluirCategoria.BackColor = Color.Transparent;
            btnExcluirCategoria.BorderColor = Color.Gray;
            btnExcluirCategoria.BorderRadius = 5;
            btnExcluirCategoria.BorderThickness = 1;
            btnExcluirCategoria.Cursor = Cursors.Hand;
            btnExcluirCategoria.CustomizableEdges = customizableEdges3;
            btnExcluirCategoria.DisabledState.BorderColor = Color.DarkGray;
            btnExcluirCategoria.DisabledState.CustomBorderColor = Color.DarkGray;
            btnExcluirCategoria.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnExcluirCategoria.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnExcluirCategoria.FillColor = Color.FromArgb(64, 64, 64);
            btnExcluirCategoria.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnExcluirCategoria.ForeColor = Color.White;
            btnExcluirCategoria.Location = new Point(277, 13);
            btnExcluirCategoria.Name = "btnExcluirCategoria";
            btnExcluirCategoria.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnExcluirCategoria.Size = new Size(97, 45);
            btnExcluirCategoria.TabIndex = 12;
            btnExcluirCategoria.Text = "🗑 EXCLUIR";
            btnExcluirCategoria.Click += btnExcluirCategoria_Click;
            // 
            // btnEditarCategoria
            // 
            btnEditarCategoria.BackColor = Color.Transparent;
            btnEditarCategoria.BorderColor = Color.Gray;
            btnEditarCategoria.BorderRadius = 5;
            btnEditarCategoria.BorderThickness = 1;
            btnEditarCategoria.Cursor = Cursors.Hand;
            btnEditarCategoria.CustomizableEdges = customizableEdges5;
            btnEditarCategoria.DisabledState.BorderColor = Color.DarkGray;
            btnEditarCategoria.DisabledState.CustomBorderColor = Color.DarkGray;
            btnEditarCategoria.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnEditarCategoria.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnEditarCategoria.FillColor = Color.FromArgb(64, 64, 64);
            btnEditarCategoria.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnEditarCategoria.ForeColor = Color.White;
            btnEditarCategoria.Location = new Point(166, 12);
            btnEditarCategoria.Name = "btnEditarCategoria";
            btnEditarCategoria.ShadowDecoration.CustomizableEdges = customizableEdges6;
            btnEditarCategoria.Size = new Size(103, 45);
            btnEditarCategoria.TabIndex = 11;
            btnEditarCategoria.Text = "🖊 EDITAR";
            btnEditarCategoria.Click += btnEditarCategoria_Click;
            // 
            // btnNovaCategoria
            // 
            btnNovaCategoria.BorderRadius = 5;
            btnNovaCategoria.Cursor = Cursors.Hand;
            btnNovaCategoria.CustomizableEdges = customizableEdges7;
            btnNovaCategoria.DisabledState.BorderColor = Color.DarkGray;
            btnNovaCategoria.DisabledState.CustomBorderColor = Color.DarkGray;
            btnNovaCategoria.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnNovaCategoria.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnNovaCategoria.FillColor = Color.GreenYellow;
            btnNovaCategoria.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnNovaCategoria.ForeColor = Color.Black;
            btnNovaCategoria.Location = new Point(6, 12);
            btnNovaCategoria.Name = "btnNovaCategoria";
            btnNovaCategoria.ShadowDecoration.CustomizableEdges = customizableEdges8;
            btnNovaCategoria.Size = new Size(148, 45);
            btnNovaCategoria.TabIndex = 10;
            btnNovaCategoria.Text = "+ NOVA CATEGORIA";
            btnNovaCategoria.Click += btnNovaCategoria_Click;
            // 
            // gridCategorias
            // 
            dataGridViewCellStyle1.BackColor = Color.White;
            gridCategorias.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.Gray;
            dataGridViewCellStyle2.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle2.ForeColor = Color.Gainsboro;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            gridCategorias.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            gridCategorias.ColumnHeadersHeight = 19;
            gridCategorias.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            gridCategorias.Columns.AddRange(new DataGridViewColumn[] { colId, colNome, Column1, colIsActive, colCreatedAt });
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle3.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            gridCategorias.DefaultCellStyle = dataGridViewCellStyle3;
            gridCategorias.GridColor = Color.FromArgb(231, 229, 255);
            gridCategorias.Location = new Point(16, 161);
            gridCategorias.MultiSelect = false;
            gridCategorias.Name = "gridCategorias";
            gridCategorias.RowHeadersVisible = false;
            gridCategorias.Size = new Size(648, 294);
            gridCategorias.TabIndex = 10;
            gridCategorias.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            gridCategorias.ThemeStyle.HeaderStyle.BackColor = Color.Gray;
            gridCategorias.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            gridCategorias.ThemeStyle.HeaderStyle.ForeColor = Color.Gainsboro;
            gridCategorias.ThemeStyle.HeaderStyle.Height = 19;
            gridCategorias.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            gridCategorias.ThemeStyle.RowsStyle.Height = 25;
            // 
            // colId
            // 
            colId.HeaderText = "ID";
            colId.Name = "colId";
            // 
            // colNome
            // 
            colNome.HeaderText = "NOME";
            colNome.Name = "colNome";
            // 
            // Column1
            // 
            Column1.HeaderText = "TOTAL DE PRODUTOS";
            Column1.Name = "Column1";
            // 
            // colIsActive
            // 
            colIsActive.HeaderText = "ATIVO";
            colIsActive.Name = "colIsActive";
            // 
            // colCreatedAt
            // 
            colCreatedAt.HeaderText = "CRIADO EM";
            colCreatedAt.Name = "colCreatedAt";
            // 
            // CategoriasUserControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(64, 64, 64);
            Controls.Add(gridCategorias);
            Controls.Add(pnlBotoesCategorias);
            Controls.Add(guna2HtmlLabel1);
            Controls.Add(lblTituloCategoria);
            Controls.Add(btnAtualizarProdutos);
            Name = "CategoriasUserControl";
            Size = new Size(677, 474);
            Load += CategoriesUserControl_Load;
            pnlBotoesCategorias.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridCategorias).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Button btnAtualizarProdutos;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTituloCategoria;
        private Guna.UI2.WinForms.Guna2HtmlLabel guna2HtmlLabel1;
        private Guna.UI2.WinForms.Guna2Panel pnlBotoesCategorias;
        private Guna.UI2.WinForms.Guna2Button btnNovaCategoria;
        private Guna.UI2.WinForms.Guna2Button btnEditarCategoria;
        private Guna.UI2.WinForms.Guna2Button btnExcluirCategoria;
        private Guna.UI2.WinForms.Guna2DataGridView gridCategorias;
        private DataGridViewTextBoxColumn colId;
        private DataGridViewTextBoxColumn colNome;
        private DataGridViewTextBoxColumn Column1;
        private DataGridViewTextBoxColumn colIsActive;
        private DataGridViewTextBoxColumn colCreatedAt;
    }
}
