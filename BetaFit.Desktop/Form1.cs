<<<<<<< HEAD
using BetaFit.Desktop.Helpers;
using BetaFit.Desktop.UserControls;
using BetaFit.Desktop.Themes;

namespace BetaFit.Desktop
{
    /// <summary>
    /// Janela principal do BetaFit Desktop.
    /// Funciona como shell/container: o conteúdo administrativo é carregado
    /// por UserControls. O primeiro módulo exibido após o login é o Dashboard.
    /// </summary>
    public partial class Form1 : Form
    {
        private DashboardUserControl? _dashboard;

        public Form1()
        {
            InitializeComponent();
            ConfigurarUsuario();
            AbrirDashboard();
        }

        private void ConfigurarUsuario()
        {
            var sessao = SessionManager.Instance;
            lblUsuario.Text = sessao.GetDisplayName().ToUpperInvariant();
            lblPerfil.Text = sessao.IsAdmin ? "ADMINISTRADOR" : "USUÁRIO";
        }

        private void AbrirDashboard()
        {
            _dashboard?.Dispose();
            _dashboard = new DashboardUserControl
            {
                Dock = DockStyle.Fill
            };

            pnlConteudo.Controls.Clear();
            pnlConteudo.Controls.Add(_dashboard);
        }

        private async void btnSair_Click(object sender, EventArgs e)
        {
            btnSair.Enabled = false;

            try
            {
                var auth = new BetaFit.Desktop.Services.AuthApiService();
                await auth.LogoutAsync();
            }
            catch
            {
                // Mesmo que a API esteja indisponível, o logout local acontece.
            }
            finally
            {
                SessionManager.Instance.Clear();
                Close();
            }
=======
namespace BetaFit.Desktop
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
>>>>>>> 9aa9898d3f31f5e28e0f9360943b6a83a794de8c
        }
    }
}
