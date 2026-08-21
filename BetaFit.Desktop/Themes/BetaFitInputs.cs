// =============================================================================
// BetaFit.Desktop - Themes/BetaFitInputs.cs
// =============================================================================
//  Estilização de campos de formulário no padrão BetaFit (.bf-input /
//  .bf-form input do site.css): fundo branco, borda reta cor "Linha",
//  e borda preta ao ganhar foco.
//
//  O TextBox padrão do WinForms não permite trocar a cor da borda
//  diretamente. Para reproduzir fielmente o visual do site (borda que
//  reage ao foco), este helper usa o truque clássico de "painel-moldura":
//  um Panel com BackColor = cor-da-borda envolve o TextBox (com
//  BorderStyle.None e Dock.Fill), simulando uma borda de 1px que muda
//  de cor conforme o estado de foco.
// =============================================================================

using System;
using System.Windows.Forms;

namespace BetaFit.Desktop.Themes
{
    /// <summary>
    /// Helpers para estilizar campos de entrada (TextBox) no padrão BetaFit.
    /// </summary>
    public static class BetaFitInputs
    {
        /// <summary>
        /// Envolve um TextBox em um Panel-moldura que simula a borda reta
        /// do site (cinza "Linha", ficando preta no foco). Use este método
        /// no lugar de estilizar o TextBox diretamente.
        /// </summary>
        /// <param name="container">Panel que já contém o TextBox (ex.: um Panel no Designer com o TextBox dentro, Dock=Fill)</param>
        /// <param name="campo">O TextBox a estilizar</param>
        public static void AplicarMoldura(Panel container, TextBox campo)
        {
            container.Padding = new Padding(1);
            container.BackColor = BetaFitTheme.InputBorda;

            campo.BorderStyle = BorderStyle.None;
            campo.Dock = DockStyle.Fill;
            campo.BackColor = BetaFitTheme.InputFundo;
            campo.ForeColor = BetaFitTheme.InputTexto;
            campo.Font = BetaFitTheme.FonteNormal;

            campo.Enter += (s, e) => container.BackColor = BetaFitTheme.InputBordaFoco;
            campo.Leave += (s, e) => container.BackColor = BetaFitTheme.InputBorda;
        }

        /// <summary>
        /// Estiliza um TextBox "solto" (sem painel-moldura), usado quando
        /// não é necessário o efeito de foco — ex.: campos somente leitura,
        /// caixas de busca simples, TextBox multiline dentro de outro container.
        /// </summary>
        public static void EstilizarSimples(TextBox campo)
        {
            campo.BorderStyle = BorderStyle.FixedSingle;
            campo.BackColor = BetaFitTheme.InputFundo;
            campo.ForeColor = BetaFitTheme.InputTexto;
            campo.Font = BetaFitTheme.FonteNormal;
        }

        /// <summary>Estiliza um ComboBox no mesmo padrão visual dos inputs.</summary>
        public static void EstilizarComboBox(ComboBox combo)
        {
            combo.FlatStyle = FlatStyle.Flat;
            combo.BackColor = BetaFitTheme.InputFundo;
            combo.ForeColor = BetaFitTheme.InputTexto;
            combo.Font = BetaFitTheme.FonteNormal;
        }

        /// <summary>Estiliza um Label usado como rótulo de campo (.bf-form label): caixa alta, cinza-escuro, bold.</summary>
        public static void EstilizarRotulo(Label lbl)
        {
            lbl.Font = BetaFitTheme.FonteRotulo;
            lbl.ForeColor = BetaFitTheme.TextoMuted;
            if (!string.IsNullOrEmpty(lbl.Text))
                lbl.Text = lbl.Text.ToUpperInvariant();
        }
    }
}