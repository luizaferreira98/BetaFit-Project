namespace BetaFit.Desktop.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Variável necessária do designer.
        /// </summary>
        private System.ComponentModel.IContainer? components = null;

        /// <summary>
        /// Limpa os recursos que estão sendo usados.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Designer do Windows Forms

        private void InitializeComponent()
        {
            pnlSidebar = new Panel();
            pnlNav = new FlowLayoutPanel();
            btnDashboard = new Button();
            btnProdutos = new Button();
            btnCategorias = new Button();
            btnPedidos = new Button();
            pnlRodape = new Panel();
            btnSair = new Button();
            lblMarca = new Label();
            pnlConteudo = new Panel();
            pnlSidebar.SuspendLayout();
            pnlNav.SuspendLayout();
            pnlRodape.SuspendLayout();
            SuspendLayout();
            // 
            // pnlSidebar
            // 
            pnlSidebar.BackColor = Color.FromArgb(11, 11, 11);
            pnlSidebar.Controls.Add(pnlNav);
            pnlSidebar.Controls.Add(pnlRodape);
            pnlSidebar.Controls.Add(lblMarca);
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Location = new Point(0, 0);
            pnlSidebar.Name = "pnlSidebar";
            pnlSidebar.Padding = new Padding(16, 22, 16, 16);
            pnlSidebar.Size = new Size(245, 689);
            pnlSidebar.TabIndex = 0;
            // 
            // pnlNav
            // 
            pnlNav.Controls.Add(btnDashboard);
            pnlNav.Controls.Add(btnProdutos);
            pnlNav.Controls.Add(btnCategorias);
            pnlNav.Controls.Add(btnPedidos);
            pnlNav.Dock = DockStyle.Fill;
            pnlNav.FlowDirection = FlowDirection.TopDown;
            pnlNav.Location = new Point(16, 80);
            pnlNav.Name = "pnlNav";
            pnlNav.Padding = new Padding(0, 20, 0, 0);
            pnlNav.Size = new Size(213, 511);
            pnlNav.TabIndex = 1;
            pnlNav.WrapContents = false;
            // 
            // btnDashboard
            // 
            btnDashboard.FlatStyle = FlatStyle.Flat;
            btnDashboard.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnDashboard.ForeColor = Color.White;
            btnDashboard.Location = new Point(0, 20);
            btnDashboard.Margin = new Padding(0, 0, 0, 5);
            btnDashboard.Name = "btnDashboard";
            btnDashboard.Padding = new Padding(14, 0, 0, 0);
            btnDashboard.Size = new Size(211, 46);
            btnDashboard.TabIndex = 0;
            btnDashboard.Text = "VISÃO GERAL";
            btnDashboard.TextAlign = ContentAlignment.MiddleLeft;
            btnDashboard.UseVisualStyleBackColor = false;
            btnDashboard.Click += btnDashboard_Click;
            // 
            // btnProdutos
            // 
            btnProdutos.FlatStyle = FlatStyle.Flat;
            btnProdutos.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnProdutos.ForeColor = Color.White;
            btnProdutos.Location = new Point(0, 71);
            btnProdutos.Margin = new Padding(0, 0, 0, 5);
            btnProdutos.Name = "btnProdutos";
            btnProdutos.Padding = new Padding(14, 0, 0, 0);
            btnProdutos.Size = new Size(211, 46);
            btnProdutos.TabIndex = 1;
            btnProdutos.Text = "PRODUTOS";
            btnProdutos.TextAlign = ContentAlignment.MiddleLeft;
            btnProdutos.UseVisualStyleBackColor = false;
            btnProdutos.Click += btnProdutos_Click;
            // 
            // btnCategorias
            // 
            btnCategorias.FlatStyle = FlatStyle.Flat;
            btnCategorias.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnCategorias.ForeColor = Color.White;
            btnCategorias.Location = new Point(0, 122);
            btnCategorias.Margin = new Padding(0, 0, 0, 5);
            btnCategorias.Name = "btnCategorias";
            btnCategorias.Padding = new Padding(14, 0, 0, 0);
            btnCategorias.Size = new Size(211, 46);
            btnCategorias.TabIndex = 2;
            btnCategorias.Text = "CATEGORIAS";
            btnCategorias.TextAlign = ContentAlignment.MiddleLeft;
            btnCategorias.UseVisualStyleBackColor = false;
            btnCategorias.Click += btnCategorias_Click;
            // 
            // btnPedidos
            // 
            btnPedidos.FlatStyle = FlatStyle.Flat;
            btnPedidos.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnPedidos.ForeColor = Color.White;
            btnPedidos.Location = new Point(0, 173);
            btnPedidos.Margin = new Padding(0, 0, 0, 5);
            btnPedidos.Name = "btnPedidos";
            btnPedidos.Padding = new Padding(14, 0, 0, 0);
            btnPedidos.Size = new Size(211, 46);
            btnPedidos.TabIndex = 3;
            btnPedidos.Text = "PEDIDOS";
            btnPedidos.TextAlign = ContentAlignment.MiddleLeft;
            btnPedidos.UseVisualStyleBackColor = false;
            btnPedidos.Click += btnPedidos_Click;
            // 
            // pnlRodape
            // 
            pnlRodape.Controls.Add(btnSair);
            pnlRodape.Dock = DockStyle.Bottom;
            pnlRodape.Location = new Point(16, 591);
            pnlRodape.Name = "pnlRodape";
            pnlRodape.Size = new Size(213, 82);
            pnlRodape.TabIndex = 2;
            // 
            // btnSair
            // 
            btnSair.Dock = DockStyle.Bottom;
            btnSair.FlatStyle = FlatStyle.Flat;
            btnSair.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            btnSair.ForeColor = Color.White;
            btnSair.Location = new Point(0, 36);
            btnSair.Name = "btnSair";
            btnSair.Padding = new Padding(14, 0, 0, 0);
            btnSair.Size = new Size(213, 46);
            btnSair.TabIndex = 0;
            btnSair.Text = "SAIR";
            btnSair.TextAlign = ContentAlignment.MiddleLeft;
            btnSair.UseVisualStyleBackColor = false;
            btnSair.Click += btnSair_Click;
            // 
            // lblMarca
            // 
            lblMarca.Dock = DockStyle.Top;
            lblMarca.Font = new Font("Segoe UI", 21F, FontStyle.Bold);
            lblMarca.ForeColor = Color.FromArgb(201, 255, 34);
            lblMarca.Location = new Point(16, 22);
            lblMarca.Name = "lblMarca";
            lblMarca.Size = new Size(213, 58);
            lblMarca.TabIndex = 0;
            lblMarca.Text = "BETA FIT";
            lblMarca.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pnlConteudo
            // 
            pnlConteudo.Dock = DockStyle.Fill;
            pnlConteudo.Location = new Point(245, 0);
            pnlConteudo.Name = "pnlConteudo";
            pnlConteudo.Size = new Size(821, 689);
            pnlConteudo.TabIndex = 1;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1050, 650);
            Controls.Add(pnlConteudo);
            Controls.Add(pnlSidebar);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(1066, 689);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "BetaFit Desktop";
            WindowState = FormWindowState.Maximized;
            Load += MainForm_Load;
            pnlSidebar.ResumeLayout(false);
            pnlNav.ResumeLayout(false);
            pnlRodape.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlSidebar;
        private Label lblMarca;
        private FlowLayoutPanel pnlNav;
        private Button btnDashboard;
        private Button btnProdutos;
        private Button btnCategorias;
        private Button btnPedidos;
        private Panel pnlRodape;
        private Button btnSair;
        private Panel pnlConteudo;
    }
}
