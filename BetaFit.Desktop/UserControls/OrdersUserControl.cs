// =============================================================================
// BetaFit.Desktop - UserControls/OrdersUserControl.cs
// =============================================================================
//  CONCEITO: UserControl de Pedidos
//
// Permite acompanhar os pedidos realizados na loja e atualizar o status:
//   GET /api/orders                Listar
//   PUT /api/orders/{id}/status    Atualizar status
//
// Todo o layout (grid, toolbar de status) é definido em
// OrdersUserControl.Designer.cs. Esta classe cuida apenas dos dados.
// =============================================================================

using BetaFit.Desktop.DTOs;
using BetaFit.Desktop.Services;
using BetaFit.Desktop.Themes;

namespace BetaFit.Desktop.UserControls
{
    public partial class OrdersUserControl : UserControl
    {
        private static readonly string[] StatusDisponiveis =
        {
            "Pendente", "Pago", "Enviado", "Entregue", "Cancelado"
        };

        private readonly OrdersApiService _ordersService = new();
        private List<OrderResponseDto> _orders = new();

        public OrdersUserControl()
        {
            InitializeComponent();
        }

        private async void OrdersUserControl_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;

            pnlTop.BackColor = BetaFitTheme.PretoPrimario;
            lblTitle.ForeColor = Color.White;
            lblSubtitle.ForeColor = BetaFitTheme.TextoMuted;
            BackColor = BetaFitTheme.Superficie;
            pnlToolbar.BackColor = BetaFitTheme.Superficie;

            BetaFitButtons.EstilizarEscuro(btnUpdateStatus);
            BetaFitButtons.EstilizarFantasma(btnRefresh);
            BetaFitInputs.EstilizarComboBox(cmbStatus);
            BetaFitInputs.EstilizarRotulo(lblStatus);

            cmbStatus.Items.AddRange(StatusDisponiveis);
            if (cmbStatus.Items.Count > 0) cmbStatus.SelectedIndex = 0;

            BetaFitTheme.AplicarEstiloGrid(gridOrders);

            await CarregarDadosAsync();
        }

        private async Task CarregarDadosAsync()
        {
            gridOrders.Rows.Clear();
            try
            {
                _orders = await _ordersService.GetAllAsync();
                foreach (var order in _orders)
                {
                    gridOrders.Rows.Add(
                        order.Id,
                        order.UserId,
                        order.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                        order.Total.ToString("C2"),
                        order.Status);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar pedidos: {ex.Message}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }

        private async void btnRefresh_Click(object sender, EventArgs e) => await CarregarDadosAsync();

        private async void btnUpdateStatus_Click(object sender, EventArgs e)
        {
            var pedido = ObterPedidoSelecionado();
            if (pedido == null)
            {
                MessageBox.Show("Selecione um pedido para atualizar o status.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var novoStatus = cmbStatus.SelectedItem?.ToString();
            if (string.IsNullOrWhiteSpace(novoStatus))
            {
                MessageBox.Show("Selecione o novo status.", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var (success, error) = await _ordersService.UpdateStatusAsync(pedido.Id, novoStatus);
            if (success)
            {
                MessageBox.Show("Status atualizado com sucesso!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                await CarregarDadosAsync();
            }
            else
            {
                MessageBox.Show(error, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private OrderResponseDto? ObterPedidoSelecionado()
        {
            if (gridOrders.SelectedRows.Count == 0) return null;
            var id = Convert.ToInt32(gridOrders.SelectedRows[0].Cells[colId.Name].Value);
            return _orders.FirstOrDefault(o => o.Id == id);
        }
    }
}
