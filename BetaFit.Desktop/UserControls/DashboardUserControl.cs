using BetaFit.Desktop.Forms;
using BetaFit.Desktop.Helpers;
using BetaFit.Desktop.Services;
using BetaFit.Desktop.Themes;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetaFit.Desktop.UserControls
{
    public partial class DashboardUserControl : UserControl
    {
        // Serviços de API para o Dashboard
        private CategoriesApiService _categoriesApiService = null;
        private ProductsApiService _productsApiService = null!; //Caso nao tenha nada ele nao vai dar erro, por isso o null! (null-forgiving operator)
        private UsersApiService _usersApiService = null;
        private OrdersApiService _ordersApiService = null!;
        private List<BetaFit.Desktop.DTOs.OrderResponseDto> _todosPedidos = new();


        public DashboardUserControl()
        {
            InitializeComponent();
        }

        //Carregamento do UserControl
        private async void DashboardUserControl_Load(object sender, EventArgs e)
        {
            //Guard: não executa em tempo de design
            if (DesignMode) return;

            //Instancia os serviços de API
            _categoriesApiService = new CategoriesApiService();
            _productsApiService = new ProductsApiService();
            _usersApiService = new UsersApiService();
            _ordersApiService = new OrdersApiService();

            // Tema escuro do grid (mesmo usado em Pedidos/Produtos/Categorias)
            BetaFitTheme.AplicarEstiloGridEscuro(gridUltimosPedidos);

            // Pintura customizada de célula — mesma técnica usada em
            // PedidosUserControl (avatar+nome do cliente, pílula colorida
            // de status). Trocado de AplicarBadgeStatusNoGrid porque esse
            // método usa a paleta "clara" (CorStatusPedido) e deixava esse
            // grid com uma cara diferente do de Pedidos.
            gridUltimosPedidos.CellPainting += gridUltimosPedidos_CellPainting;

            // Ícones dos 4 cards de estatística — desenhados via GDI+ (não
            // emoji), mesmo motivo do estado vazio: emoji grande em
            // Guna2HtmlLabel/Guna2PictureBox não renderiza direito.
            ptbIconeFaturamento.Image = CriarIconeClipboard(ptbIconeFaturamento.Size);
            ptbIconeCategorias.Image = CriarIconeEtiqueta(ptbIconeCategorias.Size);
            ptbIconeProdutos.Image = CriarIconeCaixa(ptbIconeProdutos.Size);
            ptbIconeClientes.Image = CriarIconePessoas(ptbIconeClientes.Size);

            ptbIconeFaturamento.SizeMode = PictureBoxSizeMode.CenterImage;
            ptbIconeCategorias.SizeMode = PictureBoxSizeMode.CenterImage;
            ptbIconeProdutos.SizeMode = PictureBoxSizeMode.CenterImage;
            ptbIconeClientes.SizeMode = PictureBoxSizeMode.CenterImage;

            // Ícone grande do estado vazio (sacola), também via GDI+.
            pctIconeVazio.Image = CriarIconeSacolaVazia();
            pctIconeVazio.SizeMode = PictureBoxSizeMode.CenterImage;

            // Navegação do botão "VER PEDIDOS" do estado vazio.
            btnIrParaProdutos.Click += btnIrParaProdutos_Click;

            //Carrega os dados do Dashboard
            await CarregarDadosAsync();

        }


        //Carregar dados do usuario
        private async Task CarregarDadosAsync()
        {
            try
            {
                // Carrega dados de categorias, produtos e usuários
                var categorias = await _categoriesApiService.GetAllAsync();
                var produtos = await _productsApiService.GetAllAsync();
                var usuarios = await _usersApiService.GetAllAsync();
                // Carrega pedidos recentes
                _todosPedidos = await _ordersApiService.GetAllAsync();
                // Atualiza os labels com os contadores
                lblValorCategorias.Text = categorias.Count.ToString();
                lblValorProdutos.Text = produtos.Count.ToString();
                lblValorClientes.Text = usuarios.Count.ToString();

                // Card FATURAMENTO: soma do Total dos pedidos já finalizados
                // (status "Entregue"), não a contagem deles.
                var faturamento = _todosPedidos
                    .Where(p => string.Equals(p.Status, "Entregue", StringComparison.OrdinalIgnoreCase))
                    .Sum(p => p.Total);
                lblValorFaturamento.Text = faturamento.ToString("C");

                // Popular grid de últimos pedidos (exibe os 5 mais recentes)
                PopularUltimosPedidos(_todosPedidos);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Erro ao carregar dados do dashboard: {ex.Message}",
                "Erro",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error);
            }
        }

        private void PopularUltimosPedidos(List<BetaFit.Desktop.DTOs.OrderResponseDto> pedidos)
        {
            // Limpa linhas existentes
            gridUltimosPedidos.Rows.Clear();

            var ultimos5 = pedidos.OrderByDescending(p => p.CreatedAt).Take(5).ToList();

            foreach (var pedido in ultimos5)
            {
                gridUltimosPedidos.Rows.Add(
                    pedido.Id,
                    pedido.UserName,
                    pedido.CreatedAt.ToString("dd/MM/yyyy HH:mm"),
                    pedido.Status,
                    pedido.Total.ToString("C")
                );
            }

            AtualizarRodapePaginacaoDashboard(pedidos.Count, ultimos5.Count);
            AtualizarEstadoVazioDashboard();
        }

        // Alterna entre o grid (pnlTabela) e o estado vazio (PnlPedidosVazios)
        // — sem isso os dois ficam visíveis ao mesmo tempo, sobrepostos.
        private void AtualizarEstadoVazioDashboard()
        {
            bool semResultados = _todosPedidos.Count == 0;
            pnlTabela.Visible = !semResultados;
            PnlPedidosVazios.Visible = semResultados;
        }

        // Rodapé estático: o Dashboard sempre mostra só os 5 pedidos mais
        // recentes, não pagina de verdade — por isso os botões ‹ › ficam
        // sempre desabilitados (já fixado no Designer) e só o texto muda.
        private void AtualizarRodapePaginacaoDashboard(int totalPedidos, int exibidos)
        {
            lblResumoPag.Text = $"Exibindo {exibidos} de {totalPedidos} pedidos";
        }

        private async void btnAtualizarPedidos_Click(object sender, EventArgs e)
        {
            await CarregarDadosAsync();
        }

        // Botão "VER PEDIDOS" do estado vazio — leva pra tela de Pedidos
        // (mesma técnica do btnIrParaProdutos_Click da PedidosUserControl,
        // trocando o destino).
        private void btnIrParaProdutos_Click(object sender, EventArgs e)
        {
            (this.FindForm() as MainForm)?.NavegarParaPedidos();
        }

        // =====================================================================
        // PINTURA CUSTOMIZADA DO GRID — mesma técnica de PedidosUserControl,
        // pra os dois grids ficarem visualmente idênticos (avatar do cliente
        // e pílula colorida de status, com rótulo traduzido).
        // =====================================================================

        private void gridUltimosPedidos_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.ColumnIndex < 0 || e.RowIndex < 0) return;
            var nomeColuna = gridUltimosPedidos.Columns[e.ColumnIndex].Name;

            switch (nomeColuna)
            {
                case "colClienteUP":
                    PintarCelulaClienteDashboard(e);
                    break;
                case "colStatusUP":
                    PintarCelulaStatusDashboard(e);
                    break;
            }
        }

        // Avatar (bolinha com iniciais) + nome do cliente
        private void PintarCelulaClienteDashboard(DataGridViewCellPaintingEventArgs e)
        {
            e.PaintBackground(e.CellBounds, true);
            var nome = e.Value?.ToString() ?? "";
            if (string.IsNullOrWhiteSpace(nome)) { e.Handled = true; return; }

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            const int diametro = 28;
            int x = e.CellBounds.Left + 8;
            int y = e.CellBounds.Top + (e.CellBounds.Height - diametro) / 2;

            using (var brush = new SolidBrush(BetaFitTheme.CorAvatar(nome)))
                e.Graphics.FillEllipse(brush, x, y, diametro, diametro);

            string iniciais = ObterIniciaisDashboard(nome);
            using var fonteIniciais = new Font("Segoe UI", 8.5F, FontStyle.Bold);
            var tamIniciais = e.Graphics.MeasureString(iniciais, fonteIniciais);
            using (var brushTexto = new SolidBrush(Color.White))
                e.Graphics.DrawString(iniciais, fonteIniciais, brushTexto,
                    x + (diametro - tamIniciais.Width) / 2, y + (diametro - tamIniciais.Height) / 2);

            using var fonteNome = new Font("Segoe UI", 9F);
            var tamNome = e.Graphics.MeasureString(nome, fonteNome);
            using (var brushNome = new SolidBrush(BetaFitTheme.Admin.TextoPrincipal))
                e.Graphics.DrawString(nome, fonteNome, brushNome,
                    x + diametro + 10, e.CellBounds.Top + (e.CellBounds.Height - tamNome.Height) / 2);

            e.Handled = true;
        }

        private static string ObterIniciaisDashboard(string nomeCompleto)
        {
            var partes = nomeCompleto.Trim().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            if (partes.Length == 0) return "?";
            if (partes.Length == 1) return partes[0][..1].ToUpper();
            return $"{partes[0][0]}{partes[^1][0]}".ToUpper();
        }

        // Badge preenchido (pílula) com o rótulo traduzido do status —
        // exatamente igual ao PintarCelulaStatus da tela de Pedidos.
        private void PintarCelulaStatusDashboard(DataGridViewCellPaintingEventArgs e)
        {
            e.PaintBackground(e.CellBounds, true);
            var status = e.Value?.ToString() ?? "";
            if (string.IsNullOrEmpty(status)) { e.Handled = true; return; }

            var (fundo, texto) = BetaFitTheme.CorBadgeStatusPedidoAdmin(status);
            var rotulo = BetaFitTheme.RotuloStatusPedido(status);

            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using var fonte = new Font("Segoe UI", 9F, FontStyle.Bold);
            var tam = e.Graphics.MeasureString(rotulo, fonte);

            int largura = (int)tam.Width + 24;
            int altura = 26;
            int x = e.CellBounds.Left + 8;
            int y = e.CellBounds.Top + (e.CellBounds.Height - altura) / 2;

            using (var path = RetanguloArredondadoDashboard(new Rectangle(x, y, largura, altura), altura / 2))
            using (var brush = new SolidBrush(fundo))
                e.Graphics.FillPath(brush, path);

            using (var brushTexto = new SolidBrush(texto))
                e.Graphics.DrawString(rotulo, fonte, brushTexto,
                    x + (largura - tam.Width) / 2, y + (altura - tam.Height) / 2);

            e.Handled = true;
        }

        private static GraphicsPath RetanguloArredondadoDashboard(Rectangle bounds, int raio)
        {
            var path = new GraphicsPath();
            int d = raio * 2;
            path.AddArc(bounds.X, bounds.Y, d, d, 180, 90);
            path.AddArc(bounds.Right - d, bounds.Y, d, d, 270, 90);
            path.AddArc(bounds.Right - d, bounds.Bottom - d, d, d, 0, 90);
            path.AddArc(bounds.X, bounds.Bottom - d, d, d, 90, 90);
            path.CloseFigure();
            return path;
        }

        // =====================================================================
        // ÍCONES DOS CARDS — desenhados via GDI+ (lima sobre fundo transparente,
        // pra combinar com o círculo verde-escuro atrás). Traço fino porque o
        // ícone é pequeno (dentro de um círculo de 48px).
        // =====================================================================

        private static Image CriarIconeClipboard(Size tamanho)
        {
            var bmp = new Bitmap(Math.Max(tamanho.Width, 1), Math.Max(tamanho.Height, 1));
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);

            using var caneta = new Pen(Color.GreenYellow, 2f)
            {
                LineJoin = LineJoin.Round,
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };

            // Prancheta
            var corpo = new RectangleF(5, 4, 20, 24);
            g.DrawRectangle(caneta, corpo.X, corpo.Y, corpo.Width, corpo.Height);

            // Clipe no topo
            var clipe = new RectangleF(11, 2, 8, 5);
            g.FillRectangle(new SolidBrush(Color.GreenYellow), clipe);

            // "Check" dentro da prancheta
            g.DrawLines(caneta, new[]
            {
                new PointF(9, 16), new PointF(13, 20), new PointF(21, 10)
            });

            return bmp;
        }

        private static Image CriarIconeEtiqueta(Size tamanho)
        {
            var bmp = new Bitmap(Math.Max(tamanho.Width, 1), Math.Max(tamanho.Height, 1));
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);

            using var caneta = new Pen(Color.GreenYellow, 2f)
            {
                LineJoin = LineJoin.Round,
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };

            // Corpo da etiqueta (losango com canto reto)
            var pontos = new[]
            {
                new PointF(4, 14), new PointF(15, 3), new PointF(27, 3),
                new PointF(27, 15), new PointF(16, 26)
            };
            g.DrawPolygon(caneta, pontos);

            // Furinho
            g.DrawEllipse(caneta, 20, 6, 4, 4);

            return bmp;
        }

        private static Image CriarIconeCaixa(Size tamanho)
        {
            var bmp = new Bitmap(Math.Max(tamanho.Width, 1), Math.Max(tamanho.Height, 1));
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);

            using var caneta = new Pen(Color.GreenYellow, 2f)
            {
                LineJoin = LineJoin.Round,
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };

            // Caixa (cubo isométrico simples)
            var topo = new[] { new PointF(15, 3), new PointF(27, 9), new PointF(15, 15), new PointF(3, 9) };
            g.DrawPolygon(caneta, topo);
            g.DrawLine(caneta, 3, 9, 3, 21);
            g.DrawLine(caneta, 27, 9, 27, 21);
            g.DrawLine(caneta, 15, 15, 15, 27);
            g.DrawLine(caneta, 3, 21, 15, 27);
            g.DrawLine(caneta, 27, 21, 15, 27);

            return bmp;
        }

        private static Image CriarIconePessoas(Size tamanho)
        {
            var bmp = new Bitmap(Math.Max(tamanho.Width, 1), Math.Max(tamanho.Height, 1));
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);

            using var caneta = new Pen(Color.GreenYellow, 2f)
            {
                LineJoin = LineJoin.Round,
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };

            // Pessoa da frente (maior)
            g.DrawEllipse(caneta, 8, 3, 9, 9);
            g.DrawArc(caneta, 3, 14, 19, 14, 180, 180);

            // Pessoa de trás (menor, deslocada)
            g.DrawEllipse(caneta, 18, 1, 7, 7);
            g.DrawArc(caneta, 20, 11, 9, 12, 200, 160);

            return bmp;
        }

        // Ícone grande do estado vazio (sacola de compras), desenhado via
        // GDI+ — sem depender de nenhuma fonte de emoji (a mesma pegadinha
        // documentada em PedidosUserControl.CriarIconeCarrinhoVazio).
        private static Image CriarIconeSacolaVazia()
        {
            var bmp = new Bitmap(64, 64);
            using var g = Graphics.FromImage(bmp);
            g.SmoothingMode = SmoothingMode.AntiAlias;
            g.Clear(Color.Transparent);

            using var canetaSacola = new Pen(Color.FromArgb(130, 130, 130), 2.5f)
            {
                LineJoin = LineJoin.Round,
                StartCap = LineCap.Round,
                EndCap = LineCap.Round
            };

            // Corpo da sacola (trapézio)
            PointF pTopoEsq = new(18, 22), pTopoDir = new(46, 22),
                   pBaseDir = new(50, 54), pBaseEsq = new(14, 54);
            g.DrawLine(canetaSacola, pTopoEsq, pTopoDir);
            g.DrawLine(canetaSacola, pTopoDir, pBaseDir);
            g.DrawLine(canetaSacola, pBaseDir, pBaseEsq);
            g.DrawLine(canetaSacola, pBaseEsq, pTopoEsq);

            // Alça (arco)
            g.DrawArc(canetaSacola, 24, 10, 16, 20, 180, 180);

            return bmp;
        }
    }
}