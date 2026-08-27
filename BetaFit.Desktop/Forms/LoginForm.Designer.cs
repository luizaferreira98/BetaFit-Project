namespace BetaFit.Desktop.Forms
{
    partial class LoginForm
    {
        /// <summary>
        /// Variável necessária do designer.
        /// </summary>
        private System.ComponentModel.IContainer? components = null;

        /// <summary>
        /// Limpa os recursos que estão sendo usados.
        /// </summary>
        /// <param name="disposing">true se recursos gerenciados devem ser descartados; caso contrário, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Código gerado pelo Designer do Windows Forms

        /// <summary>
        /// Método necessário para suporte ao Designer - não modifique
        /// o conteúdo deste método com o editor de código.
        /// </summary>
        private void InitializeComponent()
        {
            pnlEsquerda = new Panel();
            lblSlogan = new Label();
            lblMarca = new Label();
            pnlDireita = new Panel();
            lblErro = new Label();
            btnEntrar = new Button();
            txtSenha = new TextBox();
            lblSenha = new Label();
            txtEmail = new TextBox();
            lblEmail = new Label();
            lblEntrar = new Label();
            pnlEsquerda.SuspendLayout();
            pnlDireita.SuspendLayout();
            SuspendLayout();
            // 
            // pnlEsquerda
            // 
            pnlEsquerda.BackColor = Color.FromArgb(11, 11, 11);
            pnlEsquerda.Controls.Add(lblSlogan);
            pnlEsquerda.Controls.Add(lblMarca);
            pnlEsquerda.Dock = DockStyle.Left;
            pnlEsquerda.Location = new Point(0, 0);
            pnlEsquerda.Name = "pnlEsquerda";
            pnlEsquerda.Size = new Size(539, 620);
            pnlEsquerda.TabIndex = 0;
            // 
            // lblSlogan
            // 
            lblSlogan.AutoSize = true;
            lblSlogan.Font = new Font("Segoe UI", 13F, FontStyle.Bold);
            lblSlogan.ForeColor = Color.White;
            lblSlogan.Location = new Point(62, 245);
            lblSlogan.Name = "lblSlogan";
            lblSlogan.Size = new Size(236, 50);
            lblSlogan.TabIndex = 1;
            lblSlogan.Text = "PAINEL ADMINISTRATIVO\r\nGERENCIE SUA LOJA.";
            // 
            // lblMarca
            // 
            lblMarca.AutoSize = true;
            lblMarca.Font = new Font("Segoe UI", 34F, FontStyle.Bold);
            lblMarca.ForeColor = Color.FromArgb(201, 255, 34);
            lblMarca.Location = new Point(58, 170);
            lblMarca.Name = "lblMarca";
            lblMarca.Size = new Size(215, 62);
            lblMarca.TabIndex = 0;
            lblMarca.Text = "BETA FIT";
            // 
            // pnlDireita
            // 
            pnlDireita.BackColor = Color.White;
            pnlDireita.Controls.Add(lblErro);
            pnlDireita.Controls.Add(btnEntrar);
            pnlDireita.Controls.Add(txtSenha);
            pnlDireita.Controls.Add(lblSenha);
            pnlDireita.Controls.Add(txtEmail);
            pnlDireita.Controls.Add(lblEmail);
            pnlDireita.Controls.Add(lblEntrar);
            pnlDireita.Dock = DockStyle.Fill;
            pnlDireita.Location = new Point(539, 0);
            pnlDireita.Name = "pnlDireita";
            pnlDireita.Padding = new Padding(55, 105, 55, 80);
            pnlDireita.Size = new Size(441, 620);
            pnlDireita.TabIndex = 1;
            // 
            // lblErro
            // 
            lblErro.AutoSize = true;
            lblErro.ForeColor = Color.FromArgb(184, 58, 52);
            lblErro.Location = new Point(55, 385);
            lblErro.Name = "lblErro";
            lblErro.Size = new Size(0, 15);
            lblErro.TabIndex = 6;
            // 
            // btnEntrar
            // 
            btnEntrar.BackColor = Color.FromArgb(201, 255, 34);
            btnEntrar.FlatStyle = FlatStyle.Flat;
            btnEntrar.Location = new Point(55, 325);
            btnEntrar.Name = "btnEntrar";
            btnEntrar.Size = new Size(300, 45);
            btnEntrar.TabIndex = 5;
            btnEntrar.Text = "ENTRAR";
            btnEntrar.UseVisualStyleBackColor = false;
            btnEntrar.Click += btnEntrar_Click;
            // 
            // txtSenha
            // 
            txtSenha.Location = new Point(54, 272);
            txtSenha.Name = "txtSenha";
            txtSenha.PlaceholderText = "••••••••";
            txtSenha.Size = new Size(300, 23);
            txtSenha.TabIndex = 4;
            txtSenha.UseSystemPasswordChar = true;
            txtSenha.KeyDown += txtSenha_KeyDown;
            // 
            // lblSenha
            // 
            lblSenha.AutoSize = true;
            lblSenha.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblSenha.ForeColor = Color.FromArgb(111, 112, 108);
            lblSenha.Location = new Point(54, 248);
            lblSenha.Name = "lblSenha";
            lblSenha.Size = new Size(44, 13);
            lblSenha.TabIndex = 3;
            lblSenha.Text = "SENHA";
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(54, 204);
            txtEmail.Name = "txtEmail";
            txtEmail.PlaceholderText = "admin@betafit.com";
            txtEmail.Size = new Size(300, 23);
            txtEmail.TabIndex = 2;
            txtEmail.KeyDown += txtEmail_KeyDown;
            // 
            // lblEmail
            // 
            lblEmail.AutoSize = true;
            lblEmail.Font = new Font("Segoe UI", 8F, FontStyle.Bold);
            lblEmail.ForeColor = Color.FromArgb(111, 112, 108);
            lblEmail.Location = new Point(53, 188);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(45, 13);
            lblEmail.TabIndex = 1;
            lblEmail.Text = "E-MAIL";
            // 
            // lblEntrar
            // 
            lblEntrar.AutoSize = true;
            lblEntrar.Font = new Font("Segoe UI", 25F, FontStyle.Bold);
            lblEntrar.ForeColor = Color.FromArgb(17, 17, 17);
            lblEntrar.Location = new Point(54, 105);
            lblEntrar.Name = "lblEntrar";
            lblEntrar.Size = new Size(153, 46);
            lblEntrar.TabIndex = 0;
            lblEntrar.Text = "ENTRAR";
            // 
            // LoginForm
            // 
            AcceptButton = btnEntrar;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(980, 620);
            Controls.Add(pnlDireita);
            Controls.Add(pnlEsquerda);
            FormBorderStyle = FormBorderStyle.None;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "BetaFit";
            Load += LoginForm_Load;
            pnlEsquerda.ResumeLayout(false);
            pnlEsquerda.PerformLayout();
            pnlDireita.ResumeLayout(false);
            pnlDireita.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlEsquerda;
        private Label lblMarca;
        private Label lblSlogan;
        private Panel pnlDireita;
        private Label lblEntrar;
        private Label lblEmail;
        private TextBox txtEmail;
        private Label lblSenha;
        private TextBox txtSenha;
        private Button btnEntrar;
        private Label lblErro;
    }
}
