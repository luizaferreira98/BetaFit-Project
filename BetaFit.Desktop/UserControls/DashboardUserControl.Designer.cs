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
            components = new System.ComponentModel.Container();
            pnlHero = new Panel();
            lblHeroEyebrow = new Label();
            lblHeroTitulo = new Label();
            lblHeroDescricao = new Label();
            pnlHeroMarca = new Panel();
            lblHeroMarca = new Label();
            pnlConteudo = new Panel();
            flpMetricas = new FlowLayoutPanel();
            pnlListaCabecalho = new Panel();
            lblListaEyebrow = new Label();
            lblListaTitulo = new Label();
            lblListaQuantidade = new Label();
            pnlProdutos = new Panel();
            pnlEstado = new Panel();
            lblEstado = new Label();

            cardProdutos = CriarCard(out lblProdutosRotulo, out lblProdutosValor, out lblProdutosDetalhe, "01", "PRODUTOS", "TOTAL CADASTRADO");
            cardCategorias = CriarCard(out lblCategoriasRotulo, out lblCategoriasValor, out lblCategoriasDetalhe, "02", "CATEGORIAS", "DISPONÍVEIS");
            cardDestaque = CriarCard(out lblDestaqueRotulo, out lblDestaqueValor, out lblDestaqueDetalhe, "03", "EM DESTAQUE", "NA VITRINE");
            cardAtivos = CriarCard(out lblAtivosRotulo, out lblAtivosValor, out lblAtivosDetalhe, "04", "ATIVOS", "NO CATÁLOGO");

            pnlHero.SuspendLayout();
            pnlHeroMarca.SuspendLayout();
            pnlConteudo.SuspendLayout();
            flpMetricas.SuspendLayout();
            pnlListaCabecalho.SuspendLayout();
            pnlEstado.SuspendLayout();
            SuspendLayout();

            // HERO — linguagem visual da BetaFit UI
            pnlHero.BackColor = BetaFitTheme.PretoPrimario;
            pnlHero.Dock = DockStyle.Top;
            pnlHero.Height = 158;
            pnlHero.Padding = new Padding(30, 22, 30, 22);
            pnlHero.Controls.Add(lblHeroDescricao);
            pnlHero.Controls.Add(lblHeroTitulo);
            pnlHero.Controls.Add(lblHeroEyebrow);
            pnlHero.Controls.Add(pnlHeroMarca);

            lblHeroEyebrow.AutoSize = true;
            lblHeroEyebrow.Font = BetaFitTheme.FonteRotulo;
            lblHeroEyebrow.ForeColor = BetaFitTheme.Lima;
            lblHeroEyebrow.Location = new Point(30, 20);
            lblHeroEyebrow.Text = "BETAFIT / ADMIN";

            lblHeroTitulo.AutoSize = true;
            lblHeroTitulo.Font = new Font(BetaFitTheme.FonteBase, 29F, FontStyle.Bold);
            lblHeroTitulo.ForeColor = BetaFitTheme.Branco;
            lblHeroTitulo.Location = new Point(27, 48);
            lblHeroTitulo.Text = "VISÃO GERAL";

            lblHeroDescricao.AutoSize = true;
            lblHeroDescricao.Font = BetaFitTheme.FonteMedia;
            lblHeroDescricao.ForeColor = Color.FromArgb(160, 160, 155);
            lblHeroDescricao.Location = new Point(30, 93);
            lblHeroDescricao.Text = "ACOMPANHE O CATÁLOGO, OS PRODUTOS E O DESEMPENHO DA LOJA.";

            pnlHeroMarca.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            pnlHeroMarca.BackColor = BetaFitTheme.Lima;
            pnlHeroMarca.Location = new Point(0, 24);
            pnlHeroMarca.Size = new Size(150, 92);
            pnlHeroMarca.Controls.Add(lblHeroMarca);

            lblHeroMarca.Dock = DockStyle.Fill;
            lblHeroMarca.Font = new Font(BetaFitTheme.FonteBase, 19F, FontStyle.Bold);
            lblHeroMarca.ForeColor = BetaFitTheme.PretoPrimario;
            lblHeroMarca.Text = "BETA\nFIT";
            lblHeroMarca.TextAlign = ContentAlignment.MiddleCenter;


            // CONTEÚDO
            pnlConteudo.AutoScroll = true;
            pnlConteudo.BackColor = BetaFitTheme.Superficie;
            pnlConteudo.Dock = DockStyle.Fill;
            pnlConteudo.Padding = new Padding(30, 26, 30, 30);
            pnlConteudo.Controls.Add(pnlProdutos);
            pnlConteudo.Controls.Add(pnlListaCabecalho);
            pnlConteudo.Controls.Add(flpMetricas);

            // MÉTRICAS
            flpMetricas.Dock = DockStyle.Top;
            flpMetricas.Height = 145;
            flpMetricas.WrapContents = false;
            flpMetricas.FlowDirection = FlowDirection.LeftToRight;
            flpMetricas.Margin = new Padding(0);
            flpMetricas.Padding = new Padding(0);
            flpMetricas.BackColor = BetaFitTheme.Superficie;
            flpMetricas.Controls.Add(cardProdutos);
            flpMetricas.Controls.Add(cardCategorias);
            flpMetricas.Controls.Add(cardDestaque);
            flpMetricas.Controls.Add(cardAtivos);

            ConfigurarCard(cardProdutos, true);
            ConfigurarCard(cardCategorias, false);
            ConfigurarCard(cardDestaque, false);
            ConfigurarCard(cardAtivos, false);

            // CABEÇALHO DOS PRODUTOS
            pnlListaCabecalho.Dock = DockStyle.Top;
            pnlListaCabecalho.Height = 88;
            pnlListaCabecalho.BackColor = BetaFitTheme.Superficie;
            pnlListaCabecalho.Controls.Add(lblListaQuantidade);
            pnlListaCabecalho.Controls.Add(lblListaTitulo);
            pnlListaCabecalho.Controls.Add(lblListaEyebrow);

            lblListaEyebrow.AutoSize = true;
            lblListaEyebrow.Font = BetaFitTheme.FonteRotulo;
            lblListaEyebrow.ForeColor = BetaFitTheme.LimaEscuro;
            lblListaEyebrow.Location = new Point(0, 24);
            lblListaEyebrow.Text = "CATÁLOGO";

            lblListaTitulo.AutoSize = true;
            lblListaTitulo.Font = new Font(BetaFitTheme.FonteBase, 21F, FontStyle.Bold);
            lblListaTitulo.ForeColor = BetaFitTheme.Tinta;
            lblListaTitulo.Location = new Point(0, 44);
            lblListaTitulo.Text = "PRODUTOS RECENTES";

            lblListaQuantidade.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblListaQuantidade.AutoSize = true;
            lblListaQuantidade.Font = BetaFitTheme.FonteRotulo;
            lblListaQuantidade.ForeColor = BetaFitTheme.TextoMuted;
            lblListaQuantidade.Location = new Point(0, 47);
            lblListaQuantidade.Text = "05 ITENS";

            // LISTA — sem editar/excluir, somente visualização
            pnlProdutos.Dock = DockStyle.Top;
            pnlProdutos.Height = 500;
            pnlProdutos.Padding = new Padding(0, 34, 0, 0);
            pnlProdutos.BackColor = BetaFitTheme.Branco;
            pnlProdutos.BorderStyle = BorderStyle.None;
            pnlProdutos.Paint += PnlProdutos_Paint;

            // ESTADO / ERRO
            pnlEstado.BackColor = BetaFitTheme.Branco;
            pnlEstado.Dock = DockStyle.Fill;
            pnlEstado.Visible = false;
            pnlEstado.Padding = new Padding(30);
            pnlEstado.Controls.Add(lblEstado);

            lblEstado.Dock = DockStyle.Fill;
            lblEstado.Font = BetaFitTheme.FonteSubtitulo;
            lblEstado.ForeColor = BetaFitTheme.TextoMuted;
            lblEstado.TextAlign = ContentAlignment.MiddleCenter;

            // USER CONTROL
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = BetaFitTheme.Superficie;
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
            flpMetricas.ResumeLayout(false);
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
