# Manual do aplicativo Desktop

[Voltar ao README](../README.md)

## Finalidade e requisitos

O `BetaFit.Desktop` é o aplicativo de gestão da equipe BetaFit, desenvolvido em Windows Forms com componentes Guna.UI2. Requer Windows e utiliza o framework `net8.0-windows`.

Para executar pelo código-fonte, instale o .NET SDK 8 e conclua a [configuração da API e do banco](instalacao.md). O Desktop consome a API por HTTP. A UI fornece as imagens locais dos produtos.

## Iniciar

1. Inicie a API e aguarde migrations e seed terminarem.
2. Inicie a UI para disponibilizar as imagens.
3. Em outro terminal, na raiz do projeto, execute:

```powershell
dotnet restore BetaFit.Desktop/BetaFit.Desktop.csproj
dotnet run --project BetaFit.Desktop/BetaFit.Desktop.csproj
```

Faça login com uma conta da equipe. Para a primeira execução em banco novo, use a conta `Admin` informada no [README](../README.md). Contas comuns de cliente não têm acesso ao aplicativo de gestão.

## Descoberta dos endereços

| Endereço | Primeira origem consultada | Fallback |
| --- | --- | --- |
| API | `BetaFit.API/Properties/launchSettings.json` | `ApiSettings:BaseUrl` no JSON junto ao executável |
| UI, para imagens | `BetaFit.UI/Properties/launchSettings.json` | `UiSettings:BaseUrl` no JSON junto ao executável |

Os resolvers priorizam o perfil `http`, depois `https` e `IIS Express`. Preserve a estrutura de pastas durante o desenvolvimento. Se os endereços mudarem, reinicie o Desktop, pois a resolução é armazenada em cache.

Para uma execução fora da árvore do código, na qual os arquivos de perfil não sejam encontrados, prepare um `appsettings.json` junto ao executável:

```json
{
  "ApiSettings": {
    "BaseUrl": "https://localhost:7204/"
  },
  "UiSettings": {
    "BaseUrl": "https://localhost:7110/"
  },
  "AppSettings": {
    "AppName": "BetaFit Desktop",
    "Version": "1.0.0",
    "Timeout": "30"
  }
}
```

As URLs são exemplos locais e devem apontar para os processos disponíveis. O projeto Desktop não declara uma regra própria de cópia desse arquivo: confira sua presença e conteúdo no diretório final, sem pressupor que ele foi preparado automaticamente. Essa configuração é um fallback; os perfis encontrados continuam tendo prioridade.

## Módulos

| Módulo | Uso |
| --- | --- |
| Dashboard | Consultar indicadores disponibilizados pela API |
| Produtos | Consultar e manter catálogo, imagens, preços e dados de estoque |
| Categorias | Organizar as categorias dos produtos |
| Pedidos | Consultar compras, detalhes e atualizar andamento |
| Funcionários | Administrar contas internas, conforme permissão de administrador |
| Perfil | Consultar e alterar dados da conta |

Use as opções exibidas no menu lateral. A disponibilidade visual e a autorização do backend podem diferir; em caso de `403`, confira as permissões da rota em [API](api.md).

## Rotina de gestão sugerida

1. Entre com o administrador de demonstração.
2. Confira as categorias e os produtos inicializados pelo seed.
3. Abra um produto e revise preço, disponibilidade, imagens e variações antes de salvar alterações.
4. Faça uma compra de teste pela UI com uma conta de cliente.
5. Localize o pedido no Desktop e confira os itens.
6. Atualize o andamento conforme o fluxo disponível e consulte o resultado pela UI.

O estoque pode ser separado por tamanho e cor. A API restringe ajustes rápidos em produtos com variações e exige manutenção do estoque por variação. Pedidos possuem regras que bloqueiam regressões e mudanças em estados finais.

## Permissões

O código utiliza perfis como `Admin`, `Gerente`, `Estoquista`, `Funcionario` e `Usuario`, mas eles não são intercambiáveis. Por exemplo, o dashboard aceita `Admin` e `Gerente`, e a API de produtos aceita `Admin`, `Funcionario` e `Estoquista`.

O seed inicial cria `Admin`, `Usuario` e `Funcionario`. A criação e atribuição dos demais perfis devem ser conferidas no fluxo de gestão de usuários. Para verificar todos os módulos em uma apresentação inicial, utilize a conta `Admin` do banco de demonstração.

## Imagens e conexão

Se os dados carregarem e as imagens não, confira se a UI está iniciada e se `UiBaseUrl` corresponde ao endereço acessível. Os caminhos relativos dos produtos são resolvidos a partir da UI.

Se o login falhar por conexão, confira a URL da API apresentada na tela, os processos em execução e o certificado HTTPS. Consulte [solução de problemas](solucao-de-problemas.md).
