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
                    MessageBox.Show(
                        "Não foi possível carregar os dados do seu perfil.",
                        "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                _perfilAtual = perfil;
                PreencherCampos(perfil);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar perfil: {ex.Message}",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
            dtpNascimento.Value = perfil.BirthDate ?? DateTime.Today.AddYears(-18);

            lblNomeCompleto.Text = string.IsNullOrWhiteSpace(perfil.FullName)
                ? "Usuário"
                : perfil.FullName;
            guna2HtmlLabel3.Text = perfil.Email;
            guna2HtmlLabel2.Text = perfil.IsAdmin ? "Administrador" : "Usuário";
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
                MessageBox.Show("Informe seu nome completo.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Informe seu e-mail.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtTelefone.Text))
            {
                MessageBox.Show("Informe seu telefone.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var dto = new UpdateProfileDto
            {
                FullName = txtNome.Text.Trim(),
                Email = txtEmail.Text.Trim(),
                PhoneNumber = txtTelefone.Text.Trim(),
                BirthDate = dtpNascimento.Value.Date
            };

            var (success, perfilAtualizado, error) = await _profileApiService.UpdateAsync(dto);
            if (success && perfilAtualizado != null)
            {
                _perfilAtual = perfilAtualizado;
                PreencherCampos(perfilAtualizado);

                MessageBox.Show("✅ Perfil atualizado com sucesso!",
                    "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"❌ {error}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        //=================================================
        // ALTERAR SENHA (ABRE DIALOG SIMPLES CRIADO EM CÓDIGO)
        //=================================================
        private async void BtnAlterarSenha_Click(object sender, EventArgs e)
        {
            if (_perfilAtual == null)
            {
                MessageBox.Show("Aguarde o carregamento do perfil e tente novamente.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Warning);
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
                MessageBox.Show("✅ Senha alterada com sucesso!",
                    "Sucesso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show($"❌ {error}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Não foi possível identificar o usuário logado.",
                    "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            var conf = MessageBox.Show(
                "Tem certeza que deseja excluir sua conta?\n" +
                "Todos os seus dados serão removidos permanentemente. Esta ação não pode ser desfeita.",
                "Confirmar Exclusão de Conta",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Warning);

            if (conf != DialogResult.Yes) return;

            var (success, error) = await _usersApiService.DeleteAsync(userId);
            if (!success)
            {
                MessageBox.Show($"❌ {error}", "Erro",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show("Sua conta foi excluída. Você será desconectado.",
                "Conta Excluída", MessageBoxButtons.OK, MessageBoxIcon.Information);

            SessionManager.Instance.Clear();

            // Fecha o MainForm (pai desta UserControl), voltando para a tela de Login.
            FindForm()?.Close();
        }
    }
}