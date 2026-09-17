# Solução de problemas

[Voltar ao README](../README.md)

Este guia reúne pontos identificados na leitura do código e verificações para execução local.

## A API não inicia

Confira `BetaFit.API/appsettings.json`, a conexão `DefaultConnection` e a disponibilidade da instância SQL Server. A aplicação executa migrations antes de começar a atender requisições; erro de conexão ou de permissão no banco pode impedir a inicialização.

O LocalDB do arquivo de exemplo precisa estar instalado. Ter o .NET SDK instalado não significa que o SQL Server LocalDB também esteja disponível.

## A solução `.slnx` não abre na ferramenta instalada

Os projetos usam .NET 8, mas a solução está no formato `.slnx`. Se sua versão do SDK ou IDE não reconhecer esse formato, execute os projetos individualmente com os comandos `.csproj` do [guia de instalação](instalacao.md), ou utilize uma ferramenta que reconheça a solução.

## O Swagger não abre em `/swagger`

No código analisado, `RoutePrefix = string.Empty` coloca a interface na raiz: `https://localhost:7204/`. O perfil ainda contém `launchUrl = "swagger"`, podendo abrir um endereço que não corresponde à interface. O JSON permanece em `/swagger/v1/swagger.json`.

## Erro de certificado HTTPS

Execute `dotnet dev-certs https --trust` e reinicie os processos. A UI e o Desktop podem resolver uma URL HTTP e receber redirecionamento para HTTPS; confiar no certificado também é relevante nesse caso.

## A UI consulta um servidor remoto ou uma porta incorreta

`BetaFit.UI/appsettings.json` foi fornecido com uma URL hospedada. Para execução local, configure `ApiSettings:BaseUrl` com o endereço local e confira a descoberta de `BetaFit.API/Properties/launchSettings.json`.

Em desenvolvimento, o resolver tem prioridade e procura primeiro o perfil `http`. Alterar apenas o JSON da UI pode não mudar a URL usada. Preserve as pastas, sincronize os perfis e reinicie os processos após qualquer alteração.

## A UI ou o Desktop não consegue conectar à API

Confirme que a API terminou as migrations e que o endereço exibido no terminal responde. Ao usar as portas originais, inicie a API com o perfil `https`, que inclui `https://localhost:7204` e `http://localhost:5168`.

Se uma porta estiver ocupada, identifique o processo responsável ou escolha outra porta e atualize os perfis e configurações relacionados. A descoberta das URLs é armazenada em cache pelos clientes.

## Login retorna `401`

Confira e-mail e senha. Para banco novo, o seed cria `admin@betafit.com` com a senha do [README](../README.md); ele não redefine automaticamente a senha de uma conta que já existe.

Ao testar a API diretamente, mantenha os cookies entre chamadas. A autenticação implementada não usa JWT. Na UI, sair e entrar novamente pode atualizar a sessão após alterações na conta.

## Uma operação retorna `403`

A conta está autenticada, mas não tem a permissão exigida. Os conjuntos de perfis diferem entre rotas e também podem diferir das opções visuais do Desktop. Compare a operação com a [tabela da API](api.md); não trate todos os perfis da equipe como equivalentes.

## E-mails ou confirmações não chegam

Em `Development`, use `Email:Mode = Outbox` e consulte o terminal da API. Esse modo somente registra o conteúdo da mensagem. Para envio externo, configure `Resend:ApiKey` e um remetente válido em `Resend:From`.

O modo `Outbox` não é ativado apenas por deixar a chave vazia. A confirmação de mudanças de conta pode falhar sem um mecanismo de envio configurado.

## Imagens não aparecem

Verifique os arquivos em `BetaFit.UI/wwwroot/images/products`, seus nomes e os caminhos registrados nos produtos. No Desktop, mantenha a UI iniciada e confira `UiSettings:BaseUrl` quando o fallback for usado.

A API não registra `UseStaticFiles()` no `Program.cs` analisado. Não pressuponha que arquivos copiados apenas para `BetaFit.API/wwwroot` serão servidos ao navegador.

## O checkout rejeita o pedido

Leia a mensagem devolvida pela API. Confira estoque por variação, preço atualizado, cupom, endereço, CPF de teste, regra de frete e total esperado. Refaça a cotação após alterações no carrinho. A cotação pode responder HTTP `200` e ainda informar problemas nos campos `shippingError` ou `couponError`.

Para crédito ou débito, selecione um cartão demonstrativo cadastrado e compatível com o método. Não confunda a rota de integração externa com a confirmação demonstrativa.

## Mercado Pago retorna `503`

A criação de preferência exige `MercadoPago:AccessToken`. Sem a chave, o controller retorna `503`. A ausência dessa chave não impede, por si só, o uso das rotas demonstrativas de pagamento. A integração externa precisa de validação própria antes de qualquer uso real.

## Dados do catálogo mudam após reiniciar

A API executa o seed em toda inicialização, e ele contém lógica de atualização do catálogo inicial. Consulte `BetaFit.Infraestructure/Identity/SeedData.cs` ao investigar alterações em produtos associados aos dados de exemplo.

## Falha interna com `traceId`

O tratamento global de exceções retorna mensagem genérica e uma referência `traceId`. Procure essa referência no log da API para identificar a causa. Não há um arquivo de log persistente configurado por esta documentação.

## Limite da verificação documental

A documentação foi conferida por leitura dos projetos, controllers, serviços, configurações e contratos presentes no código fornecido. Não houve execução completa com SQL Server, interface gráfica ou provedores externos. Os passos de instalação e os roteiros dos manuais devem ser verificados no ambiente em que o projeto será apresentado.
