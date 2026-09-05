(() => {
    const storageKey = "object-service-theme";
    const paletteStorageKey = "object-service-theme-palette-v2";
    const themeStudioStorageKey = "object-service-theme-studio-open";
    const root = document.documentElement;
    const toggle = document.querySelector("[data-theme-toggle]");
    const themeStudioToggle = document.querySelector(
        "[data-theme-studio-toggle]",
    );
    const themeStudioPanel = document.querySelector("[data-theme-studio]");
    const paletteForm = document.querySelector("[data-theme-palette-form]");
    const darkModeMediaQuery = window.matchMedia(
        "(prefers-color-scheme: dark)",
    );
    const themeTools = window.objectServiceThemeTools;

    if (!themeTools) {
        return;
    }

    const paletteFields = themeTools.paletteFields.map((name) => ({
        name,
        picker: paletteForm?.querySelector(`[data-palette-picker="${name}"]`),
        input: paletteForm?.querySelector(`[data-palette-input="${name}"]`),
    }));
    const paletteStatus = paletteForm?.querySelector(
        "[data-theme-palette-status]",
    );
    const paletteResetButton = paletteForm?.querySelector(
        "[data-theme-palette-reset]",
    );

    const getStoredTheme = () => {
        const storedTheme = themeTools.readStorage(storageKey);
        return storedTheme === "light" || storedTheme === "dark"
            ? storedTheme
            : null;
    };

    const storeTheme = (theme) => {
        themeTools.writeStorage(storageKey, theme);
    };

    const getStoredPalette = () =>
        themeTools.getStoredPalette(paletteStorageKey);

    const storePalette = (palette) => {
        themeTools.writeStorage(paletteStorageKey, JSON.stringify(palette));
    };

    const clearStoredPalette = () => {
        themeTools.removeStorage(paletteStorageKey);
    };

    const getStoredThemeStudioOpen = () =>
        themeTools.readStorage(themeStudioStorageKey) === "true";

    const storeThemeStudioOpen = (isOpen) => {
        themeTools.writeStorage(themeStudioStorageKey, String(isOpen));
    };

    const resolveTheme = () =>
        getStoredTheme() ?? (darkModeMediaQuery.matches ? "dark" : "light");

    const applyTheme = (theme) => {
        root.dataset.theme = theme;

        const palette = getStoredPalette() ?? themeTools.defaultPalette;
        themeTools.applyPalette(root, theme, palette);

        if (!toggle) {
            return;
        }

        const isDarkTheme = theme === "dark";
        toggle.textContent = isDarkTheme ? "Light mode" : "Dark mode";
        toggle.setAttribute("aria-pressed", String(isDarkTheme));
    };

    const applyThemeStudioState = (isOpen) => {
        if (!themeStudioToggle || !themeStudioPanel) {
            return;
        }

        themeStudioToggle.hidden = false;
        themeStudioPanel.hidden = !isOpen;
        themeStudioToggle.textContent = isOpen
            ? "Hide palette"
            : "Theme palette";
        themeStudioToggle.setAttribute("aria-expanded", String(isOpen));
    };

    const setPaletteStatus = (message, isError = false) => {
        if (!paletteStatus) {
            return;
        }

        paletteStatus.textContent = message;
        paletteStatus.dataset.state = isError ? "error" : "default";
    };

    const setPaletteInputs = (palette) => {
        for (const field of paletteFields) {
            if (!field.input || !field.picker) {
                continue;
            }

            const value = palette[field.name];
            field.input.value = value;
            field.picker.value = value;
            field.input.classList.remove("palette-input-invalid");
        }

        updateVariantSwatches(palette);
    };

    const updateVariantSwatches = (palette) => {
        for (const key of themeTools.paletteFields) {
            const container = paletteForm?.querySelector(
                `[data-variant-swatches="${key}"]`,
            );
            if (!container) {
                continue;
            }

            const variants = themeTools.generateVariants(palette[key]);

            // Build swatch labels: [dark→bright, …, dark→dark]
            // In light mode, positions 0=darkest, 4=lightest
            // In dark mode, positions flip: 0=lightest (flipped from 4), 4=darkest (flipped from 0)
            const lightLabels = ["Mid–Dk", "Mid–", "Mid", "Mid+", "Mid++"];
            const darkLabels =  ["Mid++", "Mid+", "Mid", "Mid–", "Mid–Dk"];

            container.innerHTML = variants
                .map((hex, i) => {
                    const lightLabel = i === 2 ? "Both" : lightLabels[i];
                    const darkLabel  = i === 2 ? "Both" : darkLabels[i];
                    return (
                        `<span class="variant-swatch" style="background:${hex}" title="Light: ${lightLabel}  ·  Dark: ${darkLabel}  ·  ${hex}">` +
                        `<span class="variant-swatch-label">Light: ${lightLabel}<br/>Dark: ${darkLabel}<br/>${hex}</span>` +
                        `</span>`
                    );
                })
                .join("");
        }
    };

    const readPaletteFromInputs = () => {
        const palette = {};
        let isValid = true;

        for (const field of paletteFields) {
            if (!field.input || !field.picker) {
                continue;
            }

            const value = themeTools.normaliseHex(field.input.value);
            if (!value) {
                field.input.classList.add("palette-input-invalid");
                isValid = false;
                continue;
            }

            field.input.classList.remove("palette-input-invalid");
            field.input.value = value;
            field.picker.value = value;
            palette[field.name] = value;
        }

        return isValid ? palette : null;
    };

    const applyCustomPalette = (palette, persist) => {
        themeTools.applyPalette(
            root,
            root.dataset.theme || resolveTheme(),
            palette,
        );

        if (persist) {
            storePalette(palette);
        }

        setPaletteInputs(palette);
    };

    applyTheme(root.dataset.theme || resolveTheme());

    if (themeStudioToggle) {
        if (themeStudioPanel) {
            applyThemeStudioState(getStoredThemeStudioOpen());
        } else {
            themeStudioToggle.hidden = true;
        }
    }

    const storedPalette = getStoredPalette();
    if (paletteForm) {
        setPaletteInputs(storedPalette ?? themeTools.defaultPalette);
    }

    if (toggle) {
        toggle.addEventListener("click", () => {
            const nextTheme = root.dataset.theme === "dark" ? "light" : "dark";
            storeTheme(nextTheme);
            applyTheme(nextTheme);
        });
    }

    darkModeMediaQuery.addEventListener("change", (event) => {
        if (getStoredTheme() !== null) {
            return;
        }

        applyTheme(event.matches ? "dark" : "light");
    });

    themeStudioToggle?.addEventListener("click", () => {
        if (!themeStudioPanel) {
            return;
        }

        const nextState = themeStudioPanel.hidden;
        applyThemeStudioState(nextState);
        storeThemeStudioOpen(nextState);
    });

    if (!paletteForm) {
        return;
    }

    for (const field of paletteFields) {
        if (!field.input || !field.picker) {
            continue;
        }

        field.picker.addEventListener("input", () => {
            field.input.value = field.picker.value.toUpperCase();
            field.input.classList.remove("palette-input-invalid");

            const palette = readPaletteFromInputs();
            if (!palette) {
                return;
            }

            applyCustomPalette(palette, true);
            setPaletteStatus("Applied custom palette.");
        });

        field.input.addEventListener("change", () => {
            const value = themeTools.normaliseHex(field.input.value);
            if (!value) {
                field.input.classList.add("palette-input-invalid");
                setPaletteStatus(
                    "Use six-digit hex colours such as #F6C945.",
                    true,
                );
                return;
            }

            field.input.value = value;
            field.picker.value = value;
            field.input.classList.remove("palette-input-invalid");

            const palette = readPaletteFromInputs();
            if (!palette) {
                return;
            }

            applyCustomPalette(palette, true);
            setPaletteStatus("Applied custom palette.");
        });
    }

    paletteForm.addEventListener("submit", (event) => {
        event.preventDefault();

        const palette = readPaletteFromInputs();
        if (!palette) {
            setPaletteStatus(
                "Use six-digit hex colours such as #F6C945.",
                true,
            );
            return;
        }

        applyCustomPalette(palette, true);
        setPaletteStatus("Applied custom palette.");
    });

    paletteResetButton?.addEventListener("click", () => {
        clearStoredPalette();
        const theme = root.dataset.theme || resolveTheme();
        themeTools.applyPalette(root, theme, themeTools.defaultPalette);
        setPaletteInputs(themeTools.defaultPalette);
        setPaletteStatus("Reverted to the default palette.");
    });
})();
