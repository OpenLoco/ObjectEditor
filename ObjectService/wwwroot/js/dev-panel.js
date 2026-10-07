(() => {
    // Show/hide the "Developer mode" panel. Collapsed by default; the choice is
    // remembered in this browser. Only present in Development for admins.
    const storageKey = "object-service-dev-panel-open";
    const toggle = document.querySelector("[data-dev-panel-toggle]");
    const panel = document.querySelector("[data-dev-panel]");

    if (!toggle || !panel) {
        return;
    }

    const readOpen = () => {
        try {
            return window.localStorage.getItem(storageKey) === "true";
        } catch {
            return false;
        }
    };

    const writeOpen = (isOpen) => {
        try {
            window.localStorage.setItem(storageKey, String(isOpen));
        } catch {
        }
    };

    const apply = (isOpen) => {
        panel.hidden = !isOpen;
        toggle.setAttribute("aria-expanded", String(isOpen));
        toggle.textContent = isOpen ? "Hide developer" : "Developer";
    };

    apply(readOpen());

    toggle.addEventListener("click", () => {
        const nextOpen = panel.hidden;
        apply(nextOpen);
        writeOpen(nextOpen);
    });
})();
