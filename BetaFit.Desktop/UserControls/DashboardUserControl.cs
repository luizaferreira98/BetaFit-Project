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
    public partial class DashboardUserControl : UserControl
    {
        // Serviços de API para o Dashboard
        private CategoriesApiService _categoriesApiService = null;
        private ProductsApiService _productsApiService = null!; //Caso nao tenha nada ele nao vai dar erro, por isso o null! (null-forgiving operator)
        private UsersApiService _usersApiService = null;
        private OrdersApiService _ordersApiService = null!;
        private List<BetaFit.Desktop.DTOs.OrderResponseDto> _todosPedidos = new();


        public DashboardUserControl()
        {
            InitializeComponent();
        }

        //Carregamento do UserControl
        private async void DashboardUserControl_Load(object sender, EventArgs e)
        {
            //Guard: não executa em tempo de design
            if (DesignMode) return;

            //Instancia os serviços de API
            _categoriesApiService = new CategoriesApiService();
            _productsApiService = new ProductsApiService();
            _usersApiService = new UsersApiService();
            _ordersApiService = new OrdersApiService();

            // Aplica estilo ao grid de últimos pedidos
            BetaFit.Desktop.Themes.BetaFitTheme.AplicarEstiloGrid(gridUltimosPedidos);
            BetaFit.Desktop.Themes.BetaFitTheme.AplicarBadgeStatusNoGrid(gridUltimosPedidos, nameof(colStatusUP));

            //Carrega os dados do Dashboard
            await CarregarDadosAsync();

        }


        //Carregar dados do usuario
        private async Task CarregarDadosAsync()
        {
            try
            {
                // Carrega dados de categorias, produtos e usuários
                var categorias = await _categoriesApiService.GetAllAsync();
                var produtos = await _productsApiService.GetAllAsync();
                var usuarios = await _usersApiService.GetAllAsync();
                // Carrega pedidos recentes
                _todosPedidos = await _ordersApiService.GetAllAsync();
                // Atualiza os labels com os contadores
                lblValorCategorias.Text = categorias.Count.ToString();
                lblValorProdutos.Text = produtos.Count.ToString();
                lblValorClientes.Text = usuarios.Count.ToString();

                // Atualiza card de FINALIZADOS (apenas pedidos com status "Entregue")
                var finalizados = _todosPedidos.Count(p => string.Equals(p.Status, "Entregue", StringComparison.OrdinalIgnoreCase));
                lblValorFaturamento.Text = finalizados.ToString();

                // Popular grid de últimos pedidos (exibe os 5 mais recentes)
                PopularUltimosPedidos(_todosPedidos);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados do dashboard: {ex.Message}",
                "Erro",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            }
        }

        private void PopularUltimosPedidos(List<BetaFit.Desktop.DTOs.OrderResponseDto> pedidos)
        {
            // Limpa linhas existentes
            gridUltimosPedidos.Rows.Clear();

            foreach (var pedido in pedidos.OrderByDescending(p => p.CreatedAt).Take(5))
            {
                gridUltimosPedidos.Rows.Add(
                    pedido.Id,
                    pedido.UserName,
                    pedido.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                    pedido.Status,
                    pedido.Total.ToString("C")
                );
            }
        }


    }
}
