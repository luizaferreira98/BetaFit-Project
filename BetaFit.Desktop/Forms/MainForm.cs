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

            //Abre a primeira tela que esse papel pode ver
            if (btnDashBoard.Visible)
                NavegarParaDashboard();
            else
                Navegar(new ProdutosUserControl(), btnProdutos);
        }

        // Configura as permissões de visibilidade dos botões com base no perfil do usuário.
        //
        //   Admin      - vê tudo
        //   Gerente    - vê tudo, menos gestão de funcionários (sem tela própria ainda)
        //   Estoquista - só Produtos/Categorias (foco em catálogo/estoque);
        //                não vê Dashboard nem Pedidos
        private void ConfigurarPermissoes()
        {
            var isAdmin = SessionManager.Instance.IsAdmin;
            var isGerente = SessionManager.Instance.IsGerente;
            var isEstoquista = SessionManager.Instance.IsEstoquista;

            // Dashboard e Pedidos: visão da operação da loja como um todo —
            // só Admin e Gerente. A API (DashboardController/OrdersController)
            // já bloqueia isso pra Estoquista, então aqui é só pra não deixar
            // o botão visível levando a uma tela que vai dar erro de acesso.
            btnDashBoard.Visible = isAdmin || isGerente;
            btnPedidos.Visible = isAdmin || isGerente;

            // Categorias: qualquer funcionário que mexe no catálogo
            // (antes só Admin via esse botão).
            btnCategorias.Visible = isAdmin || isGerente || isEstoquista;

            // Produtos e Meu Perfil não têm restrição — todo funcionário usa.
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
            // Reseta todos os botões da sidebar para o estado "inativo"
            foreach (var b in new[] { btnDashBoard, btnProdutos, btnCategorias, btnPedidos, btnPerfil })
            {
                b.FillColor = Color.Transparent;
                b.ForeColor = Color.White;
                b.BorderThickness = 0;
            }

            _botaoAtivo = botao;
            if (_botaoAtivo != null)
            {
                _botaoAtivo.FillColor = BetaFitTheme.Admin.AtivoFundoNav;  // verde bem escuro
                _botaoAtivo.ForeColor = BetaFitTheme.Admin.Lima;           // texto lima
                _botaoAtivo.BorderThickness = 1;
                _botaoAtivo.BorderColor = BetaFitTheme.Admin.Lima;
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