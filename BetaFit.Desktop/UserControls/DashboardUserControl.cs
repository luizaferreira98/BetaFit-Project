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
                // Carrega pedidos para calcular os itens mais pedidos
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

                // Popular grid com os 5 itens mais pedidos
                PopularItensMaisPedidos(_todosPedidos);
            }
            catch (Exception ex)
            {
                BetaFitMessageBox.Erro(this, $"Erro ao carregar dados do dashboard: {ex.Message}");
            }
        }

        private void PopularItensMaisPedidos(List<BetaFit.Desktop.DTOs.OrderResponseDto> pedidos)
        {
            gridUltimosPedidos.Rows.Clear();

            // Considera apenas pedidos entregues para representar vendas
            // efetivamente concluídas no ranking.
            var itensMaisPedidos = pedidos
                .Where(p => string.Equals(
                    p.Status,
                    "Entregue",
                    StringComparison.OrdinalIgnoreCase))
                .SelectMany(p => p.Items ?? new List<BetaFit.Desktop.DTOs.OrderItemResponseDto>())
                .Where(i => i.Quantity > 0)
                .GroupBy(i => new { i.ProductId, i.ProductName })
                .Select(g => new
                {
                    Produto = string.IsNullOrWhiteSpace(g.Key.ProductName)
                        ? $"Produto #{g.Key.ProductId}"
                        : g.Key.ProductName,
                    Unidades = g.Sum(i => i.Quantity),
                    Pedidos = g.Count()
                })
                .OrderByDescending(x => x.Unidades)
                .ThenByDescending(x => x.Pedidos)
                .Take(5)
                .ToList();

            foreach (var item in itensMaisPedidos)
            {
                gridUltimosPedidos.Rows.Add(
                    item.Produto,
                    item.Unidades,
                    item.Pedidos
                );
            }

            AtualizarRodapePaginacaoDashboard(itensMaisPedidos.Count, itensMaisPedidos.Count);
            AtualizarEstadoVazioDashboard(itensMaisPedidos.Count == 0);
        }

        // Alterna entre o grid e o estado vazio.
        private void AtualizarEstadoVazioDashboard(bool semResultados)
        {
            pnlTabela.Visible = !semResultados;
            PnlPedidosVazios.Visible = semResultados;
        }

        private void AtualizarRodapePaginacaoDashboard(int exibidos, int totalItens)
        {
            lblResumoPag.Text = $"Exibindo {exibidos} de {totalItens} itens";
        }

        private async void btnAtualizarPedidos_Click(object sender, EventArgs e)
        {
            await CarregarDadosAsync();
        }

        // Botão do estado vazio — leva para a tela de Pedidos.
        private void btnIrParaProdutos_Click(object sender, EventArgs e)
        {
            (this.FindForm() as MainForm)?.NavegarParaPedidos();
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