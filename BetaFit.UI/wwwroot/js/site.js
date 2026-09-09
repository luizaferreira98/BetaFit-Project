

(() => {
    "use strict";

    const root = document.documentElement;
    const body = document.body;

    // ---------------------------------------------------------------------
    // Tema claro/escuro persistente
    // ---------------------------------------------------------------------
    const themeToggle = document.querySelector("[data-theme-toggle]");
    const themeLabel = document.querySelector("[data-theme-label]");
    const applyTheme = (theme) => {
        document.documentElement.dataset.theme = theme;
        try { localStorage.setItem("betafit-theme", theme); } catch { }
        if (themeToggle) themeToggle.setAttribute("aria-pressed", theme === "dark");
        if (themeLabel) themeLabel.textContent = theme === "dark" ? "Claro" : "Escuro";
    };
    const savedTheme = (() => { try { return localStorage.getItem("betafit-theme"); } catch { return null; } })();
    const initialTheme = savedTheme || (window.matchMedia && window.matchMedia("(prefers-color-scheme: dark)").matches ? "dark" : "light");
    applyTheme(initialTheme);
    themeToggle?.addEventListener("click", () => applyTheme(document.documentElement.dataset.theme === "dark" ? "light" : "dark"));

    // ---------------------------------------------------------------------
    // Loading e feedback de operações
    // ---------------------------------------------------------------------
    const pageLoader = document.querySelector("[data-page-loader]");
    const showLoader = () => pageLoader?.classList.add("is-visible");
    document.querySelectorAll("form[data-busy-form]").forEach((form) => {
        form.addEventListener("submit", (event) => {
            if (!form.checkValidity()) return;
            form.classList.add("is-busy");
            const button = form.querySelector("button[type=submit]");
            if (button) {
                button.dataset.originalText ??= button.textContent;
                button.textContent = button.dataset.busyLabel || "Processando...";
                button.disabled = true;
            }
            showLoader();
        });
    });

    document.querySelectorAll(".bf-alert").forEach((alert) => {
        window.setTimeout(() => {
            alert.animate([{ opacity: 1, transform: "translateY(0)" }, { opacity: 0, transform: "translateY(-6px)" }], { duration: 220, fill: "forwards" });
            window.setTimeout(() => alert.remove(), 240);
        }, 5200);
    });

    const toastRegion = document.querySelector("[data-toast-region]");
    window.betaFitToast = (message, type = "success") => {
        if (!toastRegion || !message) return;
        const toast = document.createElement("div");
        toast.className = `bf-toast bf-toast--${type}`;
        toast.textContent = message;
        toastRegion.appendChild(toast);
        window.setTimeout(() => toast.remove(), 4200);
    };


    root.classList.add("bf-js");

    const prefersReducedMotion = window.matchMedia("(prefers-reduced-motion: reduce)").matches;

    // ---------------------------------------------------------------------
    // Global page motion layer
    // ---------------------------------------------------------------------
    const transition = document.createElement("div");
    transition.className = "bf-page-transition";
    transition.setAttribute("aria-hidden", "true");
    transition.innerHTML = '<span class="bf-page-transition__mark">BF</span>';
    body.appendChild(transition);

    requestAnimationFrame(() => body.classList.add("bf-page-ready"));

    window.addEventListener("pageshow", () => {
        transition.classList.remove("is-leaving");
        body.classList.add("bf-page-ready");
    });

    // Smooth visual transition between Razor pages. Forms and external links
    // are intentionally untouched so existing application behavior remains.
    if (!prefersReducedMotion) {
        document.addEventListener("click", (event) => {
            const link = event.target.closest("a[href]");
            if (!link) return;
            if (event.defaultPrevented) return;
            if (link.target && link.target !== "_self") return;
            if (link.hasAttribute("download")) return;

            const rawHref = link.getAttribute("href");
            if (!rawHref || rawHref.startsWith("#") || rawHref.startsWith("javascript:")) return;

            let url;
            try {
                url = new URL(rawHref, window.location.href);
            } catch {
                return;
            }

            if (url.origin !== window.location.origin) return;
            if (url.pathname === window.location.pathname && url.search === window.location.search) return;

            event.preventDefault();
            transition.classList.add("is-leaving");
            body.classList.add("bf-page-leaving");

            window.setTimeout(() => {
                window.location.href = url.href;
            }, 180);
        });
    }

    // ---------------------------------------------------------------------
    // Scroll reveal with staggered motion
    // ---------------------------------------------------------------------
    const revealSelectors = [
        ".bf-hero__copy > *",
        ".bf-hero__visual",
        ".bf-benefit-strip__grid > div",
        ".bf-section__head",
        ".bf-product-card",
        ".bf-category-tile",
        ".bf-story__intro",
        ".bf-story__content",
        ".bf-story__facts span",
        ".bf-cta__inner > *",
        ".bf-page-hero__inner > *",
        ".bf-filter-bar",
        ".bf-product-detail__media",
        ".bf-product-detail__content > *",
        ".bf-form-page",
        ".bf-simple-page__card",
        ".bf-confirm-card",
        ".bf-footer__main > *"
    ];

    const revealElements = document.querySelectorAll(revealSelectors.join(","));

    revealElements.forEach((element, index) => {
        element.classList.add("bf-reveal");
        element.style.setProperty("--reveal-delay", `${Math.min(index % 8, 7) * 55}ms`);
    });

    if (prefersReducedMotion || !("IntersectionObserver" in window)) {
        revealElements.forEach((element) => element.classList.add("is-visible"));
    } else {
        const revealObserver = new IntersectionObserver((entries, observer) => {
            entries.forEach((entry) => {
                if (!entry.isIntersecting) return;
                entry.target.classList.add("is-visible");
                observer.unobserve(entry.target);
            });
        }, {
            threshold: 0.12,
            rootMargin: "0px 0px -45px 0px"
        });

        revealElements.forEach((element) => revealObserver.observe(element));
    }

    // ---------------------------------------------------------------------
    // Image entrance animation
    // ---------------------------------------------------------------------
    document.querySelectorAll("img").forEach((image) => {
        image.classList.add("bf-image-reveal");

        const show = () => image.classList.add("is-loaded");
        if (image.complete) {
            window.requestAnimationFrame(show);
        } else {
            image.addEventListener("load", show, { once: true });
            image.addEventListener("error", show, { once: true });
        }
    });

    // ---------------------------------------------------------------------
    // Premium product-card tilt (desktop/pointer devices only)
    // ---------------------------------------------------------------------
    if (!prefersReducedMotion && window.matchMedia("(hover: hover) and (pointer: fine)").matches) {
        document.querySelectorAll(".bf-product-card, .bf-category-tile, .bf-confirm-card").forEach((card) => {
            let frame = null;

            const reset = () => {
                if (frame) cancelAnimationFrame(frame);
                card.style.transform = "";
                card.style.setProperty("--mx", "50%");
                card.style.setProperty("--my", "50%");
            };

            card.addEventListener("pointermove", (event) => {
                const rect = card.getBoundingClientRect();
                const x = (event.clientX - rect.left) / rect.width;
                const y = (event.clientY - rect.top) / rect.height;
                const rotateY = (x - 0.5) * 5.5;
                const rotateX = (0.5 - y) * 5.5;

                card.style.setProperty("--mx", `${x * 100}%`);
                card.style.setProperty("--my", `${y * 100}%`);

                if (frame) cancelAnimationFrame(frame);
                frame = requestAnimationFrame(() => {
                    card.style.transform = `perspective(900px) rotateX(${rotateX}deg) rotateY(${rotateY}deg) translateY(-5px)`;
                });
            });

            card.addEventListener("pointerleave", reset);
        });
    }

    // ---------------------------------------------------------------------
    // Magnetic CTA buttons
    // ---------------------------------------------------------------------
    if (!prefersReducedMotion && window.matchMedia("(hover: hover) and (pointer: fine)").matches) {
        document.querySelectorAll(".bf-btn, .bf-section-link, .bf-account-link").forEach((button) => {
            button.addEventListener("pointermove", (event) => {
                const rect = button.getBoundingClientRect();
                const x = (event.clientX - rect.left - rect.width / 2) * 0.08;
                const y = (event.clientY - rect.top - rect.height / 2) * 0.08;
                button.style.setProperty("--mag-x", `${x}px`);
                button.style.setProperty("--mag-y", `${y}px`);
            });

            button.addEventListener("pointerleave", () => {
                button.style.setProperty("--mag-x", "0px");
                button.style.setProperty("--mag-y", "0px");
            });
        });
    }

    // ---------------------------------------------------------------------
    // Button ripple feedback
    // ---------------------------------------------------------------------
    document.addEventListener("pointerdown", (event) => {
        const button = event.target.closest(".bf-btn, .bf-icon-button");
        if (!button || prefersReducedMotion) return;

        const rect = button.getBoundingClientRect();
        const ripple = document.createElement("span");
        ripple.className = "bf-ripple";
        ripple.style.left = `${event.clientX - rect.left}px`;
        ripple.style.top = `${event.clientY - rect.top}px`;
        button.appendChild(ripple);

        ripple.addEventListener("animationend", () => ripple.remove(), { once: true });
    });

    // ---------------------------------------------------------------------
    // Hero depth/parallax
    // ---------------------------------------------------------------------
    const hero = document.querySelector(".bf-hero");
    const heroVisual = document.querySelector(".bf-hero__visual");
    const heroImage = document.querySelector(".bf-hero__image");

    if (hero && heroVisual && !prefersReducedMotion && window.matchMedia("(hover: hover) and (pointer: fine)").matches) {
        hero.addEventListener("pointermove", (event) => {
            const rect = hero.getBoundingClientRect();
            const x = (event.clientX - rect.left) / rect.width - 0.5;
            const y = (event.clientY - rect.top) / rect.height - 0.5;

            heroVisual.style.setProperty("--hero-x", `${x * 12}px`);
            heroVisual.style.setProperty("--hero-y", `${y * 8}px`);
            if (heroImage) {
                heroImage.style.setProperty("--hero-image-x", `${x * -18}px`);
                heroImage.style.setProperty("--hero-image-y", `${y * -12}px`);
            }
        });

        hero.addEventListener("pointerleave", () => {
            heroVisual.style.setProperty("--hero-x", "0px");
            heroVisual.style.setProperty("--hero-y", "0px");
            if (heroImage) {
                heroImage.style.setProperty("--hero-image-x", "0px");
                heroImage.style.setProperty("--hero-image-y", "0px");
            }
        });
    }

    // ---------------------------------------------------------------------
    // Scroll-aware header + progress indicator
    // ---------------------------------------------------------------------
    const header = document.querySelector(".bf-header");
    const progress = document.createElement("div");
    progress.className = "bf-scroll-progress";
    progress.setAttribute("aria-hidden", "true");
    body.appendChild(progress);

    let ticking = false;
    const updateScrollUI = () => {
        const scrollTop = window.scrollY || document.documentElement.scrollTop;
        const maxScroll = document.documentElement.scrollHeight - window.innerHeight;
        const percentage = maxScroll > 0 ? Math.min(100, Math.max(0, (scrollTop / maxScroll) * 100)) : 0;

        progress.style.setProperty("--scroll-progress", `${percentage}%`);
        if (header) header.classList.toggle("is-scrolled", scrollTop > 18);
        ticking = false;
    };

    window.addEventListener("scroll", () => {
        if (!ticking) {
            window.requestAnimationFrame(updateScrollUI);
            ticking = true;
        }
    }, { passive: true });
    updateScrollUI();

    // ---------------------------------------------------------------------
    // Desktop cursor glow. Purely visual and disabled on touch/reduced motion.
    // ---------------------------------------------------------------------
    if (!prefersReducedMotion && window.matchMedia("(hover: hover) and (pointer: fine)").matches) {
        const cursorGlow = document.createElement("div");
        cursorGlow.className = "bf-cursor-glow";
        cursorGlow.setAttribute("aria-hidden", "true");
        body.appendChild(cursorGlow);

        let glowX = -100;
        let glowY = -100;
        let targetX = -100;
        let targetY = -100;

        document.addEventListener("pointermove", (event) => {
            targetX = event.clientX;
            targetY = event.clientY;
        }, { passive: true });

        const animateGlow = () => {
            glowX += (targetX - glowX) * 0.12;
            glowY += (targetY - glowY) * 0.12;
            cursorGlow.style.transform = `translate3d(${glowX}px, ${glowY}px, 0)`;
            requestAnimationFrame(animateGlow);
        };
        animateGlow();
    }

    // ---------------------------------------------------------------------
    // Smart product size UI
    // ---------------------------------------------------------------------
    const updateSizePicker = () => {
        document.querySelectorAll("[data-size-picker]").forEach((picker) => {
            const select = document.getElementById("product-category");
            if (!select) return;
            const option = select.options[select.selectedIndex];
            const requires = option?.dataset.requiresSize === "true";
            const category = (option?.textContent || "").toLowerCase();
            const shoe = /tênis|tenis|calçado|calcado|calçados/.test(category);
            picker.classList.toggle("is-not-required", !requires);
            picker.querySelectorAll('input[name="AvailableSizes"]').forEach((input) => {
                const value = input.value;
                const numeric = /^\d+$/.test(value);
                const show = requires && (shoe ? numeric : !numeric);
                input.disabled = !show;
                input.closest(".bf-size-option")?.classList.toggle("is-hidden", !show);
                if (!show) input.checked = false;
            });
            const status = document.getElementById("size-status");
            const helper = document.getElementById("size-helper");
            if (status) status.textContent = !requires ? "Não se aplica" : shoe ? "Numeração do calçado" : "Tamanho da peça";
            if (helper) helper.textContent = !requires ? "A seleção de tamanho não será exibida para este tipo de produto." : shoe ? "Selecione a numeração disponível (ex.: 37, 38, 39, 40)." : "Selecione os tamanhos que o cliente poderá escolher.";
        });
    };
    document.getElementById("product-category")?.addEventListener("change", updateSizePicker);
    updateSizePicker();

    // ---------------------------------------------------------------------
    // Premium product image upload: drag/drop, ordering and remove
    // ---------------------------------------------------------------------
    document.querySelectorAll("[data-image-preview-input]").forEach((input) => {
        const previewId = input.getAttribute("data-image-preview-input");
        const preview = previewId ? document.getElementById(previewId) : null;
        if (!preview || !(input instanceof HTMLInputElement)) return;

        const grid = preview.querySelector(".bf-image-upload-preview__grid");
        const zone = input.closest("[data-upload-zone]");
        const count = zone?.querySelector("[data-upload-count]");
        const allowedExtensions = [".jpg", ".jpeg", ".png", ".webp"];
        const isAllowedImage = (file) => {
            const extension = `.${(file.name || "").split(".").pop().toLowerCase()}`;
            return ["image/jpeg", "image/png", "image/webp", "image/jpg"].includes(file.type) || allowedExtensions.includes(extension);
        };
        let files = [];

        const syncInput = () => {
            const dt = new DataTransfer();
            files.forEach(file => dt.items.add(file));
            input.files = dt.files;
        };

        const render = () => {
            if (!grid) return;
            grid.innerHTML = "";
            files.forEach((file, index) => {
                const url = URL.createObjectURL(file);
                const item = document.createElement("div");
                item.className = "bf-image-preview-thumb";
                item.draggable = true;
                item.dataset.index = String(index);
                item.innerHTML = `
                    <div class="bf-image-preview-thumb__image">
                        <img src="${url}" alt="Pré-visualização de ${file.name}">
                        <button type="button" class="bf-image-preview-remove" aria-label="Remover ${file.name}">×</button>
                    </div>
                    <div class="bf-image-preview-thumb__meta">
                        <strong>${index === 0 ? "Principal" : `Foto ${index + 1}`}</strong>
                        <span>${file.name}</span>
                    </div>`;
                grid.appendChild(item);
                item.querySelector(".bf-image-preview-remove")?.addEventListener("click", (event) => {
                    event.preventDefault();
                    files.splice(index, 1);
                    syncInput();
                    render();
                });
                item.addEventListener("dragstart", () => item.classList.add("is-dragging"));
                item.addEventListener("dragend", () => {
                    item.classList.remove("is-dragging");
                    const from = Number(item.dataset.index);
                    const target = [...grid.children].findIndex(el => el.getBoundingClientRect().left > item.getBoundingClientRect().left);
                    if (target >= 0 && target !== from) {
                        const moved = files.splice(from, 1)[0];
                        files.splice(target, 0, moved);
                        syncInput();
                        render();
                    }
                });
            });
            preview.classList.toggle("is-ready", files.length > 0);
            const placeholder = preview.querySelector(".bf-image-upload-preview__placeholder");
            if (placeholder) placeholder.hidden = files.length > 0;
            if (count) count.textContent = `${files.length} ${files.length === 1 ? "foto" : "fotos"}`;
        };

        const acceptFiles = (incoming) => {
            const valid = Array.from(incoming).filter(file => isAllowedImage(file) && file.size <= 5 * 1024 * 1024);
            files = [...files, ...valid];
            syncInput();
            render();
        };

        input.addEventListener("change", () => acceptFiles(input.files || []));
        zone?.addEventListener("dragover", event => { event.preventDefault(); zone.classList.add("is-dragover"); });
        zone?.addEventListener("dragleave", () => zone.classList.remove("is-dragover"));
        zone?.addEventListener("drop", event => {
            event.preventDefault();
            zone.classList.remove("is-dragover");
            acceptFiles(event.dataTransfer.files || []);
        });
    });

    // ---------------------------------------------------------------------
    // Keep the logout confirmation from being replaced by the page transition.
    // ---------------------------------------------------------------------
    document.querySelectorAll(".bf-logout-form").forEach((form) => {
        form.addEventListener("submit", () => {
            body.classList.add("bf-page-leaving");
        });
    });
})();


// Quantity controls work on both product and cart forms.
document.addEventListener("click", (event) => {
    const button = event.target.closest("[data-quantity-minus],[data-quantity-plus]"); if (!button) return;
    const control = button.closest("[data-quantity-control]"); const input = control?.querySelector(".bf-quantity-input"); if (!input) return;
    const min = Number(input.min || 1), max = Number(input.max || 99), current = Number(input.value || min);
    input.value = String(Math.min(max, Math.max(min, current + (button.hasAttribute("data-quantity-plus") ? 1 : -1))));
    const form = control.closest("form"); if (form && control.dataset.autoSubmit === "true") window.setTimeout(() => form.requestSubmit(), 80);
});
document.addEventListener("input", (event) => {
    const input = event.target.closest(".bf-quantity-input"); if (!input) return; const min = Number(input.min || 1), max = Number(input.max || 99); let v = Number(input.value); if (Number.isFinite(v)) input.value = String(Math.min(max, Math.max(min, Math.floor(v))));
});
// Brazilian phone mask with a hard 11-digit limit.
document.querySelectorAll("[data-phone-mask]").forEach(input => input.addEventListener("input", () => { let d = input.value.replace(/\D/g, "").slice(0, 11); if (d.length <= 10) input.value = d.replace(/(\d{2})(\d{4})(\d{0,4})/, "($1) $2-$3").replace(/-$/, ''); else input.value = d.replace(/(\d{2})(\d{5})(\d{0,4})/, "($1) $2-$3").replace(/-$/, ''); }));
// Product gallery.
document.querySelectorAll("[data-gallery-image]").forEach(button => button.addEventListener("click", () => { const main = document.getElementById("bf-main-product-image"); if (main) main.src = button.dataset.galleryImage || ""; }));


// Product-card image carousel: desktop hover + touch swipe on mobile.
(() => {
    const reduced = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
    document.querySelectorAll("[data-product-carousel]").forEach((media) => {
        const slides = [...media.querySelectorAll(".bf-product-card__slides > img")];
        const dots = [...media.querySelectorAll(".bf-product-card__carousel-dots > i")];
        if (slides.length <= 1) return;
        let index = 0;
        let timer = null;
        let startX = 0;

        const show = (next) => {
            index = (next + slides.length) % slides.length;
            slides.forEach((img, i) => img.classList.toggle("is-active", i === index));
            dots.forEach((dot, i) => dot.classList.toggle("is-active", i === index));
        };
        show(0);
        media.classList.add("is-carousel-ready");

        const start = () => {
            if (reduced || timer) return;
            timer = window.setInterval(() => show(index + 1), 900);
        };
        const stop = () => {
            if (timer) window.clearInterval(timer);
            timer = null;
            show(0);
        };

        media.addEventListener("mouseenter", start);
        media.addEventListener("mouseleave", stop);
        media.addEventListener("touchstart", e => { startX = e.changedTouches[0].clientX; }, { passive: true });
        media.addEventListener("touchend", e => {
            const delta = e.changedTouches[0].clientX - startX;
            if (Math.abs(delta) < 35) return;
            show(index + (delta < 0 ? 1 : -1));
        }, { passive: true });
    });
})();

// Password visibility + live registration validation.
(() => {
    const form = document.getElementById("register-form");
    if (!form) return;

    const password = document.getElementById("Password");
    const confirm = document.getElementById("ConfirmPassword");
    const terms = document.getElementById("terms");
    const submit = document.getElementById("register-submit");
    const bar = document.getElementById("password-meter-bar");
    const feedback = document.getElementById("confirm-password-feedback");

    const update = () => {
        if (!password || !confirm || !submit) return;
        const value = password.value;
        const rules = {
            length: value.length >= 6,
            upper: /[A-Z]/.test(value),
            special: /[^a-zA-Z0-9]/.test(value)
        };
        Object.entries(rules).forEach(([key, ok]) => {
            const el = form.querySelector(`[data-rule="${key}"]`);
            el?.classList.toggle("is-valid", ok);
        });
        const score = Object.values(rules).filter(Boolean).length;
        if (bar) bar.style.width = `${(score / 3) * 100}%`;
        const matching = value.length > 0 && value === confirm.value;
        if (feedback) {
            feedback.textContent = confirm.value ? (matching ? "✓ As senhas coincidem" : "As senhas ainda não coincidem") : "";
            feedback.classList.toggle("is-valid", matching);
        }
        submit.disabled = !(score === 3 && matching && terms?.checked);
    };

    [password, confirm, terms].forEach(el => el?.addEventListener("input", update));
    terms?.addEventListener("change", update);
    update();
})();


// Global password visibility, including Login/Profile/Reset screens.
document.querySelectorAll("[data-password-toggle]").forEach(button => {
    button.addEventListener("click", () => {
        const input = document.querySelector(button.dataset.passwordToggle);
        if (!input) return;
        const visible = input.type === "text";
        input.type = visible ? "password" : "text";
        button.textContent = visible ? "Mostrar" : "Ocultar";
        button.setAttribute("aria-label", visible ? "Mostrar senha" : "Ocultar senha");
    });
});

// Prevent future birth dates on every birth-date input.
document.querySelectorAll('input[type="date"][name="BirthDate"]').forEach(input => {
    input.max = new Date().toISOString().slice(0, 10);
});

// Product image zoom: hover on desktop and click-to-zoom on touch devices.
(() => {
    document.querySelectorAll(".bf-gallery__main").forEach(main => {
        const img = main.querySelector("img"); if (!img) return;
        const reset = () => { img.style.transform = "scale(1)"; img.style.transformOrigin = "50% 50%"; };
        main.addEventListener("pointermove", e => { if (!window.matchMedia("(hover:hover) and (pointer:fine)").matches) return; const r = main.getBoundingClientRect(); const x = (e.clientX - r.left) / r.width * 100; const y = (e.clientY - r.top) / r.height * 100; img.style.transformOrigin = `${x}% ${y}%`; img.style.transform = "scale(1.8)"; });
        main.addEventListener("pointerleave", reset);
        main.addEventListener("click", () => { if (window.matchMedia("(hover:hover) and (pointer:fine)").matches) return; img.classList.toggle("is-zoomed"); img.style.transform = img.classList.contains("is-zoomed") ? "scale(1.7)" : "scale(1)"; });
    });
})();

// Profile password strength + confirmation feedback.
(() => {
    const password = document.getElementById("NewPassword"), confirm = document.getElementById("ConfirmNewPassword"); if (!password || !confirm) return;
    const bar = document.getElementById("profile-password-meter-bar"), feedback = document.getElementById("profile-confirm-feedback");
    const update = () => { const v = password.value; const rules = { length: v.length >= 6, upper: /[A-Z]/.test(v), special: /[^a-zA-Z0-9]/.test(v) }; Object.entries(rules).forEach(([k, ok]) => document.querySelector(`[data-profile-rule="${k}"]`)?.classList.toggle("is-valid", ok)); if (bar) bar.style.width = `${Object.values(rules).filter(Boolean).length / 3 * 100}%`; const match = v.length > 0 && v === confirm.value; if (feedback) { feedback.textContent = confirm.value ? (match ? "✓ As senhas coincidem" : "As senhas ainda não coincidem") : ""; feedback.classList.toggle("is-valid", match); } };
    password.addEventListener("input", update); confirm.addEventListener("input", update); update();
})();


// BetaFit: gallery/carousel persisted by backend URLs.
document.addEventListener('DOMContentLoaded', () => {
    document.querySelectorAll('[data-gallery-image]').forEach(btn => btn.addEventListener('click', () => {
        const main = document.getElementById('bf-main-product-image'); if (main) main.src = btn.dataset.galleryImage;
    }));
    const modal = document.querySelector('[data-gallery-modal]'); const modalImg = modal?.querySelector('[data-gallery-modal-image]');
    const urls = [...document.querySelectorAll('[data-gallery-modal-thumb]')].map(x => x.dataset.galleryModalThumb).filter(Boolean); let idx = 0;
    function open(url) { if (!modal || !modalImg) return; idx = Math.max(0, urls.indexOf(url)); modalImg.src = url; modal.setAttribute('aria-hidden', 'false'); document.body.classList.add('bf-modal-open'); }
    function close() { if (!modal) return; modal.setAttribute('aria-hidden', 'true'); document.body.classList.remove('bf-modal-open'); }
    document.querySelectorAll('[data-gallery-image]').forEach(button => button.addEventListener('click', () => { const url = button.dataset.galleryImage; if (!url) return; const main = document.getElementById('bf-main-product-image'); if (main) main.src = url; open(url); }));
    document.querySelector('[data-gallery-open]')?.addEventListener('click', () => open(document.getElementById('bf-main-product-image')?.src || urls[0]));
    document.querySelectorAll('[data-gallery-modal-thumb]').forEach(b => b.addEventListener('click', () => open(b.dataset.galleryModalThumb)));
    modal?.querySelectorAll('[data-gallery-close]').forEach(b => b.addEventListener('click', close));
    modal?.querySelector('[data-gallery-prev]')?.addEventListener('click', () => { if (urls.length) { idx = (idx - 1 + urls.length) % urls.length; modalImg.src = urls[idx]; } });
    modal?.querySelector('[data-gallery-next]')?.addEventListener('click', () => { if (urls.length) { idx = (idx + 1) % urls.length; modalImg.src = urls[idx]; } });
    document.addEventListener('keydown', e => { if (!modal || modal.getAttribute('aria-hidden') === 'true') return; if (e.key === 'Escape') close(); if (e.key === 'ArrowLeft') modal.querySelector('[data-gallery-prev]')?.click(); if (e.key === 'ArrowRight') modal.querySelector('[data-gallery-next]')?.click(); });

    function digits(value) {
        return (value || "").replace(/\D/g, "");
    }

    function formatarCpf(value) {
        const numeros = digits(value).slice(0, 11);

        return numeros
            .replace(/^(\d{3})(\d)/, "$1.$2")
            .replace(/^(\d{3})\.(\d{3})(\d)/, "$1.$2.$3")
            .replace(/\.(\d{3})(\d)/, ".$1-$2");
    }

    /* Máscara dos campos de CPF */
    document.querySelectorAll("[data-cpf-mask]").forEach(input => {
        input.value = formatarCpf(input.value);

        input.addEventListener("input", () => {
            input.value = formatarCpf(input.value);
        });
    });

    /* Formata o CPF mostrado no resumo do perfil */
    document.querySelectorAll("[data-cpf-display]").forEach(element => {
        const value = element.textContent.trim();

        if (value && value !== "Não informado") {
            element.textContent = formatarCpf(value);
        }
    });

    /* Máscara do CEP */
    document.querySelectorAll("[data-cep-mask]").forEach(input => {
        const formatarCep = () => {
            let value = digits(input.value).slice(0, 8);

            if (value.length > 5) {
                value = value.replace(
                    /(\d{5})(\d{0,3})/,
                    "$1-$2"
                );
            }

            input.value = value;
        };

        formatarCep();
        input.addEventListener("input", formatarCep);
    });

    const cat = document.getElementById('product-category'); const opts = [...document.querySelectorAll('[data-size-options] [data-size-value]')];
    function filterSizes() { if (!cat) return; const text = cat.options[cat.selectedIndex]?.text || ''; const shoe = /tênis|tenis|calçado|calcado|sapato/i.test(text); opts.forEach(o => { const numeric = /^\d+$/.test(o.dataset.sizeValue || ''); o.style.display = shoe === numeric ? '' : 'none'; }); }
    cat?.addEventListener('change', filterSizes); filterSizes();
});

// Product detail gallery: thumbnails, hover-revealed navigation and white full-screen viewer.
(() => {
    const gallery = document.querySelector('[data-product-gallery]');
    const lightbox = document.querySelector('[data-product-lightbox]');
    if (!gallery || !lightbox) return;

    const thumbnails = [...gallery.querySelectorAll('[data-gallery-select]')];
    const images = thumbnails.map(button => button.querySelector('img')?.src).filter(Boolean);
    const mainImage = gallery.querySelector('[data-gallery-main]');
    const count = gallery.querySelector('[data-gallery-count]');
    const lightboxImage = lightbox.querySelector('[data-lightbox-image]');
    const lightboxCount = lightbox.querySelector('[data-lightbox-count]');
    const lightboxThumbnails = [...lightbox.querySelectorAll('[data-lightbox-select]')];
    const zoomArea = lightbox.querySelector('[data-lightbox-zoom-area]');
    const lens = lightbox.querySelector('[data-lightbox-lens]');
    const zoomButton = lightbox.querySelector('[data-lightbox-zoom]');
    let zoomEnabled = false;
    let current = 0;

    if (!mainImage || !lightboxImage || images.length === 0) return;

    const normalize = index => (index + images.length) % images.length;
    const update = (index) => {
        current = normalize(index);
        const source = images[current];
        mainImage.src = source;
        lightboxImage.src = source;
        if (lens) lens.style.backgroundImage = `url("${source}")`;
        const label = `${current + 1} / ${images.length}`;
        if (count) count.textContent = label;
        if (lightboxCount) lightboxCount.textContent = label;
        thumbnails.forEach((button, position) => {
            const active = position === current;
            button.classList.toggle('is-active', active);
            button.setAttribute('aria-current', active ? 'true' : 'false');
        });
        lightboxThumbnails.forEach((button, position) => button.classList.toggle('is-active', position === current));
    };
    const open = () => {
        update(current);
        lightbox.setAttribute('aria-hidden', 'false');
        document.body.classList.add('bf-modal-open');
    };
    const close = () => {
        lightbox.setAttribute('aria-hidden', 'true');
        document.body.classList.remove('bf-modal-open');
        zoomEnabled = false;
        zoomArea?.classList.remove('is-zoom-enabled');
        zoomButton?.setAttribute('aria-pressed', 'false');
    };

    thumbnails.forEach((button, index) => button.addEventListener('click', () => update(index)));
    gallery.querySelector('[data-gallery-previous]')?.addEventListener('click', () => update(current - 1));
    gallery.querySelector('[data-gallery-next]')?.addEventListener('click', () => update(current + 1));
    gallery.querySelector('[data-gallery-expand]')?.addEventListener('click', open);
    lightbox.querySelectorAll('[data-lightbox-close]').forEach(button => button.addEventListener('click', close));
    lightbox.querySelector('[data-lightbox-previous]')?.addEventListener('click', () => update(current - 1));
    lightbox.querySelector('[data-lightbox-next]')?.addEventListener('click', () => update(current + 1));
    lightboxThumbnails.forEach((button, index) => button.addEventListener('click', () => update(index)));
    zoomButton?.addEventListener('click', () => {
        zoomEnabled = !zoomEnabled;
        zoomArea?.classList.toggle('is-zoom-enabled', zoomEnabled);
        zoomButton.setAttribute('aria-pressed', String(zoomEnabled));
    });
    zoomArea?.addEventListener('pointermove', event => {
        if (!zoomEnabled || !lens || event.pointerType === 'touch') return;
        const rect = zoomArea.getBoundingClientRect();
        const x = Math.max(0, Math.min(event.clientX - rect.left, rect.width));
        const y = Math.max(0, Math.min(event.clientY - rect.top, rect.height));
        const zoom = 2.4;
        const halfLens = lens.offsetWidth / 2;
        lens.style.left = `${x}px`;
        lens.style.top = `${y}px`;
        lens.style.backgroundSize = `${rect.width * zoom}px ${rect.height * zoom}px`;
        lens.style.backgroundPosition = `${halfLens - x * zoom}px ${halfLens - y * zoom}px`;
    });
    document.addEventListener('keydown', event => {
        if (lightbox.getAttribute('aria-hidden') !== 'false') return;
        if (event.key === 'Escape') close();
        if (event.key === 'ArrowLeft') update(current - 1);
        if (event.key === 'ArrowRight') update(current + 1);
    });
    update(0);
})();

// First-checkout progress: makes each completed part of the delivery and payment form visible.
(() => {
    const journey = document.querySelector('[data-checkout-journey]');
    if (!journey) return;

    const groups = ['profile', 'address', 'payment'];
    const steps = groups.map(name => journey.querySelector(`[data-checkout-step="${name}"]`));
    const lines = [...journey.querySelectorAll('.bf-checkout-journey__line')];
    const filled = element => {
        if (element.type === 'radio') {
            return [...document.querySelectorAll(`[data-checkout-group="${element.closest('[data-checkout-group]')?.dataset.checkoutGroup}"] input[type="radio"]`)].some(input => input.checked);
        }
        return element.value.trim().length > 0;
    };
    const completeGroup = name => {
        const container = document.querySelector(`[data-checkout-group="${name}"]`);
        if (!container) return false;
        const directFields = [...document.querySelectorAll(`input[data-checkout-group="${name}"]`)];
        const fields = directFields.length ? directFields : [...container.querySelectorAll('input:not([type="hidden"])')];
        return fields.length > 0 && fields.every(filled);
    };
    const update = () => {
        const complete = groups.map(completeGroup);
        steps.forEach((step, index) => {
            if (!step) return;
            step.classList.toggle('is-complete', complete[index]);
            step.classList.toggle('is-active', !complete[index] && (index === 0 || complete.slice(0, index).every(Boolean)));
        });
        lines.forEach((line, index) => line.classList.toggle('is-complete', complete[index]));
    };
    document.querySelectorAll('[data-checkout-group] input').forEach(input => {
        input.addEventListener('input', update);
        input.addEventListener('change', update);
    });
    update();
})();

// ViaCEP: preenche somente os campos que o serviço consegue identificar.
(() => {
    document.querySelectorAll('[data-viacep]').forEach(input => {
        const form = input.closest('form');
        const status = form?.querySelector('[data-viacep-status]');
        let lastCep = '';
        const lookup = async () => {
            const cep = (input.value || '').replace(/\D/g, '').slice(0, 8);
            if (cep.length !== 8 || cep === lastCep) return;
            lastCep = cep;
            if (status) status.textContent = 'Consultando CEP...';
            try {
                const response = await fetch(`https://viacep.com.br/ws/${cep}/json/`, { headers: { Accept: 'application/json' } });
                if (!response.ok) throw new Error('CEP indisponível');
                const data = await response.json();
                if (data.erro) throw new Error('CEP não encontrado');
                const values = { '[data-address-street]': data.logradouro, '[data-address-complement]': data.complemento, '[data-address-neighborhood]': data.bairro, '[data-address-city]': data.localidade, '[data-address-state]': data.uf };
                Object.entries(values).forEach(([selector, value]) => { const field = form?.querySelector(selector); if (field && value) { field.value = value; field.dispatchEvent(new Event('input', { bubbles: true })); } });
                form?.querySelector('[data-address-number]')?.focus();
                if (status) status.textContent = 'Endereço preenchido pelo ViaCEP.';
            } catch {
                if (status) status.textContent = 'Não foi possível localizar o CEP. Preencha manualmente.';
            }
        };
        input.addEventListener('blur', lookup);
        input.addEventListener('input', () => { if ((input.value || '').replace(/\D/g, '').length === 8) lookup(); });
    });
})();

// Pagamento com cartão demonstrativo: mostra o cadastro apenas em crédito/débito.
(() => {
    const panel = document.querySelector('[data-card-checkout]'); if (!panel) return;
    const radios = [...document.querySelectorAll('input[name="PaymentMethod"]')];
    const saved = panel.querySelector('[data-use-saved-card]');
    const fields = panel.querySelector('[data-new-card-fields]');
    const number = panel.querySelector('[data-card-number]'), holder = panel.querySelector('[data-card-holder]'), expiry = panel.querySelector('[data-card-expiry]'), cvv = panel.querySelector('[data-card-cvv]');
    const update = () => {
        const card = radios.some(r => r.checked && (r.value === 'Credito' || r.value === 'Debito'));
        panel.hidden = !card;
        const useSaved = card && saved?.checked;
        if (fields) fields.hidden = Boolean(useSaved);
        [number, holder, expiry, cvv].forEach(input => { if (input) input.required = card && !useSaved; });
    };
    radios.forEach(r => r.addEventListener('change', update)); saved?.addEventListener('change', update);
    number?.addEventListener('input', () => { number.value = number.value.replace(/\D/g, '').slice(0, 19); const preview = panel.querySelector('[data-card-preview-number]'); if (preview) preview.textContent = (number.value || '••••••••••••••••').replace(/(.{4})/g, '$1 ').trim(); });
    holder?.addEventListener('input', () => { const preview = panel.querySelector('[data-card-preview-name]'); if (preview) preview.textContent = (holder.value || 'NOME NO CARTÃO').toUpperCase().slice(0, 24); });
    expiry?.addEventListener('input', () => { let v = expiry.value.replace(/\D/g, '').slice(0, 4); if (v.length > 2) v = `${v.slice(0, 2)}/${v.slice(2)}`; expiry.value = v; const preview = panel.querySelector('[data-card-preview-expiry]'); if (preview) preview.textContent = v || 'MM/AA'; });
    cvv?.addEventListener('input', () => { cvv.value = cvv.value.replace(/\D/g, '').slice(0, 4); });
    update();
})();

document.querySelectorAll('[data-card-form]').forEach(form => {
    const number = form.querySelector('[data-card-number]'), expiry = form.querySelector('[data-card-expiry]');
    number?.addEventListener('input', () => number.value = number.value.replace(/\D/g, '').slice(0, 19));
    expiry?.addEventListener('input', () => { let v = expiry.value.replace(/\D/g, '').slice(0, 4); expiry.value = v.length > 2 ? `${v.slice(0, 2)}/${v.slice(2)}` : v; });
});

// Hero carousel configured by the administrator.
document.querySelectorAll('[data-hero-carousel]').forEach(carousel => {
    const slides = [...carousel.querySelectorAll('[data-hero-slide]')], dots = [...carousel.querySelectorAll('[data-hero-dot]')]; let current = 0, timer;
    const show = index => { current = (index + slides.length) % slides.length; slides.forEach((s, i) => s.classList.toggle('is-active', i === current)); dots.forEach((d, i) => d.classList.toggle('is-active', i === current)); };
    const start = () => { clearInterval(timer); if (slides.length > 1) timer = setInterval(() => show(current + 1), 6500); };
    carousel.querySelector('[data-hero-prev]')?.addEventListener('click', () => { show(current - 1); start(); }); carousel.querySelector('[data-hero-next]')?.addEventListener('click', () => { show(current + 1); start(); }); dots.forEach((d, i) => d.addEventListener('click', () => { show(i); start(); })); start();
});
document.querySelectorAll('[data-product-rail]').forEach(rail => { const track = rail.querySelector('.bf-product-rail__track'); rail.querySelector('[data-rail-prev]')?.addEventListener('click', () => track.scrollBy({ left: -340, behavior: 'smooth' })); rail.querySelector('[data-rail-next]')?.addEventListener('click', () => track.scrollBy({ left: 340, behavior: 'smooth' })); });
const drawer = document.querySelector('[data-filter-drawer]'); document.querySelector('[data-filter-open]')?.addEventListener('click', () => { drawer?.classList.add('is-open'); drawer?.setAttribute('aria-hidden', 'false'); }); drawer?.querySelectorAll('[data-filter-close]').forEach(x => x.addEventListener('click', () => { drawer.classList.remove('is-open'); drawer.setAttribute('aria-hidden', 'true'); }));
document.querySelectorAll('[data-select-all]').forEach(all => all.addEventListener('change', () => all.closest('form')?.querySelectorAll('[data-select-item]').forEach(x => x.checked = all.checked)));
document.querySelectorAll('[data-notifications-toggle]').forEach(button => button.addEventListener('click', event => { event.stopPropagation(); button.closest('[data-notifications]')?.classList.toggle('is-open'); })); document.addEventListener('click', () => document.querySelectorAll('[data-notifications].is-open').forEach(x => x.classList.remove('is-open')));
document.querySelectorAll('[data-color-choice]').forEach(input => input.addEventListener('change', () => { const label = document.querySelector('[data-selected-color]'); if (label) label.textContent = input.value; const image = input.dataset.colorImage; const main = document.querySelector('[data-gallery-main]'); if (image && main) main.src = image; })); document.querySelectorAll('input[name="size"]').forEach(input => input.addEventListener('change', () => { const label = document.querySelector('[data-selected-size]'); if (label) label.textContent = input.value; }));
const sizeGuide = document.querySelector('[data-size-guide]'); document.querySelector('[data-size-guide-open]')?.addEventListener('click', () => { sizeGuide?.classList.add('is-open'); sizeGuide?.setAttribute('aria-hidden', 'false'); }); sizeGuide?.querySelectorAll('[data-size-guide-close]').forEach(x => x.addEventListener('click', () => { sizeGuide.classList.remove('is-open'); sizeGuide.setAttribute('aria-hidden', 'true'); }));
