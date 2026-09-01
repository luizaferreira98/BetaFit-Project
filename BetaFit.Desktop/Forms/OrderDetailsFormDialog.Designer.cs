namespace BetaFit.Desktop.Forms
{
    partial class OrderDetailsFormDialog
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
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges10 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges11 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges7 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges8 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            Guna.UI2.WinForms.Suite.CustomizableEdges customizableEdges9 = new Guna.UI2.WinForms.Suite.CustomizableEdges();
            pnlItensWrapper = new Guna.UI2.WinForms.Guna2Panel();
            pnlItens = new FlowLayoutPanel();
            pnlInfo = new Guna.UI2.WinForms.Guna2Panel();
            lblData = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblCliente = new Guna.UI2.WinForms.Guna2HtmlLabel();
            pnlFooter = new Guna.UI2.WinForms.Guna2Panel();
            lblTotalValor = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblTotalLabel = new Guna.UI2.WinForms.Guna2HtmlLabel();
            lblQndItens = new Guna.UI2.WinForms.Guna2HtmlLabel();
            pnlHeader = new Guna.UI2.WinForms.Guna2Panel();
            btnFechar = new Guna.UI2.WinForms.Guna2CircleButton();
            lblTitulo = new Guna.UI2.WinForms.Guna2HtmlLabel();
            pnlStatusBadge = new Guna.UI2.WinForms.Guna2Panel();
            lblStatusBadge = new Guna.UI2.WinForms.Guna2HtmlLabel();
            pnlItensWrapper.SuspendLayout();
            pnlInfo.SuspendLayout();
            pnlFooter.SuspendLayout();
            pnlHeader.SuspendLayout();
            pnlStatusBadge.SuspendLayout();
            SuspendLayout();
            // 
            // pnlItensWrapper
            // 
            pnlItensWrapper.Controls.Add(pnlItens);
            pnlItensWrapper.CustomizableEdges = customizableEdges1;
            pnlItensWrapper.Dock = DockStyle.Fill;
            pnlItensWrapper.Location = new Point(0, 128);
            pnlItensWrapper.Name = "pnlItensWrapper";
            pnlItensWrapper.Padding = new Padding(12);
            pnlItensWrapper.ShadowDecoration.CustomizableEdges = customizableEdges2;
            pnlItensWrapper.Size = new Size(460, 356);
            pnlItensWrapper.TabIndex = 0;
            // 
            // pnlItens
            // 
            pnlItens.AutoScroll = true;
            pnlItens.Dock = DockStyle.Fill;
            pnlItens.FlowDirection = FlowDirection.TopDown;
            pnlItens.Location = new Point(12, 12);
            pnlItens.Name = "pnlItens";
            pnlItens.Size = new Size(436, 332);
            pnlItens.TabIndex = 0;
            pnlItens.WrapContents = false;
            // 
            // pnlInfo
            // 
            pnlInfo.BackColor = Color.FromArgb(246, 246, 243);
            pnlInfo.Controls.Add(lblData);
            pnlInfo.Controls.Add(lblCliente);
            pnlInfo.CustomizableEdges = customizableEdges3;
            pnlInfo.Dock = DockStyle.Top;
            pnlInfo.Location = new Point(0, 84);
            pnlInfo.Name = "pnlInfo";
            pnlInfo.ShadowDecoration.CustomizableEdges = customizableEdges4;
            pnlInfo.Size = new Size(460, 44);
            pnlInfo.TabIndex = 1;
            // 
            // lblData
            // 
            lblData.BackColor = Color.Transparent;
            lblData.Font = new Font("Segoe UI", 8.25F, FontStyle.Regular, GraphicsUnit.Point, 0);
            lblData.ForeColor = Color.FromArgb(111, 112, 108);
            lblData.Location = new Point(20, 20);
            lblData.Name = "lblData";
            lblData.Size = new Size(14, 15);
            lblData.TabIndex = 1;
            lblData.Text = "—";
            // 
            // lblCliente
            // 
            lblCliente.BackColor = Color.Transparent;
            lblCliente.ForeColor = Color.FromArgb(17, 17, 17);
            lblCliente.Location = new Point(20, 6);
            lblCliente.Name = "lblCliente";
            lblCliente.Size = new Size(40, 17);
            lblCliente.TabIndex = 0;
            lblCliente.Text = "Cliente";
            
            // 
            // pnlFooter
            // 
            pnlFooter.BackColor = Color.FromArgb(11, 11, 11);
            pnlFooter.Controls.Add(lblTotalValor);
            pnlFooter.Controls.Add(lblTotalLabel);
            pnlFooter.Controls.Add(lblQndItens);
            pnlFooter.CustomizableEdges = customizableEdges5;
            pnlFooter.Dock = DockStyle.Bottom;
            pnlFooter.Location = new Point(0, 484);
            pnlFooter.Name = "pnlFooter";
            pnlFooter.ShadowDecoration.CustomizableEdges = customizableEdges6;
            pnlFooter.Size = new Size(460, 76);
            pnlFooter.TabIndex = 2;
            // 
            // lblTotalValor
            // 
            lblTotalValor.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblTotalValor.BackColor = Color.Transparent;
            lblTotalValor.Font = new Font("Segoe UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotalValor.ForeColor = Color.FromArgb(201, 255, 34);
            lblTotalValor.Location = new Point(100, 30);
            lblTotalValor.Name = "lblTotalValor";
            lblTotalValor.Size = new Size(99, 39);
            lblTotalValor.TabIndex = 2;
            lblTotalValor.Text = "R$ 0,00";
            // 
            // lblTotalLabel
            // 
            lblTotalLabel.BackColor = Color.Transparent;
            lblTotalLabel.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTotalLabel.ForeColor = Color.White;
            lblTotalLabel.Location = new Point(20, 38);
            lblTotalLabel.Name = "lblTotalLabel";
            lblTotalLabel.Size = new Size(37, 15);
            lblTotalLabel.TabIndex = 1;
            lblTotalLabel.Text = "TOTAL";
            // 
            // lblQndItens
            // 
            lblQndItens.BackColor = Color.Transparent;
            lblQndItens.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblQndItens.ForeColor = Color.FromArgb(111, 112, 108);
            lblQndItens.Location = new Point(20, 14);
            lblQndItens.Name = "lblQndItens";
            lblQndItens.Size = new Size(61, 15);
            lblQndItens.TabIndex = 0;
            lblQndItens.Text = "0 ITEM(NS)";
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(11, 11, 11);
            pnlHeader.Controls.Add(btnFechar);
            pnlHeader.Controls.Add(lblTitulo);
            pnlHeader.Controls.Add(pnlStatusBadge);
            pnlHeader.CustomizableEdges = customizableEdges10;
            pnlHeader.Dock = DockStyle.Top;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.ShadowDecoration.CustomizableEdges = customizableEdges11;
            pnlHeader.Size = new Size(460, 84);
            pnlHeader.TabIndex = 1;
            // 
            // btnFechar
            // 
            btnFechar.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnFechar.Cursor = Cursors.Hand;
            btnFechar.DisabledState.BorderColor = Color.DarkGray;
            btnFechar.DisabledState.CustomBorderColor = Color.DarkGray;
            btnFechar.DisabledState.FillColor = Color.FromArgb(169, 169, 169);
            btnFechar.DisabledState.ForeColor = Color.FromArgb(141, 141, 141);
            btnFechar.FillColor = Color.FromArgb(22, 22, 22);
            btnFechar.Font = new Font("Segoe UI", 9.75F, FontStyle.Bold, GraphicsUnit.Point, 0);
            btnFechar.ForeColor = Color.White;
            btnFechar.Location = new Point(410, 14);
            btnFechar.Name = "btnFechar";
            btnFechar.ShadowDecoration.CustomizableEdges = customizableEdges7;
            btnFechar.ShadowDecoration.Mode = Guna.UI2.WinForms.Enums.ShadowMode.Circle;
            btnFechar.Size = new Size(32, 32);
            btnFechar.TabIndex = 2;
            btnFechar.Text = "X";
            btnFechar.Click += btnFechar_Click;
            // 
            // lblTitulo
            // 
            lblTitulo.BackColor = Color.Transparent;
            lblTitulo.Font = new Font("Segoe UI", 20.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblTitulo.ForeColor = Color.White;
            lblTitulo.Location = new Point(20, 14);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(142, 39);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "PEDIDO #0";
            // 
            // pnlStatusBadge
            // 
            pnlStatusBadge.BackColor = Color.FromArgb(201, 255, 34);
            pnlStatusBadge.BorderRadius = 2;
            pnlStatusBadge.Controls.Add(lblStatusBadge);
            pnlStatusBadge.CustomizableEdges = customizableEdges8;
            pnlStatusBadge.Location = new Point(20, 50);
            pnlStatusBadge.Name = "pnlStatusBadge";
            pnlStatusBadge.ShadowDecoration.CustomizableEdges = customizableEdges9;
            pnlStatusBadge.Size = new Size(90, 22);
            pnlStatusBadge.TabIndex = 1;
            // 
            // lblStatusBadge
            // 
            lblStatusBadge.BackColor = Color.Transparent;
            lblStatusBadge.Font = new Font("Segoe UI", 8.25F, FontStyle.Bold, GraphicsUnit.Point, 0);
            lblStatusBadge.ForeColor = Color.Black;
            lblStatusBadge.Location = new Point(8, 3);
            lblStatusBadge.Name = "lblStatusBadge";
            lblStatusBadge.Size = new Size(43, 15);
            lblStatusBadge.TabIndex = 0;
            lblStatusBadge.Text = "STATUS";
            // 
            // OrderDetailsFormDialog
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(460, 560);
            Controls.Add(pnlItensWrapper);
            Controls.Add(pnlInfo);
            Controls.Add(pnlFooter);
            Controls.Add(pnlHeader);
            FormBorderStyle = FormBorderStyle.None;
            Name = "OrderDetailsFormDialog";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterParent;
            Text = "Detalhes do Pedido";
            pnlItensWrapper.ResumeLayout(false);
            pnlInfo.ResumeLayout(false);
            pnlInfo.PerformLayout();
            pnlFooter.ResumeLayout(false);
            pnlFooter.PerformLayout();
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            pnlStatusBadge.ResumeLayout(false);
            pnlStatusBadge.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Guna.UI2.WinForms.Guna2Panel pnlItensWrapper;
        private FlowLayoutPanel pnlItens;
        private Guna.UI2.WinForms.Guna2Panel pnlInfo;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblCliente;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblData;
        private Guna.UI2.WinForms.Guna2Panel pnlFooter;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblQndItens;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTotalLabel;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTotalValor;
        private Guna.UI2.WinForms.Guna2Panel pnlHeader;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblTitulo;
        private Guna.UI2.WinForms.Guna2Panel pnlStatusBadge;
        private Guna.UI2.WinForms.Guna2HtmlLabel lblStatusBadge;
        private Guna.UI2.WinForms.Guna2CircleButton btnFechar;
    }
}