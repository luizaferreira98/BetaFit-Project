using Guna.UI2.WinForms;
using BetaFit.Desktop.Helpers;
using BetaFit.Desktop.Services;
using BetaFit.Desktop.Themes;
using BetaFit.Desktop.UserControls;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetaFit.Desktop.Forms
{
    public partial class MainForm : Form
    {
        //=================================================
        // SERVIÇOS (Inicilizados no Load)
        //=================================================
        // UserControl atualmente exibido no painel de conteudo (pnlConteudo)
        private UserControl? _controleAtual;

        // Botão da sidebar atualmente ativo.
        private Guna2Button? _botaoAtivo;

        // Serviço de autenticação para logout.
        private AuthApiService _authService = null;

        public MainForm()
        {
            InitializeComponent();
        }

        //MainForm Carregamento
        private void MainForm_Load(object sender, EventArgs e)
        {
            //Guard: não executa em tempo de design
            if (DesignMode) return;

            //Instancia o serviço
            _authService = new AuthApiService();

            // Atualiza o título com a versão
            this.Text = $"BetaFit Desktop - {AppConfig.Version}";

            ////Preenche dados dinâmicos de sessão no header
            //lblUsuario.Text = $"👷‍ {SessionManager.Instance.GetDisplayName()}";
            //lblPerfil.Text = SessionManager.Instance.IsAdmin ? "🔑 Administrador" : "👀 Usuário Comum";
            //lblPerfil.ForeColor = SessionManager.Instance.IsAdmin
            //    ? BetaFitThemes.PretoPrimario
            //    : BetaFitThemes.Lima;
            //lblSessao.Text = $"🟢 {SessionManager.Instance.GetEmail()}";

            // Configura permissões baseadas no perfil do usuário
            ConfigurarPermissoes();

            //Abre o DashBoard como tela inicial
            NavegarParaDashboard();
        }

        // Configura as permissões de visibilidade dos botões com base no perfil do usuário.
        private void ConfigurarPermissoes()
        {
            var isAdmin = SessionManager.Instance.IsAdmin;

            btnCategorias.Visible = isAdmin;
            //btnUsuarios.Visible = isAdmin;
        }

        // Navega para o Dashboard
        private void NavegarParaDashboard()
        {
            Navegar(new DashboardUserControl(), btnDashBoard);
        }

        // Navega para a tela de Categorias
        private void Navegar(UserControl control, Guna2Button? botao = null)
        {
            //Remove o UserControl anterior
            if (_controleAtual != null)
            {
                pnlConteudo.Controls.Remove(_controleAtual);
                _controleAtual.Dispose();
                _controleAtual = null;
            }

            //Adiona o novo UserControl(Tela interna)
            control.Dock = DockStyle.Fill;
            pnlConteudo.Controls.Add(control);
            _controleAtual = control;

            AtualizarBotaoAtivo(botao);
        }


        // Atualiza o estado visual do botão ativo na sidebar.
        private void AtualizarBotaoAtivo(Guna2Button? botao)
        {
            if (_botaoAtivo != null)
            {
                _botaoAtivo.FillColor = Color.Transparent;
                _botaoAtivo.ForeColor = Color.White;

                _botaoAtivo = botao;
                if (_botaoAtivo != null)
                {
                    _botaoAtivo.FillColor = Color.FromArgb(0, 50, 110);
                    _botaoAtivo.ForeColor = Color.White;
                    _botaoAtivo.CustomBorderColor = BetaFitTheme.PretoPrimario;

                }
            }
        }

        //Botão de Logout
        private async void btnLogout_Click(object sender, EventArgs e)
        {
            //Mensagem para ver se o usuario deseja realmente sair do sistema
            var resposta = MessageBox.Show(
                "Deseja realmente sair do sistema?",
                "Confirmar Logout",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (resposta != DialogResult.Yes) return;

            try
            {
                await _authService.LogoutAsync();
            }
            catch
            {
                // Mesmo se a API falhar, limpa a sessão local
            }
            finally
            {
                SessionManager.Instance.Clear();
                this.Close();
            }
        }


        // Botoes para ir para outras paginas
        private void btnDashBoard_Click(object sender, EventArgs e) => Navegar(new DashboardUserControl(), btnDashBoard);
        private void btnProdutos_Click(object sender, EventArgs e) => Navegar(new ProdutosUserControl(), btnProdutos);
        private void btnCategorias_Click(object sender, EventArgs e) => Navegar(new CategoriasUserControl(), btnCategorias);
        private void btnPedidos_Click(object sender, EventArgs e) => Navegar(new PedidosUserControl(), btnPedidos);
        private void btnPerfil_Click(object sender, EventArgs e) => Navegar(new PerfilUserControl(), btnPerfil);
    }
}
