using BetaFit.Desktop.Themes;
using System.Drawing.Drawing2D;

namespace BetaFit.Desktop.UserControls
{
    partial class DashboardUserControl
    {
        private System.ComponentModel.IContainer? components = null;
        private Panel pnlHero;
        private Label lblHeroEyebrow;
        private Label lblHeroTitulo;
        private Label lblHeroDescricao;
        private Panel pnlHeroMarca;
        private Label lblHeroMarca;
        private Panel pnlConteudo;
        private FlowLayoutPanel flpMetricas;
        private Panel cardProdutos;
        private Panel cardCategorias;
        private Panel cardDestaque;
        private Panel cardAtivos;
        private Label lblProdutosRotulo;
        private Label lblProdutosValor;
        private Label lblProdutosDetalhe;
        private Label lblCategoriasRotulo;
        private Label lblCategoriasValor;
        private Label lblCategoriasDetalhe;
        private Label lblDestaqueRotulo;
        private Label lblDestaqueValor;
        private Label lblDestaqueDetalhe;
        private Label lblAtivosRotulo;
        private Label lblAtivosValor;
        private Label lblAtivosDetalhe;
        private Panel pnlListaCabecalho;
        private Label lblListaEyebrow;
        private Label lblListaTitulo;
        private Label lblListaQuantidade;
        private Panel pnlProdutos;
        private Panel pnlEstado;
        private Label lblEstado;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null)) components.Dispose();
            base.Dispose(disposing);
        }

        private void InitializeComponent()
        {
            pnlHero = new Panel();
            lblHeroDescricao = new Label();
            lblHeroTitulo = new Label();
            lblHeroEyebrow = new Label();
            pnlHeroMarca = new Panel();
            lblHeroMarca = new Label();
            pnlConteudo = new Panel();
            pnlProdutos = new Panel();
            pnlListaCabecalho = new Panel();
            lblListaQuantidade = new Label();
            lblListaTitulo = new Label();
            lblListaEyebrow = new Label();
            flpMetricas = new FlowLayoutPanel();
            pnlEstado = new Panel();
            lblEstado = new Label();
            pnlHero.SuspendLayout();
            pnlHeroMarca.SuspendLayout();
            pnlConteudo.SuspendLayout();
            pnlListaCabecalho.SuspendLayout();
            pnlEstado.SuspendLayout();
            SuspendLayout();
            // 
            // pnlHero
            // 
            pnlHero.BackColor = Color.FromArgb(11, 11, 11);
            pnlHero.Controls.Add(lblHeroDescricao);
            pnlHero.Controls.Add(lblHeroTitulo);
            pnlHero.Controls.Add(lblHeroEyebrow);
            pnlHero.Controls.Add(pnlHeroMarca);
            pnlHero.Dock = DockStyle.Top;
            pnlHero.Location = new Point(0, 0);
            pnlHero.Name = "pnlHero";
            pnlHero.Padding = new Padding(30, 22, 30, 22);
            pnlHero.Size = new Size(1050, 158);
            pnlHero.TabIndex = 2;
            // 
            // lblHeroDescricao
            // 
            lblHeroDescricao.AutoSize = true;
            lblHeroDescricao.Font = new Font("Segoe UI", 10F);
            lblHeroDescricao.ForeColor = Color.FromArgb(160, 160, 155);
            lblHeroDescricao.Location = new Point(30, 93);
            lblHeroDescricao.Name = "lblHeroDescricao";
            lblHeroDescricao.Size = new Size(466, 19);
            lblHeroDescricao.TabIndex = 0;
            lblHeroDescricao.Text = "ACOMPANHE O CATÁLOGO, OS PRODUTOS E O DESEMPENHO DA LOJA.";
            // 
            // lblHeroTitulo
            // 
            lblHeroTitulo.AutoSize = true;
            lblHeroTitulo.Font = new Font("Segoe UI", 29F, FontStyle.Bold);
            lblHeroTitulo.ForeColor = Color.White;
            lblHeroTitulo.Location = new Point(27, 48);
            lblHeroTitulo.Name = "lblHeroTitulo";
            lblHeroTitulo.Size = new Size(271, 52);
            lblHeroTitulo.TabIndex = 1;
            lblHeroTitulo.Text = "VISÃO GERAL";
            // 
            // lblHeroEyebrow
            // 
            lblHeroEyebrow.AutoSize = true;
            lblHeroEyebrow.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblHeroEyebrow.ForeColor = Color.FromArgb(201, 255, 34);
            lblHeroEyebrow.Location = new Point(30, 20);
            lblHeroEyebrow.Name = "lblHeroEyebrow";
            lblHeroEyebrow.Size = new Size(104, 15);
            lblHeroEyebrow.TabIndex = 2;
            lblHeroEyebrow.Text = "BETAFIT / ADMIN";
            // 
            // pnlHeroMarca
            // 
            pnlHeroMarca.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pnlHeroMarca.BackColor = Color.FromArgb(201, 255, 34);
            pnlHeroMarca.Controls.Add(lblHeroMarca);
            pnlHeroMarca.Location = new Point(850, 24);
            pnlHeroMarca.Name = "pnlHeroMarca";
            pnlHeroMarca.Size = new Size(150, 92);
            pnlHeroMarca.TabIndex = 3;
            // 
            // lblHeroMarca
            // 
            lblHeroMarca.Dock = DockStyle.Fill;
            lblHeroMarca.Font = new Font("Segoe UI", 19F, FontStyle.Bold);
            lblHeroMarca.ForeColor = Color.FromArgb(11, 11, 11);
            lblHeroMarca.Location = new Point(0, 0);
            lblHeroMarca.Name = "lblHeroMarca";
            lblHeroMarca.Size = new Size(150, 92);
            lblHeroMarca.TabIndex = 0;
            lblHeroMarca.Text = "BETA\nFIT";
            lblHeroMarca.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // pnlConteudo
            // 
            pnlConteudo.AutoScroll = true;
            pnlConteudo.BackColor = Color.FromArgb(246, 246, 243);
            pnlConteudo.Controls.Add(pnlProdutos);
            pnlConteudo.Controls.Add(pnlListaCabecalho);
            pnlConteudo.Controls.Add(flpMetricas);
            pnlConteudo.Dock = DockStyle.Fill;
            pnlConteudo.Location = new Point(0, 158);
            pnlConteudo.Name = "pnlConteudo";
            pnlConteudo.Padding = new Padding(30, 26, 30, 30);
            pnlConteudo.Size = new Size(1050, 542);
            pnlConteudo.TabIndex = 1;
            // 
            // pnlProdutos
            // 
            pnlProdutos.BackColor = Color.White;
            pnlProdutos.Dock = DockStyle.Top;
            pnlProdutos.Location = new Point(30, 259);
            pnlProdutos.Name = "pnlProdutos";
            pnlProdutos.Padding = new Padding(0, 34, 0, 0);
            pnlProdutos.Size = new Size(973, 500);
            pnlProdutos.TabIndex = 0;
            pnlProdutos.Paint += PnlProdutos_Paint;
            // 
            // pnlListaCabecalho
            // 
            pnlListaCabecalho.BackColor = Color.FromArgb(246, 246, 243);
            pnlListaCabecalho.Controls.Add(lblListaQuantidade);
            pnlListaCabecalho.Controls.Add(lblListaTitulo);
            pnlListaCabecalho.Controls.Add(lblListaEyebrow);
            pnlListaCabecalho.Dock = DockStyle.Top;
            pnlListaCabecalho.Location = new Point(30, 171);
            pnlListaCabecalho.Name = "pnlListaCabecalho";
            pnlListaCabecalho.Size = new Size(973, 88);
            pnlListaCabecalho.TabIndex = 1;
            // 
            // lblListaQuantidade
            // 
            lblListaQuantidade.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblListaQuantidade.AutoSize = true;
            lblListaQuantidade.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblListaQuantidade.ForeColor = Color.FromArgb(111, 112, 108);
            lblListaQuantidade.Location = new Point(773, 47);
            lblListaQuantidade.Name = "lblListaQuantidade";
            lblListaQuantidade.Size = new Size(57, 15);
            lblListaQuantidade.TabIndex = 0;
            lblListaQuantidade.Text = "05 ITENS";
            // 
            // lblListaTitulo
            // 
            lblListaTitulo.AutoSize = true;
            lblListaTitulo.Font = new Font("Segoe UI", 21F, FontStyle.Bold);
            lblListaTitulo.ForeColor = Color.FromArgb(17, 17, 17);
            lblListaTitulo.Location = new Point(0, 44);
            lblListaTitulo.Name = "lblListaTitulo";
            lblListaTitulo.Size = new Size(308, 38);
            lblListaTitulo.TabIndex = 1;
            lblListaTitulo.Text = "PRODUTOS RECENTES";
            // 
            // lblListaEyebrow
            // 
            lblListaEyebrow.AutoSize = true;
            lblListaEyebrow.Font = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            lblListaEyebrow.ForeColor = Color.FromArgb(155, 201, 0);
            lblListaEyebrow.Location = new Point(0, 24);
            lblListaEyebrow.Name = "lblListaEyebrow";
            lblListaEyebrow.Size = new Size(68, 15);
            lblListaEyebrow.TabIndex = 2;
            lblListaEyebrow.Text = "CATÁLOGO";
            // 
            // flpMetricas
            // 
            flpMetricas.BackColor = Color.FromArgb(246, 246, 243);
            flpMetricas.Dock = DockStyle.Top;
            flpMetricas.Location = new Point(30, 26);
            flpMetricas.Margin = new Padding(0);
            flpMetricas.Name = "flpMetricas";
            flpMetricas.Size = new Size(973, 145);
            flpMetricas.TabIndex = 2;
            flpMetricas.WrapContents = false;
            // 
            // pnlEstado
            // 
            pnlEstado.BackColor = Color.White;
            pnlEstado.Controls.Add(lblEstado);
            pnlEstado.Dock = DockStyle.Fill;
            pnlEstado.Location = new Point(0, 158);
            pnlEstado.Name = "pnlEstado";
            pnlEstado.Padding = new Padding(30);
            pnlEstado.Size = new Size(1050, 542);
            pnlEstado.TabIndex = 0;
            pnlEstado.Visible = false;
            // 
            // lblEstado
            // 
            lblEstado.Dock = DockStyle.Fill;
            lblEstado.Font = new Font("Segoe UI", 11F, FontStyle.Bold);
            lblEstado.ForeColor = Color.FromArgb(111, 112, 108);
            lblEstado.Location = new Point(30, 30);
            lblEstado.Name = "lblEstado";
            lblEstado.Size = new Size(990, 482);
            lblEstado.TabIndex = 0;
            lblEstado.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // DashboardUserControl
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.FromArgb(246, 246, 243);
            Controls.Add(pnlEstado);
            Controls.Add(pnlConteudo);
            Controls.Add(pnlHero);
            Name = "DashboardUserControl";
            Size = new Size(1050, 700);
            Resize += DashboardUserControl_Resize;
            pnlHero.ResumeLayout(false);
            pnlHero.PerformLayout();
            pnlHeroMarca.ResumeLayout(false);
            pnlConteudo.ResumeLayout(false);
            pnlListaCabecalho.ResumeLayout(false);
            pnlListaCabecalho.PerformLayout();
            pnlEstado.ResumeLayout(false);
            ResumeLayout(false);
        }

        private static Panel CriarCard(out Label rotulo, out Label valor, out Label detalhe, string numero, string titulo, string textoDetalhe)
        {
            var card = new Panel();
            rotulo = new Label();
            valor = new Label();
            detalhe = new Label();
            var marcador = new Label();

            marcador.Name = "lblNumeroCard";
            marcador.Text = numero;
            marcador.Font = BetaFitTheme.FontePequena;
            marcador.AutoSize = true;
            marcador.Location = new Point(16, 14);

            rotulo.AutoSize = true;
            rotulo.Font = BetaFitTheme.FonteRotulo;
            rotulo.Location = new Point(16, 35);
            rotulo.Text = titulo;

            valor.AutoSize = true;
            valor.Font = new Font(BetaFitTheme.FonteBase, 34F, FontStyle.Bold);
            valor.Location = new Point(14, 58);
            valor.Text = "00";

            detalhe.AutoSize = true;
            detalhe.Font = BetaFitTheme.FontePequena;
            detalhe.Location = new Point(17, 114);
            detalhe.Text = textoDetalhe;

            card.Controls.Add(detalhe);
            card.Controls.Add(valor);
            card.Controls.Add(rotulo);
            card.Controls.Add(marcador);
            return card;
        }

        private static void ConfigurarCard(Panel card, bool destaque)
        {
            card.Width = 250;
            card.Height = 128;
            card.Margin = new Padding(0, 0, 14, 0);
            card.Padding = new Padding(0);
            card.BorderStyle = BorderStyle.None;
            card.BackColor = destaque ? BetaFitTheme.PretoPrimario : BetaFitTheme.Branco;
            card.Paint += (_, e) =>
            {
                using var border = new Pen(BetaFitTheme.Linha);
                if (!destaque) e.Graphics.DrawRectangle(border, 0, 0, card.Width - 1, card.Height - 1);
                using var lime = new SolidBrush(BetaFitTheme.Lima);
                e.Graphics.FillRectangle(lime, 0, 0, destaque ? card.Width : 4, 4);
                if (!destaque) e.Graphics.FillRectangle(lime, 0, 0, 4, card.Height);
            };

            foreach (Control control in card.Controls)
            {
                if (control.Name == "lblNumeroCard")
                    control.ForeColor = destaque ? Color.FromArgb(105, 105, 100) : Color.FromArgb(155, 155, 150);
                else if (control == card.Controls[1])
                    control.ForeColor = destaque ? BetaFitTheme.Lima : BetaFitTheme.Tinta;
                else if (control == card.Controls[0])
                    control.ForeColor = destaque ? Color.FromArgb(150, 150, 145) : BetaFitTheme.TextoMuted;
                else
                    control.ForeColor = destaque ? BetaFitTheme.Branco : BetaFitTheme.Tinta;
            }
        }

        private void PnlProdutos_Paint(object? sender, PaintEventArgs e)
        {
            using var pen = new Pen(BetaFitTheme.Linha);
            e.Graphics.DrawRectangle(pen, 0, 0, pnlProdutos.Width - 1, pnlProdutos.Height - 1);

            using var brush = new SolidBrush(BetaFitTheme.PretoPrimario);
            e.Graphics.FillRectangle(brush, 0, 0, pnlProdutos.Width, 34);

            using var font = BetaFitTheme.FontePequena;
            using var textBrush = new SolidBrush(BetaFitTheme.Branco);
            e.Graphics.DrawString("PRODUTO", font, textBrush, 16, 11);
            e.Graphics.DrawString("CATEGORIA / GÊNERO", font, textBrush, 96, 11);
            e.Graphics.DrawString("PREÇO", font, textBrush, Math.Max(600, pnlProdutos.Width - 205), 11);
            e.Graphics.DrawString("STATUS", font, textBrush, Math.Max(700, pnlProdutos.Width - 95), 11);
        }

        private void DashboardUserControl_Resize(object? sender, EventArgs e)
        {
            var largura = Math.Max(185, (flpMetricas.ClientSize.Width - 42) / 4);
            foreach (Control card in flpMetricas.Controls)
                card.Width = largura;

            lblListaQuantidade.Left = pnlListaCabecalho.ClientSize.Width - lblListaQuantidade.Width;
            pnlHeroMarca.Left = pnlHero.ClientSize.Width - pnlHeroMarca.Width - 30;
        }
    }
}
