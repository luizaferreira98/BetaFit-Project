using BetaFit.Desktop.DTOs;
using BetaFit.Desktop.Forms;
using BetaFit.Desktop.Helpers;
using BetaFit.Desktop.Services;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetaFit.Desktop.UserControls
{
    public partial class UsuariosUserControl : UserControl
    {
        //=================================================
        // SERVIÇOS
        //=================================================
        private UsersApiService? _usersService = null;

        //=================================================
        // DADOS
        //=================================================
        private List<UsersResponseDto> _todosUsuarios = new();

        private List<string> _perfis = new();

        //=================================================
        // CONSTRUTOR
        //=================================================
        public UsuariosUserControl()
        {
            InitializeComponent();
        }

        //=================================================
        // LOAD
        //=================================================
        private async void UsuariosUserControl_Load(object sender, EventArgs e)
        {
            if (DesignMode)
                return;

            try
            {
                _usersService = new UsersApiService();

                ConfigurarPermissoes();

                // Aplica o tema BetaFit ao grid (preto/lima, ver Themes/BetaFitTheme.cs)
                BetaFit.Desktop.Themes.BetaFitTheme.AplicarEstiloGrid(gridUsuarios);

                gridUsuarios.SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect;

                gridUsuarios.MultiSelect = false;

                await CarregarDadosAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao inicializar usuários: {ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
        }

        //=================================================
        // PERMISSÕES
        //=================================================
        private void ConfigurarPermissoes()
        {
            bool isAdmin = SessionManager.Instance.IsAdmin;

            btnNovoUsuario.Visible = isAdmin;
            btnEditarUsuario.Visible = isAdmin;
            btnExcluirUsuario.Visible = isAdmin;
        }

        //=================================================
        // CARREGAR DADOS
        //=================================================
        private async Task CarregarDadosAsync()
        {
            if (_usersService == null)
                return;

            try
            {
                var usuarios = await _usersService.GetAllAsync();

                _todosUsuarios = usuarios ?? new List<UsersResponseDto>();

                PopularGrid(_todosUsuarios);
            }
            catch (Exception ex)
            {
                gridUsuarios.Rows.Clear();

                MessageBox.Show(
                    $"Erro ao carregar usuários: {ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        //=================================================
        // POPULAR GRID
        //=================================================
        private void PopularGrid(List<UsersResponseDto> usuarios)
        {
            gridUsuarios.Rows.Clear();

            foreach (var usuario in usuarios)
            {
                gridUsuarios.Rows.Add(
                    usuario.Id,
                    usuario.UserName,
                    usuario.Email
                );
            }
        }

        //=================================================
        // NOVO USUÁRIO
        //=================================================
        private async void btnNovoUsuario_Click(object sender, EventArgs e)
        {
            using var form = new UsersFormDialog(_perfis, null);
            if (form.ShowDialog() == DialogResult.OK && form.CreateDto != null)
            {
                var (success, _, error) = await _usersService!.CreateAsync(form.CreateDto);
                if (success)
                {
                    MessageBox.Show("✅ Game criado com sucesso!",
                        "Sucesso",
                        MessageBoxButtons.OK,
                        MessageBoxIcon.Information);
                    await CarregarDadosAsync();
                }
                else
                {
                    MessageBox.Show($"❌ {error}",
                      "Erro",
                      MessageBoxButtons.OK,
                      MessageBoxIcon.Error);
                }
            }
        }

        //=================================================
        // OBTÉM CONTROLE PRIVADO DO FORMULÁRIO
        //=================================================
        private static Control? ObterControlePrivado(
            Control formulario,
            string nomeControle)
        {
            try
            {
                FieldInfo? field = formulario
                    .GetType()
                    .GetField(
                        nomeControle,
                        BindingFlags.Instance |
                        BindingFlags.NonPublic);

                return field?.GetValue(formulario) as Control;
            }
            catch
            {
                return null;
            }
        }

        //=================================================
        // OBTÉM TEXTO DE CONTROLE
        //=================================================
        private static string ObterTextoControle(
            Control formulario,
            string nomeControle)
        {
            Control? controle =
                ObterControlePrivado(formulario, nomeControle);

            if (controle == null)
                return string.Empty;

            try
            {
                PropertyInfo? textProperty =
                    controle.GetType().GetProperty("Text");

                if (textProperty != null)
                {
                    return textProperty
                        .GetValue(controle)?
                        .ToString()
                        ?.Trim() ?? string.Empty;
                }

                PropertyInfo? selectedItemProperty =
                    controle.GetType().GetProperty("SelectedItem");

                if (selectedItemProperty != null)
                {
                    return selectedItemProperty
                        .GetValue(controle)?
                        .ToString()
                        ?.Trim() ?? string.Empty;
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}