# API: rotas e utilização

[Voltar ao README](../README.md)

## Acesso

- Base local: `https://localhost:7204`.
- Swagger UI: `https://localhost:7204/`.
- Documento OpenAPI: `https://localhost:7204/swagger/v1/swagger.json`.
- Conteúdo usual de requisição: `application/json`.

As rotas abaixo foram identificadas nos controllers do código fornecido. O Swagger apresenta os schemas completos dos contratos. As restrições `:int` dos parâmetros foram omitidas das tabelas para facilitar a leitura; IDs de produtos, categorias e pedidos são numéricos, enquanto IDs de usuários e cartões são strings.

## Autenticação e autorização

O login retorna os dados da conta e emite um cookie de autenticação. Preserve e reenvie esse cookie nas próximas requisições. Não utilize um Bearer token como substituto da sessão do Identity.

Nas tabelas:

- **Público:** sem exigência de login na rota.
- **Autenticado:** exige sessão; as ações ainda podem validar titularidade e estado do recurso.
- **Equipe:** admite `Admin`, `Funcionario` ou `Estoquista`.
- Perfis indicados nominalmente correspondem às restrições declaradas nos controllers.

### Exemplo no PowerShell

Com a API local iniciada e o certificado confiável:

```powershell
$baseApi = 'https://localhost:7204'
$dadosLogin = @{
    email = 'admin@betafit.com'
    password = 'Admin@123'
} | ConvertTo-Json

Invoke-RestMethod -Uri "$baseApi/api/auth/login" -Method Post -ContentType 'application/json' -Body $dadosLogin -SessionVariable sessaoBetaFit
Invoke-RestMethod -Uri "$baseApi/api/auth/me" -WebSession $sessaoBetaFit
Invoke-RestMethod -Uri "$baseApi/api/products" -WebSession $sessaoBetaFit
Invoke-RestMethod -Uri "$baseApi/api/auth/logout" -Method Post -WebSession $sessaoBetaFit
```

As credenciais são as do seed de demonstração para banco novo. Para outra conta ou senha alterada, ajuste o corpo do login.

## Contas

| Método | Rota | Acesso | Operação |
| --- | --- | --- | --- |
| POST | `/api/auth/register` | Público | Cadastrar cliente |
| POST | `/api/auth/login` | Público | Autenticar e emitir cookie |
| POST | `/api/auth/logout` | Autenticado | Encerrar sessão |
| GET | `/api/auth/me` | Autenticado | Consultar usuário atual |
| POST | `/api/password/forgot` | Público | Solicitar recuperação de senha |
| POST | `/api/password/reset` | Público | Redefinir senha com os dados de recuperação |

O cadastro utiliza `RegisterDto`, com nome, e-mail, telefone, data de nascimento, gênero, senha e confirmação. O controller exige idade mínima de 18 anos e restringe o domínio interno da loja. A política de senha exige pelo menos seis caracteres, incluindo maiúscula, minúscula, número e caractere não alfanumérico.

## Catálogo

| Método | Rota | Acesso | Operação |
| --- | --- | --- | --- |
| GET | `/api/products` | Público | Listar produtos |
| GET | `/api/products/{id}` | Público | Consultar produto |
| GET | `/api/products/featured` | Público | Consultar destaques |
| GET | `/api/products/category/{categoryId}` | Público | Consultar por categoria |
| POST | `/api/products` | Equipe | Criar produto |
| PUT | `/api/products/{id}` | Equipe | Atualizar produto |
| DELETE | `/api/products/{id}` | Equipe | Excluir produto conforme regras do serviço |
| GET | `/api/categories` | Público | Listar categorias |
| GET | `/api/categories/{id}` | Público | Consultar categoria |
| POST | `/api/categories` | Equipe | Criar categoria |
| PUT | `/api/categories/{id}` | Equipe | Atualizar categoria |
| DELETE | `/api/categories/{id}` | Equipe | Excluir categoria conforme regras do serviço |

## Carrinho e favoritos

Todas estas rotas exigem autenticação e operam sobre o usuário atual.

| Método | Rota | Operação |
| --- | --- | --- |
| GET | `/api/cart` | Consultar carrinho |
| GET | `/api/cart/coupon-preview?code={codigo}` | Simular aplicação de cupom ao carrinho |
| POST | `/api/cart` | Adicionar produto e variação |
| PUT | `/api/cart/{productId}` | Alterar quantidade da variação informada |
| DELETE | `/api/cart/{productId}?size={tamanho}&color={cor}` | Remover item da variação informada |
| DELETE | `/api/cart` | Limpar carrinho |
| GET | `/api/favorites` | Listar favoritos |
| GET | `/api/favorites/{productId}` | Consultar favorito |
| POST | `/api/favorites/{productId}` | Adicionar favorito |
| DELETE | `/api/favorites/{productId}` | Remover favorito |

`POST /api/cart` e `PUT /api/cart/{productId}` recebem `AddCartItemDto`: `productId`, `quantity`, `size` e `color`. Os valores devem corresponder ao catálogo e ao estoque disponível. Tamanho e cor identificam a variação, além do produto.

## Pedidos e jornada do cliente

| Método | Rota | Acesso | Operação |
| --- | --- | --- | --- |
| GET | `/api/orders` | Equipe | Listar pedidos da loja |
| GET | `/api/orders/mine` | Autenticado | Listar os próprios pedidos |
| GET | `/api/orders/{id}` | Titular ou equipe | Consultar pedido |
| POST | `/api/orders` | Autenticado | Criar pedido |
| POST | `/api/orders/{id}/cancel` | Titular | Cancelar pedido elegível |
| PUT | `/api/orders/{id}/delivery` | Titular | Corrigir endereço antes da preparação |
| POST | `/api/orders/{id}/confirm-demo-payment` | Titular | Confirmar pagamento demonstrativo |
| PATCH | `/api/orders/{id}/status` | Equipe | Atualizar status |
| POST | `/api/orders/{id}/received` | Titular | Confirmar recebimento de pedido enviado |
| POST | `/api/orders/{id}/refund` | Titular | Solicitar reembolso após entrega |
| POST | `/api/orders/{id}/experience` | Titular | Avaliar experiência após entrega |
| POST | `/api/orders/{id}/tracking` | Equipe | Atualizar rastreamento |
| GET | `/api/orders/{id}/messages` | Titular ou equipe | Listar mensagens do pedido |
| POST | `/api/orders/{id}/messages` | Titular ou equipe | Enviar mensagem do pedido |

`CreateOrderDto` inclui `items`, `shippingRuleId`, `expectedTotal`, `paymentMethod`, CPF de demonstração e endereço. Pode incluir `couponCode`, `savedCardId` e `installments`. O serviço recalcula os valores e valida os dados; não basta enviar apenas um total.

Formas aceitas no DTO: `Pix`, `Credito`, `Debito` e `Boleto`. Crédito e débito dependem de cartão demonstrativo cadastrado. A confirmação de pagamento demonstrativo não realiza cobrança financeira.

`PATCH /api/orders/{id}/status` recebe uma **string JSON**, por exemplo:

```json
"EmPreparacao"
```

Não envie `{ "status": "EmPreparacao" }` nessa rota: o parâmetro do controller é `[FromBody] string status`.

Cancelamento pelo cliente e correção de endereço são permitidos em `Pendente` ou `Confirmado`. A mudança de CEP em pedido com frete calculado exige cancelar e refazer o pedido. O serviço restringe regressões de status e alterações em estados finais. A solicitação de reembolso apenas registra a solicitação; não processa estorno real.

## Perfil

Todas as rotas exigem autenticação.

| Método | Rota | Operação |
| --- | --- | --- |
| GET | `/api/profile` | Consultar perfil |
| GET | `/api/profile/pending-change` | Consultar alteração pendente |
| POST | `/api/profile/email-change` | Solicitar troca de e-mail |
| PUT | `/api/profile` | Atualizar perfil e solicitar alterações sensíveis |
| PUT | `/api/profile/checkout-address` | Atualizar endereço de checkout |
| PUT | `/api/profile/card` | Cadastrar cartão demonstrativo |
| DELETE | `/api/profile/cards/{id}` | Remover cartão |
| POST | `/api/profile/confirm-change` | Confirmar alteração pendente |

Use os contratos de `AuthDto.cs` e `AccountRecoveryDtos.cs` em `BetaFit.Application/DTOs`. Operações com confirmação por e-mail dependem de Resend ou do modo local `Outbox`.

## Frete e pagamentos

| Método | Rota | Acesso | Operação |
| --- | --- | --- | --- |
| POST | `/api/shipping/quote` | Público | Cotar frete e valores dos itens |
| GET | `/api/shipping/rules` | Admin | Listar regras de frete |
| POST | `/api/shipping/rules` | Admin | Criar regra |
| PUT | `/api/shipping/rules/{id}` | Admin | Atualizar regra |
| POST | `/api/payments/preferences` | Titular ou equipe | Criar preferência no gateway configurado |
| POST | `/api/payments/webhook` | Público | Receber evento e consultar pagamento no gateway |

`POST /api/shipping/quote` recebe `ShippingQuoteRequest`. O cupom depende de usuário autenticado, mesmo quando a cotação de frete é pública. A resposta pode conter `couponError` ou `shippingError` com HTTP `200`; confira esses campos.

`POST /api/payments/preferences` recebe um **inteiro JSON**, como `12`, representando o pedido. O webhook recebe o formato `MpWebhook`, com `type` e `data.id`. Consulte [instalação](instalacao.md) para limites e configuração da integração externa.

## Avaliações

| Método | Rota | Acesso | Operação |
| --- | --- | --- | --- |
| GET | `/api/reviews/product/{productId}` | Público | Listar avaliações do produto |
| GET | `/api/reviews/order/{orderId}` | Autenticado | Consultar avaliações do pedido conforme titularidade |
| POST | `/api/reviews/order/{orderId}/product/{productId}` | Autenticado | Avaliar item elegível |
| GET | `/api/reviews/moderation` | Admin | Consultar avaliações para moderação |
| PUT | `/api/reviews/{id}/moderation` | Admin | Moderar avaliação |

A aplicação verifica a elegibilidade do pedido e valida comentários. Consulte `ReviewDtos.cs` e `ReviewPolicy.cs` para os contratos e critérios.

## Administração

| Método | Rota | Acesso | Operação |
| --- | --- | --- | --- |
| GET | `/api/usuarios` | Admin | Listar usuários |
| POST | `/api/usuarios` | Admin | Criar usuário interno |
| PUT | `/api/usuarios/{id}` | Admin | Atualizar usuário |
| DELETE | `/api/usuarios/{id}` | Admin | Excluir usuário conforme regras do serviço |
| GET | `/api/dashboard` | Admin ou Gerente | Consultar indicadores |
| GET | `/api/reports` | Admin | Consultar relatório da loja |
| GET | `/api/coupons` | Admin | Listar cupons |
| POST | `/api/coupons` | Admin | Criar cupom |
| POST | `/api/coupons/{id}/toggle` | Admin | Alternar ativação do cupom |
| POST | `/api/management/categories/reorder` | Equipe | Reordenar categorias |
| POST | `/api/management/categories/{id}/toggle` | Equipe | Definir ativação da categoria |
| POST | `/api/management/products/{id}/quick` | Equipe | Ajustar preço ou estoque |
| GET | `/api/site-settings` | Público | Consultar configurações públicas do site |
| PUT | `/api/site-settings` | Admin | Atualizar configurações do site |

Reordenação recebe uma lista JSON com todos os IDs das categorias, sem repetição. Ativação da categoria recebe um booleano JSON (`true` ou `false`). Produtos com variações exigem edição de estoque por variação, em vez do ajuste rápido.

## Suporte e notificações

Todas as rotas exigem autenticação. O suporte verifica o acesso à conversa conforme a conta.

| Método | Rota | Operação |
| --- | --- | --- |
| GET | `/api/support` | Listar conversas disponíveis |
| GET | `/api/support/{id}` | Consultar conversa |
| POST | `/api/support` | Abrir atendimento |
| POST | `/api/support/{id}/messages` | Enviar mensagem |
| GET | `/api/notifications` | Consultar notificações |
| POST | `/api/notifications/read-all` | Marcar notificações como lidas |

## SignalR e rota de exemplo

O hub SignalR está em `/hubs/orders`. O evento `OrderFinalized` transporta `{ "orderId": 12 }` quando disparado pela atualização de entrega no controller de pedidos. Trata-se de um hub, não de uma rota REST comum.

O projeto mantém `GET /WeatherForecast`, endpoint público de exemplo do template, sem relação funcional com a loja.

## Respostas e erros

| Código | Significado no projeto |
| --- | --- |
| 200 | Operação concluída, geralmente com corpo |
| 201 | Recurso criado, quando usado pelo controller |
| 204 | Operação concluída sem corpo |
| 400 | Dados inválidos ou regra de negócio não atendida |
| 401 | Sessão ausente ou login inválido |
| 403 | Permissão insuficiente |
| 404 | Recurso não encontrado ou indisponível no escopo da conta |
| 500 | Falha não tratada; a resposta global inclui `message` e `traceId` |
| 502 | Erro de comunicação ou resposta inválida do gateway |
| 503 | Gateway não configurado na criação de preferência |

Não há um envelope único para todas as respostas: existem DTOs, listas, objetos com `message`, erros de validação e respostas vazias. Os códigos exatos dependem da ação. Para implementação de clientes, confira a assinatura do controller e o schema OpenAPI da versão em execução.
