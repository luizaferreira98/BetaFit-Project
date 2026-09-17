# Manual da interface web

[Voltar ao README](../README.md)

## Iniciar e acessar

Conclua a [instalação](instalacao.md), mantenha a API iniciada e execute:

```powershell
dotnet run --project BetaFit.UI/BetaFit.UI.csproj --launch-profile https
```

Abra `https://localhost:7110/`. A UI utiliza ASP.NET Core MVC, Razor e cultura `pt-BR`, consumindo a API por HTTP.

## Cadastro e login

1. Abra a opção de cadastro da loja ou `/Account/Register`.
2. Preencha nome, e-mail, telefone, data de nascimento, gênero, senha e confirmação.
3. Utilize uma conta de cliente com idade mínima de 18 anos. Endereços do domínio interno BetaFit são reservados à administração.
4. Entre por `/Account/Login` com e-mail e senha.

A senha deve ter pelo menos seis caracteres, com maiúscula, minúscula, número e caractere não alfanumérico. Use a opção de sair para encerrar a sessão.

## Catálogo, favoritos e carrinho

Consulte os produtos e categorias na loja. Na página de produto, observe preço, imagens e opções de tamanho e cor. Escolha as variações disponíveis antes de adicionar ao carrinho.

Carrinho e favoritos dependem de login e são persistidos pela API por usuário. Quantidades são limitadas pelo estoque da variação selecionada. Produtos indisponíveis podem impedir a atualização do carrinho ou a conclusão da compra.

## Finalizar uma compra de demonstração

1. Entre com uma conta de cliente e adicione produtos ao carrinho.
2. Confira quantidades e variações.
3. Informe CEP, endereço completo e CPF de teste no fluxo de checkout.
4. Aplique um cupom, se houver um válido para sua conta e compra.
5. Selecione uma opção de frete retornada pela cotação.
6. Escolha a forma de pagamento: Pix, crédito, débito ou boleto, conforme oferecido pelo fluxo.
7. Para crédito ou débito, selecione um cartão demonstrativo cadastrado no perfil. O parcelamento de crédito aceita de 1 a 12 parcelas; outras formas usam uma parcela.
8. Confira o total atualizado e conclua o pedido.

O servidor recalcula os valores. Se preço, frete, cupom ou estoque mudarem, revise o carrinho e faça uma nova cotação. Os fluxos de cartão, Pix e boleto possuem comportamento demonstrativo; um status de pagamento no sistema não comprova uma transação financeira real.

## Acompanhar pedidos

Abra a área de pedidos e consulte os detalhes. As ações disponíveis dependem do estado do pedido:

| Ação | Condição relevante |
| --- | --- |
| Cancelar como cliente | Pedido `Pendente` ou `Confirmado` |
| Corrigir endereço | Antes da preparação; mudança de CEP com frete exige novo pedido |
| Confirmar recebimento | Pedido `Enviado` |
| Avaliar experiência | Pedido `Entregue`, ainda sem avaliação de experiência |
| Solicitar reembolso | Pedido `Entregue`; a ação registra a solicitação, sem estorno real |

As mensagens do pedido permitem comunicação sobre aquela compra. As avaliações de produtos têm regras próprias de elegibilidade e validação de comentário.

## Perfil, senha e e-mail

Acesse `/Account/Profile` para consultar o perfil e as opções de alteração de dados, endereço e cartões demonstrativos. Alterações sensíveis podem exigir confirmação por e-mail.

Para recuperação de senha e confirmações funcionarem, a API precisa de envio de e-mail configurado ou do modo `Email:Mode = Outbox` em desenvolvimento. Em `Outbox`, consulte o terminal da API para obter o conteúdo da mensagem; não haverá envio para uma caixa de entrada.

## Administração web

Contas com permissão podem acessar funcionalidades administrativas de catálogo, pedidos, usuários, cupons, frete, relatórios e configurações. A disponibilidade depende da tela e do perfil; a API valida a autorização em cada requisição.

Para a primeira demonstração, use o administrador criado pelo seed, documentado no [README](../README.md). Não presuma que `Gerente`, `Funcionario` e `Estoquista` possuem as mesmas permissões: o backend usa conjuntos diferentes conforme a operação.

## Suporte e notificações

Use a área de suporte para abrir ou acompanhar conversas. A área de notificações reúne avisos relacionados à conta. O backend também possui notificações de pedido por SignalR e e-mails, conforme a integração e configuração do ambiente.

Se a loja abrir sem produtos ou imagens, consulte [solução de problemas](solucao-de-problemas.md).
