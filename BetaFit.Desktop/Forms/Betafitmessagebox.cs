// =============================================================================
// BetaFit.Desktop - Forms/BetaFitMessageBox.cs
// =============================================================================
//  CONCEITO: MessageBox padronizado
//
// Substitui TODOS os usos de System.Windows.Forms.MessageBox.Show(...) do
// projeto desktop por um popup com a identidade visual BetaFit (mesmo
// padrão do OrderDetailsFormDialog / ProductFormDialog / AlterarSenhaDialog):
// fundo preto (#0F0F0F), painéis em #0A0A0A, bordas levemente arredondadas,
// tipografia forte, botão primário em lima e botão "fantasma" para ações
// secundárias/cancelar. Antes, os MessageBox.Show(...) usavam o diálogo
// cru do Windows (branco, ícones do sistema), destoando do resto da UI.
//
// Construído 100% em código (sem Designer.cs), para ficar em um único
// arquivo fácil de manter e reaproveitar.
//
// USO (nos lugares que antes chamavam MessageBox.Show):
//   BetaFitMessageBox.Sucesso(this, "Categoria criada com sucesso!");
//   BetaFitMessageBox.Erro(this, "Não foi possível salvar.");
//   BetaFitMessageBox.Aviso(this, "Informe o nome da categoria.");
//   BetaFitMessageBox.Info(this, "Sua conta foi excluída. Você será desconectado.");
//   bool ok = BetaFitMessageBox.Confirmar(this, "Deseja realmente sair?", "Confirmar Logout");
// =============================================================================

using BetaFit.Desktop.Themes;
using Guna.UI2.WinForms;
using System;
using System.Drawing;
using System.Windows.Forms;

namespace BetaFit.Desktop.Forms
{
    /// <summary>Tipo visual do popup — define ícone, cor de destaque e título padrão.</summary>
    public enum BetaFitMessageType
    {
        Sucesso,
        Erro,
        Aviso,
        Info,
        Pergunta
    }

    /// <summary>
    /// Popup de mensagem/confirmação no padrão visual BetaFit. Substitui
    /// System.Windows.Forms.MessageBox em todo o projeto desktop.
    /// </summary>
    public partial class BetaFitMessageBox : Form
    {
        private readonly BetaFitMessageType _tipo;
        private readonly string _mensagem;
        private readonly bool _confirmacao;

        private Guna2Button _btnPrimario = null!;

        private BetaFitMessageBox(string mensagem, string titulo, BetaFitMessageType tipo, bool confirmacao)
        {
            _tipo = tipo;
            _mensagem = mensagem;
            _confirmacao = confirmacao;

            InitializeComponent(titulo);
        }

        // =====================================================================
        // API PÚBLICA — substitui diretamente MessageBox.Show(...)
        // =====================================================================

        /// <summary>Popup de sucesso (equivalente a MessageBoxIcon.Information em confirmações positivas).</summary>
        public static void Sucesso(IWin32Window? owner, string mensagem, string titulo = "Sucesso") =>
            Exibir(owner, mensagem, titulo, BetaFitMessageType.Sucesso, confirmacao: false);

        /// <summary>Popup de erro (equivalente a MessageBoxIcon.Error).</summary>
        public static void Erro(IWin32Window? owner, string mensagem, string titulo = "Erro") =>
            Exibir(owner, mensagem, titulo, BetaFitMessageType.Erro, confirmacao: false);

        /// <summary>Popup de aviso/validação (equivalente a MessageBoxIcon.Warning).</summary>
        public static void Aviso(IWin32Window? owner, string mensagem, string titulo = "Aviso") =>
            Exibir(owner, mensagem, titulo, BetaFitMessageType.Aviso, confirmacao: false);

        /// <summary>Popup informativo neutro (equivalente a MessageBoxIcon.Information).</summary>
        public static void Info(IWin32Window? owner, string mensagem, string titulo = "Aviso") =>
            Exibir(owner, mensagem, titulo, BetaFitMessageType.Info, confirmacao: false);

        /// <summary>
        /// Popup de confirmação (equivalente a MessageBox.Show(..., MessageBoxButtons.YesNo)).
        /// Retorna true quando o usuário confirma (botão primário / Sim).
        /// </summary>
        public static bool Confirmar(IWin32Window? owner, string mensagem, string titulo = "Confirmar")
        {
            using var dialog = new BetaFitMessageBox(mensagem, titulo, BetaFitMessageType.Pergunta, confirmacao: true);
            var resultado = owner != null ? dialog.ShowDialog(owner) : dialog.ShowDialog();
            return resultado == DialogResult.Yes;
        }

        private static void Exibir(IWin32Window? owner, string mensagem, string titulo, BetaFitMessageType tipo, bool confirmacao)
        {
            using var dialog = new BetaFitMessageBox(mensagem, titulo, tipo, confirmacao);
            if (owner != null) dialog.ShowDialog(owner);
            else dialog.ShowDialog();
        }

        // =====================================================================
        // MONTAGEM VISUAL (segue o mesmo padrão de OrderDetailsFormDialog /
        // ProductFormDialog: FormBorderStyle.None, fundo #0F0F0F, painel
        // escuro #0A0A0A, cantos levemente arredondados, close "X" no topo).
        // =====================================================================

        private void InitializeComponent(string titulo)
        {
            SuspendLayout();

            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterParent;
            ShowInTaskbar = false;
            ClientSize = new Size(420, 240);
            BackColor = Color.FromArgb(15, 15, 15);
            Text = titulo;
            KeyPreview = true;

            var (corDestaque, glifo) = ObterAcento();

            // -----------------------------------------------------------
            // Painel principal (o "cartão" escuro com cantos suaves)
            // -----------------------------------------------------------
            var pnlCartao = new Guna2Panel
            {
                BackColor = Color.FromArgb(10, 10, 10),
                BorderRadius = BetaFitTheme.Admin.ModalBorderRadius,
                Location = new Point(0, 0),
                Size = ClientSize,
                Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right
            };
            Controls.Add(pnlCartao);

            // Botão fechar "X"
            var btnFechar = new Guna2CircleButton
            {
                Anchor = AnchorStyles.Top | AnchorStyles.Right,
                Cursor = Cursors.Hand,
                FillColor = Color.Transparent,
                Font = new Font(BetaFitTheme.FonteBase, 9.75F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(ClientSize.Width - 44, 12),
                Size = new Size(32, 32),
                Text = "X",
                TabStop = false
            };
            btnFechar.Click += (s, e) =>
            {
                DialogResult = _confirmacao ? DialogResult.No : DialogResult.None;
                Close();
            };
            pnlCartao.Controls.Add(btnFechar);

            // -----------------------------------------------------------
            // Selo/ícone circular com a cor de destaque do tipo de mensagem
            // -----------------------------------------------------------
            var pnlIcone = new Guna2Panel
            {
                BackColor = corDestaque,
                BorderRadius = 24,
                Size = new Size(48, 48),
                Location = new Point(28, 26)
            };
            pnlCartao.Controls.Add(pnlIcone);

            var lblGlifo = new Guna2HtmlLabel
            {
                BackColor = Color.Transparent,
                Text = glifo,
                Font = new Font(BetaFitTheme.FonteBase, 18F, FontStyle.Bold),
                ForeColor = CorTextoSobre(corDestaque),
                AutoSize = false,
                Size = pnlIcone.Size,
                TextAlignment = ContentAlignment.MiddleCenter
            };
            pnlIcone.Controls.Add(lblGlifo);

            // -----------------------------------------------------------
            // Título
            // -----------------------------------------------------------
            var lblTitulo = new Guna2HtmlLabel
            {
                BackColor = Color.Transparent,
                Text = titulo.ToUpperInvariant(),
                Font = new Font(BetaFitTheme.FonteBase, 14F, FontStyle.Bold),
                ForeColor = Color.White,
                Location = new Point(92, 30),
                Size = new Size(ClientSize.Width - 92 - 56, 32),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right
            };
            pnlCartao.Controls.Add(lblTitulo);

            // -----------------------------------------------------------
            // Mensagem
            // -----------------------------------------------------------
            var lblMensagem = new Guna2HtmlLabel
            {
                BackColor = Color.Transparent,
                Text = _mensagem,
                Font = new Font(BetaFitTheme.FonteBase, 10F, FontStyle.Regular),
                ForeColor = Color.FromArgb(210, 210, 210),
                Location = new Point(28, 90),
                Size = new Size(ClientSize.Width - 56, 88),
                Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right | AnchorStyles.Bottom
            };
            pnlCartao.Controls.Add(lblMensagem);

            // -----------------------------------------------------------
            // Rodapé de botões
            // -----------------------------------------------------------
            int alturaBotao = 40;
            int larguraBotaoPrimario = _confirmacao ? 130 : 140;
            int y = ClientSize.Height - alturaBotao - 24;

            _btnPrimario = new Guna2Button
            {
                Text = _confirmacao ? "SIM, CONFIRMAR" : "ENTENDI",
                Font = new Font(BetaFitTheme.FonteBase, 9.5F, FontStyle.Bold),
                Size = new Size(larguraBotaoPrimario, alturaBotao),
                Location = new Point(ClientSize.Width - larguraBotaoPrimario - 24, y),
                Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                BorderRadius = 6,
                FillColor = _tipo == BetaFitMessageType.Erro ? BetaFitTheme.Perigo : BetaFitTheme.Admin.Lima,
                ForeColor = _tipo == BetaFitMessageType.Erro ? Color.White : BetaFitTheme.PretoPrimario,
                Cursor = Cursors.Hand,
                TabIndex = 0
            };
            _btnPrimario.Click += (s, e) =>
            {
                DialogResult = _confirmacao ? DialogResult.Yes : DialogResult.OK;
                Close();
            };
            pnlCartao.Controls.Add(_btnPrimario);
            AcceptButton = null; // evita disparo por Enter em campos de fundo enquanto o modal está aberto

            if (_confirmacao)
            {
                var btnSecundario = new Guna2Button
                {
                    Text = "CANCELAR",
                    Font = new Font(BetaFitTheme.FonteBase, 9.5F, FontStyle.Bold),
                    Size = new Size(110, alturaBotao),
                    Location = new Point(_btnPrimario.Left - 110 - 12, y),
                    Anchor = AnchorStyles.Bottom | AnchorStyles.Right,
                    BorderRadius = 6,
                    FillColor = BetaFitTheme.Admin.BotaoSecundarioFundo,
                    ForeColor = BetaFitTheme.Admin.BotaoSecundarioTexto,
                    BorderColor = BetaFitTheme.Admin.BotaoSecundarioBorda,
                    BorderThickness = 1,
                    Cursor = Cursors.Hand,
                    TabIndex = 1
                };
                btnSecundario.Click += (s, e) =>
                {
                    DialogResult = DialogResult.No;
                    Close();
                };
                pnlCartao.Controls.Add(btnSecundario);
            }

            // Esc fecha o popup (equivale a Cancelar/Não em confirmações)
            KeyDown += (s, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                {
                    DialogResult = _confirmacao ? DialogResult.No : DialogResult.None;
                    Close();
                }
                else if (e.KeyCode == Keys.Enter)
                {
                    _btnPrimario.PerformClick();
                }
            };

            ResumeLayout(false);
        }

        /// <summary>Cor de destaque + glifo (texto) do selo, de acordo com o tipo de mensagem.</summary>
        private (Color cor, string glifo) ObterAcento() => _tipo switch
        {
            BetaFitMessageType.Sucesso => (BetaFitTheme.Admin.Lima, "✓"),
            BetaFitMessageType.Erro => (BetaFitTheme.Perigo, "✕"),
            BetaFitMessageType.Aviso => (Color.FromArgb(240, 217, 140), "!"),
            BetaFitMessageType.Pergunta => (BetaFitTheme.Admin.Lima, "?"),
            _ => (Color.FromArgb(185, 203, 214), "i"),
        };

        /// <summary>Preto para selos claros (lima/amarelo), branco para selos escuros (perigo).</summary>
        private static Color CorTextoSobre(Color fundo) =>
            fundo == BetaFitTheme.Perigo ? Color.White : BetaFitTheme.PretoPrimario;
    }
}