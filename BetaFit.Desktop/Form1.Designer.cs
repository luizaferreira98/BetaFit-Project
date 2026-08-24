<<<<<<< HEAD
using BetaFit.Desktop.Themes;
namespace BetaFit.Desktop
{
    partial class Form1
    {
        private System.ComponentModel.IContainer? components = null;
        private Panel pnlSidebar;
        private Label lblLogo;
        private Label lblSecao;
        private Label lblDashboard;
        private Panel pnlUsuario;
        private Label lblUsuario;
        private Label lblPerfil;
        private Button btnSair;
        private Panel pnlConteudo;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
                components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            pnlSidebar = new Panel();
            btnSair = new Button();
            pnlUsuario = new Panel();
            lblPerfil = new Label();
            lblUsuario = new Label();
            lblDashboard = new Label();
            lblSecao = new Label();
            lblLogo = new Label();
            pnlConteudo = new Panel();
            guna2BorderlessForm1 = new Guna.UI2.WinForms.Guna2BorderlessForm(components);
            pnlSidebar.SuspendLayout();
            pnlUsuario.SuspendLayout();
            SuspendLayout();
            // 
            // pnlSidebar
            // 
            pnlSidebar.BackColor = Color.FromArgb(11, 11, 11);
            pnlSidebar.Controls.Add(btnSair);
            pnlSidebar.Controls.Add(pnlUsuario);
            pnlSidebar.Controls.Add(lblDashboard);
            pnlSidebar.Controls.Add(lblSecao);
            pnlSidebar.Controls.Add(lblLogo);
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Location = new Point(0, 0);
            pnlSidebar.Name = "pnlSidebar";
            pnlSidebar.Padding = new Padding(20);
            pnlSidebar.Size = new Size(224, 749);
            pnlSidebar.TabIndex = 1;
            // 
            // btnSair
            // 
            btnSair.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            btnSair.FlatAppearance.BorderSize = 0;
            btnSair.FlatStyle = FlatStyle.Flat;
            btnSair.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            btnSair.ForeColor = Color.FromArgb(170, 170, 165);
            btnSair.Location = new Point(20, 1179);
            btnSair.Name = "btnSair";
            btnSair.Size = new Size(184, 34);
            btnSair.TabIndex = 0;
            btnSair.Text = "SAIR";
            btnSair.UseVisualStyleBackColor = false;
            btnSair.Click += btnSair_Click;
            // 
            // pnlUsuario
            // 
            pnlUsuario.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            pnlUsuario.BackColor = Color.FromArgb(21, 21, 21);
            pnlUsuario.Controls.Add(lblPerfil);
            pnlUsuario.Controls.Add(lblUsuario);
            pnlUsuario.Location = new Point(20, 1223);
            pnlUsuario.Name = "pnlUsuario";
            pnlUsuario.Padding = new Padding(12, 10, 12, 8);
            pnlUsuario.Size = new Size(184, 72);
            pnlUsuario.TabIndex = 1;
            // 
            // lblPerfil
            // 
            lblPerfil.AutoSize = true;
            lblPerfil.Font = new Font("Segoe UI", 8F);
            lblPerfil.ForeColor = Color.FromArgb(201, 255, 34);
            lblPerfil.Location = new Point(12, 37);
            lblPerfil.Name = "lblPerfil";
            lblPerfil.Size = new Size(95, 13);
            lblPerfil.TabIndex = 0;
            lblPerfil.Text = "ADMINISTRADOR";
            // 
            // lblUsuario
            // 
            lblUsuario.AutoEllipsis = true;
            lblUsuario.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblUsuario.ForeColor = Color.White;
            lblUsuario.Location = new Point(12, 10);
            lblUsuario.Name = "lblUsuario";
            lblUsuario.Size = new Size(160, 22);
            lblUsuario.TabIndex = 1;
            lblUsuario.Text = "ADMIN";
            // 
            // lblDashboard
            // 
            lblDashboard.BackColor = Color.FromArgb(21, 21, 21);
            lblDashboard.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblDashboard.ForeColor = Color.White;
            lblDashboard.Location = new Point(20, 112);
            lblDashboard.Name = "lblDashboard";
            lblDashboard.Size = new Size(184, 40);
            lblDashboard.TabIndex = 2;
            lblDashboard.Text = "  DASHBOARD";
            lblDashboard.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblSecao
            // 
            lblSecao.AutoSize = true;
            lblSecao.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblSecao.ForeColor = Color.FromArgb(110, 110, 105);
            lblSecao.Location = new Point(22, 86);
            lblSecao.Name = "lblSecao";
            lblSecao.Size = new Size(106, 15);
            lblSecao.TabIndex = 3;
            lblSecao.Text = "ADMINISTRAÇÃO";
            // 
            // lblLogo
            // 
            lblLogo.AutoSize = true;
            lblLogo.Font = new Font("Segoe UI", 24F, FontStyle.Bold);
            lblLogo.ForeColor = Color.FromArgb(201, 255, 34);
            lblLogo.Location = new Point(20, 25);
            lblLogo.Name = "lblLogo";
            lblLogo.Size = new Size(144, 45);
            lblLogo.TabIndex = 4;
            lblLogo.Text = "BETAFIT";
            // 
            // pnlConteudo
            // 
            pnlConteudo.BackColor = Color.FromArgb(246, 246, 243);
            pnlConteudo.Dock = DockStyle.Fill;
            pnlConteudo.Location = new Point(224, 0);
            pnlConteudo.Name = "pnlConteudo";
            pnlConteudo.Size = new Size(1056, 749);
            pnlConteudo.TabIndex = 0;
            // 
            // guna2BorderlessForm1
            // 
            guna2BorderlessForm1.ContainerControl = this;
            guna2BorderlessForm1.DockIndicatorTransparencyValue = 0.6D;
            guna2BorderlessForm1.TransparentWhileDrag = true;
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(246, 246, 243);
            ClientSize = new Size(1280, 749);
            Controls.Add(pnlConteudo);
            Controls.Add(pnlSidebar);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(1050, 650);
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "BetaFit — Administração";
            WindowState = FormWindowState.Normal;
            pnlSidebar.ResumeLayout(false);
            pnlSidebar.PerformLayout();
            pnlUsuario.ResumeLayout(false);
            pnlUsuario.PerformLayout();
            ResumeLayout(false);
        }

        private Guna.UI2.WinForms.Guna2BorderlessForm guna2BorderlessForm1;
=======
﻿namespace BetaFit.Desktop
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Text = "Form1";
        }

        #endregion
>>>>>>> 9aa9898d3f31f5e28e0f9360943b6a83a794de8c
    }
}
