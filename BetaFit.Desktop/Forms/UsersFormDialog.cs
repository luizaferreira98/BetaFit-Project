using BetaFit.Desktop.DTOs;
using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace BetaFit.Desktop.Forms
{
    /// <summary>
    /// Formulário de criação/edição de usuários.
    /// Retorna CreateUsersDto (criação) ou UpdateUsersDto (edição).
    /// </summary>
    public partial class UsersFormDialog : Form
    {
        // =====================================================================
        // PROPRIEDADES DE SAÍDA
        // =====================================================================
        /// <summary>DTO preenchido quando no modo de criação (OK)</summary>
        public CreateUsersDto? CreateDto { get; private set; }

        /// <summary>DTO preenchido quando no modo de edição (OK)</summary>
        public UpdateUsersDto? UpdateDto { get; private set; }

        // =====================================================================
        // CAMPOS PRIVADOS
        // =====================================================================

        private readonly UsersResponseDto? _usuarioExistente;

        private List<string> _perfis = new();

        // =====================================================================
        // CONSTRUTOR
        // =====================================================================
        /// <param name="usuariosExistentes">Lista de usuários já cadastrados (para validação de duplicidade, etc.)</param>
        /// <param name="usuarioExistente">Usuário a editar; null = modo criação</param>
        public UsersFormDialog(List<string> perfis, UsersResponseDto? usuarioExistente)
        {
            InitializeComponent();

            _perfis = perfis;
            _usuarioExistente = usuarioExistente;

            PreencherComboPerfis();

            if (_usuarioExistente != null)
            {

                lblTitulo.Text = "Editar Usuário";
                lblNome.Text = _usuarioExistente.UserName;
                txtEmail.Text = _usuarioExistente.Email;


                if (cmbRoles.Items.Contains(_usuarioExistente.Roles))
                {
                    cmbRoles.SelectedItem = _usuarioExistente.Roles;
                }
            }
            else
            {
                lblTitulo.Text = "Novo Usuário";
                if (cmbRoles.Items.Count > 0)
                    cmbRoles.SelectedIndex = 0;
            }
        }



        // =====================================================================
        // PREENCHER COMBO DE PERFIS
        // =====================================================================
        private void PreencherComboPerfis()
        {
            cmbRoles.Items.Clear();
            foreach (var p in _perfis)
            {
                cmbRoles.Items.Add(p);
            }
        }

        // =====================================================================
        // BOTÃO SALVAR — Ajuste os nomes dos controles conforme seu Designer
        // ====================================================================


        private void btnSalvar_Click(object sender, EventArgs e)
        {




            if (string.IsNullOrWhiteSpace(txtEmail.Text))
            {
                MessageBox.Show("Email é obrigatório.", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_usuarioExistente == null && string.IsNullOrWhiteSpace(txtSenha.Text))
            {
                MessageBox.Show("Senha é obrigatória para novos usuários.", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (txtSenha.Text != txtConfSenha.Text)
            {
                MessageBox.Show("As senhas não coincidem.", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (cmbRoles.SelectedItem == null)
            {
                MessageBox.Show("Selecione um perfil.", "Validação",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_usuarioExistente == null)
            {
                CreateDto = new CreateUsersDto
                {
                    Email = txtEmail.Text.Trim(),
                    Password = txtSenha.Text,
                    ConfirmPassword = txtConfSenha.Text,
                    Role = cmbRoles.SelectedItem.ToString()!
                };
            }
            else
            {
                UpdateDto = new UpdateUsersDto
                {
                    Email = txtEmail.Text.Trim(),
                    Password = string.IsNullOrEmpty(txtSenha.Text) ? null : txtSenha.Text,
                    ConfirmPassword = string.IsNullOrEmpty(txtConfSenha.Text) ? null : txtConfSenha.Text,
                    Role = cmbRoles.SelectedItem.ToString()!
                };
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }

        private void btnCancelar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }

        private void btnFechar_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
            this.Close();
        }
    }
}
