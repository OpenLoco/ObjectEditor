(() => {
    // Light/dark only. The palette itself lives in base.css; this just flips
    // data-theme on <html> and remembers the choice.
    const storageKey = "object-service-theme";
    const root = document.documentElement;
    const toggle = document.querySelector("[data-theme-toggle]");
    const themeSwitch = document.querySelector("[data-theme-switch]");
    const darkModeMediaQuery = window.matchMedia(
        "(prefers-color-scheme: dark)",
    );

    const readStorage = () => {
        try {
            return window.localStorage.getItem(storageKey);
        } catch {
            return null;
        }
    };

    const writeStorage = (value) => {
        try {
            window.localStorage.setItem(storageKey, value);
        } catch {
        }
    };

    const getStoredTheme = () => {
        const storedTheme = readStorage();
        return storedTheme === "light" || storedTheme === "dark"
            ? storedTheme
            : null;
    };

    const resolveTheme = () =>
        getStoredTheme() ?? (darkModeMediaQuery.matches ? "dark" : "light");

    const applyTheme = (theme) => {
        root.dataset.theme = theme;

        if (toggle) {
            toggle.setAttribute("aria-checked", String(theme === "dark"));
            themeSwitch?.setAttribute("data-theme", theme);
        }
    };

    applyTheme(root.dataset.theme || resolveTheme());

    if (toggle) {
        toggle.addEventListener("click", () => {
            const nextTheme = root.dataset.theme === "dark" ? "light" : "dark";
            writeStorage(nextTheme);
            applyTheme(nextTheme);
        });
    }

    darkModeMediaQuery.addEventListener("change", (event) => {
        if (getStoredTheme() !== null) {
            return;
        }

        applyTheme(event.matches ? "dark" : "light");
    });
})();
