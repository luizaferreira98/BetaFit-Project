using BetaFit.Desktop.DTOs;
using BetaFit.Desktop.Forms;
using BetaFit.Desktop.Helpers;
using BetaFit.Desktop.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetaFit.Desktop.UserControls
{
    public partial class PedidosUserControl : UserControl
    {
        //=================================================
        // SERVIÇOS (Inicializados no Load)
        //=================================================
        private OrdersApiService _ordersApiService = null!;

        //=================================================
        // DADOS (LISTA DOS PEDIDOS VINDOS DA API)
        //=================================================
        private List<OrderResponseDto> _todosPedidos = new();

        // Precisa bater exatamente com o enum OrderStatus do BetaFit.Domain,
        // pois a API faz Enum.TryParse<OrderStatus>(status, true, ...)
        private static readonly string[] StatusDisponiveis =
        {
            "Pendente", "EmPreparacao", "Pronto", "Entregue", "Cancelado"
        };

        //=================================================
        // CONSTRUTOR
        //=================================================
        public PedidosUserControl()
        {
            InitializeComponent();
        }

        //=================================================
        // LOAD DO USER CONTROL
        //=================================================
        private async void PedidosUserControl_Load(object sender, EventArgs e)
        {
            // Guard: não executa em tempo de Design
            if (DesignMode) return;

            _ordersApiService = new OrdersApiService();

            // Aplica o tema BetaFit ao grid (preto/lima, ver Themes/BetaFitTheme.cs)
            BetaFit.Desktop.Themes.BetaFitTheme.AplicarEstiloGrid(gridPedidos);

            // Colore a coluna Status como badge (Pendente/EmPreparacao/Pronto/Entregue/Cancelado)
            BetaFit.Desktop.Themes.BetaFitTheme.AplicarBadgeStatusNoGrid(gridPedidos, nameof(colStatus));

            cboStatusPedido.Items.Clear();
            cboStatusPedido.Items.AddRange(StatusDisponiveis);

            await CarregarDadosAsync();
        }

        //=================================================
        // CARREGAR DADOS (busca os pedidos salvos no banco via API)
        //=================================================
        private async Task CarregarDadosAsync()
        {
            try
            {
                // GET /api/orders -> a API lê do banco e devolve todos os pedidos
                // (o pedido foi criado lá na UI/checkout; aqui o Desktop só consome)
                _todosPedidos = await _ordersApiService.GetAllAsync();
                PopularGrid(_todosPedidos);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar pedidos: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //=================================================
        // POPULAR GRID COM OS PEDIDOS
        //=================================================
        private void PopularGrid(List<OrderResponseDto> pedidos)
        {
            gridPedidos.Rows.Clear();

            foreach (var pedido in pedidos.OrderByDescending(p => p.CreatedAt))
            {
                gridPedidos.Rows.Add(
                    pedido.Id,
                    pedido.UserName,
                    pedido.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                    pedido.Total.ToString("C"),
                    pedido.Items.Count,
                    pedido.Status
                );
            }
        }

        //=================================================
        // OBTER PEDIDO SELECIONADO (RETORNA O OBJETO SELECIONADO NO GRID)
        //=================================================
        private OrderResponseDto? ObterPedidoSelecionado()
        {
            if (gridPedidos.SelectedRows.Count == 0) return null;
            var row = gridPedidos.SelectedRows[0];
            var id = Convert.ToInt32(row.Cells["colId"].Value);
            return _todosPedidos.FirstOrDefault(p => p.Id == id);
        }

        //=================================================
        // AO SELECIONAR UM PEDIDO, PRÉ-SELECIONA O STATUS ATUAL NO COMBO
        //=================================================
        private void gridPedidos_SelectionChanged(object sender, EventArgs e)
        {
            var pedido = ObterPedidoSelecionado();
            if (pedido == null) return;

            if (cboStatusPedido.Items.Contains(pedido.Status))
                cboStatusPedido.SelectedItem = pedido.Status;
        }

        //=================================================
        // DUPLO CLIQUE NA LINHA -> ABRE O POPUP DE DETALHES DO PEDIDO
        //=================================================
        private void gridPedidos_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            var pedido = ObterPedidoSelecionado();
            if (pedido == null) return;

            using var dialog = new OrderDetailsFormDialog(pedido);
            dialog.ShowDialog(this.FindForm());
        }

        //=================================================
        // ATUALIZAR PEDIDOS (BOTÃO) - RECARREGA DADOS DA API
        //=================================================
        private async void btnAtualizarPedidos_Click(object sender, EventArgs e)
            => await CarregarDadosAsync();

        //=================================================
        // ATUALIZAR STATUS DO PEDIDO SELECIONADO
        //=================================================
        private async void btnAtualizarStatusPedido_Click(object sender, EventArgs e)
        {
            var pedido = ObterPedidoSelecionado();
            if (pedido == null)
            {
                MessageBox.Show("Selecione um pedido na lista.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cboStatusPedido.SelectedItem == null)
            {
                MessageBox.Show("Selecione o novo status.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var novoStatus = cboStatusPedido.SelectedItem.ToString()!;

            var (success, error) = await _ordersApiService.UpdateStatusAsync(pedido.Id, novoStatus);
            if (success)
            {
                MessageBox.Show("✅ Status atualizado com sucesso!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                await CarregarDadosAsync();
            }
            else
            {
                MessageBox.Show($"❌ {error}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
}