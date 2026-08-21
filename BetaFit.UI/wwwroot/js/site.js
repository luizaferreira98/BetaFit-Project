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
    // Live product image preview
    // ---------------------------------------------------------------------
    document.querySelectorAll("[data-image-preview-input]").forEach((input) => {
        const previewId = input.getAttribute("data-image-preview-input");
        const preview = previewId ? document.getElementById(previewId) : null;
        if (!preview || !(input instanceof HTMLInputElement)) return;

        const image = preview.querySelector(".bf-image-upload-preview__image");
        const placeholder = preview.querySelector(".bf-image-upload-preview__placeholder");
        const meta = preview.querySelector(".bf-image-upload-preview__meta");
        const name = preview.querySelector(".bf-image-upload-preview__name");
        const size = preview.querySelector(".bf-image-upload-preview__size");
        let objectUrl = null;

        input.addEventListener("change", () => {
            const file = input.files && input.files[0];
            if (!file) return;

            const allowed = ["image/jpeg", "image/png", "image/webp"];
            if (!allowed.includes(file.type)) {
                input.value = "";
                preview.classList.remove("is-ready");
                if (image) image.hidden = true;
                if (meta) meta.hidden = true;
                if (placeholder) placeholder.hidden = false;
                return;
            }

            if (objectUrl) URL.revokeObjectURL(objectUrl);
            objectUrl = URL.createObjectURL(file);

            if (image) {
                image.src = objectUrl;
                image.hidden = false;
            }
            if (placeholder) placeholder.hidden = true;
            if (meta) meta.hidden = false;
            if (name) name.textContent = file.name;
            if (size) size.textContent = formatFileSize(file.size);
            preview.classList.add("is-ready");
        });
    });

    function formatFileSize(bytes) {
        if (bytes < 1024) return `${bytes} B`;
        if (bytes < 1024 * 1024) return `${(bytes / 1024).toFixed(1)} KB`;
        return `${(bytes / (1024 * 1024)).toFixed(1)} MB`;
    }

    // ---------------------------------------------------------------------
    // Keep the logout confirmation from being replaced by the page transition.
    // ---------------------------------------------------------------------
    document.querySelectorAll(".bf-logout-form").forEach((form) => {
        form.addEventListener("submit", () => {
            body.classList.add("bf-page-leaving");
        });
    });
})();

// Beta Fit - quantity controls. The form remains the source of truth so
// the existing server/session cart behavior is preserved.
document.addEventListener('click', (event) => {
    const button = event.target.closest('[data-quantity-minus], [data-quantity-plus]');
    if (!button) return;
    const control = button.closest('[data-quantity-control]');
    const input = control?.querySelector('.bf-quantity-input');
    if (!input) return;
    const min = Number(input.min || 1);
    const max = Number(input.max || 99);
    const current = Number(input.value || min);
    input.value = String(Math.min(max, Math.max(min, current + (button.hasAttribute('data-quantity-plus') ? 1 : -1))));
    input.dispatchEvent(new Event('change', { bubbles: true }));
});

document.addEventListener('input', (event) => {
    const input = event.target.closest('.bf-quantity-input');
    if (!input) return;
    const min = Number(input.min || 1);
    const max = Number(input.max || 99);
    let value = Number(input.value);
    if (!Number.isFinite(value)) return;
    input.value = String(Math.min(max, Math.max(min, Math.floor(value))));
});
