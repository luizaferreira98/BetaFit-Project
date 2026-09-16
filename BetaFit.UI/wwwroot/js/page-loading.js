/* Real navigation feedback. No artificial delay on completed pages. */
(() => {
    const root = document.documentElement;
    root.classList.add('bf-document-loading');
    let timer, recovery;
    const clear = () => {
        clearTimeout(timer); clearTimeout(recovery);
        root.classList.remove('bf-document-loading', 'bf-navigating');
        const loader = document.getElementById('bf-page-loading');
        if (loader) loader.hidden = true;
        document.querySelector('main')?.removeAttribute('aria-busy');
    };
    const start = () => {
        clearTimeout(timer);
        {
            const loader = document.getElementById('bf-page-loading');
            if (!loader) return;
            loader.hidden = false;
            root.classList.add('bf-navigating');
            document.querySelector('main')?.setAttribute('aria-busy', 'true');
            recovery = setTimeout(clear, 12000);
        }
    };
    document.addEventListener('DOMContentLoaded', clear, { once: true });
    window.addEventListener('pageshow', clear);
    document.addEventListener('keydown', event => { if (event.key === 'Escape') clear(); });
    document.addEventListener('click', event => {
        const link = event.target.closest('a[href]');
        if (!link || event.defaultPrevented || event.button !== 0 || event.ctrlKey || event.metaKey || event.shiftKey || event.altKey || link.hasAttribute('download') || link.target && link.target !== '_self') return;
        const url = new URL(link.href, location.href);
        if (url.origin !== location.origin || url.hash || /format=|export|download|\.(pdf|xlsx|zip|png|jpg|webp)$/i.test(url.href) || url.href === location.href) return;
        start();
    });
    document.addEventListener('submit', event => {
        const form = event.target;
        if (event.defaultPrevented || form.target || form.method.toLowerCase() !== 'get' || form.closest('[data-support-chat]') || form.querySelector('[name="format"]')) return;
        start();
    });
    recovery = setTimeout(clear, 12000);
})();
