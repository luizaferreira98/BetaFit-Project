# BetaFit

Sistema de comércio eletrônico de roupas, calçados e acessórios fitness, com loja virtual, API e aplicativo desktop para gestão da loja.

O **BetaFit foi desenvolvido em grupo como projeto final do Senac**, com o objetivo de aplicar conhecimentos de desenvolvimento de sistemas, integração entre aplicações, banco de dados e organização de software em camadas.

## Funcionalidades

- Catálogo com categorias, produtos em destaque, imagens, tamanhos, cores e controle de estoque.
- Cadastro e autenticação de clientes, perfil, endereços e recuperação de senha.
- Carrinho, favoritos, cupons, cálculo de frete e criação de pedidos.
- Acompanhamento de pedidos, avaliações, notificações e atendimento.
- Administração de produtos, categorias, usuários, pedidos e relatórios, conforme as permissões da conta.
- Fluxos demonstrativos de pagamento e código de integração com Mercado Pago.

## Tecnologias e organização

| Tecnologia ou padrão | Utilização |
| --- | --- |
| C# e .NET 8 | Base da solução |
| ASP.NET Core Web API | Endpoints HTTP e integração entre módulos |
| ASP.NET Core MVC e Razor | Interface web, com HTML, CSS e JavaScript |
| Windows Forms e Guna.UI2.WinForms | Aplicativo desktop para Windows |
| Entity Framework Core 8 e SQL Server | Persistência e migrations |
| ASP.NET Core Identity | Usuários, senhas, perfis de acesso e autenticação por cookies |
| Swagger / Swashbuckle | Documentação interativa da API |
| SignalR | Canal de notificações de pedidos |
| Injeção de dependência, DTOs e repositórios | Separação de responsabilidades |

A solução possui uma organização em camadas inspirada em Clean Architecture. A implementação também contém controllers que acessam diretamente a infraestrutura, portanto a separação não é uma implementação estrita desse padrão. Veja [arquitetura](docs/arquitetura.md).

## Estrutura

```text
BetaFit.slnx
BetaFit.API/              # API, autenticação e inicialização do banco
BetaFit.Application/      # Serviços, DTOs, interfaces e ViewModels
BetaFit.Domain/           # Entidades e contratos de repositórios
BetaFit.Infraestructure/  # EF Core, Identity, repositórios e migrations
BetaFit.UI/               # Loja virtual e administração web
BetaFit.Desktop/          # Aplicativo Windows Forms
docs/                    # Documentação complementar
```

O nome `BetaFit.Infraestructure` reproduz a grafia utilizada no projeto.

## Pré-requisitos

- Windows para executar todos os módulos, incluindo o Desktop e o SQL Server LocalDB do exemplo.
- .NET SDK 8 instalado.
- SQL Server LocalDB ou outra instância SQL Server acessível.
- Git, se o projeto for obtido por clonagem.
- Acesso à internet para restaurar os pacotes NuGet.

Os comandos abaixo usam PowerShell e arquivos `.csproj`, sem depender do suporte da ferramenta ao formato de solução `.slnx`.

## Executar localmente

### 1. Obter o projeto

Substitua a URL ilustrativa pela URL do seu repositório:

```powershell
git clone https://github.com/SEU-USUARIO/SEU-REPOSITORIO.git BetaFit
cd BetaFit
```

Se você baixou um ZIP, extraia-o e abra um terminal na pasta que contém `BetaFit.slnx`.

### 2. Configurar

```powershell
Copy-Item BetaFit.API/appsettings.example.json BetaFit.API/appsettings.json
dotnet dev-certs https --trust
```

Copie o arquivo de exemplo somente se ainda não existir uma configuração local. Em `BetaFit.API/appsettings.json`, ajuste `ConnectionStrings:DefaultConnection` para sua instância SQL Server. O exemplo usa `(localdb)\mssqllocaldb` e o banco `BetaFitDb`.

Para demonstração local sem envio externo de e-mails, adicione esta seção ao JSON da API, preservando as demais:

```json
"Email": { "Mode": "Outbox" }
```

Nesse modo, em ambiente `Development`, o conteúdo dos e-mails aparece no terminal da API. Consulte [instalação e configuração](docs/instalacao.md) para os detalhes.

### 3. Iniciar a API

```powershell
dotnet restore BetaFit.API/BetaFit.API.csproj
dotnet run --project BetaFit.API/BetaFit.API.csproj --launch-profile https
```

A API aplica as migrations e executa o seed na inicialização. Aguarde a conclusão e abra **https://localhost:7204/** para acessar o Swagger. A interface do Swagger está na raiz; o endereço `/swagger` presente no perfil de execução não corresponde à configuração atual da interface.

### 4. Iniciar a interface web

Em outro terminal, na raiz do projeto:

```powershell
dotnet restore BetaFit.UI/BetaFit.UI.csproj
dotnet run --project BetaFit.UI/BetaFit.UI.csproj --launch-profile https
```

Acesse **https://localhost:7110/**. Mantenha a API em execução.

### 5. Iniciar o Desktop

Em um terceiro terminal, no Windows:

```powershell
dotnet restore BetaFit.Desktop/BetaFit.Desktop.csproj
dotnet run --project BetaFit.Desktop/BetaFit.Desktop.csproj
```

Mantenha também a UI em execução para disponibilizar as imagens dos produtos. O Desktop localiza as URLs pelos arquivos `launchSettings.json`. Consulte o [manual Desktop](docs/manual-desktop.md).

## Primeiro acesso

Em um banco novo, o seed cria a conta demonstrativa:

| E-mail | Senha inicial | Perfil |
| --- | --- | --- |
| `admin@betafit.com` | `Admin@123` | `Admin` |

Esses dados pertencem ao seed do projeto e destinam-se à demonstração local. Uma conta já existente mantém sua senha. Antes de disponibilizar uma instalação publicamente, substitua a credencial inicial e revise o seed.

Clientes podem usar o cadastro da loja. O domínio interno `@betafit` é reservado pela aplicação para contas criadas pela administração.

## Documentação

- [Instalação e configuração](docs/instalacao.md)
- [Arquitetura e fluxos de dados](docs/arquitetura.md)
- [Rotas e uso da API](docs/api.md)
- [Manual da interface web](docs/manual-ui.md)
- [Manual do aplicativo Desktop](docs/manual-desktop.md)
- [Solução de problemas](docs/solucao-de-problemas.md)

## Contexto acadêmico e autoria

Projeto final realizado no **Senac**, desenvolvido de forma colaborativa por um grupo de estudantes. A autoria é coletiva; os nomes dos integrantes podem ser acrescentados posteriormente nesta seção.

Esta documentação descreve o código disponibilizado. Os fluxos demonstrativos de cartão, Pix, boleto e solicitação de reembolso não comprovam cobrança ou estorno real. A execução completa com banco e serviços externos não foi validada durante a elaboração da documentação.
