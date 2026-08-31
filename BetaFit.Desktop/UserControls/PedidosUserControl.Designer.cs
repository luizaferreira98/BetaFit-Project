namespace BetaFit.Desktop.UserControls
{
    partial class PedidosUserControl
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges15 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges16 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges11 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges12 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges13 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges14 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle6 = new DataGridViewCellStyle();
            lblTituloPedidos = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblDescricaoPedidos = new Guna.UI2.WinForms.Guna2HtmlLabel();
            btnAtualizarPedidos = new Guna.UI2.WinForms.Guna2Button();
            guna2Panel1 = new Guna.UI2.WinForms.Guna2Panel();
            btnAtualizarStatusPedido = new Guna.UI2.WinForms.Guna2Button();
            cboStatusPedido = new Guna.UI2.WinForms.Guna2ComboBox();
            lblNovoStatus = new Guna.UI2.WinForms.Guna2HtmlLabel();
            gridProdutos = new Guna.UI2.WinForms.Guna2DataGridView();
            colId = new DataGridViewTextBoxColumn();
            colCliente = new DataGridViewTextBoxColumn();
            colData = new DataGridViewTextBoxColumn();
            colTotal = new DataGridViewTextBoxColumn();
            colItens = new DataGridViewTextBoxColumn();
            colStatus = new DataGridViewTextBoxColumn();
            guna2Panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridProdutos).BeginInit();
            SuspendLayout();
            // 
            // lblTituloPedidos
            // 
            lblTituloPedidos.BackColor = Color.Transparent;
            lblTituloPedidos.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTituloPedidos.ForeColor = SystemColors.ControlLightLight;
            lblTituloPedidos.Location = new Point(15, 18);
            lblTituloPedidos.Name = "lblTituloPedidos";
            lblTituloPedidos.Size = new Size(106, 34);
            lblTituloPedidos.TabIndex = 8;
            lblTituloPedidos.Text = "PEDIDOS";
            // 
            // lblDescricaoPedidos
            // 
            lblDescricaoPedidos.BackColor = Color.Transparent;
            lblDescricaoPedidos.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblDescricaoPedidos.ForeColor = SystemColors.ActiveBorder;
            lblDescricaoPedidos.Location = new Point(16, 54);
            lblDescricaoPedidos.Name = "lblDescricaoPedidos";
            lblDescricaoPedidos.Size = new Size(205, 19);
            lblDescricaoPedidos.TabIndex = 9;
            lblDescricaoPedidos.Text = "Acompanhe as operações da loja";
            // 
            // btnAtualizarPedidos
            // 
            btnAtualizarPedidos.BorderRadius = 5;
            btnAtualizarPedidos.Cursor = Cursors.Hand;
            btnAtualizarPedidos.CustomizableEdges = customizableEdges9;
            btnAtualizarPedidos.DisabledState.BorderColor = Color.DarkGray;
            btnAtualizarPedidos.DisabledState.CustomBorderColor = Color.DarkGray;
            btnAtualizarPedidos.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnAtualizarPedidos.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnAtualizarPedidos.FillColor = Color.GreenYellow;
            btnAtualizarPedidos.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAtualizarPedidos.ForeColor = Color.Black;
            btnAtualizarPedidos.Location = new Point(512, 21);
            btnAtualizarPedidos.Name = "btnAtualizarPedidos";
            btnAtualizarPedidos.ShadowDecoration.CustomizableEdges = customizableEdges10;
            btnAtualizarPedidos.Size = new Size(148, 45);
            btnAtualizarPedidos.TabIndex = 10;
            btnAtualizarPedidos.Text = "🔃 ATUALIZAR";
            btnAtualizarPedidos.Click += btnAtualizarPedidos_Click;
            // 
            // guna2Panel1
            // 
            guna2Panel1.BackColor = Color.DimGray;
            guna2Panel1.Controls.Add(btnAtualizarStatusPedido);
            guna2Panel1.Controls.Add(cboStatusPedido);
            guna2Panel1.Controls.Add(lblNovoStatus);
            guna2Panel1.CustomizableEdges = customizableEdges15;
            guna2Panel1.Location = new Point(15, 79);
            guna2Panel1.Name = "guna2Panel1";
            guna2Panel1.ShadowDecoration.CustomizableEdges = customizableEdges16;
            guna2Panel1.Size = new Size(645, 64);
            guna2Panel1.TabIndex = 11;
            // 
            // btnAtualizarStatusPedido
            // 
            btnAtualizarStatusPedido.BorderColor = Color.GreenYellow;
            btnAtualizarStatusPedido.BorderRadius = 5;
            btnAtualizarStatusPedido.BorderThickness = 1;
            btnAtualizarStatusPedido.Cursor = Cursors.Hand;
            btnAtualizarStatusPedido.CustomBorderColor = Color.GreenYellow;
            btnAtualizarStatusPedido.CustomizableEdges = customizableEdges11;
            btnAtualizarStatusPedido.DisabledState.BorderColor = Color.DarkGray;
            btnAtualizarStatusPedido.DisabledState.CustomBorderColor = Color.DarkGray;
            btnAtualizarStatusPedido.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnAtualizarStatusPedido.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnAtualizarStatusPedido.FillColor = Color.FromArgb(64, 64, 64);
            btnAtualizarStatusPedido.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnAtualizarStatusPedido.ForeColor = Color.GreenYellow;
            btnAtualizarStatusPedido.Location = new Point(319, 13);
            btnAtualizarStatusPedido.Name = "btnAtualizarStatusPedido";
            btnAtualizarStatusPedido.ShadowDecoration.CustomizableEdges = customizableEdges12;
            btnAtualizarStatusPedido.Size = new Size(157, 36);
            btnAtualizarStatusPedido.TabIndex = 12;
            btnAtualizarStatusPedido.Text = "🔃 ATUALIZAR STATUS";
            btnAtualizarStatusPedido.Click += btnAtualizarStatusPedido_Click;
            // 
            // cboStatusPedido
            // 
            cboStatusPedido.BackColor = SystemColors.ControlDarkDark;
            cboStatusPedido.CustomizableEdges = customizableEdges13;
            cboStatusPedido.DrawMode = DrawMode.OwnerDrawFixed;
            cboStatusPedido.DropDownStyle = ComboBoxStyle.DropDownList;
            cboStatusPedido.FillColor = Color.FromArgb(64, 64, 64);
            cboStatusPedido.FocusedColor = Color.FromArgb(94, 148, 255);
            cboStatusPedido.FocusedState.BorderColor = Color.FromArgb(94, 148, 255);
            cboStatusPedido.Font = new Font("Segoe UI", 10F);
            cboStatusPedido.ForeColor = Color.LightGray;
            cboStatusPedido.ItemHeight = 30;
            cboStatusPedido.Location = new Point(112, 13);
            cboStatusPedido.Name = "cboStatusPedido";
            cboStatusPedido.ShadowDecoration.CustomizableEdges = customizableEdges14;
            cboStatusPedido.Size = new Size(192, 36);
            cboStatusPedido.TabIndex = 14;
            // 
            // lblNovoStatus
            // 
            lblNovoStatus.BackColor = Color.Transparent;
            lblNovoStatus.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblNovoStatus.ForeColor = Color.WhiteSmoke;
            lblNovoStatus.Location = new Point(17, 21);
            lblNovoStatus.Name = "lblNovoStatus";
            lblNovoStatus.Size = new Size(89, 17);
            lblNovoStatus.TabIndex = 0;
            lblNovoStatus.Text = "NOVO STATUS:";
            // 
            // gridProdutos
            // 
            dataGridViewCellStyle4.BackColor = Color.White;
            gridProdutos.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle4;
            gridProdutos.BackgroundColor = Color.DimGray;
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = Color.DimGray;
            dataGridViewCellStyle5.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            dataGridViewCellStyle5.ForeColor = Color.White;
            dataGridViewCellStyle5.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle5.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle5.WrapMode = DataGridViewTriState.True;
            gridProdutos.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle5;
            gridProdutos.ColumnHeadersHeight = 17;
            gridProdutos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.EnableResizing;
            gridProdutos.Columns.AddRange(new DataGridViewColumn[] { colId, colCliente, colData, colTotal, colItens, colStatus });
            dataGridViewCellStyle6.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle6.BackColor = Color.White;
            dataGridViewCellStyle6.Font = new Font("Segoe UI", 9F);
            dataGridViewCellStyle6.ForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle6.SelectionBackColor = Color.FromArgb(231, 229, 255);
            dataGridViewCellStyle6.SelectionForeColor = Color.FromArgb(71, 69, 94);
            dataGridViewCellStyle6.WrapMode = DataGridViewTriState.False;
            gridProdutos.DefaultCellStyle = dataGridViewCellStyle6;
            gridProdutos.GridColor = Color.Silver;
            gridProdutos.Location = new Point(15, 159);
            gridProdutos.MultiSelect = false;
            gridProdutos.Name = "gridProdutos";
            gridProdutos.RowHeadersVisible = false;
            gridProdutos.Size = new Size(645, 295);
            gridProdutos.TabIndex = 12;
            gridProdutos.ThemeStyle.AlternatingRowsStyle.BackColor = Color.White;
            gridProdutos.ThemeStyle.BackColor = Color.DimGray;
            gridProdutos.ThemeStyle.GridColor = Color.Silver;
            gridProdutos.ThemeStyle.HeaderStyle.BackColor = Color.DimGray;
            gridProdutos.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            gridProdutos.ThemeStyle.HeaderStyle.Height = 17;
            gridProdutos.ThemeStyle.RowsStyle.Font = new Font("Segoe UI", 9F);
            gridProdutos.ThemeStyle.RowsStyle.Height = 25;
            gridProdutos.SelectionChanged += gridProdutos_SelectionChanged;
            gridProdutos.CellDoubleClick += gridProdutos_CellDoubleClick;
            // 
            // colId
            // 
            colId.HeaderText = "ID";
            colId.Name = "colId";
            colId.ReadOnly = true;
            // 
            // colCliente
            // 
            colCliente.HeaderText = "Cliente (UserId)";
            colCliente.Name = "colCliente";
            colCliente.ReadOnly = true;
            // 
            // colData
            // 
            colData.HeaderText = "Data";
            colData.Name = "colData";
            colData.ReadOnly = true;
            // 
            // colTotal
            // 
            colTotal.HeaderText = "Total";
            colTotal.Name = "colTotal";
            colTotal.ReadOnly = true;
            // 
            // colItens
            // 
            colItens.HeaderText = "Itens";
            colItens.Name = "colItens";
            colItens.ReadOnly = true;
            // 
            // colStatus
            // 
            colStatus.HeaderText = "Status";
            colStatus.Name = "colStatus";
            colStatus.ReadOnly = true;
            // 
            // PedidosUserControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(64, 64, 64);
            Controls.Add(gridProdutos);
            Controls.Add(guna2Panel1);
            Controls.Add(btnAtualizarPedidos);
            Controls.Add(lblDescricaoPedidos);
            Controls.Add(lblTituloPedidos);
            Name = "PedidosUserControl";
            Size = new Size(677, 474);
            Load += PedidosUserControl_Load;
            guna2Panel1.ResumeLayout(false);
            guna2Panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)gridProdutos).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2HtmlLabel lblTituloPedidos;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblDescricaoPedidos;
        private Guna.UI2.WinForms.Guna2Button btnAtualizarPedidos;
        private Guna.UI2.WinForms.Guna2Panel guna2Panel1;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblNovoStatus;
        private Guna.UI2.WinForms.Guna2Button btnAtualizarStatusPedido;
        private Guna.UI2.WinForms.Guna2ComboBox cboStatusPedido;
        private Guna.UI2.WinForms.Guna2DataGridView gridProdutos;
        private DataGridViewTextBoxColumn colId;
        private DataGridViewTextBoxColumn colCliente;
        private DataGridViewTextBoxColumn colData;
        private DataGridViewTextBoxColumn colTotal;
        private DataGridViewTextBoxColumn colItens;
        private DataGridViewTextBoxColumn colStatus;
    }
}