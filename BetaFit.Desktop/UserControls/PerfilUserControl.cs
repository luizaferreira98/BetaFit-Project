using BetaFit.Desktop.DTOs;
using BetaFit.Desktop.Forms;
using BetaFit.Desktop.Helpers;
using BetaFit.Desktop.Services;
using System;
using System.Windows.Forms;

namespace BetaFit.Desktop.UserControls
{
    public partial class PerfilUserControl : UserControl
    {
        //=================================================
        // SERVIÇOS (Inicilizados no Load)
        //=================================================
        private ProfileApiService _profileApiService = null!;
        private UsersApiService _usersApiService = null!;

        //=================================================
        // DADOS (perfil carregado da API, usado para reenviar
        // os campos obrigatórios do PUT /api/profile mesmo quando
        // só a senha está sendo trocada)
        //=================================================
        private ProfileResponseDto? _perfilAtual;

        public PerfilUserControl()
        {
            InitializeComponent();
        }

        //=================================================
        // LOAD DO USER CONTROL
        //=================================================
        private async void PerfilUserControl_Load(object sender, EventArgs e)
        {
            //Guard: não executa em tempo de Design
            if (DesignMode) return;

            _profileApiService = new ProfileApiService();
            _usersApiService = new UsersApiService();

            AjustarColunasFormulario();

            await CarregarPerfilAsync();
        }

        //=================================================
        // CARREGAR DADOS DO PERFIL (GET /api/profile)
        //=================================================
        private async Task CarregarPerfilAsync()
        {
            try
            {
                var perfil = await _profileApiService.GetAsync();
                if (perfil == null)
                {
                    BetaFitMessageBox.Erro(this, "Não foi possível carregar os dados do seu perfil.");
                    return;
                }

                _perfilAtual = perfil;
                PreencherCampos(perfil);
            }
            catch (Exception ex)
            {
                BetaFitMessageBox.Erro(this, $"Erro ao carregar perfil: {ex.Message}");
            }
        }

        //=================================================
        // PREENCHE OS CAMPOS DE TELA COM OS DADOS DO PERFIL
        //=================================================
        private void PreencherCampos(ProfileResponseDto perfil)
        {
            txtNome.Text = perfil.FullName;
            txtEmail.Text = perfil.Email;
            txtTelefone.Text = perfil.PhoneNumber;
            dtpDNascimento.Value = perfil.BirthDate ?? DateTime.Today.AddYears(-18);

            lblNomeCompleto.Text = string.IsNullOrWhiteSpace(perfil.FullName)
                ? "Usuário"
                : perfil.FullName;
            lblEmailCabecalho.Text = perfil.Email;
            lblCargo.Text = perfil.IsAdmin ? "Administrador" : "Usuário";
        }

        //=================================================
        // LAYOUT RESPONSIVO
        //=================================================
        // Os painéis já esticam sozinhos (Anchor Left|Right). O que o Anchor
        // não faz é redistribuir as DUAS COLUNAS de campos dentro do painel —
        // ele só empurraria a coluna da direita e deixaria um buraco no meio.
        // Por isso as larguras são recalculadas aqui, mesma ideia do
        // DistribuirCards() do DashboardUserControl.
        //=================================================
        private void PerfilUserControl_Resize(object sender, EventArgs e)
        {
            if (DesignMode) return;
            AjustarColunasFormulario();
        }

        private void AjustarColunasFormulario()
        {
            if (pnlDadosPessoais == null || pnlDadosPessoais.Width <= 0) return;

            const int margem = 24;   // padding interno do card
            const int vao = 24;      // espaço entre as duas colunas
            const int larguraMinima = 220;

            int larguraColuna = (pnlDadosPessoais.Width - (margem * 2) - vao) / 2;
            if (larguraColuna < larguraMinima) larguraColuna = larguraMinima;

            int colEsquerda = margem;
            int colDireita = margem + larguraColuna + vao;

            // Coluna da esquerda: nome e e-mail
            foreach (var c in new Control[] { lblNome, txtNome, lblEmail, txtEmail })
                c.Left = colEsquerda;

            // Coluna da direita: telefone e nascimento
            foreach (var c in new Control[] { lblTelefone, txtTelefone, lblNascimento, dtpDNascimento })
                c.Left = colDireita;

            // Só os campos acompanham a largura; os rótulos ficam no tamanho do texto
            txtNome.Width = larguraColuna;
            txtEmail.Width = larguraColuna;
            txtTelefone.Width = larguraColuna;
            dtpDNascimento.Width = larguraColuna;
        }

        //=================================================
        // ATUALIZAR (BOTÃO) - RECARREGA DADOS DO SERVIDOR
        //=================================================
        private async void btnAtualizarPerfil_Click(object sender, EventArgs e) => await CarregarPerfilAsync();

        //=================================================
        // SALVAR PERFIL (NOME / E-MAIL / TELEFONE / NASCIMENTO)
        //=================================================
        private async void btnSalvarPerfil_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNome.Text))
            {
                BetaFitMessageBox.Aviso(this, "Informe seu nome completo.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                BetaFitMessageBox.Aviso(this, "Informe seu e-mail.");
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTelefone.Text))
            {
                BetaFitMessageBox.Aviso(this, "Informe seu telefone.");
                return;
            }

            var dto = new UpdateProfileDto
            {
                FullName = txtNome.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                PhoneNumber = txtTelefone.Text.Trim(),
                BirthDate = dtpDNascimento.Value.Date
            };

            var (success, perfilAtualizado, error) = await _profileApiService.UpdateAsync(dto);
            if (success && perfilAtualizado != null)
            {
                _perfilAtual = perfilAtualizado;
                PreencherCampos(perfilAtualizado);

                BetaFitMessageBox.Sucesso(this, "Perfil atualizado com sucesso!");
            }
            else
            {
                BetaFitMessageBox.Erro(this, error);
            }
        }

        //=================================================
        // ALTERAR SENHA (ABRE DIALOG SIMPLES CRIADO EM CÓDIGO)
        //=================================================
        private async void BtnAlterarSenha_Click(object sender, EventArgs e)
        {
            if (_perfilAtual == null)
            {
                BetaFitMessageBox.Aviso(this, "Aguarde o carregamento do perfil e tente novamente.");
                return;
            }

            // A API exige FullName/Email/PhoneNumber/BirthDate em TODO PUT
            // em /api/profile, mesmo quando é só a senha que está mudando.
            // Se o perfil ainda tiver campo obrigatório vazio (ex: telefone
            // nunca cadastrado), o PUT falharia com 400 antes mesmo de chegar
            // a validar a senha — então avisamos e paramos aqui.
            if (string.IsNullOrWhiteSpace(_perfilAtual.PhoneNumber))
            {
                BetaFitMessageBox.Aviso(
                    this,
                    "Antes de alterar a senha, preencha e salve seu telefone na aba " +
                    "\"Dados Pessoais\". A API exige esse campo em toda atualização de perfil.",
                    "Complete seu perfil");
                return;
            }

            using var dialog = new AlterarSenhaDialog();
            if (dialog.ShowDialog(this) != DialogResult.OK) return;

            // A API exige os campos obrigatórios do perfil em todo PUT,
            // então reenviamos os dados atuais junto com a nova senha.
            var dto = new UpdateProfileDto
            {
                FullName = _perfilAtual.FullName,
                Email = _perfilAtual.Email,
                PhoneNumber = _perfilAtual.PhoneNumber,
                BirthDate = _perfilAtual.BirthDate ?? DateTime.Today.AddYears(-18),
                CurrentPassword = dialog.SenhaAtual,
                NewPassword = dialog.NovaSenha,
                ConfirmNewPassword = dialog.ConfirmarNovaSenha
            };

            var (success, _, error) = await _profileApiService.UpdateAsync(dto);
            if (success)
            {
                BetaFitMessageBox.Sucesso(this, "Senha alterada com sucesso!");
            }
            else
            {
                BetaFitMessageBox.Erro(this, error);
            }
        }

        //=================================================
        // EXCLUIR CONTA
        //=================================================
        private async void BtnExcuirCnta_Click(object sender, EventArgs e)
        {
            var userId = SessionManager.Instance.CurrentUser?.Id;
            if (string.IsNullOrWhiteSpace(userId))
            {
                BetaFitMessageBox.Erro(this, "Não foi possível identificar o usuário logado.");
                return;
            }

            // Administradores não podem excluir a própria conta por aqui — a tela de
            // Funcionários já bloqueia isso ao gerenciar outras contas, mas o botão
            // "Excluir Minha Conta" do Perfil não tinha essa mesma verificação, o que
            // permitia um admin se auto-excluir (e, no limite, deixar a loja sem
            // nenhum administrador). A exclusão de um admin deve passar por outro
            // administrador, na tela de Funcionários.
            if (SessionManager.Instance.IsAdmin)
            {
                BetaFitMessageBox.Aviso(
                    this,
                    "Contas de administrador não podem ser excluídas pelo próprio titular. " +
                    "Peça para outro administrador excluir sua conta pela tela de Funcionários.",
                    "Operação não permitida");
                return;
            }

            bool conf = BetaFitMessageBox.Confirmar(
                this,
                "Tem certeza que deseja excluir sua conta?\n" +
                "Todos os seus dados serão removidos permanentemente. Esta ação não pode ser desfeita.",
                "Confirmar Exclusão de Conta");

            if (!conf) return;

            var (success, error) = await _usersApiService.DeleteAsync(userId);
            if (!success)
            {
                BetaFitMessageBox.Erro(this, error);
                return;
            }

            BetaFitMessageBox.Sucesso(this, "Sua conta foi excluída. Você será desconectado.", "Conta Excluída");

            SessionManager.Instance.Clear();

            // Fecha o MainForm (pai desta UserControl), voltando para a tela de Login.
            FindForm()?.Close();
        }
    }
}