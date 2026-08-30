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
                // Atualiza os labels com os contadores
                lblValorCategorias.Text = categorias.Count.ToString();
                lblValorProdutos.Text = produtos.Count.ToString();
                lblValorClientes.Text = usuarios.Count.ToString();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados do dashboard: {ex.Message}",
                "Erro",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            }
        }
    }
}
