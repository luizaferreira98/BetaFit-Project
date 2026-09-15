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

        // Margem entre o botão SAIR e a borda de baixo da sidebar.
        private const int MargemRodapeSidebar = 24;

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

            // Prepara o comportamento de janela (maximizar sem cobrir a
            // barra de tarefas + botão SAIR grudado no rodapé da sidebar).
            AtualizarLimitesDeMaximizacao();
            PosicionarBotaoLogout();

            ////Preenche dados dinâmicos de sessão no header
            //lblUsuario.Text = $"👷‍ {SessionManager.Instance.GetDisplayName()}";
            //lblPerfil.Text = SessionManager.Instance.IsAdmin ? "🔑 Administrador" : "👀 Funcionário Comum";
            //lblPerfil.ForeColor = SessionManager.Instance.IsAdmin
            //    ? BetaFitThemes.PretoPrimario
            //    : BetaFitThemes.Lima;
            //lblSessao.Text = $"🟢 {SessionManager.Instance.GetEmail()}";

            // Configura permissões baseadas no perfil do funcionário
            ConfigurarPermissoes();

            //Abre a primeira tela que esse papel pode ver
            if (btnDashBoard.Visible)
                NavegarParaDashboard();
            else
                Navegar(new ProdutosUserControl(), btnProdutos);
        }

        // =====================================================================
        // COMPORTAMENTO DE JANELA
        // =====================================================================
        // A janela usa FormBorderStyle.None (barra de título própria). Nesse
        // modo o Windows NÃO respeita a área de trabalho ao maximizar: a
        // janela cobre a barra de tarefas inteira. MaximizedBounds resolve
        // isso, e precisa ser recalculado quando o usuário arrasta a janela
        // para outro monitor (resoluções e posição da taskbar podem diferir).
        // =====================================================================
        private void AtualizarLimitesDeMaximizacao()
        {
            var tela = Screen.FromHandle(this.Handle);
            MaximizedBounds = tela.WorkingArea;
        }

        private void MainForm_Resize(object sender, EventArgs e)
        {
            if (DesignMode) return;

            // Mantém o glifo do botão coerente com o estado atual da janela.
            btnMaximizarJanela.Text = WindowState == FormWindowState.Maximized ? "❐" : "□";
        }

        // O botão SAIR foi desenhado com Y fixo (744). Com 800px de altura de
        // cliente e 38px de barra de título, a sidebar tem 762px — ou seja,
        // ele já nascia cortado; maximizado, ficava boiando no meio.
        //
        // Anchor = Bottom não serve aqui: pnlMenu é criado com Size(240, 0) no
        // Designer e só ganha altura real depois do Dock, então a distância
        // que o Anchor memoriza sairia errada. Reposicionar no Resize é
        // determinístico e faz o botão acompanhar qualquer altura de janela.
        private void PosicionarBotaoLogout()
        {
            if (pnlMenu.ClientSize.Height <= 0) return;

            btnLogout.Top = pnlMenu.ClientSize.Height - btnLogout.Height - MargemRodapeSidebar;
            btnLogout.Left = 16;
        }

        private void pnlMenu_Resize(object sender, EventArgs e)
        {
            if (DesignMode) return;
            PosicionarBotaoLogout();
        }

        // Duplo clique na barra de título alterna maximizar/restaurar —
        // comportamento padrão de qualquer janela do Windows, que a barra
        // customizada não tinha.
        private void pnlBarraTitulo_DoubleClick(object? sender, EventArgs e)
        {
            AlternarMaximizar();
        }

        private void AlternarMaximizar()
        {
            AtualizarLimitesDeMaximizacao();

            WindowState = WindowState == FormWindowState.Maximized
                ? FormWindowState.Normal
                : FormWindowState.Maximized;
        }

        // Configura as permissões de visibilidade dos botões com base no perfil do funcionário.
        //
        //   Admin      - vê tudo
        //   Gerente    - vê tudo, menos gestão de funcionários
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

            // Gestão de funcionários: somente o Administrador.
            btnFuncionarios.Visible = isAdmin;

            // Produtos e Meu Perfil não têm restrição — todo funcionário usa.
        }

        // Navega para o Dashboard
        private void NavegarParaDashboard()
        {
            Navegar(new DashboardUserControl(), btnDashBoard);
        }

        // Público de propósito: chamado a partir de dentro de outro UserControl
        // (ex.: botão "IR PARA PRODUTOS" do estado vazio em PedidosUserControl),
        // que não tem acesso direto ao método privado Navegar/aos botões da sidebar.
        public void NavegarParaProdutos()
        {
            Navegar(new ProdutosUserControl(), btnProdutos);
        }

        // Público de propósito: chamado a partir do botão "VER PEDIDOS" do
        // estado vazio em DashboardUserControl, mesmo esquema do
        // NavegarParaProdutos acima.
        public void NavegarParaPedidos()
        {
            Navegar(new PedidosUserControl(), btnPedidos);
        }

        // Navega para uma tela interna do conteúdo
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
            foreach (var b in new[] { btnDashBoard, btnProdutos, btnCategorias, btnPedidos, btnFuncionarios, btnPerfil })
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
            bool confirmou = BetaFitMessageBox.Confirmar(
                this,
                "Deseja realmente sair do sistema?",
                "Confirmar Logout");

            if (!confirmou) return;

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
        private void btnFuncionarios_Click(object sender, EventArgs e) => Navegar(new FuncionariosUserControl(), btnFuncionarios);
        private void btnPerfil_Click(object sender, EventArgs e) => Navegar(new PerfilUserControl(), btnPerfil);
        private void btnFecharJanela_Click(object? sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit();
        }

        private void btnMaximizarJanela_Click(object? sender, EventArgs e)
        {
            AlternarMaximizar();
        }

        private void btnMinimizarJanela_Click(object? sender, EventArgs e)
        {
            WindowState = FormWindowState.Minimized;
        }

    }
}