using BetaFit.Desktop.Themes;

namespace BetaFit.Desktop.Forms
{
    partial class CategoriesFormDialog
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges1 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges2 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges3 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges4 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges5 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges6 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            lblTituloNovaCategoria = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblNomeCategoria = new Guna.UI2.WinForms.Guna2HtmlLabel();
            txtNomeCategoria = new Guna.UI2.WinForms.Guna2TextBox();
            btnFecharNovaCategoria = new Guna.UI2.WinForms.Guna2CircleButton();
            btnCancelarCategoria = new Guna.UI2.WinForms.Guna2Button();
            btnSalvarCategoria = new Guna.UI2.WinForms.Guna2Button();
            SuspendLayout();
            // 
            // lblTituloNovaCategoria
            // 
            lblTituloNovaCategoria.BackColor = Color.Transparent;
            lblTituloNovaCategoria.Font = new Font("Segoe UI", 18F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTituloNovaCategoria.ForeColor = Color.White;
            lblTituloNovaCategoria.Location = new Point(12, 12);
            lblTituloNovaCategoria.Name = "lblTituloNovaCategoria";
            lblTituloNovaCategoria.Size = new Size(180, 34);
            lblTituloNovaCategoria.TabIndex = 0;
            lblTituloNovaCategoria.Text = "Nova Categoria";
            // 
            // lblNomeCategoria
            // 
            lblNomeCategoria.BackColor = Color.Transparent;
            lblNomeCategoria.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblNomeCategoria.ForeColor = Color.White;
            lblNomeCategoria.Location = new Point(12, 61);
            lblNomeCategoria.Name = "lblNomeCategoria";
            lblNomeCategoria.Size = new Size(127, 17);
            lblNomeCategoria.TabIndex = 1;
            lblNomeCategoria.Text = "NOME DA CATEGORIA";
            // 
            // txtNomeCategoria
            // 
            txtNomeCategoria.BackColor = Color.FromArgb(24, 24, 24);
            txtNomeCategoria.BorderColor = Color.FromArgb(45, 45, 45);
            txtNomeCategoria.BorderRadius = 8;
            txtNomeCategoria.CustomizableEdges = customizableEdges1;
            txtNomeCategoria.DefaultText = "";
            txtNomeCategoria.DisabledState.BorderColor = Color.FromArgb(208, 208, 208);
            txtNomeCategoria.DisabledState.FillColor = Color.FromArgb(226, 226, 226);
            txtNomeCategoria.DisabledState.ForeColor = Color.FromArgb(138, 138, 138);
            txtNomeCategoria.DisabledState.PlaceholderForeColor = Color.FromArgb(138, 138, 138);
            txtNomeCategoria.FillColor = Color.FromArgb(24, 24, 24);
            txtNomeCategoria.FocusedState.BorderColor = Color.FromArgb(198, 255, 40);
            txtNomeCategoria.Font = new Font("Segoe UI", 9F);
            txtNomeCategoria.ForeColor = Color.White;
            txtNomeCategoria.HoverState.BorderColor = Color.FromArgb(198, 255, 40);
            txtNomeCategoria.Location = new Point(12, 85);
            txtNomeCategoria.Name = "txtNomeCategoria";
            txtNomeCategoria.PlaceholderForeColor = Color.FromArgb(140, 140, 140);
            txtNomeCategoria.PlaceholderText = "Ex: Suplementos, Roupas, Acessórios...";
            txtNomeCategoria.SelectedText = "";
            txtNomeCategoria.ShadowDecoration.CustomizableEdges = customizableEdges2;
            txtNomeCategoria.Size = new Size(385, 36);
            txtNomeCategoria.TabIndex = 5;
            // 
            // btnFecharNovaCategoria
            // 
            btnFecharNovaCategoria.Cursor = Cursors.Hand;
            btnFecharNovaCategoria.DisabledState.BorderColor = Color.DarkGray;
            btnFecharNovaCategoria.DisabledState.CustomBorderColor = Color.DarkGray;
            btnFecharNovaCategoria.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnFecharNovaCategoria.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnFecharNovaCategoria.FillColor = Color.Transparent;
            btnFecharNovaCategoria.Font = new Font("Segoe UI", 12F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnFecharNovaCategoria.ForeColor = Color.White;
            btnFecharNovaCategoria.Location = new Point(357, 12);
            btnFecharNovaCategoria.Name = "btnFecharNovaCategoria";
            btnFecharNovaCategoria.ShadowDecoration.CustomizableEdges = customizableEdges3;
            btnFecharNovaCategoria.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            btnFecharNovaCategoria.Size = new Size(40, 28);
            btnFecharNovaCategoria.TabIndex = 6;
            btnFecharNovaCategoria.Text = "X";
            btnFecharNovaCategoria.Click += btnFecharNovaCategoria_Click;
            // 
            // btnCancelarCategoria
            // 
            btnCancelarCategoria.BorderColor = Color.FromArgb(60, 60, 60);
            btnCancelarCategoria.BorderRadius = 8;
            btnCancelarCategoria.BorderThickness = 1;
            btnCancelarCategoria.Cursor = Cursors.Hand;
            btnCancelarCategoria.CustomizableEdges = customizableEdges4;
            btnCancelarCategoria.DisabledState.BorderColor = Color.DarkGray;
            btnCancelarCategoria.DisabledState.CustomBorderColor = Color.DarkGray;
            btnCancelarCategoria.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnCancelarCategoria.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnCancelarCategoria.FillColor = Color.FromArgb(30, 30, 30);
            btnCancelarCategoria.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnCancelarCategoria.ForeColor = Color.White;
            btnCancelarCategoria.Location = new Point(14, 148);
            btnCancelarCategoria.Name = "btnCancelarCategoria";
            btnCancelarCategoria.ShadowDecoration.CustomizableEdges = customizableEdges5;
            btnCancelarCategoria.Size = new Size(161, 45);
            btnCancelarCategoria.TabIndex = 22;
            btnCancelarCategoria.Text = "CANCELAR";
            btnCancelarCategoria.Click += btnCancelarCategoria_Click;
            // 
            // btnSalvarCategoria
            // 
            btnSalvarCategoria.BorderRadius = 8;
            btnSalvarCategoria.Cursor = Cursors.Hand;
            btnSalvarCategoria.CustomizableEdges = customizableEdges6;
            btnSalvarCategoria.DisabledState.BorderColor = Color.DarkGray;
            btnSalvarCategoria.DisabledState.CustomBorderColor = Color.DarkGray;
            btnSalvarCategoria.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnSalvarCategoria.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnSalvarCategoria.FillColor = Color.FromArgb(198, 255, 40);
            btnSalvarCategoria.Font = new Font("Segoe UI", 9F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnSalvarCategoria.ForeColor = Color.Black;
            btnSalvarCategoria.Location = new Point(237, 148);
            btnSalvarCategoria.Name = "btnSalvarCategoria";
            btnSalvarCategoria.ShadowDecoration.CustomizableEdges = customizableEdges7;
            btnSalvarCategoria.Size = new Size(157, 45);
            btnSalvarCategoria.TabIndex = 23;
            btnSalvarCategoria.Text = "💾 SALVAR";
            btnSalvarCategoria.Click += btnSalvarCategoria_Click;
            // 
            // CategoriesFormDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(15, 15, 15);
            ClientSize = new Size(412, 214);
            Controls.Add(btnSalvarCategoria);
            Controls.Add(btnCancelarCategoria);
            Controls.Add(btnFecharNovaCategoria);
            Controls.Add(txtNomeCategoria);
            Controls.Add(lblNomeCategoria);
            Controls.Add(lblTituloNovaCategoria);
            FormBorderStyle = FormBorderStyle.None;
            Name = "CategoriesFormDialog";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "CategoriesFormDialog";
            Load += CategoriesFormDialog_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Guna.UI2.WinForms.Guna2HtmlLabel lblTituloNovaCategoria;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblNomeCategoria;
        private Guna.UI2.WinForms.Guna2TextBox txtNomeCategoria;
        private Guna.UI2.WinForms.Guna2CircleButton btnFecharNovaCategoria;
        private Guna.UI2.WinForms.Guna2Button btnCancelarCategoria;
        private Guna.UI2.WinForms.Guna2Button btnSalvarCategoria;
    }
}