(() => {
    const storageKey = "object-service-theme";
    const root = document.documentElement;
    const toggle = document.querySelector("[data-theme-toggle]");
    const darkModeMediaQuery = window.matchMedia(
        "(prefers-color-scheme: dark)",
    );

    const getStoredTheme = () => {
        try {
            const storedTheme = window.localStorage.getItem(storageKey);
            return storedTheme === "light" || storedTheme === "dark"
                ? storedTheme
                : null;
        } catch {
            return null;
        }
    };

    const storeTheme = (theme) => {
        try {
            window.localStorage.setItem(storageKey, theme);
        } catch {}
    };

    const resolveTheme = () =>
        getStoredTheme() ?? (darkModeMediaQuery.matches ? "dark" : "light");

    const applyTheme = (theme) => {
        root.dataset.theme = theme;

        if (!toggle) {
            return;
        }

        const isDarkTheme = theme === "dark";
        toggle.textContent = isDarkTheme ? "Light mode" : "Dark mode";
        toggle.setAttribute("aria-pressed", String(isDarkTheme));
    };

    applyTheme(root.dataset.theme || resolveTheme());

    if (!toggle) {
        return;
    }

    toggle.addEventListener("click", () => {
        const nextTheme = root.dataset.theme === "dark" ? "light" : "dark";
        storeTheme(nextTheme);
        applyTheme(nextTheme);
    });

    darkModeMediaQuery.addEventListener("change", (event) => {
        if (getStoredTheme() !== null) {
            return;
        }

        applyTheme(event.matches ? "dark" : "light");
    });
})();
