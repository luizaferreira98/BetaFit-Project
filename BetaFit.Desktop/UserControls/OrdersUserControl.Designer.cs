namespace BetaFit.Desktop.UserControls
{
    partial class OrdersUserControl
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
            btnUpdateStatus = new Button();
            cmbStatus = new ComboBox();
            lblStatus = new Label();
            gridOrders = new DataGridView();
            colId = new DataGridViewTextBoxColumn();
            colUserId = new DataGridViewTextBoxColumn();
            colCreatedAt = new DataGridViewTextBoxColumn();
            colTotal = new DataGridViewTextBoxColumn();
            colStatus = new DataGridViewTextBoxColumn();
            pnlTop.SuspendLayout();
            pnlToolbar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridOrders).BeginInit();
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
            lblSubtitle.Text = "ACOMPANHE AS OPERAÇÕES DA LOJA";
            //
            // lblTitle
            //
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitle.Location = new Point(27, 20);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(180, 37);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "PEDIDOS";
            //
            // pnlToolbar
            //
            pnlToolbar.Controls.Add(btnUpdateStatus);
            pnlToolbar.Controls.Add(cmbStatus);
            pnlToolbar.Controls.Add(lblStatus);
            pnlToolbar.Dock = DockStyle.Top;
            pnlToolbar.Location = new Point(0, 105);
            pnlToolbar.Name = "pnlToolbar";
            pnlToolbar.Padding = new Padding(30, 14, 30, 14);
            pnlToolbar.Size = new Size(900, 70);
            pnlToolbar.TabIndex = 1;
            //
            // btnUpdateStatus
            //
            btnUpdateStatus.FlatStyle = FlatStyle.Flat;
            btnUpdateStatus.Location = new Point(300, 16);
            btnUpdateStatus.Name = "btnUpdateStatus";
            btnUpdateStatus.Size = new Size(190, 40);
            btnUpdateStatus.TabIndex = 2;
            btnUpdateStatus.Text = "ATUALIZAR STATUS";
            btnUpdateStatus.UseVisualStyleBackColor = true;
            btnUpdateStatus.Click += btnUpdateStatus_Click;
            //
            // cmbStatus
            //
            cmbStatus.DropDownStyle = ComboBoxStyle.DropDownList;
            cmbStatus.FormattingEnabled = true;
            cmbStatus.Location = new Point(120, 22);
            cmbStatus.Name = "cmbStatus";
            cmbStatus.Size = new Size(160, 23);
            cmbStatus.TabIndex = 1;
            //
            // lblStatus
            //
            lblStatus.AutoSize = true;
            lblStatus.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblStatus.Location = new Point(8, 26);
            lblStatus.Name = "lblStatus";
            lblStatus.Size = new Size(100, 13);
            lblStatus.TabIndex = 0;
            lblStatus.Text = "NOVO STATUS:";
            //
            // gridOrders
            //
            gridOrders.AllowUserToAddRows = false;
            gridOrders.AllowUserToDeleteRows = false;
            gridOrders.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridOrders.Columns.AddRange(new DataGridViewColumn[] { colId, colUserId, colCreatedAt, colTotal, colStatus });
            gridOrders.Dock = DockStyle.Fill;
            gridOrders.Location = new Point(0, 175);
            gridOrders.Name = "gridOrders";
            gridOrders.ReadOnly = true;
            gridOrders.RowHeadersVisible = false;
            gridOrders.Size = new Size(900, 425);
            gridOrders.TabIndex = 2;
            //
            // colId
            //
            colId.HeaderText = "ID";
            colId.Name = "colId";
            //
            // colUserId
            //
            colUserId.HeaderText = "CLIENTE";
            colUserId.Name = "colUserId";
            colUserId.Width = 220;
            //
            // colCreatedAt
            //
            colCreatedAt.HeaderText = "DATA";
            colCreatedAt.Name = "colCreatedAt";
            //
            // colTotal
            //
            colTotal.HeaderText = "TOTAL";
            colTotal.Name = "colTotal";
            //
            // colStatus
            //
            colStatus.HeaderText = "STATUS";
            colStatus.Name = "colStatus";
            //
            // OrdersUserControl
            //
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            Controls.Add(gridOrders);
            Controls.Add(pnlToolbar);
            Controls.Add(pnlTop);
            Name = "OrdersUserControl";
            Size = new Size(900, 600);
            Load += OrdersUserControl_Load;
            pnlTop.ResumeLayout(false);
            pnlTop.PerformLayout();
            pnlToolbar.ResumeLayout(false);
            pnlToolbar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)gridOrders).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlTop;
        private Label lblTitle;
        private Label lblSubtitle;
        private Button btnRefresh;
        private Panel pnlToolbar;
        private Label lblStatus;
        private ComboBox cmbStatus;
        private Button btnUpdateStatus;
        private DataGridView gridOrders;
        private DataGridViewTextBoxColumn colId;
        private DataGridViewTextBoxColumn colUserId;
        private DataGridViewTextBoxColumn colCreatedAt;
        private DataGridViewTextBoxColumn colTotal;
        private DataGridViewTextBoxColumn colStatus;
    }
}
