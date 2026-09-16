(() => {
    'use strict';
    const menu = document.querySelector('[data-account-menu]');
    const toggle = menu?.querySelector('[data-account-toggle]');
    const panel = menu?.querySelector('[data-account-panel]');
    if (toggle && panel) {
        const setOpen = open => {
            toggle.setAttribute('aria-expanded', String(open));
            panel.hidden = !open;
            menu.classList.toggle('is-open', open);
        };
        setOpen(false);
        toggle.addEventListener('click', () => setOpen(panel.hidden));
        toggle.addEventListener('keydown', event => {
            if (event.key === 'ArrowDown') { event.preventDefault(); setOpen(true); panel.querySelector('a')?.focus(); }
        });
        document.addEventListener('click', event => { if (!menu.contains(event.target)) setOpen(false); });
        document.addEventListener('keydown', event => { if (event.key === 'Escape' && !panel.hidden) { setOpen(false); toggle.focus(); } });
        menu.addEventListener('focusout', event => { if (!menu.contains(event.relatedTarget)) setOpen(false); });
    }
    const paths = {
        user: '<circle cx="12" cy="8" r="4"/><path d="M4 21v-2a8 8 0 0 1 16 0v2"/>',
        heart: '<path d="M20 5c-3-3-6-1-8 1-2-2-5-4-8-1-4 4 2 9 8 14 6-5 12-10 8-14Z"/>',
        box: '<path d="m3 7 9-5 9 5v10l-9 5-9-5V7Zm0 0 9 5 9-5M12 12v10M8 4l9 5"/>',
        grid: '<rect x="3" y="3" width="7" height="7" rx="1"/><rect x="14" y="3" width="7" height="7" rx="1"/><rect x="3" y="14" width="7" height="7" rx="1"/><rect x="14" y="14" width="7" height="7" rx="1"/>',
        list: '<path d="M9 5h12M9 12h12M9 19h12M3 5h1M3 12h1M3 19h1"/>',
        truck: '<path d="M3 5h11v12H3V5Zm11 5h4l3 4v3h-7"/><circle cx="7" cy="18" r="2"/><circle cx="18" cy="18" r="2"/>',
        chart: '<path d="M4 3v18h17M8 16v-5M13 16V7M18 16v-3"/>',
        star: '<path d="m12 3 3 6 7 1-5 5 1 7-6-3-6 3 1-7-5-5 7-1 3-6Z"/>',
        logout: '<path d="M9 4H4v16h5M9 12h12m-5-5 5 5-5 5"/>',
        arrow: '<path d="M5 19 19 5M5 5h14v14"/>',
        coupon: '<path d="M3 7h18v4a2 2 0 0 0 0 4v4H3v-4a2 2 0 0 0 0-4V7Zm6 9 6-6M9 10h.01M15 16h.01"/>',
        chat: '<path d="M21 11a9 9 0 0 1-9 9H4l-2 2V11a9 9 0 0 1 19 0ZM7 10h10M7 14h6"/>',
        send: '<path d="m22 2-7 20-4-9-9-4 20-7ZM11 13 22 2"/>',
        card: '<rect x="2" y="4" width="20" height="16" rx="3"/><path d="M2 9h20M6 15h4"/>',
        leaf: '<path d="M20 3C6 2 1 8 5 16s17 4 15-13ZM4 21 16 8"/>',
        download: '<path d="M12 3v12m-5-5 5 5 5-5M4 16v5h16v-5"/>',
        print: '<path d="M6 8V3h12v5M6 17H3V8h18v9h-3M6 14h12v7H6v-7Z"/>',
    };
    const icon = name => {
        const svg = document.createElementNS('http://www.w3.org/2000/svg', 'svg');
        svg.setAttribute('viewBox','0 0 24 24'); svg.setAttribute('aria-hidden','true');
        svg.classList.add('bf-icon'); svg.innerHTML = paths[name] || paths.grid; return svg;
    };
    document.querySelectorAll('[data-icon]').forEach(el => el.replaceChildren(icon(el.dataset.icon)));
    const replace = (selector, names) => document.querySelectorAll(selector).forEach((el,i) => el.replaceChildren(icon(names[i % names.length])));
    replace('.bf-account-menu__icon', ['user','heart','box','logout']);
    replace('.bf-nav-icon', ['grid']);
    replace('.bf-product-information > button > span:first-child', ['card','star','leaf','arrow']);
    replace('.bf-help-tab > span', ['chat']);
    document.querySelectorAll('.bf-admin-sidebar nav a').forEach(el => {
        const label=el.textContent;
        const name=/estoque/.test(label)?'box':/Categorias/.test(label)?'list':/Pedidos/.test(label)?'truck':/Usuários/.test(label)?'user':/Moderação/.test(label)?'star':/Fretes/.test(label)?'truck':/Relatórios/.test(label)?'chart':/Cupons/.test(label)?'coupon':'arrow';
        const node=[...el.childNodes].find(n=>n.nodeType===Node.TEXT_NODE && n.textContent.trim());
        if(node) node.textContent=node.textContent.replace(/^\s*[^\p{L}]+\s*/u,'');
        el.prepend(icon(name));
    });
    document.querySelectorAll('[data-report-print]').forEach(el=>el.prepend(icon('print')));
    document.querySelectorAll('a[href*="format=excel"]').forEach(el=>{el.setAttribute('download','');el.prepend(icon('download'));});
    document.querySelectorAll('.bf-product-card__media img, .bf-product-gallery__stage img').forEach(img=>{
        const finish=()=>img.classList.remove('bf-image-pending');
        if(!img.complete)img.classList.add('bf-image-pending');
        img.addEventListener('load',finish);img.addEventListener('error',finish);
    });
})();
