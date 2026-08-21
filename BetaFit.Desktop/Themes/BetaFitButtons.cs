// =============================================================================
// BetaFit.Desktop - Themes/BetaFitButtons.cs
// =============================================================================
//  Estilização de botões no padrão BetaFit (.bf-btn / --primary / --dark /
//  --ghost / --danger do site.css), usando WinForms puro (sem dependência
//  de bibliotecas de terceiros).
//
//  Características replicadas do site:
//   - Caixa alta + peso forte no texto
//   - Cantos quase retos (BetaFitTheme.BorderRadius = 2px)
//   - Hover troca de cor (não anima elevação, mas troca de tom)
//   - Sem borda (exceto o botão "fantasma"/ghost)
// =============================================================================

using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace BetaFit.Desktop.Themes
{
    /// <summary>
    /// Helpers para estilizar System.Windows.Forms.Button no padrão visual do BetaFit.
    /// Uso: BetaFitButtons.EstilizarPrimario(btnSalvar);
    /// </summary>
    public static class BetaFitButtons
    {
        // =====================================================================
        // VARIANTES PÚBLICAS (equivalentes às classes .bf-btn--* do site)
        // =====================================================================

        /// <summary>Botão principal — fundo lima, texto preto. Ação de destaque (Salvar, Entrar, Confirmar).</summary>
        public static void EstilizarPrimario(Button btn) =>
            AplicarBase(btn, BetaFitTheme.BotaoPrimarioFundo, BetaFitTheme.BotaoPrimarioTexto,
                        BetaFitTheme.BotaoPrimarioHover, bordaTransparente: true);

        /// <summary>Botão escuro — fundo preto, texto branco. Ações secundárias importantes (Novo, Exportar).</summary>
        public static void EstilizarEscuro(Button btn) =>
            AplicarBase(btn, BetaFitTheme.BotaoEscuroFundo, BetaFitTheme.BotaoEscuroTexto,
                        BetaFitTheme.BotaoEscuroHover, bordaTransparente: true);

        /// <summary>Botão fantasma — transparente com borda fina. Ações neutras (Cancelar, Voltar).</summary>
        public static void EstilizarFantasma(Button btn)
        {
            AplicarBase(btn, BetaFitTheme.BotaoFantasmaFundo, BetaFitTheme.BotaoFantasmaTexto,
                        BetaFitTheme.Superficie, bordaTransparente: false);

            btn.FlatAppearance.BorderSize = 1;
            btn.FlatAppearance.BorderColor = BetaFitTheme.BotaoFantasmaBorda;

            btn.MouseEnter += (s, e) => btn.FlatAppearance.BorderColor = BetaFitTheme.BotaoFantasmaBordaHover;
            btn.MouseLeave += (s, e) => btn.FlatAppearance.BorderColor = BetaFitTheme.BotaoFantasmaBorda;
        }

        /// <summary>Botão de perigo — fundo vermelho, texto branco. Ações destrutivas (Excluir).</summary>
        public static void EstilizarPerigo(Button btn) =>
            AplicarBase(btn, BetaFitTheme.BotaoPerigoFundo, BetaFitTheme.BotaoPerigoTexto,
                        ClarearCor(BetaFitTheme.BotaoPerigoFundo, 0.12f), bordaTransparente: true);

        // =====================================================================
        // BASE COMUM
        // =====================================================================

        private static void AplicarBase(Button btn, Color fundo, Color texto, Color fundoHover, bool bordaTransparente)
        {
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = bordaTransparente ? 0 : 1;
            btn.BackColor = fundo;
            btn.ForeColor = texto;
            btn.Font = new Font(BetaFitTheme.FonteBase, 9f, FontStyle.Bold);
            btn.Cursor = Cursors.Hand;
            btn.UseVisualStyleBackColor = false;
            btn.TextAlign = ContentAlignment.MiddleCenter;

            // Estilo do site: texto do botão em caixa alta
            if (!string.IsNullOrEmpty(btn.Text))
                btn.Text = btn.Text.ToUpperInvariant();

            // Cantos levemente retos (BetaFitTheme.BorderRadius = 2px), nunca "pill"
            AplicarCantoReto(btn);
            btn.Resize += (s, e) => AplicarCantoReto(btn);

            // Hover: troca de tom (sem animação, igual ao comportamento base do site)
            btn.MouseEnter += (s, e) => btn.BackColor = fundoHover;
            btn.MouseLeave += (s, e) => btn.BackColor = btn.Enabled ? fundo : ClarearCor(fundo, 0.35f);

            // Estado desabilitado
            btn.EnabledChanged += (s, e) =>
            {
                btn.BackColor = btn.Enabled ? fundo : ClarearCor(fundo, 0.35f);
                btn.ForeColor = btn.Enabled ? texto : ClarearCor(texto, 0.35f);
            };
        }

        /// <summary>
        /// Aplica uma região com cantos levemente arredondados (BorderRadius=2),
        /// mantendo a estética "quase reta" da marca BetaFit.
        /// </summary>
        private static void AplicarCantoReto(Button btn)
        {
            int r = BetaFitTheme.BorderRadius * 2; // raio pequeno, só pra suavizar o pixel
            if (btn.Width <= 0 || btn.Height <= 0) return;

            var path = new GraphicsPath();
            var rect = new Rectangle(0, 0, btn.Width, btn.Height);

            if (r <= 0)
            {
                path.AddRectangle(rect);
            }
            else
            {
                path.AddArc(rect.X, rect.Y, r, r, 180, 90);
                path.AddArc(rect.Right - r, rect.Y, r, r, 270, 90);
                path.AddArc(rect.Right - r, rect.Bottom - r, r, r, 0, 90);
                path.AddArc(rect.X, rect.Bottom - r, r, r, 90, 90);
                path.CloseFigure();
            }

            btn.Region?.Dispose();
            btn.Region = new Region(path);
        }

        private static Color ClarearCor(Color cor, float fator)
        {
            int r = (int)(cor.R + (255 - cor.R) * fator);
            int g = (int)(cor.G + (255 - cor.G) * fator);
            int b = (int)(cor.B + (255 - cor.B) * fator);
            return Color.FromArgb(cor.A, Math.Min(r, 255), Math.Min(g, 255), Math.Min(b, 255));
        }
    }
}