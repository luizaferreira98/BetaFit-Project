// =============================================================================
// BetaFit.Desktop - Forms/LoginForm.cs
// =============================================================================
//  CONCEITO: Tela de Login
//
// Responsável apenas pela lógica de autenticação. Todo o layout visual
// (painéis, labels, campos) é definido em LoginForm.Designer.cs, seguindo
// o mesmo padrão usado no restante do projeto (arquitetura Forms Designer).
// =============================================================================

using BetaFit.Desktop.Helpers;
using BetaFit.Desktop.Services;
using BetaFit.Desktop.Themes;

namespace BetaFit.Desktop.Forms
{
    public partial class LoginForm : Form
    {
        private readonly AuthApiService _authService;

        public LoginForm()
        {
            InitializeComponent();
            _authService = new AuthApiService();
            txtEmail.Text = "admin@betafit.com";
            txtSenha.Text = "Admin@123";
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            // Guard: não executa em tempo de design
            if (DesignMode) return;

            BetaFitTheme.AplicarEstiloFormulario(pnlDireita);
            BetaFitInputs.EstilizarSimples(txtEmail);
            BetaFitInputs.EstilizarSimples(txtSenha);
            BetaFitButtons.EstilizarPrimario(btnEntrar);

            lblErro.Text = string.Empty;
        }

        private void txtEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) txtSenha.Focus();
        }

        private void txtSenha_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) btnEntrar_Click(sender, e);
        }

        private async void btnEntrar_Click(object sender, EventArgs e)
        {
            ExibirErro(string.Empty);

            if (string.IsNullOrWhiteSpace(txtEmail.Text) || string.IsNullOrWhiteSpace(txtSenha.Text))
            {
                ExibirErro("INFORME E-MAIL E SENHA.");
                return;
            }

            SetCarregando(true);
            try
            {
                var (success, user, error) = await _authService.LoginAsync(txtEmail.Text.Trim(), txtSenha.Text);

                if (!success || user is null)
                {
                    ExibirErro(string.IsNullOrWhiteSpace(error) ? "NÃO FOI POSSÍVEL ENTRAR." : error.ToUpperInvariant());
                    return;
                }

                if (!user.IsAdmin)
                {
                    await _authService.LogoutAsync();
                    ExibirErro("ACESSO RESTRITO A ADMINISTRADORES.");
                    return;
                }

                SessionManager.Instance.SetUser(user);

                Hide();
                using var mainForm = new MainForm();
                mainForm.ShowDialog(this);
                Close();
            }
            catch (HttpRequestException ex)
            {
                ExibirErro($"NÃO FOI POSSÍVEL CONECTAR À API. {ex.Message}".ToUpperInvariant());
            }
            catch (Exception ex)
            {
                ExibirErro(ex.Message.ToUpperInvariant());
            }
            finally
            {
                SetCarregando(false);
            }
        }

        private void ExibirErro(string mensagem)
        {
            lblErro.Text = mensagem;
        }

        private void SetCarregando(bool carregando)
        {
            btnEntrar.Enabled = !carregando;
            txtEmail.Enabled = !carregando;
            txtSenha.Enabled = !carregando;
            btnEntrar.Text = carregando ? "AGUARDE..." : "ENTRAR";
        }
    }
}
