// =============================================================================
// BetaFit.Desktop - Forms/MainForm.cs
// =============================================================================
//  CONCEITO: Shell principal da aplicação (sidebar + área de conteúdo)
//
// Todo o layout (sidebar, botões de navegação, área de conteúdo) é definido
// em MainForm.Designer.cs. Esta classe cuida apenas da navegação entre os
// módulos (UserControls) e do logout.
// =============================================================================

using BetaFit.Desktop.Helpers;
using BetaFit.Desktop.Services;
using BetaFit.Desktop.Themes;
using BetaFit.Desktop.UserControls;

namespace BetaFit.Desktop.Forms
{
    public partial class MainForm : Form
    {
        private readonly AuthApiService _authService = new();
        private UserControl? _telaAtual;
        private Button? _botaoAtivo;

        public MainForm()
        {
            InitializeComponent();
            Text = $"BetaFit Desktop - {AppConfig.Version}";
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            if (DesignMode) return;

            pnlConteudo.BackColor = BetaFitTheme.Superficie;
            AplicarEstiloBotaoNav(btnDashboard);
            AplicarEstiloBotaoNav(btnProdutos);
            AplicarEstiloBotaoNav(btnCategorias);
            AplicarEstiloBotaoNav(btnPedidos);
            AplicarEstiloBotaoSair(btnSair);

            Navegar(new DashboardUserControl(), btnDashboard);
        }

        // =====================================================================
        // NAVEGAÇÃO
        // =====================================================================

        private void btnDashboard_Click(object sender, EventArgs e)
            => Navegar(new DashboardUserControl(), btnDashboard);

        private void btnProdutos_Click(object sender, EventArgs e)
            => Navegar(new ProductsUserControl(), btnProdutos);

        private void btnCategorias_Click(object sender, EventArgs e)
            => Navegar(new CategoriesUserControl(), btnCategorias);

        private void btnPedidos_Click(object sender, EventArgs e)
            => Navegar(new OrdersUserControl(), btnPedidos);

        private void Navegar(UserControl proximaTela, Button botao)
        {
            _telaAtual?.Dispose();
            pnlConteudo.Controls.Clear();

            proximaTela.Dock = DockStyle.Fill;
            pnlConteudo.Controls.Add(proximaTela);
            _telaAtual = proximaTela;

            if (_botaoAtivo != null)
            {
                _botaoAtivo.BackColor = Color.Transparent;
                _botaoAtivo.ForeColor = Color.White;
            }

            _botaoAtivo = botao;
            _botaoAtivo.BackColor = BetaFitTheme.PretoSecundario;
            _botaoAtivo.ForeColor = BetaFitTheme.Lima;
        }

        // =====================================================================
        // LOGOUT
        // =====================================================================

        private async void btnSair_Click(object sender, EventArgs e)
        {
            var confirmacao = MessageBox.Show(
                "Deseja realmente sair do sistema?",
                "BetaFit",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacao != DialogResult.Yes) return;

            try { await _authService.LogoutAsync(); }
            catch { /* segue com o logout local mesmo se a API falhar */ }
            finally
            {
                SessionManager.Instance.Clear();
                Close();
            }
        }

        // =====================================================================
        // ESTILO
        // =====================================================================

        private static void AplicarEstiloBotaoNav(Button botao)
        {
            botao.FlatAppearance.BorderSize = 0;
            botao.BackColor = Color.Transparent;
            botao.Cursor = Cursors.Hand;
            botao.MouseEnter += (_, _) =>
            {
                if (botao.BackColor != BetaFitTheme.PretoSecundario)
                    botao.BackColor = BetaFitTheme.PretoSecundario;
            };
            botao.MouseLeave += (_, _) =>
            {
                if (botao.ForeColor != BetaFitTheme.Lima)
                    botao.BackColor = Color.Transparent;
            };
        }

        private static void AplicarEstiloBotaoSair(Button botao)
        {
            botao.FlatAppearance.BorderSize = 0;
            botao.BackColor = Color.Transparent;
            botao.Cursor = Cursors.Hand;
            botao.MouseEnter += (_, _) => botao.BackColor = BetaFitTheme.Perigo;
            botao.MouseLeave += (_, _) => botao.BackColor = Color.Transparent;
        }
    }
}
