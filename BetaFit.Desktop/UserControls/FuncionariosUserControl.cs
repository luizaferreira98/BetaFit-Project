
using BetaFit.Desktop.DTOs;
using BetaFit.Desktop.Forms;
using BetaFit.Desktop.Helpers;
using BetaFit.Desktop.Services;
using Microsoft.Extensions.Logging.Abstractions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetaFit.Desktop.UserControls
{
    public partial class FuncionariosUserControl : UserControl
    {
        //=================================================
        // SERVIÇOS
        //=================================================
        private UsersApiService? _usersService = null;

        //=================================================
        // DADOS
        //=================================================
        private List<UsersResponseDto> _todosUsuarios = new();
        private List<UsersResponseDto> _funcionariosFiltrados = new();
        private int _paginaAtual = 1;
        private const int TamanhoPagina = 10;

        private List<string> _perfis = new() { "Admin", "Gerente", "Estoquista" };

        //=================================================
        // CONSTRUTOR
        //=================================================
        public FuncionariosUserControl()
        {
            InitializeComponent();
        }

        //=================================================
        // LOAD
        //=================================================
        private async void FuncionariosUserControl_Load(object sender, EventArgs e)
        {
            if (DesignMode)
                return;

            try
            {
                _usersService = new UsersApiService();

                ConfigurarPermissoes();

                // A tela representa exclusivamente a equipe interna.
                _perfis = new List<string> { "Admin", "Gerente", "Estoquista" };

                // Aplica o tema BetaFit ao grid (preto/lima, ver Themes/BetaFitTheme.cs)
                BetaFit.Desktop.Themes.BetaFitTheme.AplicarEstiloGridEscuro(gridFuncionarios);

                gridFuncionarios.SelectionMode =
                    DataGridViewSelectionMode.FullRowSelect;

                gridFuncionarios.MultiSelect = false;

                await CarregarDadosAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show(
                    $"Erro ao inicializar funcionários: {ex.Message}",
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

            btnNovoFuncionario.Visible = isAdmin;
            btnEditarFuncionario.Visible = isAdmin;
            btnExcluirFuncionario.Visible = isAdmin;
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

                _todosUsuarios = (usuarios ?? new List<UsersResponseDto>())
                    .Where(u => u.IsFuncionario)
                    .ToList();

                _funcionariosFiltrados = _todosUsuarios;
                _paginaAtual = 1;
                PopularGrid();
            }
            catch (Exception ex)
            {
                gridFuncionarios.Rows.Clear();

                MessageBox.Show(
                    $"Erro ao carregar funcionários: {ex.Message}",
                    "Erro",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
            }
        }

        //=================================================
        // POPULAR GRID
        //=================================================
        private void PopularGrid()
        {
            gridFuncionarios.Rows.Clear();

            int inicio = (_paginaAtual - 1) * TamanhoPagina;
            var pagina = _funcionariosFiltrados.Skip(inicio).Take(TamanhoPagina);

            foreach (var funcionario in pagina)
            {
                var cargo = funcionario.Roles.FirstOrDefault() ?? "-";
                var status = funcionario.Ativo ? "ATIVO" : "INATIVO";

                gridFuncionarios.Rows.Add(
                    funcionario.Id,
                    funcionario.UserName,
                    funcionario.Email,
                    cargo.ToUpperInvariant(),
                    status);
            }

            AtualizarPaginacao();
        }

        private void AtualizarPaginacao()
        {
            int total = _funcionariosFiltrados.Count;
            int totalPaginas = Math.Max(1, (int)Math.Ceiling(total / (double)TamanhoPagina));
            if (_paginaAtual > totalPaginas) _paginaAtual = totalPaginas;

            int inicio = total == 0 ? 0 : ((_paginaAtual - 1) * TamanhoPagina) + 1;
            int ate = Math.Min(_paginaAtual * TamanhoPagina, total);

            lblResumoFuncionarios.Text = total == 0
                ? "Nenhum funcionário encontrado"
                : $"Exibindo {inicio} a {ate} de {total} funcionários";

            btnPaginaAtual.Text = _paginaAtual.ToString();
            btnPaginaAnterior.Enabled = _paginaAtual > 1;
            btnProximaPagina.Enabled = _paginaAtual < totalPaginas;
        }

        private void txtPesquisa_TextChanged(object? sender, EventArgs e)
        {
            string termo = txtPesquisa.Text.Trim();

            _funcionariosFiltrados = string.IsNullOrWhiteSpace(termo)
                ? _todosUsuarios
                : _todosUsuarios.Where(u =>
                    u.UserName.Contains(termo, StringComparison.OrdinalIgnoreCase) ||
                    u.Email.Contains(termo, StringComparison.OrdinalIgnoreCase) ||
                    u.Roles.Any(r => r.Contains(termo, StringComparison.OrdinalIgnoreCase)))
                  .ToList();

            _paginaAtual = 1;
            PopularGrid();
        }

        private void btnPaginaAnterior_Click(object? sender, EventArgs e)
        {
            if (_paginaAtual <= 1) return;
            _paginaAtual--;
            PopularGrid();
        }

        private void btnProximaPagina_Click(object? sender, EventArgs e)
        {
            int totalPaginas = Math.Max(1, (int)Math.Ceiling(_funcionariosFiltrados.Count / (double)TamanhoPagina));
            if (_paginaAtual >= totalPaginas) return;
            _paginaAtual++;
            PopularGrid();
        }

        private void gridFuncionarios_CellClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            gridFuncionarios.ClearSelection();
            gridFuncionarios.Rows[e.RowIndex].Selected = true;
        }

        //=================================================
        // NOVO FUNCIONÁRIO
        //=================================================
        private async void btnNovoFuncionario_Click(object sender, EventArgs e)
        {
            using var form = new FuncionariosFormDialog(_perfis, null);
            if (form.ShowDialog() == DialogResult.OK && form.CreateDto != null)
            {
                var (success, _, error) = await _usersService!.CreateAsync(form.CreateDto);
                if (success)
                {
                    MessageBox.Show("Funcionário criado com sucesso!",
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
        // EDITAR FUNCIONÁRIO
        //=================================================
        private async void btnEditarFuncionario_Click(object sender, EventArgs e)
        {
            if (!SessionManager.Instance.IsAdmin)
                return;

            var funcionario = ObterFuncionarioSelecionado();
            if (funcionario == null)
            {
                MessageBox.Show("Selecione um funcionário para editar.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            using var form = new FuncionariosFormDialog(_perfis, funcionario);
            if (form.ShowDialog() == DialogResult.OK && form.UpdateDto != null)
            {
                var (success, _, error) = await _usersService!.UpdateAsync(funcionario.Id, form.UpdateDto);
                if (success)
                {
                    MessageBox.Show("Funcionário atualizado com sucesso!", "Sucesso",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await CarregarDadosAsync();
                }
                else
                {
                    MessageBox.Show(error, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        //=================================================
        // EXCLUIR FUNCIONÁRIO
        //=================================================
        private async void btnExcluirFuncionario_Click(object sender, EventArgs e)
        {
            if (!SessionManager.Instance.IsAdmin)
                return;

            var funcionario = ObterFuncionarioSelecionado();
            if (funcionario == null)
            {
                MessageBox.Show("Selecione um funcionário para excluir.", "Atenção",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.Equals(funcionario.Id, SessionManager.Instance.CurrentUser?.Id, StringComparison.Ordinal))
            {
                MessageBox.Show("O administrador atualmente logado não pode excluir a própria conta.",
                    "Operação não permitida", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            bool confirmou = BetaFitMessageBox.Confirmar(
                FindForm()!,
                $"Deseja realmente excluir o funcionário {funcionario.UserName}?",
                "Confirmar exclusão");

            if (!confirmou)
                return;

            var (success, error) = await _usersService!.DeleteAsync(funcionario.Id);
            if (success)
            {
                MessageBox.Show("Funcionário excluído com sucesso!", "Sucesso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                await CarregarDadosAsync();
            }
            else
            {
                MessageBox.Show(error, "Erro", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private UsersResponseDto? ObterFuncionarioSelecionado()
        {
            if (gridFuncionarios.SelectedRows.Count == 0)
                return null;

            var id = gridFuncionarios.SelectedRows[0].Cells[0].Value?.ToString();
            return _todosUsuarios.FirstOrDefault(u => string.Equals(u.Id, id, StringComparison.Ordinal));
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