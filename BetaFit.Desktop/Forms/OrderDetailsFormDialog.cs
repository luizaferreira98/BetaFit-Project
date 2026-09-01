// =============================================================================
// BetaFit.Desktop - Forms/OrderDetailsDialog.cs
// =============================================================================
// Popup de detalhes do pedido (substitui o antigo MessageBox.Show usado no
// duplo clique do gridPedidos em PedidosUserControl).
//
// Segue a identidade visual BetaFit (BetaFitTheme): preto + lima, bordas
// retas, tipografia forte em caixa alta. Construído 100% em código (sem
// Designer.cs) para ficar em um único arquivo, fácil de plugar no projeto.
//
// Uso (em PedidosUserControl.cs, no lugar do MessageBox):
//   using var dialog = new OrderDetailsDialog(pedido);
//   dialog.ShowDialog(this.FindForm());
// =============================================================================

using BetaFit.Desktop.DTOs;
using BetaFit.Desktop.Themes;
using Guna.UI2.WinForms;
using System;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;

namespace BetaFit.Desktop.Forms
{
    public partial class OrderDetailsFormDialog : Form
    {
        private readonly OrderResponseDto _pedido;

        // Construtor "de design" — não deve ser usado em produção, mas
        // evita quebrar caso algum designer tente instanciar sem parâmetros.
        public OrderDetailsFormDialog() : this(new OrderResponseDto()) { }

        public OrderDetailsFormDialog(OrderResponseDto pedido)
        {
            _pedido = pedido;
            InitializeComponent();
            MontarConteudo();
        }

        //=================================================
        // MONTA TODO O CONTEÚDO DO POPUP A PARTIR DO PEDIDO
        //=================================================
        private void MontarConteudo()
        {
            lblTitulo.Text = $"PEDIDO #{_pedido.Id}";
            lblCliente.Text = $"Cliente: {_pedido.UserName}";
            lblData.Text = _pedido.CreatedAt.ToString("dd/MM/yyyy 'às' HH:mm");

            // Badge de status reaproveitando as mesmas cores do grid de Pedidos
            var (fundo, texto) = BetaFitTheme.CorStatusPedido(_pedido.Status);
            lblStatusBadge.Text = FormatarStatus(_pedido.Status);
            lblStatusBadge.ForeColor = texto;
            pnlStatusBadge.BackColor = fundo;
            pnlStatusBadge.Width = lblStatusBadge.PreferredSize.Width + 16;

            pnlItens.Controls.Clear();

            if (_pedido.Items == null || _pedido.Items.Count == 0)
            {
                var lblVazio = new Guna2HtmlLabel
                {
                    Text = "Este pedido não possui itens.",
                    ForeColor = BetaFitTheme.TextoMuted,
                    Font = BetaFitTheme.FonteNormal,
                    AutoSize = false,
                    Width = pnlItens.Width - 24,
                    Height = 40,
                    Margin = new Padding(4, 12, 4, 4),
                };
                pnlItens.Controls.Add(lblVazio);
            }
            else
            {
                foreach (var item in _pedido.Items)
                {
                    pnlItens.Controls.Add(CriarLinhaItem(item));
                }
            }

            lblQndItens.Text = $"{_pedido.Items?.Sum(i => i.Quantity) ?? 0} ITEM(NS)";
            lblTotalValor.Text = _pedido.Total.ToString("C");
        }

        //=================================================
        // CRIA UMA "LINHA CARD" PARA UM ITEM DO PEDIDO
        //=================================================
        private Guna2Panel CriarLinhaItem(OrderItemResponseDto item)
        {
            var linha = new Guna2Panel
            {
                Width = pnlItens.Width - 24,
                Height = 52,
                BackColor = BetaFitTheme.Superficie,
                BorderRadius = 2,
                BorderThickness = 1,
                BorderColor = BetaFitTheme.Linha,
                Margin = new Padding(0, 0, 0, 8),
            };

            var lblQtd = new Guna2HtmlLabel
            {
                Text = $"{item.Quantity}x",
                Font = BetaFitTheme.FonteSubtitulo,
                ForeColor = BetaFitTheme.LimaEscuro,
                Location = new Point(12, 8),
                AutoSize = true,
            };

            var lblNome = new Guna2HtmlLabel
            {
                Text = item.ProductName,
                Font = BetaFitTheme.FonteNormal,
                ForeColor = BetaFitTheme.Tinta,
                Location = new Point(56, 4),
                AutoSize = false,
                Size = new Size(linha.Width - 200, 18),
            };

            var lblUnit = new Guna2HtmlLabel
            {
                Text = $"{item.UnitPrice:C} / un.",
                Font = BetaFitTheme.FontePequena,
                ForeColor = BetaFitTheme.TextoMuted,
                Location = new Point(56, 24),
                AutoSize = true,
            };

            var lblSubtotal = new Guna2HtmlLabel
            {
                Text = item.Subtotal.ToString("C"),
                Font = BetaFitTheme.FonteSubtitulo,
                ForeColor = BetaFitTheme.Tinta,
                AutoSize = true,
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
            };
            lblSubtotal.Location = new Point(linha.Width - lblSubtotal.PreferredSize.Width - 12, 16);

            linha.Controls.Add(lblQtd);
            linha.Controls.Add(lblNome);
            linha.Controls.Add(lblUnit);
            linha.Controls.Add(lblSubtotal);

            return linha;
        }

        //=================================================
        // "EmPreparacao" -> "EM PREPARAÇÃO" (mais legível no badge)
        //=================================================
        private static string FormatarStatus(string status) => status switch
        {
            "Pendente" => "PENDENTE",
            "EmPreparacao" => "EM PREPARAÇÃO",
            "Pronto" => "PRONTO",
            "Entregue" => "ENTREGUE",
            "Cancelado" => "CANCELADO",
            _ => status.ToUpperInvariant()
        };

        //=================================================
        // FECHAR
        //=================================================
        private void btnFechar_Click(object? sender, EventArgs e) => Close();


    }
}
