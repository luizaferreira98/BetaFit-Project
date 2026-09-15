using BetaFit.Desktop.DTOs;
using BetaFit.Desktop.Services;
using BetaFit.Desktop.Themes;
using System;
using System.Globalization;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace BetaFit.Desktop.Forms
{
    /// <summary>
    /// Formulário de criação/edição de Produto.
    /// Retorna CreateProductDto (novo) ou UpdateProductDto (edição).
    /// </summary>
    /// <remarks>
    /// Este diálogo precisa preencher TODOS os campos que a API grava, porque
    /// ProductService.UpdateAsync faz sobrescrita direta — campo que não vier
    /// no JSON é gravado com o default e apaga o que existia. Por isso aqui
    /// aparecem SKU, preço de oferta, estoque, alerta de estoque baixo,
    /// tamanhos, cores e os dois switches (Ativo e Em destaque), exatamente
    /// como em Views/Admin/NewProduct.cshtml.
    /// </remarks>
    public partial class ProductFormDialog : Form
    {
        // =====================================================================
        // PROPRIEDADES DE SAÍDA
        // =====================================================================

        /// <summary>DTO preenchido quando no modo de criação (OK)</summary>
        public CreateProductDto? ProductDto { get; private set; }

        /// <summary>DTO preenchido quando no modo de edição (OK)</summary>
        public UpdateProductDto? UpdateDto { get; private set; }


        // =====================================================================
        // CAMPOS PRIVADOS
        // =====================================================================
        private List<CategoriaResponseDto> _categorias = new();
        private ProductResponseDto? _productExistente;

        // Usado exclusivamente pelo botão "Selecionar arquivo" (upload de
        // imagem local) — não afeta o restante do CRUD de produtos.
        private readonly ProductsApiService _productsApiService = new();

        // Formatos e tamanho aceitos pelo endpoint POST /api/products/upload-image
        private static readonly string[] ExtensoesImagemPermitidas = { ".jpg", ".jpeg", ".png", ".webp" };
        private const long TamanhoMaximoImagemBytes = 5 * 1024 * 1024; // 5 MB


        // =====================================================================
        // CONSTRUTORES
        // =====================================================================
        /// <summary>
        /// Construtor padrão sem parâmetros — necessário para o Designer.
        /// Use o construtor com parâmetros em produção.
        /// </summary>
        public ProductFormDialog()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Construtor de produção com categorias e produto opcional.
        /// </summary>
        /// <param name="categorias">Lista de categorias para o ComboBox</param>
        /// <param name="product">null para criação, produto existente para edição</param>
        public ProductFormDialog(List<CategoriaResponseDto> categorias, ProductResponseDto? product)
        {
            _categorias = categorias;
            _productExistente = product;
            InitializeComponent();
        }


        // =====================================================================
        // EVENTO LOAD
        // =====================================================================
        private void ProductFormDialog_Load(object sender, EventArgs e)
        {
            // Guard: não executa em modo de Design
            if (DesignMode) return;

            // 1. Configura título baseado no modo (criação/edição)
            this.Text = _productExistente == null ? "Novo Produto" : "Editar Produto";
            lblTituloFromProduto.Text = _productExistente == null ? "➕ Novo Produto" : "✏️ Editar Produto";

            // 2. Popula o ComboBox de Categorias
            cboCategoriaProduto.Items.Clear();
            cboCategoriaProduto.Items.Add("Selecione uma categoria...");
            foreach (var cat in _categorias)
            {
                cboCategoriaProduto.Items.Add(cat.Name);
            }
            cboCategoriaProduto.SelectedIndex = 0;

            // 3. Popula o ComboBox de Gêneros
            cboGenero.Items.Clear();
            cboGenero.Items.Add("Selecione um gênero...");
            cboGenero.Items.Add("Masculino");
            cboGenero.Items.Add("Feminino");
            cboGenero.Items.Add("Unissex");
            cboGenero.SelectedIndex = 0;

            // 4. Popula as listas de variação com as MESMAS opções da web
            //    (ProductVariationOptions espelha NewProduct.cshtml).
            clbTamanhos.Items.Clear();
            clbTamanhos.Items.AddRange(ProductVariationOptions.Tamanhos);

            clbCores.Items.Clear();
            clbCores.Items.AddRange(ProductVariationOptions.Cores);

            // 5. Preenche os campos (criação usa os defaults; edição usa o produto)
            PreencherCampos();
        }

        // =====================================================================
        // PREENCHIMENTO
        // =====================================================================

        private void PreencherCampos()
        {
            // --- MODO CRIAÇÃO: valores iniciais iguais aos defaults da API ---
            if (_productExistente == null)
            {
                txtEstoqueProduto.Text = "999";
                txtAlertaEstoqueProduto.Text = "5";
                swAtivo.Checked = true;       // produto novo nasce ativo
                swDestaque.Checked = false;
                return;
            }

            // --- MODO EDIÇÃO ---
            txtNomeProduto.Text = _productExistente.Name;
            txtDescricaoProduto.Text = _productExistente.Description;
            txtSkuProduto.Text = _productExistente.Sku;
            txtPrecoProduto.Text = _productExistente.Price.ToString("0.00", CultureInfo.CurrentCulture);
            txtPrecoOfertaProduto.Text = _productExistente.SalePrice.HasValue
                ? _productExistente.SalePrice.Value.ToString("0.00", CultureInfo.CurrentCulture)
                : string.Empty;
            txtEstoqueProduto.Text = _productExistente.Stock.ToString();
            txtAlertaEstoqueProduto.Text = _productExistente.LowStockThreshold.ToString();
            txtUrlImagemProduto.Text = _productExistente.ImageUrl;

            swAtivo.Checked = _productExistente.IsActive;
            swDestaque.Checked = _productExistente.IsFeatured;

            // Categoria (índice 0 é o placeholder "Selecione...")
            var idx = _categorias.FindIndex(c => c.Id == _productExistente.CategoryId);
            if (idx >= 0) cboCategoriaProduto.SelectedIndex = idx + 1;

            // Gênero
            var generoTexto = _productExistente.Gender.ToString();
            if (cboGenero.Items.Contains(generoTexto))
                cboGenero.SelectedItem = generoTexto;

            // Tamanhos e cores já cadastrados (inclusive os que vieram da web)
            MarcarItens(clbTamanhos, _productExistente.AvailableSizes);
            MarcarItens(clbCores, _productExistente.AvailableColors);
        }

        // Marca na lista os itens que já estão salvos no produto.
        private static void MarcarItens(CheckedListBox lista, List<string> selecionados)
        {
            if (selecionados == null) return;

            for (int i = 0; i < lista.Items.Count; i++)
            {
                var valor = lista.Items[i]?.ToString() ?? string.Empty;
                bool marcado = selecionados.Any(s =>
                    string.Equals(s, valor, StringComparison.OrdinalIgnoreCase));

                lista.SetItemChecked(i, marcado);
            }
        }

        // Lê os itens marcados de uma lista.
        private static List<string> LerItensMarcados(CheckedListBox lista)
        {
            return lista.CheckedItems
                .Cast<object>()
                .Select(i => i?.ToString() ?? string.Empty)
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .ToList();
        }

        // Aceita preços digitados no padrão brasileiro (149,90),
        // no padrão internacional (149.90) e com separador de milhar.
        private static bool TentarLerPreco(string texto, out decimal preco)
        {
            preco = 0m;
            if (string.IsNullOrWhiteSpace(texto)) return false;

            texto = texto.Trim().Replace("R$", "", StringComparison.OrdinalIgnoreCase).Trim();

            // Quando os dois separadores aparecem, o último é tratado como
            // separador decimal. Ex.: 1.499,90 ou 1,499.90.
            int ultimaVirgula = texto.LastIndexOf(',');
            int ultimoPonto = texto.LastIndexOf('.');

            if (ultimaVirgula >= 0 && ultimoPonto >= 0)
            {
                if (ultimaVirgula > ultimoPonto)
                    texto = texto.Replace(".", "").Replace(',', '.');
                else
                    texto = texto.Replace(",", "");

                return decimal.TryParse(
                    texto, NumberStyles.AllowDecimalPoint | NumberStyles.AllowLeadingSign,
                    CultureInfo.InvariantCulture, out preco);
            }

            if (ultimaVirgula >= 0)
            {
                return decimal.TryParse(
                    texto, NumberStyles.Number, CultureInfo.GetCultureInfo("pt-BR"), out preco);
            }

            return decimal.TryParse(
                texto, NumberStyles.Number, CultureInfo.InvariantCulture, out preco);
        }

        // Lê um inteiro simples, devolvendo o padrão quando o campo está vazio.
        private static bool TentarLerInteiro(string texto, int padrao, out int valor)
        {
            if (string.IsNullOrWhiteSpace(texto))
            {
                valor = padrao;
                return true;
            }

            return int.TryParse(texto.Trim(), out valor);
        }

        // =====================================================================
        // IMAGEM LOCAL (upload via POST /api/products/upload-image)
        // =====================================================================

        /// <summary>
        /// Abre o seletor de arquivos, mostra a prévia local imediatamente e
        /// envia a imagem para a API. Se o envio for bem-sucedido, a URL
        /// pública retornada preenche automaticamente txtUrlImagemProduto.
        /// </summary>
        private async void btnSelecionarImagemProduto_Click(object sender, EventArgs e)
        {
            using var openFileDialog = new OpenFileDialog
            {
                Title = "Selecionar imagem do produto",
                Filter = "Imagens (*.jpg;*.jpeg;*.png;*.webp)|*.jpg;*.jpeg;*.png;*.webp",
                CheckFileExists = true,
                Multiselect = false
            };

            if (openFileDialog.ShowDialog(this) != DialogResult.OK) return;

            var caminho = openFileDialog.FileName;
            var extensao = Path.GetExtension(caminho).ToLowerInvariant();
            var nomeArquivo = Path.GetFileName(caminho);

            if (!ExtensoesImagemPermitidas.Contains(extensao))
            {
                BetaFitMessageBox.Aviso(this, "Use apenas imagens JPG, PNG ou WEBP.", "Arquivo inválido");
                return;
            }

            byte[] bytes;
            try
            {
                bytes = File.ReadAllBytes(caminho);
            }
            catch (Exception ex)
            {
                BetaFitMessageBox.Erro(this, $"Não foi possível ler o arquivo selecionado.\n\n{ex.Message}");
                return;
            }

            if (bytes.LongLength > TamanhoMaximoImagemBytes)
            {
                BetaFitMessageBox.Aviso(this, "A imagem excede o tamanho máximo de 5 MB.", "Arquivo muito grande");
                return;
            }

            // Prévia local — não depende do upload dar certo.
            try
            {
                Image imagemCarregada;
                using (var ms = new MemoryStream(bytes))
                {
                    imagemCarregada = Image.FromStream(ms);
                }
                pctPreviewImagemProduto.Image?.Dispose();
                pctPreviewImagemProduto.Image = imagemCarregada;
            }
            catch (Exception ex)
            {
                BetaFitMessageBox.Erro(this, $"O arquivo não parece ser uma imagem válida.\n\n{ex.Message}");
                return;
            }

            // Envia para a API e preenche a URL automaticamente.
            btnSelecionarImagemProduto.Enabled = false;
            var textoOriginalBotao = btnSelecionarImagemProduto.Text;
            btnSelecionarImagemProduto.Text = "ENVIANDO...";
            lblArquivoSelecionadoProduto.ForeColor = Color.FromArgb(150, 150, 150);
            lblArquivoSelecionadoProduto.Text = $"{nomeArquivo} — enviando...";

            var (sucesso, url, erro) = await _productsApiService.UploadImagemAsync(bytes, nomeArquivo);

            btnSelecionarImagemProduto.Enabled = true;
            btnSelecionarImagemProduto.Text = textoOriginalBotao;

            if (sucesso)
            {
                txtUrlImagemProduto.Text = url;
                lblArquivoSelecionadoProduto.ForeColor = BetaFitTheme.Admin.Lima;
                lblArquivoSelecionadoProduto.Text = $"✓ {nomeArquivo} enviada com sucesso.";
            }
            else
            {
                lblArquivoSelecionadoProduto.ForeColor = BetaFitTheme.Perigo;
                lblArquivoSelecionadoProduto.Text = "Falha ao enviar. Tente novamente.";
                BetaFitMessageBox.Erro(this, $"Não foi possível enviar a imagem.\n\n{erro}", "Erro no upload");
            }
        }

        // =====================================================================
        // SALVAR
        // =====================================================================
        private void btnSalvarProduto_Click(object sender, EventArgs e)
        {
            // -----------------------------------------------------------------
            // 1. Validações de entrada
            // -----------------------------------------------------------------
            if (string.IsNullOrWhiteSpace(txtNomeProduto.Text))
            {
                BetaFitMessageBox.Aviso(this, "Informe o nome do produto.", "Validação");
                txtNomeProduto.Focus();
                return;
            }

            if (string.IsNullOrWhiteSpace(txtDescricaoProduto.Text))
            {
                BetaFitMessageBox.Aviso(this, "Informe a descrição do produto.", "Validação");
                txtDescricaoProduto.Focus();
                return;
            }

            if (!TentarLerPreco(txtPrecoProduto.Text, out decimal preco) || preco <= 0)
            {
                BetaFitMessageBox.Aviso(this, "Informe um preço válido maior que zero. Ex.: 149,90", "Validação");
                txtPrecoProduto.Focus();
                return;
            }

            // Preço de oferta é opcional, mas a API exige que seja positivo
            // e MENOR que o preço original (ProductService.ValidateInventory).
            decimal? precoOferta = null;
            if (!string.IsNullOrWhiteSpace(txtPrecoOfertaProduto.Text))
            {
                if (!TentarLerPreco(txtPrecoOfertaProduto.Text, out decimal oferta))
                {
                    BetaFitMessageBox.Aviso(this, "Preço de oferta inválido. Ex.: 99,90", "Validação");
                    txtPrecoOfertaProduto.Focus();
                    return;
                }

                if (oferta <= 0 || oferta >= preco)
                {
                    BetaFitMessageBox.Aviso(
                        this,
                        "O preço de oferta deve ser maior que zero e menor que o preço original.",
                        "Validação");
                    txtPrecoOfertaProduto.Focus();
                    return;
                }

                precoOferta = oferta;
            }

            if (!TentarLerInteiro(txtEstoqueProduto.Text, 999, out int estoque) ||
                estoque < 0 || estoque > 100000)
            {
                BetaFitMessageBox.Aviso(this, "Informe um estoque entre 0 e 100000.", "Validação");
                txtEstoqueProduto.Focus();
                return;
            }

            if (!TentarLerInteiro(txtAlertaEstoqueProduto.Text, 5, out int alertaEstoque) ||
                alertaEstoque < 0 || alertaEstoque > 100000)
            {
                BetaFitMessageBox.Aviso(this, "Informe um alerta de estoque entre 0 e 100000.", "Validação");
                txtAlertaEstoqueProduto.Focus();
                return;
            }

            if (cboCategoriaProduto.SelectedIndex <= 0)
            {
                BetaFitMessageBox.Aviso(this, "Selecione uma categoria.", "Validação");
                return;
            }

            if (cboGenero.SelectedIndex <= 0)
            {
                BetaFitMessageBox.Aviso(this, "Selecione um gênero.", "Validação");
                return;
            }

            // -----------------------------------------------------------------
            // 2. Extração de dados
            // -----------------------------------------------------------------
            var categoriaIdx = cboCategoriaProduto.SelectedIndex - 1;
            var categoria = _categorias[categoriaIdx];

            string generoTexto = cboGenero?.SelectedItem?.ToString()!;
            Enum.TryParse<BetaFit.Domain.Enums.Gender>(generoTexto, out var generoEnum);

            var tamanhos = LerItensMarcados(clbTamanhos);
            var cores = LerItensMarcados(clbCores);

            // Mesma regra do catálogo da web: acessório não usa tamanho.
            bool ehAcessorio = categoria.Name.Contains("acess", StringComparison.OrdinalIgnoreCase);
            if (ehAcessorio)
            {
                tamanhos.Clear();
            }
            else if (tamanhos.Count == 0)
            {
                bool seguir = BetaFitMessageBox.Confirmar(
                    this,
                    "Nenhum tamanho foi selecionado. O cliente não vai conseguir escolher " +
                    "tamanho para este produto no site.\n\nDeseja salvar mesmo assim?",
                    "Sem tamanhos");

                if (!seguir)
                {
                    clbTamanhos.Focus();
                    return;
                }
            }

            // Imagens adicionais e fotos por cor não são editáveis aqui (isso é
            // feito na galeria da web), então repassamos o que já existe para
            // não perder o cadastro no PUT.
            var imagensAdicionais = _productExistente?.ImageUrls ?? new List<string>();
            var fotosPorCor = _productExistente?.ColorImageUrls
                              ?? new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

            // -----------------------------------------------------------------
            // 3. Montagem do DTO
            // -----------------------------------------------------------------
            if (_productExistente == null)
            {
                ProductDto = new CreateProductDto
                {
                    Name = txtNomeProduto.Text.Trim(),
                    Description = txtDescricaoProduto.Text.Trim(),
                    Price = preco,
                    Sku = txtSkuProduto.Text.Trim(),
                    SalePrice = precoOferta,
                    Stock = estoque,
                    LowStockThreshold = alertaEstoque,
                    ImageUrl = txtUrlImagemProduto.Text.Trim(),
                    ImageUrls = imagensAdicionais,
                    AvailableSizes = tamanhos,
                    AvailableColors = cores,
                    ColorImageUrls = fotosPorCor,
                    CategoryId = categoria.Id,
                    Gender = generoEnum,
                    IsFeatured = swDestaque.Checked,
                    IsActive = swAtivo.Checked
                };
            }
            else
            {
                UpdateDto = new UpdateProductDto
                {
                    Name = txtNomeProduto.Text.Trim(),
                    Description = txtDescricaoProduto.Text.Trim(),
                    Price = preco,
                    Sku = txtSkuProduto.Text.Trim(),
                    SalePrice = precoOferta,
                    Stock = estoque,
                    LowStockThreshold = alertaEstoque,
                    ImageUrl = txtUrlImagemProduto.Text.Trim(),
                    ImageUrls = imagensAdicionais,
                    AvailableSizes = tamanhos,
                    AvailableColors = cores,
                    ColorImageUrls = fotosPorCor,
                    CategoryId = categoria.Id,
                    Gender = generoEnum,
                    IsFeatured = swDestaque.Checked,
                    IsActive = swAtivo.Checked
                };
            }

            this.DialogResult = DialogResult.OK;
            this.Close();
        }


        // =====================================================================
        // FECHAR O FORMULÁRIO (CANCELAR)
        // =====================================================================
        private void btnCancelarProduto_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        // =====================================================================
        // FECHAR O FORMULÁRIO (X)
        // =====================================================================
        private void btnFecharNovoProduto_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void lblGeneroProduto_Click(object sender, EventArgs e)
        {

        }

        // =====================================================================
        // HANDLERS QUE FALTAVAM NO CODE-BEHIND
        // =====================================================================
        // O Designer.cs referencia esses 9 métodos (normalmente criados ao dar
        // duplo clique em um controle no editor visual do Visual Studio), mas
        // eles nunca foram salvos aqui na classe parcial — por isso o build
        // falhava com CS1061 ("não contém uma definição para ..."). Sem esses
        // métodos o projeto simplesmente não compila, então mesmo os rótulos
        // que são só cabeçalhos de seção (sem lógica própria) precisam de um
        // stub, igual ao lblGeneroProduto_Click acima.

        private void cboGenero_SelectedIndexChanged(object sender, EventArgs e)
        {

        }

        private void lblUrlImagemProduto_Click(object sender, EventArgs e)
        {

        }

        private void txtUrlImagemProduto_TextChanged(object sender, EventArgs e)
        {

        }

        private void lblImagemLocalProduto_Click(object sender, EventArgs e)
        {

        }

        private void lblArquivoSelecionadoProduto_Click(object sender, EventArgs e)
        {

        }

        private void swAtivo_CheckedChanged(object sender, EventArgs e)
        {

        }

        // Clicar no texto "ATIVO" também alterna o switch ao lado — alvo de
        // clique maior, mais fácil de acertar do que só o switch pequeno.
        private void lblAtivo_Click(object sender, EventArgs e)
        {
            swAtivo.Checked = !swAtivo.Checked;
        }

        private void swDestaque_CheckedChanged(object sender, EventArgs e)
        {

        }

        // Mesmo padrão do "ATIVO": clicar no texto "EM DESTAQUE" alterna o switch.
        private void lblDestaque_Click(object sender, EventArgs e)
        {
            swDestaque.Checked = !swDestaque.Checked;
        }
    }
}