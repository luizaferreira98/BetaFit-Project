using BetaFit.Desktop.Helpers;
using BetaFit.Desktop.Services;
using BetaFit.Desktop.Themes;

namespace BetaFit.Desktop.Forms
{
    /// <summary>
    /// Tela de login do BetaFit Desktop.
    ///
    /// Fluxo:
    ///   1. Usuário digita e-mail/senha e clica em ENTRAR (ou tecla Enter no campo senha)
    ///   2. Chama AuthApiService.LoginAsync() → POST /api/auth/login
    ///   3. Se sucesso: guarda o usuário no SessionManager e fecha com DialogResult.OK
    ///   4. Se erro: mostra a mensagem em lblErro sem fechar a tela
    ///
    /// Uso em Program.cs:
    ///   using var login = new LoginForm();
    ///   if (login.ShowDialog() == DialogResult.OK)
    ///       Application.Run(new Form1());
    /// </summary>
    public partial class LoginForm : Form
    {
        private readonly AuthApiService _authService = new();

        public LoginForm()                     
        {
            InitializeComponent();           
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {
            txtEmail.Text = "admin@betafit.com";
            txtSenha.Text = "Admin@123";
            AplicarComportamentoAdicional();
            HabilitarArraste(pnlEsquerda);
        }

        /// <summary>
        /// Configura comportamentos que não fazem parte do "desenho" da tela
        /// (efeito de foco nos campos, Enter para logar, etc.)
        /// </summary>
        private void AplicarComportamentoAdicional()
        {
            // Borda do campo fica preta quando o usuário está digitando nele,
            // e volta pro cinza claro quando perde o foco (mesmo efeito do site).
            txtEmail.Enter += (s, e) => txtEmail.BorderColor = BetaFitTheme.InputBordaFoco;
            txtEmail.Leave += (s, e) => txtEmail.BorderColor = BetaFitTheme.InputBorda;

            txtSenha.Enter += (s, e) => txtSenha.BorderColor = BetaFitTheme.InputBordaFoco;
            txtSenha.Leave += (s, e) => txtSenha.BorderColor = BetaFitTheme.InputBorda;

            // Efeito hover do botão primário (fica lima mais claro)
            btnEntrar.HoverState.FillColor = BetaFitTheme.BotaoPrimarioHover;

            // Permite logar apertando Enter depois de digitar a senha
            txtSenha.KeyDown += TxtSenha_KeyDown;
            txtEmail.KeyDown += TxtEmail_KeyDown;
        }

        private void TxtEmail_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                txtSenha.Focus();
            }
        }

        private async void TxtSenha_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                await RealizarLoginAsync();
            }
        }

        /// <summary>Alterna entre mostrar/ocultar a senha digitada.</summary>
        private void chkMostrarSenha_CheckedChanged(object sender, EventArgs e)
        {
            txtSenha.UseSystemPasswordChar = !chkMostrarSenha.Checked;
        }

        private async void btnEntrar_Click(object sender, EventArgs e)
        {
            await RealizarLoginAsync();
        }

        /// <summary>
        /// Valida os campos, chama a API e trata sucesso/erro.
        /// </summary>
        private async Task RealizarLoginAsync()
        {
            EsconderErro();

            var email = txtEmail.Text.Trim();
            var senha = txtSenha.Text;

            if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(senha))
            {
                MostrarErro("Preencha e-mail e senha para continuar.");
                return;
            }

            DefinirCarregando(true);
            try
            {
                var (sucesso, usuario, erro) = await _authService.LoginAsync(email, senha);

                if (sucesso && usuario != null)
                {
                    SessionManager.Instance.SetUser(usuario);
                    DialogResult = DialogResult.OK;
                    Close();
                }
                else
                {
                    MostrarErro(string.IsNullOrWhiteSpace(erro)
                        ? "E-mail ou senha inválidos."
                        : erro);
                }
            }
            catch (Exception ex)
            {
                MostrarErro($"Não foi possível conectar à API.\n{ex.Message}");
            }
            finally
            {
                DefinirCarregando(false);
            }
        }

        /// <summary>Bloqueia a tela e troca o texto do botão enquanto a requisição roda.</summary>
        private void DefinirCarregando(bool carregando)
        {
            btnEntrar.Enabled = !carregando;
            btnEntrar.Text = carregando ? "ENTRANDO..." : "ENTRAR";
            txtEmail.Enabled = !carregando;
            txtSenha.Enabled = !carregando;
            UseWaitCursor = carregando;
        }

        private void MostrarErro(string mensagem)
        {
            lblErro.Text = mensagem;
            lblErro.Visible = true;
        }

        private void EsconderErro()
        {
            lblErro.Visible = false;
            lblErro.Text = string.Empty;
        }

        private Point _dragStart;
        private bool _arrastando;

        private void HabilitarArraste(Control area)
        {
            area.MouseDown += (s, e) => { _arrastando = true; _dragStart = e.Location; };
            area.MouseMove += (s, e) =>
            {
                if (_arrastando)
                {
                    var p = PointToScreen(e.Location);
                    Location = new Point(p.X - _dragStart.X, p.Y - _dragStart.Y);
                }
            };
            area.MouseUp += (s, e) => _arrastando = false;
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
