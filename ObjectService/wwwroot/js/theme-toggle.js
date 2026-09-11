(() => {
    const storageKey = "object-service-theme";
    const paletteStorageKey = "object-service-theme-palette";
    const themeStudioStorageKey = "object-service-theme-studio-open";
    const root = document.documentElement;
    const toggle = document.querySelector("[data-theme-toggle]");
    const themeSwitch = document.querySelector("[data-theme-switch]");
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
    const paletteModeButtons = Array.from(
        paletteForm?.querySelectorAll("[data-theme-palette-mode-button]") ?? [],
    );

    // Which palette (light or dark) the studio inputs are currently editing.
    let paletteMode = "light";

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
        toggle.setAttribute("aria-checked", String(isDarkTheme));
        themeSwitch?.setAttribute("data-theme", theme);
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

            const value =
                palette?.[field.name] ?? themeTools.defaultPalette[paletteMode][field.name];
            field.input.value = value;
            field.picker.value = value;
            field.input.classList.remove("palette-input-invalid");
        }
    };

    const setPaletteMode = (mode) => {
        paletteMode = mode;

        for (const button of paletteModeButtons) {
            const active = button.dataset.themePaletteModeButton === mode;
            button.classList.toggle("theme-palette-mode-active", active);
            button.setAttribute("aria-pressed", String(active));
        }

        setPaletteInputs((getStoredPalette() ?? themeTools.defaultPalette)[mode]);
        setPaletteStatus(
            `Editing the ${mode} palette. Stored locally in this browser.`,
        );
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

    const applyCustomPalette = (editedPalette) => {
        const stored = getStoredPalette() ?? themeTools.defaultPalette;
        const combined = { ...stored, [paletteMode]: editedPalette };

        storePalette(combined);

        const theme = root.dataset.theme || resolveTheme();
        themeTools.applyPalette(root, theme, combined);
        setPaletteStatus(`Applied the ${paletteMode} palette.`);
    };
    applyTheme(root.dataset.theme || resolveTheme());

    if (themeStudioToggle) {
        if (themeStudioPanel) {
            applyThemeStudioState(getStoredThemeStudioOpen());
        } else {
            themeStudioToggle.hidden = true;
        }
    }

    // Default the palette-mode toggle to the theme currently in use.
    setPaletteMode(getStoredTheme() ?? resolveTheme());

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

    for (const button of paletteModeButtons) {
        button.addEventListener("click", () => {
            setPaletteMode(button.dataset.themePaletteModeButton);
        });
    }

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

            applyCustomPalette(palette);
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

            applyCustomPalette(palette);
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

        applyCustomPalette(palette);
    });

    paletteResetButton?.addEventListener("click", () => {
        clearStoredPalette();
        const theme = root.dataset.theme || resolveTheme();
        themeTools.applyPalette(root, theme, themeTools.defaultPalette);
        setPaletteMode(paletteMode);
        setPaletteStatus("Reverted to the default palette.");
    });
})();