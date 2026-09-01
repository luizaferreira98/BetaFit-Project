using BetaFit.Desktop.Services;
using BetaFit.Desktop.Helpers;
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


            lblVersao.Text = $"Versão {AppConfig.Version} | ©️ {DateTime.Now.Year} BETAFIT";
            lblApi.Text = $"API: {AppConfig.ApiBaseUrl}";

            txtEmail.Text = "admin@betafit.com";
            txtSenha.Text = "Admin@123";
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
