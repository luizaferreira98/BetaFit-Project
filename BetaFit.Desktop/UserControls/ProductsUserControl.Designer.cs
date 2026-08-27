namespace BetaFit.Desktop.UserControls
{
    partial class ProductsUserControl
    {
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
            gridProducts = new DataGridView();
            colId = new DataGridViewTextBoxColumn();
            colName = new DataGridViewTextBoxColumn();
            colCategory = new DataGridViewTextBoxColumn();
            colPrice = new DataGridViewTextBoxColumn();
            colGender = new DataGridViewTextBoxColumn();
            colFeatured = new DataGridViewTextBoxColumn();
            colActive = new DataGridViewTextBoxColumn();
            pnlForm = new Panel();
            chkActive = new CheckBox();
            chkFeatured = new CheckBox();
            btnCancel = new Button();
            btnSave = new Button();
            cmbGender = new ComboBox();
            lblGender = new Label();
            cmbCategory = new ComboBox();
            lblCategory = new Label();
            txtImageUrl = new TextBox();
            lblImageUrl = new Label();
            txtPrice = new TextBox();
            lblPrice = new Label();
            txtDescription = new TextBox();
            lblDescription = new Label();
            txtName = new TextBox();
            lblName = new Label();
            lblFormTitle = new Label();
            pnlTop.SuspendLayout();
            pnlToolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridProducts).BeginInit();
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
            pnlTop.Size = new Size(1100, 105);
            pnlTop.TabIndex = 0;
            //
            // btnRefresh
            //
            btnRefresh.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnRefresh.FlatStyle = FlatStyle.Flat;
            btnRefresh.Location = new Point(970, 32);
            btnRefresh.Name = "btnRefresh";
            btnRefresh.Size = new Size(110, 38);
            btnRefresh.TabIndex = 1;
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
            lblSubtitle.Text = "GERENCIE O CATÁLOGO DA BETAFIT";
            //
            // lblTitle
            //
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitle.Location = new Point(27, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(190, 37);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "PRODUTOS";
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
            pnlToolbar.Size = new Size(1100, 70);
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
            btnNew.Text = "NOVO PRODUTO";
            btnNew.UseVisualStyleBackColor = true;
            btnNew.Click += btnNew_Click;
            //
            // gridProducts
            //
            gridProducts.AllowUserToAddRows = false;
            gridProducts.AllowUserToDeleteRows = false;
            gridProducts.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridProducts.Columns.AddRange(new DataGridViewColumn[] { colId, colName, colCategory, colPrice, colGender, colFeatured, colActive });
            gridProducts.Dock = DockStyle.Fill;
            gridProducts.Location = new Point(0, 175);
            gridProducts.Name = "gridProducts";
            gridProducts.ReadOnly = true;
            gridProducts.RowHeadersVisible = false;
            gridProducts.Size = new Size(1100, 425);
            gridProducts.TabIndex = 2;
            //
            // colId
            //
            colId.HeaderText = "ID";
            colId.Name = "colId";
            colId.Visible = false;
            //
            // colName
            //
            colName.HeaderText = "PRODUTO";
            colName.Name = "colName";
            colName.Width = 260;
            //
            // colCategory
            //
            colCategory.HeaderText = "CATEGORIA";
            colCategory.Name = "colCategory";
            //
            // colPrice
            //
            colPrice.HeaderText = "PREÇO";
            colPrice.Name = "colPrice";
            //
            // colGender
            //
            colGender.HeaderText = "GÊNERO";
            colGender.Name = "colGender";
            //
            // colFeatured
            //
            colFeatured.HeaderText = "DESTAQUE";
            colFeatured.Name = "colFeatured";
            //
            // colActive
            //
            colActive.HeaderText = "ATIVO";
            colActive.Name = "colActive";
            //
            // pnlForm
            //
            pnlForm.Controls.Add(chkActive);
            pnlForm.Controls.Add(chkFeatured);
            pnlForm.Controls.Add(btnCancel);
            pnlForm.Controls.Add(btnSave);
            pnlForm.Controls.Add(cmbGender);
            pnlForm.Controls.Add(lblGender);
            pnlForm.Controls.Add(cmbCategory);
            pnlForm.Controls.Add(lblCategory);
            pnlForm.Controls.Add(txtImageUrl);
            pnlForm.Controls.Add(lblImageUrl);
            pnlForm.Controls.Add(txtPrice);
            pnlForm.Controls.Add(lblPrice);
            pnlForm.Controls.Add(txtDescription);
            pnlForm.Controls.Add(lblDescription);
            pnlForm.Controls.Add(txtName);
            pnlForm.Controls.Add(lblName);
            pnlForm.Controls.Add(lblFormTitle);
            pnlForm.Location = new Point(30, 130);
            pnlForm.Name = "pnlForm";
            pnlForm.Padding = new Padding(20);
            pnlForm.Size = new Size(420, 590);
            pnlForm.TabIndex = 3;
            pnlForm.Visible = false;
            //
            // chkActive
            //
            chkActive.AutoSize = true;
            chkActive.Location = new Point(220, 495);
            chkActive.Name = "chkActive";
            chkActive.Size = new Size(60, 19);
            chkActive.TabIndex = 9;
            chkActive.Text = "ATIVO";
            chkActive.UseVisualStyleBackColor = true;
            //
            // chkFeatured
            //
            chkFeatured.AutoSize = true;
            chkFeatured.Location = new Point(20, 495);
            chkFeatured.Name = "chkFeatured";
            chkFeatured.Size = new Size(100, 19);
            chkFeatured.TabIndex = 8;
            chkFeatured.Text = "EM DESTAQUE";
            chkFeatured.UseVisualStyleBackColor = true;
            //
            // btnCancel
            //
            btnCancel.FlatStyle = FlatStyle.Flat;
            btnCancel.Location = new Point(214, 535);
            btnCancel.Name = "btnCancel";
            btnCancel.Size = new Size(174, 38);
            btnCancel.TabIndex = 11;
            btnCancel.Text = "CANCELAR";
            btnCancel.UseVisualStyleBackColor = true;
            btnCancel.Click += btnCancel_Click;
            //
            // btnSave
            //
            btnSave.FlatStyle = FlatStyle.Flat;
            btnSave.Location = new Point(20, 535);
            btnSave.Name = "btnSave";
            btnSave.Size = new Size(174, 38);
            btnSave.TabIndex = 10;
            btnSave.Text = "SALVAR";
            btnSave.UseVisualStyleBackColor = true;
            btnSave.Click += btnSave_Click;
            //
            // cmbGender
            //
            cmbGender.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbGender.FormattingEnabled = true;
            cmbGender.Location = new Point(220, 452);
            cmbGender.Name = "cmbGender";
            cmbGender.Size = new Size(174, 23);
            cmbGender.TabIndex = 7;
            //
            // lblGender
            //
            lblGender.AutoSize = true;
            lblGender.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblGender.Location = new Point(220, 430);
            lblGender.Name = "lblGender";
            lblGender.Size = new Size(60, 13);
            lblGender.TabIndex = 6;
            lblGender.Text = "GÊNERO";
            //
            // cmbCategory
            //
            cmbCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbCategory.FormattingEnabled = true;
            cmbCategory.Location = new Point(20, 452);
            cmbCategory.Name = "cmbCategory";
            cmbCategory.Size = new Size(174, 23);
            cmbCategory.TabIndex = 5;
            //
            // lblCategory
            //
            lblCategory.AutoSize = true;
            lblCategory.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblCategory.Location = new Point(20, 430);
            lblCategory.Name = "lblCategory";
            lblCategory.Size = new Size(70, 13);
            lblCategory.TabIndex = 4;
            lblCategory.Text = "CATEGORIA";
            //
            // txtImageUrl
            //
            txtImageUrl.Location = new Point(20, 396);
            txtImageUrl.Name = "txtImageUrl";
            txtImageUrl.PlaceholderText = "https://...";
            txtImageUrl.Size = new Size(374, 23);
            txtImageUrl.TabIndex = 3;
            //
            // lblImageUrl
            //
            lblImageUrl.AutoSize = true;
            lblImageUrl.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblImageUrl.Location = new Point(20, 374);
            lblImageUrl.Name = "lblImageUrl";
            lblImageUrl.Size = new Size(110, 13);
            lblImageUrl.TabIndex = 2;
            lblImageUrl.Text = "URL DA IMAGEM";
            //
            // txtPrice
            //
            txtPrice.Location = new Point(20, 340);
            txtPrice.Name = "txtPrice";
            txtPrice.PlaceholderText = "0,00";
            txtPrice.Size = new Size(174, 23);
            txtPrice.TabIndex = 1;
            //
            // lblPrice
            //
            lblPrice.AutoSize = true;
            lblPrice.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblPrice.Location = new Point(20, 318);
            lblPrice.Name = "lblPrice";
            lblPrice.Size = new Size(45, 13);
            lblPrice.TabIndex = 0;
            lblPrice.Text = "PREÇO";
            //
            // txtDescription
            //
            txtDescription.Location = new Point(20, 200);
            txtDescription.Multiline = true;
            txtDescription.Name = "txtDescription";
            txtDescription.Size = new Size(374, 100);
            txtDescription.TabIndex = 0;
            //
            // lblDescription
            //
            lblDescription.AutoSize = true;
            lblDescription.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblDescription.Location = new Point(20, 178);
            lblDescription.Name = "lblDescription";
            lblDescription.Size = new Size(75, 13);
            lblDescription.TabIndex = 0;
            lblDescription.Text = "DESCRIÇÃO";
            //
            // txtName
            //
            txtName.Location = new Point(20, 140);
            txtName.Name = "txtName";
            txtName.Size = new Size(374, 23);
            txtName.TabIndex = 0;
            //
            // lblName
            //
            lblName.AutoSize = true;
            lblName.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblName.Location = new Point(20, 118);
            lblName.Name = "lblName";
            lblName.Size = new Size(120, 13);
            lblName.TabIndex = 0;
            lblName.Text = "NOME DO PRODUTO";
            //
            // lblFormTitle
            //
            lblFormTitle.AutoSize = true;
            lblFormTitle.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblFormTitle.Location = new Point(20, 20);
            lblFormTitle.Name = "lblFormTitle";
            lblFormTitle.Size = new Size(180, 25);
            lblFormTitle.TabIndex = 0;
            lblFormTitle.Text = "Novo Produto";
            //
            // ProductsUserControl
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(pnlForm);
            Controls.Add(gridProducts);
            Controls.Add(pnlToolbar);
            Controls.Add(pnlTop);
            Name = "ProductsUserControl";
            Size = new Size(1100, 600);
            Load += ProductsUserControl_Load;
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            pnlToolbar.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridProducts).EndInit();
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
        private DataGridView gridProducts;
        private DataGridViewTextBoxColumn colId;
        private DataGridViewTextBoxColumn colName;
        private DataGridViewTextBoxColumn colCategory;
        private DataGridViewTextBoxColumn colPrice;
        private DataGridViewTextBoxColumn colGender;
        private DataGridViewTextBoxColumn colFeatured;
        private DataGridViewTextBoxColumn colActive;
        private Panel pnlForm;
        private Label lblFormTitle;
        private Label lblName;
        private TextBox txtName;
        private Label lblDescription;
        private TextBox txtDescription;
        private Label lblPrice;
        private TextBox txtPrice;
        private Label lblImageUrl;
        private TextBox txtImageUrl;
        private Label lblCategory;
        private ComboBox cmbCategory;
        private Label lblGender;
        private ComboBox cmbGender;
        private CheckBox chkFeatured;
        private CheckBox chkActive;
        private Button btnSave;
        private Button btnCancel;
    }
}
