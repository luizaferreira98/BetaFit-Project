// =============================================================================
// BetaFit.Desktop - Themes/BetaFitTheme.cs
// =============================================================================
//  CONCEITO: Design System / Theme Manager
//
// Centraliza TODAS as cores, fontes e estilos da aplicação desktop.
// Por que centralizar?
//    Mudança de cor em um lugar  aplica em toda a aplicação
//    Consistência visual garantida
//    Facilita manutenção e customização
//
// Paleta oficial extraída de BetaFit.UI/wwwroot/css/site.css (:root)
//   Preto (marca):     #0B0B0B (bf-black)  / #151515 (bf-black-2)
//   Tinta (texto):      #111111 (bf-ink)
//   Branco:             #FFFFFF
//   Superfície:         #F6F6F3 (bf-surface) / #EEEEEA (bf-surface-2)
//   Linha (bordas):     #DEDED9 (bf-line)
//   Texto secundário:   #6F706C (bf-muted)
//   Lima (destaque):    #C9FF22 (bf-lime) / #9BC900 (bf-lime-dark)
//   Perigo:             #B83A34 (bf-danger)
//   Raio de borda:      2px (bf-radius) — visual quase reto, NUNCA arredondado
//
// Estilo: minimalista, "streetwear", preto & branco com um único acento
// (lima), tipografia em CAIXA ALTA e peso forte (Montserrat 800/900),
// bordas retas, muito espaço em branco. É o oposto de um visual "fofo"
// com cantos arredondados — por isso BorderRadius aqui é propositalmente
// pequeno (2px), diferente de temas mais "corporate/fluent".
//
// Inspiração:
//   - BetaFit.UI (Razor) — wwwroot/css/site.css
//   - Estética streetwear / sneaker drop (preto + lima, tipografia bold)
// =============================================================================

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.Windows.Forms;
using Guna.UI2.WinForms;

namespace BetaFit.Desktop.Themes
{
    /// <summary>
    /// Tema visual oficial do BetaFit Desktop.
    /// Define todas as cores, fontes e dimensões usadas na interface,
    /// espelhando a paleta usada na BetaFit.UI (site.css).
    /// </summary>
    public static class BetaFitTheme
    {
        // =====================================================================
        // PALETA DE CORES BETAFIT
        // =====================================================================

        /// <summary>Preto de marca — sidebar, header admin, botões escuros, hero</summary>
        public static Color PretoPrimario => Color.FromArgb(11, 11, 11);        // #0B0B0B

        /// <summary>Preto variante — hover de superfícies escuras</summary>
        public static Color PretoSecundario => Color.FromArgb(21, 21, 21);      // #151515

        /// <summary>Tinta — cor de texto principal (quase preto, mais suave que o preto de marca)</summary>
        public static Color Tinta => Color.FromArgb(17, 17, 17);                // #111111

        /// <summary>Branco puro — fundos principais, texto sobre preto</summary>
        public static Color Branco => Color.White;

        /// <summary>Superfície — fundo levemente acinzentado (seções soft, painéis)</summary>
        public static Color Superficie => Color.FromArgb(246, 246, 243);        // #F6F6F3

        /// <summary>Superfície alternativa — hover sutil sobre a superfície</summary>
        public static Color SuperficieAlt => Color.FromArgb(238, 238, 234);     // #EEEEEA

        /// <summary>Linha — bordas e separadores</summary>
        public static Color Linha => Color.FromArgb(222, 222, 217);             // #DEDED9

        /// <summary>Texto secundário / placeholder / labels mudos</summary>
        public static Color TextoMuted => Color.FromArgb(111, 112, 108);        // #6F706C

        /// <summary>Lima — cor de destaque da marca (CTAs primários, ativo, ícones)</summary>
        public static Color Lima => Color.FromArgb(201, 255, 34);               // #C9FF22

        /// <summary>Lima escuro — hover/estado ativo sobre fundo claro, links</summary>
        public static Color LimaEscuro => Color.FromArgb(155, 201, 0);          // #9BC900

        /// <summary>Lima claro — fundo suave para badges/seleção sobre fundo lima</summary>
        public static Color LimaClaro => Color.FromArgb(242, 248, 223);         // aprox. #F2F8DF (bf-alert--success)

        /// <summary>Perigo — ações destrutivas, erros</summary>
        public static Color Perigo => Color.FromArgb(184, 58, 52);              // #B83A34

        // =====================================================================
        // CORES SEMÂNTICAS (STATUS) — valores reais extraídos dos .bf-alert do site
        // =====================================================================

        public static Color SucessoTexto => Color.FromArgb(82, 103, 11);        // #52670B
        public static Color SucessoFundo => Color.FromArgb(242, 248, 223);      // #F2F8DF
        public static Color SucessoBorda => Color.FromArgb(207, 224, 141);      // #CFE08D

        public static Color PerigoTexto => Color.FromArgb(140, 48, 43);         // #8C302B
        public static Color PerigoFundo => Color.FromArgb(255, 241, 240);       // #FFF1F0
        public static Color PerigoBorda => Color.FromArgb(231, 189, 185);       // #E7BDB9

        // Aviso e Info não existem no site (BetaFit só usa sucesso/erro/neutro),
        // mas são úteis no desktop admin — seguem a MESMA linguagem visual
        // (fundo claro + texto escuro + borda), só que dessaturados.
        public static Color AvisoTexto => Color.FromArgb(122, 93, 0);           // #7A5D00
        public static Color AvisoFundo => Color.FromArgb(255, 246, 217);        // #FFF6D9
        public static Color AvisoBorda => Color.FromArgb(240, 217, 140);        // #F0D98C

        public static Color InfoTexto => Color.FromArgb(44, 76, 97);            // #2C4C61
        public static Color InfoFundo => Color.FromArgb(231, 238, 243);         // #E7EEF3
        public static Color InfoBorda => Color.FromArgb(185, 203, 214);         // #B9CBD6

        /// <summary>Alerta neutro — mesmo cinza usado em .bf-alert (padrão, sem variante)</summary>
        public static Color NeutroTexto => Color.FromArgb(70, 71, 67);          // #464743
        public static Color NeutroFundo => Color.FromArgb(241, 241, 237);       // #F1F1ED

        // =====================================================================
        // BADGES DE STATUS (ex.: produto ativo/inativo) — valores reais do site
        // =====================================================================

        public static Color BadgeAtivoFundo => Color.FromArgb(237, 247, 200);   // #EDF7C8
        public static Color BadgeAtivoTexto => Color.FromArgb(97, 122, 0);      // #617A00
        public static Color BadgeInativoFundo => Color.FromArgb(239, 239, 236); // #EFEFEC
        public static Color BadgeInativoTexto => Color.FromArgb(119, 120, 115); // #777873

        // =====================================================================
        // SIDEBAR (fundo preto, igual ao header admin do site)
        // =====================================================================

        public static Color SidebarFundo => PretoPrimario;
        public static Color SidebarTexto => Color.White;
        public static Color SidebarTextoMuted => Color.FromArgb(158, 158, 154); // #9E9E9A
        public static Color SidebarBotaoHover => PretoSecundario;
        public static Color SidebarBotaoAtivoFundo => Lima;
        public static Color SidebarBotaoAtivoTexto => PretoPrimario;
        public static Color SidebarDivisor => Color.FromArgb(41, 41, 41);       // #292929

        // =====================================================================
        // CABEÇALHO (HEADER) — fundo claro, como o header do site
        // =====================================================================

        public static Color HeaderFundo => Branco;
        public static Color HeaderBorda => Linha;
        public static Color HeaderTexto => Tinta;

        // =====================================================================
        // CARDS (DASHBOARD)
        // =====================================================================

        public static Color CardFundo => Branco;
        public static Color CardBorda => Linha;
        public static Color CardSombra => Color.FromArgb(20, 0, 0, 0);          // rgba(0,0,0,.08)

        // =====================================================================
        // FORMULÁRIOS — no site os inputs são SEMPRE de canto reto (radius:0)
        // =====================================================================

        public static Color InputFundo => Branco;
        public static Color InputBorda => Linha;
        public static Color InputBordaFoco => PretoPrimario;
        public static Color InputTexto => PretoPrimario;
        public static Color InputPlaceholder => Color.FromArgb(161, 161, 157);  // #A1A19D

        // =====================================================================
        // BOTÕES (equivalentes a .bf-btn--primary / --dark / --ghost / --danger)
        // =====================================================================

        public static Color BotaoPrimarioFundo => Lima;
        public static Color BotaoPrimarioTexto => PretoPrimario;
        public static Color BotaoPrimarioHover => Color.FromArgb(215, 255, 84); // #D7FF54

        public static Color BotaoEscuroFundo => PretoPrimario;
        public static Color BotaoEscuroTexto => Branco;
        public static Color BotaoEscuroHover => Color.FromArgb(43, 43, 43);     // #2B2B2B

        public static Color BotaoFantasmaFundo => Color.Transparent;
        public static Color BotaoFantasmaTexto => PretoPrimario;
        public static Color BotaoFantasmaBorda => Linha;
        public static Color BotaoFantasmaBordaHover => PretoPrimario;

        public static Color BotaoPerigoFundo => Perigo;
        public static Color BotaoPerigoTexto => Branco;

        // =====================================================================
        // DATAGRIDVIEW
        // =====================================================================

        public static Color GridCabecalhoFundo => PretoPrimario;
        public static Color GridCabecalhoTexto => Branco;
        public static Color GridLinhaPar => Branco;
        public static Color GridLinhaImpar => Superficie;
        public static Color GridLinhaSelecionada => Color.FromArgb(245, 255, 209); // tint claro de Lima
        public static Color GridTextoPrincipal => Tinta;
        public static Color GridBorda => Linha;

        // =====================================================================
        // TIPOGRAFIA
        // =====================================================================

        /// <summary>
        /// Fonte da marca (site usa "Montserrat"). Se não estiver instalada
        /// na máquina do usuário (não é fonte padrão do Windows), cai para
        /// "Segoe UI" automaticamente para não quebrar o layout.
        /// </summary>
        public static string FonteBase => ObterFonteDisponivel();

        private static string? _fonteResolvida;

        private static string ObterFonteDisponivel()
        {
            if (_fonteResolvida != null) return _fonteResolvida;

            using var instaladas = new InstalledFontCollection();
            foreach (var familia in instaladas.Families)
            {
                if (string.Equals(familia.Name, "Montserrat", StringComparison.OrdinalIgnoreCase))
                {
                    _fonteResolvida = "Montserrat";
                    return _fonteResolvida;
                }
            }

            // Fallback seguro: sempre existe no Windows
            _fonteResolvida = "Segoe UI";
            return _fonteResolvida;
        }

        public static Font FontePequena => new(FonteBase, 8f);
        public static Font FonteNormal => new(FonteBase, 9f);
        public static Font FonteMedia => new(FonteBase, 10f);

        /// <summary>Rótulos em caixa alta (ex.: labels de formulário, cabeçalho de grid)</summary>
        public static Font FonteRotulo => new(FonteBase, 8.5f, FontStyle.Bold);

        public static Font FonteSubtitulo => new(FonteBase, 11f, FontStyle.Bold);
        public static Font FonteTitulo => new(FonteBase, 14f, FontStyle.Bold);
        public static Font FonteGrande => new(FonteBase, 20f, FontStyle.Bold);
        public static Font FonteNumero => new(FonteBase, 30f, FontStyle.Bold);

        // =====================================================================
        // DIMENSÕES
        // =====================================================================

        /// <summary>Largura da sidebar lateral</summary>
        public static int SidebarLargura => 230;

        /// <summary>Altura do cabeçalho superior</summary>
        public static int HeaderAltura => 64;

        /// <summary>
        /// Raio de borda padrão. Propositalmente pequeno (quase reto),
        /// pois o site usa --bf-radius: 2px — NÃO arredondar demais os
        /// controles, isso descaracteriza a identidade visual do BetaFit.
        /// </summary>
        public static int BorderRadius => 2;

        /// <summary>Espaçamento interno padrão (padding)</summary>
        public static int Padding => 16;

        // =====================================================================
        // MÉTODOS UTILITÁRIOS
        // =====================================================================

        /// <summary>
        /// Aplica o estilo BetaFit a um DataGridView: cabeçalho preto,
        /// linhas alternadas, seleção em tom de lima.
        ///
        /// IMPORTANTE sobre Guna2DataGridView:
        ///   O Guna2DataGridView NÃO pinta a partir de DefaultCellStyle /
        ///   ColumnHeadersDefaultCellStyle — ele usa seu próprio objeto
        ///   "ThemeStyle" (ThemeStyle.HeaderStyle, ThemeStyle.RowsStyle,
        ///   ThemeStyle.AlternatingRowsStyle), que tem prioridade sobre o
        ///   estilo "cru" do WinForms. Por isso, se o grid for um
        ///   Guna2DataGridView, aplicamos os dois (o estilo base, para
        ///   qualquer DataGridView comum, e o ThemeStyle, que é o que
        ///   realmente aparece na tela nesse caso).
        /// </summary>
        public static void AplicarEstiloGrid(DataGridView grid)
        {
            // Estilo geral
            grid.BackgroundColor = Superficie;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Linha;
            grid.Font = FonteNormal;

            // Cabeçalho (preto/branco, caixa alta como no site)
            grid.ColumnHeadersDefaultCellStyle.BackColor = GridCabecalhoFundo;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = GridCabecalhoTexto;
            grid.ColumnHeadersDefaultCellStyle.Font = FonteRotulo;
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(10, 4, 10, 4);
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersHeight = 42;
            grid.EnableHeadersVisualStyles = false;

            // Linhas
            grid.DefaultCellStyle.BackColor = GridLinhaPar;
            grid.DefaultCellStyle.ForeColor = GridTextoPrincipal;
            grid.DefaultCellStyle.Font = FonteNormal;
            grid.DefaultCellStyle.SelectionBackColor = GridLinhaSelecionada;
            grid.DefaultCellStyle.SelectionForeColor = Tinta;
            grid.DefaultCellStyle.Padding = new Padding(8, 4, 8, 4);

            // Linhas alternadas
            grid.AlternatingRowsDefaultCellStyle.BackColor = GridLinhaImpar;

            // Linha
            grid.RowHeadersVisible = false;
            grid.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            grid.RowTemplate.Height = 38;

            // Seleção
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.ReadOnly = true;

            // Visual
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;

            // ── Guna2DataGridView: aplica também no ThemeStyle ────────────────
            // (é o que o Guna realmente usa para pintar; sem isso, o grid
            // continua com as cores default geradas pelo Designer.)
            if (grid is Guna2DataGridView guna)
            {
                guna.ThemeStyle.BackColor = Superficie;
                guna.ThemeStyle.GridColor = Linha;

                guna.ThemeStyle.HeaderStyle.BackColor = GridCabecalhoFundo;
                guna.ThemeStyle.HeaderStyle.ForeColor = GridCabecalhoTexto;
                guna.ThemeStyle.HeaderStyle.Font = FonteRotulo;
                guna.ThemeStyle.HeaderStyle.Height = 42;

                guna.ThemeStyle.RowsStyle.BackColor = GridLinhaPar;
                guna.ThemeStyle.RowsStyle.ForeColor = GridTextoPrincipal;
                guna.ThemeStyle.RowsStyle.Font = FonteNormal;
                guna.ThemeStyle.RowsStyle.Height = 38;
                guna.ThemeStyle.RowsStyle.SelectionBackColor = GridLinhaSelecionada;
                guna.ThemeStyle.RowsStyle.SelectionForeColor = Tinta;

                guna.ThemeStyle.AlternatingRowsStyle.BackColor = GridLinhaImpar;
                guna.ThemeStyle.AlternatingRowsStyle.ForeColor = GridTextoPrincipal;
                guna.ThemeStyle.AlternatingRowsStyle.Font = FonteNormal;
            }
        }

        /// <summary>
        /// Aplica o fundo/tipografia base de uma tela (Form ou UserControl)
        /// ao padrão BetaFit (fundo branco ou superfície, fonte da marca).
        /// </summary>
        public static void AplicarEstiloFormulario(Control container, bool fundoSuperficie = false)
        {
            container.BackColor = fundoSuperficie ? Superficie : Branco;
            container.Font = FonteNormal;
            container.ForeColor = Tinta;
        }

        /// <summary>
        /// Configura um Label como "badge" de status (ex.: Ativo/Inativo),
        /// usando as mesmas cores do site (.bf-admin-product__status).
        /// </summary>
        public static void AplicarBadgeStatus(Label lbl, bool ativo)
        {
            lbl.BackColor = ativo ? BadgeAtivoFundo : BadgeInativoFundo;
            lbl.ForeColor = ativo ? BadgeAtivoTexto : BadgeInativoTexto;
            lbl.Font = FonteRotulo;
            lbl.Text = (ativo ? "ATIVO" : "INATIVO");
            lbl.TextAlign = ContentAlignment.MiddleCenter;
            lbl.AutoSize = false;
            lbl.Padding = new Padding(6, 2, 6, 2);
        }

        // =====================================================================
        // STATUS DE PEDIDO (badge dentro de célula do grid)
        // =====================================================================

        /// <summary>
        /// Cores (fundo, texto) para cada status possível de um Pedido.
        /// Precisa bater com o enum OrderStatus do BetaFit.Domain
        /// (ver PedidosUserControl.StatusDisponiveis).
        /// </summary>
        public static (Color Fundo, Color Texto) CorStatusPedido(string status) => status switch
        {
            "Pendente" => (NeutroFundo, NeutroTexto),
            "EmPreparacao" => (AvisoFundo, AvisoTexto),
            "Pronto" => (InfoFundo, InfoTexto),
            "Entregue" => (SucessoFundo, SucessoTexto),
            "Cancelado" => (PerigoFundo, PerigoTexto),
            _ => (NeutroFundo, NeutroTexto)
        };

        // =====================================================================
        // STATUS DE PEDIDO — VERSÃO "ADMIN" (tela escura, badge = bolinha + texto
        // colorido, sem fundo preenchido). O enum continua sendo Pendente /
        // EmPreparacao / Pronto / Entregue / Cancelado (é o que a API espera);
        // isso aqui é só o RÓTULO e a COR mostrados pro usuário no grid.
        //
        // Observação importante: o mockup de referência tinha 5 rótulos (Pago,
        // Em separação, Aguardando, Enviado, Entregue) que não correspondem 1:1
        // ao enum atual — não existe um "Pago" equivalente, e o enum tem
        // "Cancelado", que o mockup não tinha. Mapear "Cancelado" pra "Pago"
        // ficaria enganoso (pedido cancelado aparecendo com selo verde de
        // pago), então troquei por um rótulo/cor próprios pra ele. Se quiser
        // outro texto/cor, é só ajustar os dicionários abaixo.
        // =====================================================================

        public static readonly Dictionary<string, string> RotulosStatusPedido = new()
        {
            { "Pendente", "Aguardando" },
            { "EmPreparacao", "Em separação" },
            { "Pronto", "Enviado" },
            { "Entregue", "Entregue" },
            { "Cancelado", "Cancelado" },
        };

        public static string RotuloStatusPedido(string status) =>
            RotulosStatusPedido.TryGetValue(status, out var rotulo) ? rotulo : status;

        public static Color CorStatusPedidoAdmin(string status) => status switch
        {
            "Pendente" => Color.FromArgb(230, 168, 46),      // laranja — Aguardando
            "EmPreparacao" => Color.FromArgb(90, 155, 235),  // azul — Em separação
            "Pronto" => Color.FromArgb(90, 200, 140),        // verde — Enviado
            "Entregue" => Color.FromArgb(170, 120, 230),     // roxo — Entregue
            "Cancelado" => Color.FromArgb(220, 90, 90),      // vermelho — Cancelado
            _ => Admin.TextoMuted
        };

        // Versão "badge preenchido" (pílula colorida) do status — fundo escuro
        // na mesma família da cor + texto na cor viva, igual ao badge
        // Ativo/Inativo do grid de Produtos. Usada no lugar da versão "bolinha".
        public static (Color Fundo, Color Texto) CorBadgeStatusPedidoAdmin(string status) => status switch
        {
            "Pendente" => (Color.FromArgb(56, 42, 16), Color.FromArgb(230, 168, 46)),     // laranja — Aguardando
            "EmPreparacao" => (Color.FromArgb(20, 34, 56), Color.FromArgb(90, 155, 235)), // azul — Em separação
            "Pronto" => (Color.FromArgb(18, 46, 32), Color.FromArgb(90, 200, 140)),       // verde — Enviado
            "Entregue" => (Color.FromArgb(42, 28, 56), Color.FromArgb(170, 120, 230)),    // roxo — Entregue
            "Cancelado" => (Color.FromArgb(56, 22, 22), Color.FromArgb(220, 90, 90)),     // vermelho — Cancelado
            _ => (Admin.BadgeInativoFundo, Admin.TextoMuted)
        };

        // Paleta fixa pra "avatar" (bolinha com iniciais) do cliente no grid de
        // Pedidos — a cor é escolhida por hash do nome, então o mesmo cliente
        // sempre cai na mesma cor entre um carregamento e outro.
        public static readonly Color[] PaletaAvatar =
        {
            Color.FromArgb(46, 125, 90),   // verde
            Color.FromArgb(120, 90, 200),  // roxo
            Color.FromArgb(200, 110, 60),  // laranja queimado
            Color.FromArgb(60, 110, 180),  // azul
            Color.FromArgb(160, 70, 110),  // vinho
            Color.FromArgb(90, 140, 60),   // oliva
        };

        public static Color CorAvatar(string nome)
        {
            if (string.IsNullOrWhiteSpace(nome)) return PaletaAvatar[0];
            int hash = Math.Abs(nome.GetHashCode());
            return PaletaAvatar[hash % PaletaAvatar.Length];
        }

        /// <summary>
        /// Liga a pintura em "badge" (fundo colorido + texto colorido,
        /// caixa alta, negrito) para uma coluna de status dentro de um
        /// DataGridView/Guna2DataGridView. Chame uma vez, normalmente logo
        /// após AplicarEstiloGrid, passando o nome da coluna a colorir.
        ///
        /// Uso: BetaFitTheme.AplicarBadgeStatusNoGrid(gridPedidos, "colStatus");
        /// </summary>
        public static void AplicarBadgeStatusNoGrid(DataGridView grid, string nomeColuna)
        {
            grid.CellFormatting += (sender, e) =>
            {
                if (grid.Columns[e.ColumnIndex]?.Name != nomeColuna) return;
                if (e.Value == null || e.CellStyle == null) return;

                var status = e.Value.ToString() ?? "";
                var (fundo, texto) = CorStatusPedido(status);

                e.CellStyle.BackColor = fundo;
                e.CellStyle.ForeColor = texto;
                e.CellStyle.SelectionBackColor = fundo;
                e.CellStyle.SelectionForeColor = texto;
                e.CellStyle.Font = FonteRotulo;
                e.CellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            };
        }

        // =====================================================================
        // TEMA ESCURO "ADMIN" — paleta separada da paleta clara usada no
        // restante do sistema. Usada nas telas estilo dashboard escuro
        // (Categorias, e futuramente Produtos/Pedidos se for padronizado).
        // =====================================================================
        public static class Admin
        {
            public static Color FundoTela => Color.FromArgb(15, 15, 15);
            public static Color FundoSidebar => Color.FromArgb(10, 10, 10);
            public static Color FundoCard => Color.FromArgb(10, 10, 10);
            public static Color FundoCabecalho => Color.FromArgb(10, 10, 10);
            public static Color FundoLinhaPar => Color.FromArgb(18, 18, 18);
            public static Color FundoLinhaImpar => Color.FromArgb(24, 24, 24);
            public static Color FundoLinhaHover => Color.FromArgb(30, 30, 30);
            public static Color Borda => Color.FromArgb(45, 45, 45);
            public static Color TextoPrincipal => Color.White;
            public static Color TextoMuted => Color.FromArgb(150, 150, 150);
            public static Color Lima => Color.FromArgb(198, 255, 40);
            public static Color AtivoFundoNav => Color.FromArgb(24, 45, 10);
            public static Color BadgeAtivoFundo => Color.FromArgb(22, 46, 16);
            public static Color BadgeAtivoTexto => Color.FromArgb(140, 230, 90);
            public static Color BadgeInativoFundo => Color.FromArgb(40, 40, 40);
            public static Color BadgeInativoTexto => Color.FromArgb(150, 150, 150);
            public static Color IdBadgeFundo => Color.FromArgb(35, 40, 15);

            // =================================================================
            // MODAIS/DIÁLOGOS ESCUROS — padrão único para TODOS os popups do
            // desktop (Novo/Editar Produto, Nova Categoria, Alterar Senha,
            // Detalhes do Pedido). Antes cada Designer.cs usava cores "cruas"
            // do Guna (DimGray, GreenYellow, Silver, SystemColors.*...), o que
            // deixava cada janela com uma paleta diferente. Esses valores
            // centralizam o visual do mockup de referência ("Novo Produto"):
            // fundo preto, campos em cinza-quase-preto, foco/destaque em lima.
            // =================================================================

            /// <summary>Fundo do campo de formulário (input, combobox, textarea)</summary>
            public static Color CampoFundo => Color.FromArgb(24, 24, 24);

            /// <summary>Borda padrão (repousada) de um campo de formulário</summary>
            public static Color CampoBorda => Borda;

            /// <summary>Borda do campo em foco/hover — mesma cor de destaque (lima)</summary>
            public static Color CampoBordaFoco => Lima;

            /// <summary>Texto digitado dentro do campo</summary>
            public static Color CampoTexto => Color.White;

            /// <summary>Placeholder / texto de exemplo dentro do campo</summary>
            public static Color CampoPlaceholder => Color.FromArgb(140, 140, 140);

            /// <summary>Fundo do botão secundário ("fantasma") — ex.: Cancelar</summary>
            public static Color BotaoSecundarioFundo => Color.FromArgb(30, 30, 30);

            /// <summary>Borda do botão secundário</summary>
            public static Color BotaoSecundarioBorda => Color.FromArgb(60, 60, 60);

            /// <summary>Texto do botão secundário</summary>
            public static Color BotaoSecundarioTexto => Color.White;

            /// <summary>Fundo do botão primário (Salvar/Confirmar) — sempre lima</summary>
            public static Color BotaoPrimarioFundo => Lima;

            /// <summary>Texto do botão primário — sempre preto, para contraste com o lima</summary>
            public static Color BotaoPrimarioTexto => PretoPrimario;

            /// <summary>Estado "desligado" de um toggle/switch dentro de um modal escuro</summary>
            public static Color ToggleDesligadoFundo => Color.FromArgb(70, 70, 70);

            /// <summary>Raio de borda padrão dos modais escuros (inputs, botões) — levemente arredondado, igual ao mockup</summary>
            public static int ModalBorderRadius => 8;
        }

        /// <summary>
        /// Variante escura de AplicarEstiloGrid, para telas estilo dashboard
        /// escuro (ex.: CategoriasUserControl). Usa a paleta BetaFitTheme.Admin.
        /// </summary>
        public static void AplicarEstiloGridEscuro(DataGridView grid)
        {
            grid.BackgroundColor = Admin.FundoCard;
            grid.BorderStyle = BorderStyle.None;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.GridColor = Admin.Borda;
            grid.Font = FonteNormal;

            grid.ColumnHeadersDefaultCellStyle.BackColor = Admin.FundoCabecalho;
            grid.ColumnHeadersDefaultCellStyle.ForeColor = Admin.TextoMuted;
            grid.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            grid.ColumnHeadersDefaultCellStyle.Padding = new Padding(12, 4, 12, 4);
            grid.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            // Sem isso, a coluna com foco (CurrentCell) herda a cor azul padrão
            // do Windows (SystemColors.Highlight) vinda do estilo gerado pelo
            // Designer, deixando só o cabeçalho dela destoando dos demais.
            grid.ColumnHeadersDefaultCellStyle.SelectionBackColor = Admin.FundoCabecalho;
            grid.ColumnHeadersDefaultCellStyle.SelectionForeColor = Admin.TextoMuted;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            grid.ColumnHeadersHeight = 44;
            grid.EnableHeadersVisualStyles = false;

            grid.DefaultCellStyle.BackColor = Admin.FundoLinhaPar;
            grid.DefaultCellStyle.ForeColor = Admin.TextoPrincipal;
            grid.DefaultCellStyle.Font = FonteNormal;
            grid.DefaultCellStyle.SelectionBackColor = Admin.FundoLinhaHover;
            grid.DefaultCellStyle.SelectionForeColor = Admin.TextoPrincipal;
            grid.DefaultCellStyle.Padding = new Padding(8, 0, 8, 0);

            grid.AlternatingRowsDefaultCellStyle.BackColor = Admin.FundoLinhaImpar;
            grid.AlternatingRowsDefaultCellStyle.ForeColor = Admin.TextoPrincipal;

            grid.RowHeadersVisible = false;
            grid.RowTemplate.Height = 56;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;

            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.MultiSelect = false;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.AllowUserToResizeColumns = false;

            if (grid is Guna2DataGridView guna)
            {
                guna.ThemeStyle.BackColor = Admin.FundoCard;
                guna.ThemeStyle.GridColor = Admin.Borda;

                guna.ThemeStyle.HeaderStyle.BackColor = Admin.FundoCabecalho;
                guna.ThemeStyle.HeaderStyle.ForeColor = Admin.TextoMuted;
                guna.ThemeStyle.HeaderStyle.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
                guna.ThemeStyle.HeaderStyle.Height = 44;

                guna.ThemeStyle.RowsStyle.BackColor = Admin.FundoLinhaPar;
                guna.ThemeStyle.RowsStyle.ForeColor = Admin.TextoPrincipal;
                guna.ThemeStyle.RowsStyle.Font = FonteNormal;
                guna.ThemeStyle.RowsStyle.Height = 56;
                guna.ThemeStyle.RowsStyle.SelectionBackColor = Admin.FundoLinhaHover;
                guna.ThemeStyle.RowsStyle.SelectionForeColor = Admin.TextoPrincipal;

                guna.ThemeStyle.AlternatingRowsStyle.BackColor = Admin.FundoLinhaImpar;
                guna.ThemeStyle.AlternatingRowsStyle.ForeColor = Admin.TextoPrincipal;
            }
        }
    }
}