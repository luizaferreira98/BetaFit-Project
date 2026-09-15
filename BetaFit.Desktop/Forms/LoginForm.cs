using BetaFit.Desktop.Services;
using BetaFit.Desktop.Helpers;
using BetaFit.Desktop.Themes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetaFit.Desktop.Forms
{
    public partial class LoginForm : Form
    {
        //Encapsulamento do serviço de autenticação da API
        private AuthApiService _authService = null!; //Criando a classe
        public LoginForm()
        {
            InitializeComponent();
        }

        //BtnFechar
        private void btnFechar_Click(object sender, EventArgs e)
        {
            System.Windows.Forms.Application.Exit(); //Permite fazer uma aplicacao de fechamento completo, sem deixar processos em segundo plano
        }


        private void LoginForm_Load(object sender, EventArgs e)
        {
            //Inicializa o serviço de autenticação da API
            //Guard: não executa em tempo de design
            if (DesignMode) return; //Filtro de seguranca: ela verifica se o form está em tempo de design, se estiver, ele não executa o código abaixo

            //Instancia o serviço de autenticação da API
            _authService = new AuthApiService(); //Criando um objeto apartir da classe AuthApiService

            // ── Ícones desenhados (GDI+, sem depender de fonte de emoji) ──
            pctIconeEvolua.Image = BetaFitTheme.CriarIconeGrafico();
            pctIconeConquiste.Image = BetaFitTheme.CriarIconeHalter();
            pctIconeSupere.Image = BetaFitTheme.CriarIconeCoracao();
            pctIconeEmail.Image = BetaFitTheme.CriarIconeEnvelope();
            pctIconeSenha.Image = BetaFitTheme.CriarIconeCadeado();
            // "Aberto" = senha visível. UseSystemPasswordChar == true significa
            // que a senha está oculta, então o ícone precisa ser o inverso.
            pctToggleSenha.Image = BetaFitTheme.CriarIconeOlho(aberto: !txtSenha.UseSystemPasswordChar);

            // ── Fundo do painel esquerdo: foto + gradiente + linhas ──
            // A foto fica em Assets/academia.png (Content, copiada pro
            // diretório de saída pelo .csproj — CopyToOutputDirectory), então
            // é carregada por caminho de arquivo, não como resource embutido.
            var caminhoFoto = Path.Combine(AppContext.BaseDirectory, "Assets", "academia.png");
            if (File.Exists(caminhoFoto))
            {
                pnlEsquerdo.BackgroundImage = Image.FromFile(caminhoFoto);
                pnlEsquerdo.BackgroundImageLayout = ImageLayout.Zoom;
            }

            lblVersao.Text = $"Versão {AppConfig.Version} | ©️ {DateTime.Now.Year} BETAFIT";
            lblApi.Text = $"API: {AppConfig.ApiBaseUrl}";

            txtEmail.Text = "admin@betafit.com";
            txtSenha.Text = "Admin@123";
        }

        // Gradiente escuro + linhas diagonais desenhados por cima do painel
        // esquerdo (e por cima da foto de fundo, quando ela for importada —
        // o Paint roda depois do BackgroundImage, então não apaga a foto).
        private void pnlEsquerdo_Paint(object sender, PaintEventArgs e)
        {
            var g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            var area = pnlEsquerdo.ClientRectangle;

            // Vinheta: escurece as bordas e deixa o centro um pouco mais
            // "respirável" — fica melhor tanto com a foto quanto sem ela.
            using (var gradiente = new LinearGradientBrush(
                area,
                Color.FromArgb(235, 5, 5, 5),
                Color.FromArgb(170, 10, 40, 10),
                LinearGradientMode.Vertical))
            {
                g.FillRectangle(gradiente, area);
            }

            // Linhas de destaque lima nos dois cantos (igual ao mockup: uma
            // "moldura" leve de neon subindo do canto inferior-direito e
            // descendo do canto superior-esquerdo do painel).
            using var canetaLinha = new Pen(Color.FromArgb(90, BetaFitTheme.Admin.Lima), 2f);
            g.DrawLine(canetaLinha, 0, 60, 90, -30);
            g.DrawLine(canetaLinha, 20, 90, 110, -10);

            g.DrawLine(canetaLinha, area.Width - 90, area.Height + 30, area.Width, area.Height - 60);
            g.DrawLine(canetaLinha, area.Width - 120, area.Height + 10, area.Width - 20, area.Height - 90);
        }

        // Olho de mostrar/esconder a senha
        private void pctToggleSenha_Click(object sender, EventArgs e)
        {
            txtSenha.UseSystemPasswordChar = !txtSenha.UseSystemPasswordChar;
            // "Aberto" = senha visível (UseSystemPasswordChar == false)
            pctToggleSenha.Image = BetaFitTheme.CriarIconeOlho(aberto: !txtSenha.UseSystemPasswordChar);
        }


        //Input do Email
        private void txtEmail_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) txtSenha.Focus();//Se o usuario apertar a tecla Enter, ele vai pular para o campo de senha
        }


        //Input da Senha
        private void txtSenha_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter) btnEntrar_Click(sender, e);
        }


        //Botao de Entrar
        private async void btnEntrar_Click(object sender, EventArgs e)
        {
            //Limpa erros anteriores
            ExibirErro(string.Empty);

            //Validação dos campos
            //IsNullOrWhiteSpace == Verifica se a string é nula, vazia ou contém apenas espaços em branco
            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                ExibirErro("⚠️ Informe seu e-mail!");
                txtEmail.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtSenha.Text))
            {
                ExibirErro("⚠️ Informe sua senha!");
                txtSenha.Focus();
                return;
            }

            // ===================== Estado de carregamento ======================
            SetCarregando(true);

            try
            {
                // Chamada da API
                var (success, user, errorMessage) = await _authService.LoginAsync(
                    txtEmail.Text.Trim(),
                    txtSenha.Text);

                if (success && user != null)
                {

                    bool isFuncionario = user.IsFuncionario;

                    if (!isFuncionario)
                    {
                        // O login já foi validado e a API já emitiu o cookie de
                        // sessão — precisamos desfazer isso explicitamente, senão
                        // a sessão desse cliente fica ativa mesmo com o acesso
                        // negado na tela.
                        await _authService.LogoutAsync();

                        ExibirErro("⛔ Acesso restrito à equipe BetaFit.");
                        MessageBox.Show(
                            "⛔ Este aplicativo é de uso exclusivo da equipe BetaFit.\n" +
                            "Sua conta não tem permissão de funcionário (Admin, Gerente ou Estoquista).",
                            "Acesso negado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    // Armazena os dados do usuário na sessão (Singleton)
                    SessionManager.Instance.SetUser(user);

                    // Esconde a tela de login
                    this.Hide();

                    //Abrir a tela principal da aplicação
                    using var mainform = new MainForm();
                    mainform.ShowDialog();

                    // quando o MainForm fechar. fecha o LoginForm aparece
                    this.Show();
                }
                else
                {
                    ExibirErro($"❌ {errorMessage}");
                    MessageBox.Show($"❌ {errorMessage}");
                }

            }
            //Caso 1 de erro
            catch (HttpRequestException exHttp)
            {
                ExibirErro($"❌ Não foi possível conectar à API. \nVerifique se a API está em execução erro do sistema: {exHttp.Message}");
                MessageBox.Show($"❌ Não foi possível conectar à API. \nVerifique se a API está em execução erro do sistema: {exHttp.Message}");
            }

            //Caso 2 de erro
            catch (Exception ex)
            {
                ExibirErro($"❌ Erro inesperado: {ex.Message}");
                MessageBox.Show($"❌ Erro inesperado: {ex.Message}");
            }
            finally
            {
                SetCarregando(false);
            }
        }

        //Função de mostra os erros que tem quando for fazer login
        private void ExibirErro(string mensagem)
        {
            //IsNullOrEmpty verifica se a string é nula ou vazia
            if (string.IsNullOrEmpty(mensagem))
            {
                lblErro.Visible = false;
                lblErro.Text = string.Empty;
            }
            else
            {
                lblErro.Text = mensagem;
                lblErro.Visible = true;
            }
        }


        private void SetCarregando(bool carregando)
        {
            btnEntrar.Enabled = !carregando;
            txtEmail.Enabled = !carregando;
            txtSenha.Enabled = !carregando;
            lblCarregando.Visible = carregando;

            if (carregando)
            {
                btnEntrar.Text = "Aguarde...";
                lblErro.Visible = false;
            }
            else
            {
                btnEntrar.Text = "Entrar";
            }

        }
    }
}