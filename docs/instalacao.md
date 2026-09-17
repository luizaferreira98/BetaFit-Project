# Instalação e configuração

[Voltar ao README](../README.md)

## Ambiente local

O caminho descrito utiliza Windows, PowerShell, .NET SDK 8 e SQL Server LocalDB. Para usar outra instância SQL Server, altere a conexão. O Desktop depende de Windows Forms e do framework `net8.0-windows`.

Verifique o SDK:

```powershell
dotnet --list-sdks
```

Execute os comandos a partir da pasta que contém `BetaFit.slnx`. Preserve os diretórios originais, inclusive os arquivos de imagens e recursos.

## Banco de dados

Se ainda não existir `BetaFit.API/appsettings.json`, crie-o a partir de `appsettings.example.json`:

```powershell
Copy-Item BetaFit.API/appsettings.example.json BetaFit.API/appsettings.json
```

A conexão padrão é:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=BetaFitDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}
```

O trecho mostra apenas a seção de conexão; mantenha as outras seções do arquivo. Para SQL Server Express ou um servidor remoto, informe a instância e o método de autenticação do seu ambiente.

`BetaFit.API/Program.cs` aplica migrations com `MigrateAsync()`. Em seguida, `SeedData.SeedAsync()` aplica migrations novamente e inicializa o catálogo, perfis de acesso e administrador. A conta usada na conexão precisa ter permissão para criar ou atualizar o banco.

O seed também contém lógica de atualização de registros do catálogo. Reiniciar a API pode afetar dados associados ao catálogo inicial; consulte `BetaFit.Infraestructure/Identity/SeedData.cs` antes de usar uma base com dados próprios.

Não é necessário criar uma nova migration para executar o projeto pela primeira vez.

## Endereços e perfis

| Processo | Perfil HTTPS | Endereço HTTP também configurado |
| --- | --- | --- |
| API | `https://localhost:7204` | `http://localhost:5168` |
| UI | `https://localhost:7110` | `http://localhost:5085` |

Use o perfil `https` para iniciar ambos os projetos: ele disponibiliza os dois endereços definidos no perfil. A UI e o Desktop procuram primeiro o perfil **`http`** no `launchSettings.json` da API, mesmo quando o processo foi iniciado pelo perfil `https`. O endereço HTTP pode redirecionar para HTTPS, por isso confie no certificado local:

```powershell
dotnet dev-certs https --trust
```

Na UI, ajuste `ApiSettings:BaseUrl` para `https://localhost:7204/` em `BetaFit.UI/appsettings.json`. O arquivo fornecido aponta para uma API hospedada. Em desenvolvimento, a descoberta dos arquivos locais normalmente tem prioridade, mas o valor local evita utilizar a API hospedada como fallback.

Alterar somente `ApiSettings:BaseUrl` pode não alterar o endereço usado enquanto o resolver encontrar `launchSettings.json`. Se mudar portas, mantenha os perfis sincronizados e reinicie os clientes.

## Configurações da API

| Chave | Uso |
| --- | --- |
| `ConnectionStrings:DefaultConnection` | Conexão SQL Server |
| `Cors:Origins` | Origens de navegador permitidas, com credenciais |
| `App:PublicBaseUrl` | URL da UI usada nos retornos do checkout externo |
| `Api:PublicBaseUrl` | URL da API usada no webhook de pagamento |
| `Email:Mode` | `Outbox` registra e-mails no terminal em `Development` |
| `Resend:ApiKey` | Credencial para envio externo de e-mails |
| `Resend:From` | Remetente utilizado no envio |
| `MercadoPago:AccessToken` | Credencial utilizada pelo controller de pagamentos |
| `Reviews:BlockedWords` | Lista opcional de termos para validação de comentários |

Para desenvolvimento com os perfis indicados, use `App:PublicBaseUrl` igual a `https://localhost:7110` e `Api:PublicBaseUrl` igual a `https://localhost:7204`. Mantenha `https://localhost:7110` e `http://localhost:5085` entre as origens permitidas quando houver chamadas diretas do navegador.

### E-mails de demonstração

Acrescente ao objeto principal do JSON da API:

```json
"Email": {
  "Mode": "Outbox"
}
```

Esse modo só funciona em `Development`: nenhum e-mail é enviado e o HTML, incluindo códigos ou links de confirmação, aparece no log. Ele permite demonstrar recuperação de conta e confirmações sem configurar uma conta Resend. O conteúdo desses logs inclui informações de conta.

Para envio externo, configure chave e remetente Resend. `ResendEmailSender` também aceita `RESEND_API_KEY` como fallback da chave. Para o remetente, prefira configurar `Resend:From`: um valor vazio nessa chave não aciona o fallback `RESEND_FROM`.

### Pagamentos

O checkout acadêmico possui pagamentos demonstrativos. A rota `/api/payments/preferences` representa um caminho adicional de integração com Mercado Pago e exige `MercadoPago:AccessToken`; sem ele, retorna `503`.

Configurar essa chave não transforma automaticamente o fluxo demonstrativo em pagamento real. Para validar a integração, é necessário testar o contrato do provedor, os retornos e o webhook em um ambiente apropriado. Um provedor externo não consegue acessar um webhook em `localhost`. Esta documentação não registra validação dessa integração.

Não inclua credenciais privadas no repositório. As configurações padrão do ASP.NET Core permitem fornecer chaves por variáveis de ambiente, usando `__` entre os níveis, por exemplo `MercadoPago__AccessToken` e `Resend__ApiKey`.

## Compilar e executar

Em terminais separados:

```powershell
# Terminal 1: API
dotnet restore BetaFit.API/BetaFit.API.csproj
dotnet run --project BetaFit.API/BetaFit.API.csproj --launch-profile https
```

```powershell
# Terminal 2: UI
dotnet restore BetaFit.UI/BetaFit.UI.csproj
dotnet run --project BetaFit.UI/BetaFit.UI.csproj --launch-profile https
```

```powershell
# Terminal 3: Desktop
dotnet restore BetaFit.Desktop/BetaFit.Desktop.csproj
dotnet run --project BetaFit.Desktop/BetaFit.Desktop.csproj
```

`dotnet run` compila o projeto. Aguarde a API concluir as migrations antes de abrir os clientes.

## Conferir a instalação

1. Abra `https://localhost:7204/` e verifique o Swagger.
2. Consulte `GET /api/products` para conferir acesso ao catálogo.
3. Abra `https://localhost:7110/` e verifique produtos e imagens.
4. Faça login com a conta demonstrativa do [README](../README.md).
5. Abra o Desktop e confirme que ele consegue consultar a API.
6. Para testar a compra, use uma conta de cliente e dados de demonstração.

Essas etapas são um roteiro de verificação local; não representam testes executados durante a produção desta documentação.
