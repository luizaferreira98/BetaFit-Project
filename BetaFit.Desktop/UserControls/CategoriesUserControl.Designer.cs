namespace BetaFit.Desktop.UserControls
{
    partial class CategoriesUserControl
    {
        /// <summary>
        /// Variável necessária do designer.
        /// </summary>
        private System.ComponentModel.IContainer? components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Designer de Componentes

        private void InitializeComponent()
        {
            pnlTop = new Panel();
            btnRefresh = new Button();
            lblSubtitle = new Label();
            lblTitle = new Label();
            pnlToolbar = new Panel();
            btnDelete = new Button();
            btnEdit = new Button();
            btnNew = new Button();
            gridCategories = new DataGridView();
            colId = new DataGridViewTextBoxColumn();
            colName = new DataGridViewTextBoxColumn();
            colProductCount = new DataGridViewTextBoxColumn();
            pnlForm = new Panel();
            btnCancel = new Button();
            btnSave = new Button();
            txtName = new TextBox();
            lblName = new Label();
            lblFormTitle = new Label();
            pnlTop.SuspendLayout();
            pnlToolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridCategories).BeginInit();
            pnlForm.SuspendLayout();
            SuspendLayout();
            //
            // pnlTop
            //
            pnlTop.Controls.Add(btnRefresh);
            pnlTop.Controls.Add(lblSubtitle);
            pnlTop.Controls.Add(lblTitle);
            pnlTop.Dock = DockStyle.Top;
            pnlTop.Location = new Point(0, 0);
            pnlTop.Name = "pnlTop";
            pnlTop.Padding = new Padding(30, 18, 30, 18);
            pnlTop.Size = new Size(900, 105);
            pnlTop.TabIndex = 0;
            //
            // btnRefresh
            //
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Location = new Point(770, 32);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(110, 38);
            btnRefresh.TabIndex = 2;
            btnRefresh.Text = "ATUALIZAR";
            btnRefresh.UseVisualStyleBackColor = true;
            btnRefresh.Click += btnRefresh_Click;
            //
            // lblSubtitle
            //
            lblSubtitle.AutoSize = true;
            lblSubtitle.Font = new Font("Segoe UI", 9F);
            lblSubtitle.Location = new Point(30, 63);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(260, 15);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "ORGANIZE OS PRODUTOS DA LOJA";
            //
            // lblTitle
            //
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitle.Location = new Point(27, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(220, 37);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "CATEGORIAS";
            //
            // pnlToolbar
            //
            pnlToolbar.Controls.Add(btnDelete);
            pnlToolbar.Controls.Add(btnEdit);
            pnlToolbar.Controls.Add(btnNew);
            pnlToolbar.Dock = DockStyle.Top;
            pnlToolbar.Location = new Point(0, 105);
            pnlToolbar.Name = "pnlToolbar";
            pnlToolbar.Padding = new Padding(30, 14, 30, 14);
            pnlToolbar.Size = new Size(900, 70);
            pnlToolbar.TabIndex = 1;
            //
            // btnDelete
            //
            btnDelete.FlatStyle = FlatStyle.Flat;
            btnDelete.Location = new Point(268, 16);
            btnDelete.Name = "btnDelete";
            btnDelete.Size = new Size(120, 40);
            btnDelete.TabIndex = 2;
            btnDelete.Text = "EXCLUIR";
            btnDelete.UseVisualStyleBackColor = true;
            btnDelete.Click += btnDelete_Click;
            //
            // btnEdit
            //
            btnEdit.FlatStyle = FlatStyle.Flat;
            btnEdit.Location = new Point(138, 16);
            btnEdit.Name = "btnEdit";
            btnEdit.Size = new Size(120, 40);
            btnEdit.TabIndex = 1;
            btnEdit.Text = "EDITAR";
            btnEdit.UseVisualStyleBackColor = true;
            btnEdit.Click += btnEdit_Click;
            //
            // btnNew
            //
            btnNew.FlatStyle = FlatStyle.Flat;
            btnNew.Location = new Point(8, 16);
            btnNew.Name = "btnNew";
            btnNew.Size = new Size(120, 40);
            btnNew.TabIndex = 0;
            btnNew.Text = "NOVA CATEGORIA";
            btnNew.UseVisualStyleBackColor = true;
            btnNew.Click += btnNew_Click;
            //
            // gridCategories
            //
            gridCategories.AllowUserToAddRows = false;
            gridCategories.AllowUserToDeleteRows = false;
            gridCategories.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridCategories.Columns.AddRange(new DataGridViewColumn[] { colId, colName, colProductCount });
            gridCategories.Dock = DockStyle.Fill;
            gridCategories.Location = new Point(0, 175);
            gridCategories.Name = "gridCategories";
            gridCategories.ReadOnly = true;
            gridCategories.RowHeadersVisible = false;
            gridCategories.Size = new Size(900, 425);
            gridCategories.TabIndex = 2;
            //
            // colId
            //
            colId.HeaderText = "ID";
            colId.Name = "colId";
            colId.Visible = false;
            //
            // colName
            //
            colName.HeaderText = "NOME DA CATEGORIA";
            colName.Name = "colName";
            colName.Width = 300;
            //
            // colProductCount
            //
            colProductCount.HeaderText = "TOTAL DE PRODUTOS";
            colProductCount.Name = "colProductCount";
            //
            // pnlForm
            //
            pnlForm.Controls.Add(btnCancel);
            pnlForm.Controls.Add(btnSave);
            pnlForm.Controls.Add(txtName);
            pnlForm.Controls.Add(lblName);
            pnlForm.Controls.Add(lblFormTitle);
            pnlForm.Location = new Point(30, 130);
            pnlForm.Name = "pnlForm";
            pnlForm.Padding = new Padding(20);
            pnlForm.Size = new Size(340, 220);
            pnlForm.TabIndex = 3;
            pnlForm.Visible = false;
            //
            // btnCancel
            //
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Location = new Point(174, 160);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(140, 38);
            btnCancel.TabIndex = 4;
            btnCancel.Text = "CANCELAR";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            //
            // btnSave
            //
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Location = new Point(20, 160);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(140, 38);
            btnSave.TabIndex = 3;
            btnSave.Text = "SALVAR";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            //
            // txtName
            //
            txtName.Location = new Point(20, 110);
            txtName.Name = "txtName";
            txtName.PlaceholderText = "Ex: Suplementos, Roupas, Acessórios...";
            txtName.Size = new Size(294, 23);
            txtName.TabIndex = 2;
            //
            // lblName
            //
            lblName.AutoSize = true;
            lblName.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblName.Location = new Point(20, 88);
            lblName.Name = "lblName";
            lblName.Size = new Size(140, 13);
            lblName.TabIndex = 1;
            lblName.Text = "NOME DA CATEGORIA";
            //
            // lblFormTitle
            //
            lblFormTitle.AutoSize = true;
            lblFormTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblFormTitle.Location = new Point(20, 20);
            lblFormTitle.Name = "lblFormTitle";
            lblFormTitle.Size = new Size(180, 25);
            lblFormTitle.TabIndex = 0;
            lblFormTitle.Text = "Nova Categoria";
            //
            // CategoriesUserControl
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlForm);
            Controls.Add(gridCategories);
            Controls.Add(pnlToolbar);
            Controls.Add(pnlTop);
            Name = "CategoriesUserControl";
            Size = new Size(900, 600);
            Load += CategoriesUserControl_Load;
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            pnlToolbar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridCategories).EndInit();
            pnlForm.ResumeLayout(false);
            pnlForm.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlTop;
        private Label lblTitle;
        private Label lblSubtitle;
        private Button btnRefresh;
        private Panel pnlToolbar;
        private Button btnNew;
        private Button btnEdit;
        private Button btnDelete;
        private DataGridView gridCategories;
        private DataGridViewTextBoxColumn colId;
        private DataGridViewTextBoxColumn colName;
        private DataGridViewTextBoxColumn colProductCount;
        private Panel pnlForm;
        private Label lblFormTitle;
        private Label lblName;
        private TextBox txtName;
        private Button btnSave;
        private Button btnCancel;
    }
}
