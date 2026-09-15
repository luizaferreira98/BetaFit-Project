(() => {
    const input = document.querySelector('[data-coupon-input]');
    if (!input || document.querySelector('[data-shipping-calculator]')) return;
    const feedback = document.querySelector('[data-coupon-feedback]');
    const total = document.querySelector('[data-checkout-total]');
    const row = document.querySelector('[data-coupon-row]');
    const discount = document.querySelector('[data-coupon-discount]');
    const money = value => Number(value).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
    let subtotal = Number(total.dataset.subtotal), timer, controller, revision = 0;
    const showTotal = value => {
        total.textContent = money(value);
        document.querySelectorAll('select[name="Installments"] option').forEach(option => {
            option.textContent = `${option.value} x de ${money(value / Number(option.value))} sem juros`;
        });
    };
    const preview = async (version, code) => {
        controller = new AbortController();
        try {
            const response = await fetch(`${input.dataset.previewUrl}?code=${encodeURIComponent(code)}`, {
                signal: controller.signal, cache: 'no-store', headers: { Accept: 'application/json' }
            });
            if (response.redirected || response.status === 401) throw new Error('Entre novamente na sua conta para consultar o cupom.');
            const data = await response.json();
            if (version !== revision) return;
            if (!response.ok) throw new Error(data.message || 'Não foi possível consultar o cupom.');
            subtotal = Number(data.subtotal);
            document.querySelector('[data-checkout-subtotal]').textContent = money(subtotal);
            discount.textContent = `− ${money(data.discount)}`;
            row.hidden = false;
            showTotal(data.total);
            feedback.textContent = `${data.code}: ${data.percent}% de desconto (${money(data.discount)}). O cupom será validado novamente ao confirmar.`;
        } catch (error) {
            if (error.name === 'AbortError' || version !== revision) return;
            row.hidden = true;
            showTotal(subtotal);
            feedback.textContent = error instanceof SyntaxError ? 'Não foi possível consultar o cupom. Tente novamente.' : error.message;
        }
    };
    const update = () => {
        clearTimeout(timer);
        controller?.abort();
        const version = ++revision, code = input.value.trim();
        row.hidden = true;
        showTotal(subtotal);
        feedback.textContent = code ? 'Consultando desconto…' : 'Digite o cupom para consultar o desconto. Um uso por cliente.';
        if (code) timer = setTimeout(() => preview(version, code), 400);
    };
    input.addEventListener('input', update);
    if (input.value.trim()) update();
})();

(() => {
    const chat = document.querySelector('[data-support-chat]');
    if (!chat) return;
    const log = chat.querySelector('[data-chat-log]');
    const form = chat.querySelector('form');
    const input = form.querySelector('input');
    const normalize = value => value.normalize('NFD').replace(/[\u0300-\u036f]/g, '').toLowerCase();
    const reply = value => {
        const text = normalize(value);
        if (/frete|cep|prazo/.test(text)) return 'Informe o CEP no carrinho para consultar as opções, valores e prazos. A gratuidade depende da regra de entrega e do valor dos produtos após cupons. No checkout, escolha a entrega antes de confirmar. O prazo estimado conta em dias úteis após a postagem.';
        if (/cupom|cupon|desconto|promoc/.test(text)) return 'Digite o cupom no checkout para ver o desconto e o novo total. Confira a compra mínima, a validade e o limite de usos. Cada cliente pode usar o mesmo cupom uma vez.';
        if (/cancel|reembolso|troca|devol/.test(text)) return 'Em Minhas compras, abra o pedido. Você pode cancelar enquanto estiver pendente ou confirmado. Após a entrega, a opção de solicitar reembolso fica disponível. Para outros casos, envie uma mensagem no atendimento do pedido.';
        if (/pix|boleto|cartao|pagamento|pagar/.test(text)) return 'Escolha Pix, boleto, crédito ou débito no checkout. Os pagamentos deste projeto são demonstrativos. Para Pix e boleto, abra o pedido e use a confirmação de pagamento da simulação.';
        if (/entrega|rastre|pedido|compra|atras/.test(text)) return 'Abra Minhas compras e selecione o pedido para consultar o status e o rastreio. Se precisar de uma resposta da loja, use Atendimento Beta Fit dentro desse pedido.';
        if (/tamanho|medida|cor|produto/.test(text)) return 'Abra o produto, selecione a cor e consulte a tabela de medidas antes de escolher o tamanho. As fotos acompanham a cor selecionada.';
        if (/humano|atendente|loja|pessoa/.test(text)) return 'Para falar com a equipe, abra Minhas compras, escolha o pedido e envie uma mensagem em Atendimento Beta Fit. Este chat fornece orientações automáticas e não encaminha mensagens à equipe.';
        if (/obrigad|valeu|resolvi/.test(text)) return 'Por nada! Se precisar, escolha outro assunto abaixo.';
        if (/^(oi|ola|bom dia|boa tarde|boa noite)\b/.test(text)) return 'Olá! Sou o assistente virtual BetaFit. Posso orientar sobre pedidos, pagamentos, cupons, tamanhos e reembolsos. Qual é a sua dúvida?';
        return 'Posso ajudar com pedidos e entrega, pagamentos, cupons, tamanhos ou reembolsos. Escolha um assunto abaixo. Para tratar um pedido específico com a equipe, use o atendimento em Minhas compras.';
    };
    const message = (text, user = false) => {
        const item = document.createElement('p');
        item.className = user ? 'bf-chat-message is-user' : 'bf-chat-message';
        const label = document.createElement('strong');
        label.textContent = user ? 'Você' : 'Assistente virtual';
        item.append(label, document.createTextNode(text));
        log.append(item);
        while (log.children.length > 60) log.firstElementChild.remove();
        log.scrollTop = log.scrollHeight;
    };
    const send = text => {
        const value = text.trim().slice(0, 500);
        if (!value) return;
        message(value, true);
        message(reply(value));
        input.value = '';
        input.focus({ preventScroll: true });
    };
    form.addEventListener('submit', event => { event.preventDefault(); send(input.value); });
    chat.querySelectorAll('[data-chat-topic]').forEach(button => button.addEventListener('click', () => send(button.textContent)));
    document.querySelectorAll('[data-chat-open]').forEach(button => button.addEventListener('click', () => {
        chat.scrollIntoView({ behavior: 'smooth', block: 'start' }); input.focus({ preventScroll: true });
    }));
})();

document.querySelector('[data-report-print]')?.addEventListener('click', () => window.print());
