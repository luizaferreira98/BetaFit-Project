(() => {
    "use strict";

    const root = document.documentElement;
    const body = document.body;

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
            picker.classList.toggle("is-not-required", !requires);
            picker.querySelectorAll('input[name="AvailableSizes"]').forEach((input) => {
                input.disabled = !requires;
                if (!requires) input.checked = false;
            });
            const status = document.getElementById("size-status");
            const helper = document.getElementById("size-helper");
            if (status) status.textContent = requires ? "Obrigatório para esta categoria" : "Não se aplica";
            if (helper) helper.textContent = requires
                ? "Selecione os tamanhos que o cliente poderá escolher."
                : "A seleção de tamanho não será exibida para este tipo de produto.";
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
        const allowed = ["image/jpeg", "image/png", "image/webp"];
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
            const valid = Array.from(incoming).filter(file => allowed.includes(file.type) && file.size <= 5 * 1024 * 1024);
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
    const button=event.target.closest("[data-quantity-minus],[data-quantity-plus]"); if(!button)return;
    const control=button.closest("[data-quantity-control]"); const input=control?.querySelector(".bf-quantity-input"); if(!input)return;
    const min=Number(input.min||1), max=Number(input.max||99), current=Number(input.value||min);
    input.value=String(Math.min(max,Math.max(min,current+(button.hasAttribute("data-quantity-plus")?1:-1))));
    const form=control.closest("form"); if(form && control.dataset.autoSubmit==="true") window.setTimeout(()=>form.requestSubmit(),80);
});
document.addEventListener("input",(event)=>{
    const input=event.target.closest(".bf-quantity-input"); if(!input)return; const min=Number(input.min||1),max=Number(input.max||99); let v=Number(input.value); if(Number.isFinite(v))input.value=String(Math.min(max,Math.max(min,Math.floor(v))));
});
// Brazilian phone mask with a hard 11-digit limit.
document.querySelectorAll("[data-phone-mask]").forEach(input=>input.addEventListener("input",()=>{let d=input.value.replace(/\D/g,"").slice(0,11); if(d.length<=10)input.value=d.replace(/(\d{2})(\d{4})(\d{0,4})/,"($1) $2-$3").replace(/-$/,''); else input.value=d.replace(/(\d{2})(\d{5})(\d{0,4})/,"($1) $2-$3").replace(/-$/,'');}));
// Product gallery.
document.querySelectorAll("[data-gallery-image]").forEach(button=>button.addEventListener("click",()=>{const main=document.getElementById("bf-main-product-image");if(main)main.src=button.dataset.galleryImage||"";}));


// Product-card image carousel: desktop hover + touch swipe on mobile.
(() => {
    const reduced = window.matchMedia("(prefers-reduced-motion: reduce)").matches;
    document.querySelectorAll("[data-product-carousel]").forEach((media) => {
        const slides = [...media.querySelectorAll(".bf-product-card__slides > img")];
        if (slides.length <= 1) return;
        let index = 0;
        let timer = null;
        let startX = 0;

        const show = (next) => {
            index = (next + slides.length) % slides.length;
            slides.forEach((img, i) => img.classList.toggle("is-active", i === index));
        };
        show(0);

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
