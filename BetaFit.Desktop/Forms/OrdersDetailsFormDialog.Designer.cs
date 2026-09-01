using BetaFit.Desktop.Themes;
using Guna.UI2.WinForms;

namespace BetaFit.Desktop.Forms
{
    partial class OrdersDetailsFormDialog
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
            SuspendLayout();

            // ================= FORM =================
            Text = "Detalhes do Pedido";
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterParent;
            Size = new Size(460, 560);
            BackColor = BetaFitTheme.Branco;
            ShowInTaskbar = false;

            // ================= HEADER (preto) =================
            pnlHeader = new Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 84,
                BackColor = BetaFitTheme.PretoPrimario,
            };

            lblTitulo = new Guna2HtmlLabel
            {
                Text = "PEDIDO #0",
                Font = BetaFitTheme.FonteGrande,
                ForeColor = BetaFitTheme.Branco,
                Location = new Point(20, 14),
                AutoSize = true,
            };

            lblStatusBadge = new Guna2HtmlLabel
            {
                Text = "STATUS",
                Font = BetaFitTheme.FonteRotulo,
                Location = new Point(8, 3),
                AutoSize = true,
                BackColor = Color.Transparent,
                ForeColor = BetaFitTheme.PretoPrimario,
            };

            pnlStatusBadge = new Guna2Panel
            {
                Location = new Point(20, 50),
                Height = 22,
                Width = 90,
                BorderRadius = 2,
                BackColor = BetaFitTheme.Lima,
            };
            pnlStatusBadge.Controls.Add(lblStatusBadge);

            btnFechar = new Guna2CircleButton
            {
                Text = "✕",
                Size = new Size(32, 32),
                Location = new Point(410, 14),
                FillColor = BetaFitTheme.PretoSecundario,
                ForeColor = BetaFitTheme.Branco,
                Font = new Font("Segoe UI", 10f, FontStyle.Bold),
                Cursor = Cursors.Hand,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
            };
            btnFechar.Click += btnFechar_Click;

            pnlHeader.Controls.Add(lblTitulo);
            pnlHeader.Controls.Add(pnlStatusBadge);
            pnlHeader.Controls.Add(btnFechar);

            // ================= INFO (cliente / data) =================
            var pnlInfo = new Guna2Panel
            {
                Dock = DockStyle.Top,
                Height = 44,
                BackColor = BetaFitTheme.Superficie,
            };
            lblCliente = new Guna2HtmlLabel
            {
                Text = "Cliente: —",
                Font = BetaFitTheme.FonteNormal,
                ForeColor = BetaFitTheme.Tinta,
                Location = new Point(20, 6),
                AutoSize = true,
            };
            lblData = new Guna2HtmlLabel
            {
                Text = "—",
                Font = BetaFitTheme.FontePequena,
                ForeColor = BetaFitTheme.TextoMuted,
                Location = new Point(20, 24),
                AutoSize = true,
            };
            pnlInfo.Controls.Add(lblCliente);
            pnlInfo.Controls.Add(lblData);

            // ================= LISTA DE ITENS (scrollável) =================
            pnlItensWrapper = new Guna2Panel
            {
                Dock = DockStyle.Fill,
                BackColor = BetaFitTheme.Branco,
                Padding = new Padding(12, 12, 12, 12),
            };

            pnlItens = new FlowLayoutPanel
            {
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.TopDown,
                WrapContents = false,
                AutoScroll = true,
                BackColor = BetaFitTheme.Branco,
            };
            pnlItensWrapper.Controls.Add(pnlItens);

            // ================= FOOTER (total, em lima sobre preto) =================
            pnlFooter = new Guna2Panel
            {
                Dock = DockStyle.Bottom,
                Height = 76,
                BackColor = BetaFitTheme.PretoPrimario,
            };

            lblQtdItens = new Guna2HtmlLabel
            {
                Text = "0 ITEM(NS)",
                Font = BetaFitTheme.FonteRotulo,
                ForeColor = BetaFitTheme.TextoMuted,
                Location = new Point(20, 14),
                AutoSize = true,
            };

            lblTotalLabel = new Guna2HtmlLabel
            {
                Text = "TOTAL",
                Font = BetaFitTheme.FonteRotulo,
                ForeColor = BetaFitTheme.Branco,
                Location = new Point(20, 38),
                AutoSize = true,
            };

            lblTotalValor = new Guna2HtmlLabel
            {
                Text = "R$ 0,00",
                Font = BetaFitTheme.FonteGrande,
                ForeColor = BetaFitTheme.Lima,
                Location = new Point(100, 30),
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
            };

            pnlFooter.Controls.Add(lblQtdItens);
            pnlFooter.Controls.Add(lblTotalLabel);
            pnlFooter.Controls.Add(lblTotalValor);

            // ================= MONTAGEM FINAL =================
            Controls.Add(pnlItensWrapper);
            Controls.Add(pnlInfo);
            Controls.Add(pnlFooter);
            Controls.Add(pnlHeader);

            


            ResumeLayout(false);
        }

        #endregion

        private Guna2Panel pnlHeader = null!;
        private Guna2HtmlLabel lblTitulo = null!;
        private Guna2Panel pnlStatusBadge = null!;
        private Guna2HtmlLabel lblStatusBadge = null!;
        private Guna2CircleButton btnFechar = null!;
        private Guna2HtmlLabel lblCliente = null!;
        private Guna2HtmlLabel lblData = null!;
        private Guna2Panel pnlItensWrapper = null!;
        private FlowLayoutPanel pnlItens = null!;
        private Guna2Panel pnlFooter = null!;
        private Guna2HtmlLabel lblQtdItens = null!;
        private Guna2HtmlLabel lblTotalLabel = null!;
        private Guna2HtmlLabel lblTotalValor = null!;


    }
}
