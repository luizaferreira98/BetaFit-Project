(() => {
    const root = document.querySelector('[data-shipping-calculator]');
    if (!root) return;
    const checkout = root.dataset.checkout === 'true';
    const form = checkout ? root.closest('form') : root.querySelector('form');
    const cep = checkout ? form.querySelector('[name="Cep"]') : root.querySelector('[data-shipping-cep]');
    const coupon = document.querySelector('[data-coupon-input]');
    const status = root.querySelector('[data-shipping-status]');
    const options = root.querySelector('[data-shipping-options]');
    const submit = document.querySelector('[data-checkout-submit]');
    const expected = root.querySelector('[data-expected-total]');
    const total = document.querySelector('[data-checkout-total]');
    const cost = document.querySelector('[data-shipping-cost]');
    const totalLabel = document.querySelector('[data-total-label]');
    const feedback = document.querySelector('[data-coupon-feedback]');
    const discountRow = document.querySelector('[data-coupon-row]');
    const discountText = document.querySelector('[data-coupon-discount]');
    const money = value => Number(value).toLocaleString('pt-BR', { style: 'currency', currency: 'BRL' });
    let subtotal = Number(root.dataset.subtotal), current = null, revision = 0, controller, timer;
    let selectedId = options.querySelector('input:checked')?.value;
    try { selectedId = sessionStorage.getItem('betafit-shipping-rule') || selectedId; } catch {}

    const updateTotal = option => {
        const amount = Math.round((subtotal - (current?.discount || 0) + (option?.cost || 0)) * 100) / 100;
        total.textContent = money(amount);
        totalLabel.textContent = option ? 'Total' : 'Total sem frete';
        cost.textContent = option ? (option.cost === 0 ? 'Grátis' : money(option.cost)) : 'A calcular';
        if (expected) expected.value = option && !current?.couponError ? amount.toFixed(2) : '';
        if (submit) submit.disabled = !option || Boolean(current?.couponError);
        document.querySelectorAll('select[name="Installments"] option').forEach(item => {
            item.textContent = `${item.value} x de ${money(amount / Number(item.value))} sem juros`;
        });
    };
    const select = option => {
        selectedId = String(option.ruleId);
        try { sessionStorage.setItem('betafit-shipping-rule', selectedId); } catch {}
        updateTotal(option);
    };
    const draw = quote => {
        current = quote; subtotal = Number(quote.subtotal);
        document.querySelector('[data-checkout-subtotal]')?.replaceChildren(document.createTextNode(money(subtotal)));
        if (discountRow) discountRow.hidden = !quote.discount;
        if (discountText) discountText.textContent = `− ${money(quote.discount || 0)}`;
        if (feedback) feedback.textContent = quote.couponError || (quote.couponCode ? `${quote.couponCode}: desconto de ${money(quote.discount)} nos produtos.` : 'Digite o cupom para consultar o desconto. Um uso por cliente.');
        status.textContent = quote.shippingError || 'Escolha a entrega. Prazo estimado em dias úteis após postagem.';
        options.replaceChildren();
        const selected = quote.options.find(o => String(o.ruleId) === selectedId) || quote.options[0];
        quote.options.forEach(option => {
            const label = document.createElement('label'); label.className = 'bf-shipping-option';
            const radio = document.createElement('input'); radio.type = 'radio'; radio.name = 'ShippingRuleId'; radio.value = option.ruleId;
            radio.required = checkout; radio.checked = option === selected;
            const body = document.createElement('span'), title = document.createElement('strong'), days = document.createElement('small');
            title.textContent = `${option.name} · ${option.cost === 0 ? 'Grátis' : money(option.cost)}`;
            days.textContent = `${option.minDays} a ${option.maxDays} dias úteis após postagem`;
            body.append(title, days);
            if (option.remainingForFree > 0 && option.regularCost > 0) {
                const free = document.createElement('small');
                free.textContent = `Faltam ${money(option.remainingForFree)} em produtos após cupons para esta entrega ficar grátis.`;
                body.append(free);
            }
            label.append(radio, body); options.append(label);
            radio.addEventListener('change', () => select(option));
        });
        if (selected) select(selected); else updateTotal(null);
    };
    const calculate = async version => {
        controller = new AbortController();
        const body = new URLSearchParams({ cep: root.dataset.savedCep || cep?.value || '', couponCode: coupon?.value || '',
            useSavedAddress: String(checkout && Boolean(root.dataset.savedCep)),
            __RequestVerificationToken: form.querySelector('[name="__RequestVerificationToken"]').value });
        try {
            const response = await fetch(root.dataset.quoteUrl, { method: 'POST', body, signal: controller.signal, headers: { Accept: 'application/json' } });
            if (response.redirected || response.status === 401) throw new Error('Sua sessão expirou. Entre novamente para continuar.');
            const quote = await response.json();
            if (version !== revision) return;
            if (!response.ok) throw new Error(quote.message || 'Não foi possível calcular o frete. Confira o CEP e tente novamente.');
            draw(quote);
        } catch (error) {
            if (error.name === 'AbortError' || version !== revision) return;
            status.textContent = error instanceof TypeError || error instanceof SyntaxError ? 'Não foi possível consultar a entrega. Tente novamente.' : error.message;
            if (feedback && coupon?.value.trim()) feedback.textContent = 'Aguardando uma nova consulta para validar o cupom.';
        }
    };
    const schedule = (immediate = false) => {
        clearTimeout(timer); controller?.abort(); const version = ++revision;
        current = null; options.replaceChildren(); if (discountRow) discountRow.hidden = true;
        if (feedback) feedback.textContent = coupon?.value.trim() ? 'Consultando desconto…' : 'Digite o cupom para consultar o desconto.';
        updateTotal(null); status.textContent = 'Calculando entrega…';
        if (immediate) calculate(version); else timer = setTimeout(() => calculate(version), 450);
    };
    cep?.addEventListener('input', () => schedule());
    coupon?.addEventListener('input', () => schedule());
    if (checkout) root.querySelector('[data-shipping-calculate]').addEventListener('click', () => schedule(true));
    else form.addEventListener('submit', event => { event.preventDefault(); schedule(true); });
    if (root.dataset.savedCep || cep?.value || coupon?.value) schedule(true);
})();
