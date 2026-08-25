using Guna.UI2.WinForms;

namespace BetaFit.Desktop.Forms
{
    partial class LoginForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
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
        ///
        ///  ESTRUTURA DA TELA (de fora pra dentro):
        ///
        ///  LoginForm (900x600)
        ///  ├─ pnlEsquerda   (Panel preto, 380px)  → marca/branding
        ///  │   ├─ lblMarca      "BETA FIT"
        ///  │   └─ lblSlogan     "PERFORMANCE COMEÇA AQUI"
        ///  └─ pnlDireita    (Panel branco, 520px) → formulário
        ///      └─ pnlCard   (360x390, centralizado dentro de pnlDireita)
        ///          ├─ lblTitulo         "ENTRAR"
        ///          ├─ lblSubtitulo      "Acesse sua conta..."
        ///          ├─ lblRotuloEmail    "E-MAIL"
        ///          ├─ txtEmail          (Guna2TextBox)
        ///          ├─ lblRotuloSenha    "SENHA"
        ///          ├─ txtSenha          (Guna2TextBox)
        ///          ├─ chkMostrarSenha   (CheckBox)
        ///          ├─ lblErro           (Label vermelho, oculto até dar erro)
        ///          ├─ btnEntrar         (Guna2Button)
        ///          └─ lnkCriarConta     (LinkLabel)
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            pnlEsquerda = new Panel();
            lblSlogan = new Label();
            lblMarca = new Label();
            lnkCriarConta = new LinkLabel();
            pnlDireita = new Panel();
            btnFechar = new Guna2Button();
            pnlCard = new Panel();
            btnEntrar = new Guna2Button();
            lblErro = new Label();
            chkMostrarSenha = new CheckBox();
            txtSenha = new Guna2TextBox();
            lblRotuloSenha = new Label();
            txtEmail = new Guna2TextBox();
            lblRotuloEmail = new Label();
            lblSubtitulo = new Label();
            lblTitulo = new Label();
            pnlEsquerda.SuspendLayout();
            pnlDireita.SuspendLayout();
            pnlCard.SuspendLayout();
            SuspendLayout();
            // 
            // pnlEsquerda
            // 
            pnlEsquerda.BackColor = Color.FromArgb(11, 11, 11);
            pnlEsquerda.Controls.Add(lblSlogan);
            pnlEsquerda.Controls.Add(lblMarca);
            pnlEsquerda.Location = new Point(0, 0);
            pnlEsquerda.Name = "pnlEsquerda";
            pnlEsquerda.Size = new Size(380, 600);
            pnlEsquerda.TabIndex = 0;
            // 
            // lblSlogan
            // 
            lblSlogan.Font = new Font("Segoe UI", 9F);
            lblSlogan.ForeColor = Color.FromArgb(158, 158, 154);
            lblSlogan.Location = new Point(40, 318);
            lblSlogan.Name = "lblSlogan";
            lblSlogan.Size = new Size(300, 40);
            lblSlogan.TabIndex = 1;
            lblSlogan.Text = "PERFORMANCE COMEÇA AQUI";
            lblSlogan.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lblMarca
            // 
            lblMarca.Font = new Font("Segoe UI", 26F, FontStyle.Bold);
            lblMarca.ForeColor = Color.FromArgb(201, 255, 34);
            lblMarca.Location = new Point(40, 254);
            lblMarca.Name = "lblMarca";
            lblMarca.Size = new Size(300, 60);
            lblMarca.TabIndex = 0;
            lblMarca.Text = "BETA FIT";
            lblMarca.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lnkCriarConta
            // 
            lnkCriarConta.ActiveLinkColor = Color.FromArgb(155, 201, 0);
            lnkCriarConta.Font = new Font("Segoe UI", 9F);
            lnkCriarConta.LinkColor = Color.FromArgb(11, 11, 11);
            lnkCriarConta.Location = new Point(-3, 349);
            lnkCriarConta.Name = "lnkCriarConta";
            lnkCriarConta.Size = new Size(360, 24);
            lnkCriarConta.TabIndex = 9;
            lnkCriarConta.TabStop = true;
            lnkCriarConta.Text = "Não tem conta? Criar conta";
            lnkCriarConta.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlDireita
            // 
            pnlDireita.BackColor = Color.White;
            pnlDireita.Controls.Add(btnFechar);
            pnlDireita.Controls.Add(pnlCard);
            pnlDireita.Location = new Point(380, 0);
            pnlDireita.Name = "pnlDireita";
            pnlDireita.Size = new Size(520, 600);
            pnlDireita.TabIndex = 1;
            // 
            // btnFechar
            // 
            btnFechar.BorderRadius = 14;
            btnFechar.Cursor = Cursors.Hand;
            btnFechar.CustomizableEdges = customizableEdges1;
            btnFechar.DisabledState.BorderColor = Color.DarkGray;
            btnFechar.DisabledState.CustomBorderColor = Color.DarkGray;
            btnFechar.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnFechar.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnFechar.FillColor = Color.Transparent;
            btnFechar.Font = new Font("Segoe UI", 9F);
            btnFechar.ForeColor = Color.FromArgb(17, 17, 17);
            btnFechar.HoverState.FillColor = Color.FromArgb(238, 238, 234);
            btnFechar.Location = new Point(480, 14);
            btnFechar.Name = "btnFechar";
            btnFechar.ShadowDecoration.CustomizableEdges = customizableEdges2;
            btnFechar.Size = new Size(28, 28);
            btnFechar.TabIndex = 1;
            btnFechar.Text = "X";
            //btnFechar.Click += btnFechar_Click;
            // 
            // pnlCard
            // 
            pnlCard.BackColor = Color.White;
            pnlCard.Controls.Add(lnkCriarConta);
            pnlCard.Controls.Add(btnEntrar);
            pnlCard.Controls.Add(lblErro);
            pnlCard.Controls.Add(chkMostrarSenha);
            pnlCard.Controls.Add(txtSenha);
            pnlCard.Controls.Add(lblRotuloSenha);
            pnlCard.Controls.Add(txtEmail);
            pnlCard.Controls.Add(lblRotuloEmail);
            pnlCard.Controls.Add(lblSubtitulo);
            pnlCard.Controls.Add(lblTitulo);
            pnlCard.Location = new Point(78, 146);
            pnlCard.Name = "pnlCard";
            pnlCard.Size = new Size(360, 390);
            pnlCard.TabIndex = 0;
            // 
            // btnEntrar
            // 
            btnEntrar.BorderRadius = 3;
            btnEntrar.Cursor = Cursors.Hand;
            btnEntrar.CustomizableEdges = customizableEdges3;
            btnEntrar.DisabledState.FillColor = Color.FromArgb(224, 224, 224);
            btnEntrar.FillColor = Color.FromArgb(201, 255, 34);
            btnEntrar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btnEntrar.ForeColor = Color.FromArgb(11, 11, 11);
            btnEntrar.Location = new Point(0, 300);
            btnEntrar.Name = "btnEntrar";
            btnEntrar.ShadowDecoration.CustomizableEdges = customizableEdges4;
            btnEntrar.Size = new Size(360, 46);
            btnEntrar.TabIndex = 8;
            btnEntrar.Text = "ENTRAR";
            //btnEntrar.Click += btnEntrar_Click;
            // 
            // lblErro
            // 
            lblErro.Font = new Font("Segoe UI", 8F);
            lblErro.ForeColor = Color.FromArgb(184, 58, 52);
            lblErro.Location = new Point(0, 250);
            lblErro.Name = "lblErro";
            lblErro.Size = new Size(360, 40);
            lblErro.TabIndex = 7;
            lblErro.Visible = false;
            // 
            // chkMostrarSenha
            // 
            chkMostrarSenha.Cursor = Cursors.Hand;
            chkMostrarSenha.Font = new Font("Segoe UI", 8F);
            chkMostrarSenha.ForeColor = Color.FromArgb(111, 112, 108);
            chkMostrarSenha.Location = new Point(0, 222);
            chkMostrarSenha.Name = "chkMostrarSenha";
            chkMostrarSenha.Size = new Size(160, 24);
            chkMostrarSenha.TabIndex = 6;
            chkMostrarSenha.Text = "Mostrar senha";
            chkMostrarSenha.UseVisualStyleBackColor = true;
            //chkMostrarSenha.CheckedChanged += chkMostrarSenha_CheckedChanged;
            //// 
            // txtSenha
            // 
            txtSenha.BorderColor = Color.FromArgb(222, 222, 217);
            txtSenha.BorderRadius = 3;
            txtSenha.CustomizableEdges = customizableEdges5;
            txtSenha.DefaultText = "";
            txtSenha.Font = new Font("Segoe UI", 9F);
            txtSenha.ForeColor = Color.FromArgb(11, 11, 11);
            txtSenha.Location = new Point(0, 176);
            txtSenha.Name = "txtSenha";
            txtSenha.PlaceholderForeColor = Color.FromArgb(161, 161, 157);
            txtSenha.PlaceholderText = "•••••••••";
            txtSenha.SelectedText = "";
            txtSenha.ShadowDecoration.CustomizableEdges = customizableEdges6;
            txtSenha.Size = new Size(360, 42);
            txtSenha.TabIndex = 5;
            txtSenha.UseSystemPasswordChar = true;
            // 
            // lblRotuloSenha
            // 
            lblRotuloSenha.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblRotuloSenha.ForeColor = Color.FromArgb(111, 112, 108);
            lblRotuloSenha.Location = new Point(0, 156);
            lblRotuloSenha.Name = "lblRotuloSenha";
            lblRotuloSenha.Size = new Size(200, 18);
            lblRotuloSenha.TabIndex = 4;
            lblRotuloSenha.Text = "SENHA";
            // 
            // txtEmail
            // 
            txtEmail.BorderColor = Color.FromArgb(222, 222, 217);
            txtEmail.BorderRadius = 3;
            txtEmail.CustomizableEdges = customizableEdges7;
            txtEmail.DefaultText = "";
            txtEmail.Font = new Font("Segoe UI", 9F);
            txtEmail.ForeColor = Color.FromArgb(11, 11, 11);
            txtEmail.Location = new Point(0, 100);
            txtEmail.Name = "txtEmail";
            txtEmail.PlaceholderForeColor = Color.FromArgb(161, 161, 157);
            txtEmail.PlaceholderText = "seuemail@betafit.com";
            txtEmail.SelectedText = "";
            txtEmail.ShadowDecoration.CustomizableEdges = customizableEdges8;
            txtEmail.Size = new Size(360, 42);
            txtEmail.TabIndex = 3;
            // 
            // lblRotuloEmail
            // 
            lblRotuloEmail.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblRotuloEmail.ForeColor = Color.FromArgb(111, 112, 108);
            lblRotuloEmail.Location = new Point(0, 80);
            lblRotuloEmail.Name = "lblRotuloEmail";
            lblRotuloEmail.Size = new Size(200, 18);
            lblRotuloEmail.TabIndex = 2;
            lblRotuloEmail.Text = "E-MAIL";
            // 
            // lblSubtitulo
            // 
            lblSubtitulo.Font = new Font("Segoe UI", 9F);
            lblSubtitulo.ForeColor = Color.FromArgb(111, 112, 108);
            lblSubtitulo.Location = new Point(0, 38);
            lblSubtitulo.Name = "lblSubtitulo";
            lblSubtitulo.Size = new Size(360, 24);
            lblSubtitulo.TabIndex = 1;
            lblSubtitulo.Text = "Acesse sua conta beta para continuar";
            // 
            // lblTitulo
            // 
            lblTitulo.Font = new Font("Segoe UI", 20F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(17, 17, 17);
            lblTitulo.Location = new Point(0, 0);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(360, 36);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "ENTRAR";
            // 
            // LoginForm
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(900, 600);
            Controls.Add(pnlDireita);
            Controls.Add(pnlEsquerda);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "BetaFit - Entrar";
            Load += LoginForm_Load;
            pnlEsquerda.ResumeLayout(false);
            pnlDireita.ResumeLayout(false);
            pnlCard.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlEsquerda;
        private Label lblMarca;
        private Label lblSlogan;
        private Panel pnlDireita;
        private Panel pnlCard;
        private Label lblTitulo;
        private Label lblSubtitulo;
        private Label lblRotuloEmail;
        private Guna2TextBox txtEmail;
        private Label lblRotuloSenha;
        private Guna2TextBox txtSenha;
        private CheckBox chkMostrarSenha;
        private Label lblErro;
        private Guna2Button btnEntrar;
        private LinkLabel lnkCriarConta;
        private Guna2Button btnFechar;
    }
}
