using BetaFit.Desktop.Services;
using BetaFit.Desktop.Themes;

namespace BetaFit.Desktop.Forms;

public partial class LoginForm : Form
{
    private  AuthApiService _auth = new();
    private TextBox _email = new();
    private  TextBox _password = new();
    private  Button _login = new();
    private  Label _error = new();


    public LoginForm()
    {
        // Apply minimal theming using BetaFitTheme palette
        Text = "BETAFIT / ADMIN";
        ClientSize = new Size(980, 620);
        MinimumSize = new Size(800, 520);
        Build();
    }
    private void LoginForm_Load(object sender, EventArgs e)
    {
        //Guard: não executa em tempo de design
        if (DesignMode) return;

        _auth = new AuthApiService();

        lblVersao.Text = $"Versão {AppConfig.Version} | ©️ {DateTime.Now.Year} SENAC-SMP";
        lblApi.Text = $"API: {AppConfig.ApiBaseUrl}";

        txtEmail.Text = "admin@betafit.com";
        txtSenha.Text = "Admin@123";
    }
    private void Build()
    {
        var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, RowCount = 1, BackColor = BetaFitTheme.Superficie };
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55));
        root.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45));
        Controls.Add(root);

        var branding = new Panel { Dock = DockStyle.Fill, Padding = new Padding(60) };
        var badge = new Label { Text = "BETA FIT", AutoSize = true, ForeColor = BetaFitTheme.Lima, Font = new Font("Segoe UI", 12, FontStyle.Bold), Location = new Point(60, 100) };
        var title = new Label { Text = "BETAFIT\nADMIN", AutoSize = true, ForeColor = Color.White, Font = new Font("Segoe UI", 38, FontStyle.Bold), Location = new Point(60, 145) };
        var subtitle = new Label { Text = "CONTROLE O CATÁLOGO.\nACOMPANHE A OPERAÇÃO.\nGERENCIE SUA PLATAFORMA.", AutoSize = true, ForeColor = BetaFitTheme.TextoMuted, Font = new Font("Segoe UI", 11, FontStyle.Regular), Location = new Point(65, 275) };
        branding.Controls.AddRange([badge, title, subtitle]);
        root.Controls.Add(branding, 0, 0);

        var panel = new Panel { BackColor = BetaFitTheme.Branco };
        panel.Dock = DockStyle.Fill;
        panel.Margin = new Padding(30, 90, 55, 90);
        var inner = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = 8 };
        inner.RowStyles.Add(new RowStyle(SizeType.Absolute, 55));
        inner.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
        inner.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        inner.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
        inner.RowStyles.Add(new RowStyle(SizeType.Absolute, 28));
        inner.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
        inner.RowStyles.Add(new RowStyle(SizeType.Absolute, 45));
        inner.RowStyles.Add(new RowStyle(SizeType.Percent, 100));
        panel.Controls.Add(inner);

        var header = new Label { Text = "ENTRAR", ForeColor = Color.White, Font = new Font("Segoe UI", 20, FontStyle.Bold), Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleLeft };
        inner.Controls.Add(header, 0, 0);
        inner.Controls.Add(new Label { Text = "E-MAIL", ForeColor = BetaFitTheme.Lima, Font = new Font("Segoe UI", 9, FontStyle.Bold), Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft }, 0, 1);
        ConfigureInput(_email, "seu@email.com"); inner.Controls.Add(_email, 0, 2);
        inner.Controls.Add(new Label { Text = "SENHA", ForeColor = BetaFitTheme.Lima, Font = new Font("Segoe UI", 9, FontStyle.Bold), Dock = DockStyle.Fill, TextAlign = ContentAlignment.BottomLeft }, 0, 3);
        ConfigureInput(_password, "••••••••"); _password.UseSystemPasswordChar = true; inner.Controls.Add(_password, 0, 4);
        _login.Text = "ENTRAR"; _login.Dock = DockStyle.Top; _login.Click += LoginClicked; _login.BackColor = BetaFitTheme.Lima; _login.ForeColor = Color.Black; _login.FlatStyle = FlatStyle.Flat; inner.Controls.Add(_login, 0, 5);
        _error.Text = ""; _error.ForeColor = BetaFitTheme.Perigo; _error.AutoSize = true; _error.Dock = DockStyle.Top; inner.Controls.Add(_error, 0, 6);
        panel.Anchor = AnchorStyles.None;
        root.Controls.Add(panel, 1, 0);

        AcceptButton = _login;
    }

    private static void ConfigureInput(TextBox box, string placeholder)
    {
        box.Dock = DockStyle.Fill;
        box.PlaceholderText = placeholder;
        // Basic input styling
        box.BorderStyle = BorderStyle.FixedSingle;
        box.BackColor = Color.White;
        box.Margin = new Padding(0, 3, 0, 8);
    }

    private async void LoginClicked(object? sender, EventArgs e)
    {
        _error.Text = "";
        if (string.IsNullOrWhiteSpace(_email.Text) || string.IsNullOrWhiteSpace(_password.Text))
        { _error.Text = "INFORME E-MAIL E SENHA."; return; }
        SetBusy(true);
        var result = await _auth.LoginAsync(_email.Text, _password.Text);
        SetBusy(false);
        if (!result.Success || result.User is null)
        { _error.Text = string.IsNullOrWhiteSpace(result.Error) ? "NÃO FOI POSSÍVEL ENTRAR." : result.Error.ToUpperInvariant(); return; }
        if (!result.User.IsAdmin)
        { await _auth.LogoutAsync(); _error.Text = "ACESSO RESTRITO A ADMINISTRADORES."; return; }
        Hide();
        using var main = new MainForm();
        main.ShowDialog(this);
        Close();
    }

    private void SetBusy(bool busy)
    {
        _login.Enabled = !busy;
        _login.Text = busy ? "ENTRANDO..." : "ENTRAR";
        Cursor = busy ? Cursors.WaitCursor : Cursors.Default;
    }
}
