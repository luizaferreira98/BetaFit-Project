namespace BetaFit.Desktop.Forms
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges11 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges12 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges13 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges14 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges15 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges16 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            lblAdmin = new Label();
            pnlLogo = new Panel();
            lblSidebarLogo = new Label();
            lblSidebarSub = new Label();
            pnlSidebar = new Panel();
            btnDashboard = new Guna.UI2.WinForms.Guna2Button();
            btnProdutos = new Guna.UI2.WinForms.Guna2Button();
            btnCategorias = new Guna.UI2.WinForms.Guna2Button();
            btnUsuarios = new Guna.UI2.WinForms.Guna2Button();
            pnlHeader = new Panel();
            lblTituloApp = new Label();
            pnlConteudo = new Panel();
            pnlLogo.SuspendLayout();
            pnlSidebar.SuspendLayout();
            pnlHeader.SuspendLayout();
            SuspendLayout();
            // 
            // lblAdmin
            // 
            lblAdmin.AutoSize = true;
            lblAdmin.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblAdmin.Location = new Point(12, 30);
            lblAdmin.Name = "lblAdmin";
            lblAdmin.Size = new Size(98, 25);
            lblAdmin.TabIndex = 0;
            lblAdmin.Text = "👨‍💼 Admin";
            // 
            // pnlLogo
            // 
            pnlLogo.Controls.Add(lblSidebarSub);
            pnlLogo.Controls.Add(lblSidebarLogo);
            pnlLogo.Location = new Point(2, 95);
            pnlLogo.Name = "pnlLogo";
            pnlLogo.Size = new Size(200, 60);
            pnlLogo.TabIndex = 2;
            // 
            // lblSidebarLogo
            // 
            lblSidebarLogo.AutoSize = true;
            lblSidebarLogo.Font = new Font("Segoe UI", 14.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblSidebarLogo.Location = new Point(11, 6);
            lblSidebarLogo.Name = "lblSidebarLogo";
            lblSidebarLogo.Size = new Size(78, 25);
            lblSidebarLogo.TabIndex = 0;
            lblSidebarLogo.Text = "Beta Fit";
            // 
            // lblSidebarSub
            // 
            lblSidebarSub.AutoSize = true;
            lblSidebarSub.Location = new Point(14, 31);
            lblSidebarSub.Name = "lblSidebarSub";
            lblSidebarSub.Size = new Size(111, 15);
            lblSidebarSub.TabIndex = 1;
            lblSidebarSub.Text = "Plataforma Desktop";
            // 
            // pnlSidebar
            // 
            pnlSidebar.Controls.Add(btnUsuarios);
            pnlSidebar.Controls.Add(btnCategorias);
            pnlSidebar.Controls.Add(btnProdutos);
            pnlSidebar.Controls.Add(btnDashboard);
            pnlSidebar.Location = new Point(2, 155);
            pnlSidebar.Name = "pnlSidebar";
            pnlSidebar.Size = new Size(200, 537);
            pnlSidebar.TabIndex = 3;
            // 
            // btnDashboard
            // 
            btnDashboard.CustomizableEdges = customizableEdges9;
            btnDashboard.DisabledState.BorderColor = Color.DarkGray;
            btnDashboard.DisabledState.CustomBorderColor = Color.DarkGray;
            btnDashboard.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnDashboard.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnDashboard.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnDashboard.ForeColor = Color.White;
            btnDashboard.Location = new Point(1, 2);
            btnDashboard.Name = "btnDashboard";
            btnDashboard.ShadowDecoration.CustomizableEdges = customizableEdges10;
            btnDashboard.Size = new Size(200, 45);
            btnDashboard.TabIndex = 0;
            btnDashboard.Text = "Dashboard";
            // 
            // btnProdutos
            // 
            btnProdutos.CustomizableEdges = customizableEdges11;
            btnProdutos.DisabledState.BorderColor = Color.DarkGray;
            btnProdutos.DisabledState.CustomBorderColor = Color.DarkGray;
            btnProdutos.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnProdutos.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnProdutos.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnProdutos.ForeColor = Color.White;
            btnProdutos.Location = new Point(1, 47);
            btnProdutos.Name = "btnProdutos";
            btnProdutos.ShadowDecoration.CustomizableEdges = customizableEdges12;
            btnProdutos.Size = new Size(200, 45);
            btnProdutos.TabIndex = 1;
            btnProdutos.Text = "Produtos";
            // 
            // btnCategorias
            // 
            btnCategorias.CustomizableEdges = customizableEdges13;
            btnCategorias.DisabledState.BorderColor = Color.DarkGray;
            btnCategorias.DisabledState.CustomBorderColor = Color.DarkGray;
            btnCategorias.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnCategorias.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnCategorias.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCategorias.ForeColor = Color.White;
            btnCategorias.Location = new Point(1, 92);
            btnCategorias.Name = "btnCategorias";
            btnCategorias.ShadowDecoration.CustomizableEdges = customizableEdges14;
            btnCategorias.Size = new Size(200, 45);
            btnCategorias.TabIndex = 2;
            btnCategorias.Text = "Categorias";
            // 
            // btnUsuarios
            // 
            btnUsuarios.CustomizableEdges = customizableEdges15;
            btnUsuarios.DisabledState.BorderColor = Color.DarkGray;
            btnUsuarios.DisabledState.CustomBorderColor = Color.DarkGray;
            btnUsuarios.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnUsuarios.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnUsuarios.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnUsuarios.ForeColor = Color.White;
            btnUsuarios.Location = new Point(0, 137);
            btnUsuarios.Name = "btnUsuarios";
            btnUsuarios.ShadowDecoration.CustomizableEdges = customizableEdges16;
            btnUsuarios.Size = new Size(200, 45);
            btnUsuarios.TabIndex = 3;
            btnUsuarios.Text = "Usuários";
            // 
            // pnlHeader
            // 
            pnlHeader.Controls.Add(lblTituloApp);
            pnlHeader.Location = new Point(201, -3);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(816, 100);
            pnlHeader.TabIndex = 4;
            // 
            // lblTituloApp
            // 
            lblTituloApp.AutoSize = true;
            lblTituloApp.Font = new Font("Segoe UI", 15.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTituloApp.Location = new Point(28, 33);
            lblTituloApp.Name = "lblTituloApp";
            lblTituloApp.Size = new Size(88, 30);
            lblTituloApp.TabIndex = 0;
            lblTituloApp.Text = "Beta FIt";
            // 
            // pnlConteudo
            // 
            pnlConteudo.Location = new Point(207, 103);
            pnlConteudo.Name = "pnlConteudo";
            pnlConteudo.Size = new Size(809, 577);
            pnlConteudo.TabIndex = 5;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1023, 692);
            Controls.Add(pnlConteudo);
            Controls.Add(pnlHeader);
            Controls.Add(pnlSidebar);
            Controls.Add(pnlLogo);
            Controls.Add(lblAdmin);
            Name = "MainForm";
            Text = "MainForm";
            pnlLogo.ResumeLayout(false);
            pnlLogo.PerformLayout();
            pnlSidebar.ResumeLayout(false);
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblAdmin;
        private Panel pnlLogo;
        private Label lblSidebarSub;
        private Label lblSidebarLogo;
        private Panel pnlSidebar;
        private Guna.UI2.WinForms.Guna2Button btnDashboard;
        private Guna.UI2.WinForms.Guna2Button btnProdutos;
        private Guna.UI2.WinForms.Guna2Button btnCategorias;
        private Guna.UI2.WinForms.Guna2Button btnUsuarios;
        private Panel pnlHeader;
        private Label lblTituloApp;
        private Panel pnlConteudo;
    }
}