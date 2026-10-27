
(() => {
    const root = document.documentElement;
    const button = document.getElementById("themeToggle");
    const icon = document.getElementById("themeIcon");
    const label = document.getElementById("themeLabel");

    function applyTheme(theme) {
        const selected = theme === "dark" ? "dark" : "light";

        root.setAttribute("data-theme", selected);

        if (icon) icon.textContent = selected === "dark" ? "☀" : "☾";
        if (label) label.textContent = selected === "dark" ? "Light mode" : "Dark mode";

        if (button) {
            button.setAttribute("aria-pressed", String(selected === "dark"));
        }
    }

    let savedTheme = "light";

    try {
        savedTheme = localStorage.getItem("phezulu-theme") || "light";
    } catch {
        // Continue with light mode if storage is unavailable.
    }

    applyTheme(savedTheme);

    if (button) {
        button.addEventListener("click", () => {
            const next =
                root.getAttribute("data-theme") === "dark" ? "light" : "dark";

            applyTheme(next);

            try {
                localStorage.setItem("phezulu-theme", next);
            } catch {
                // Theme still works for this page if storage is unavailable.
            }
        });
    }
})();



(() => {
    const sidebar = document.getElementById("pzSidebar");
    const overlay = document.getElementById("pzSidebarOverlay");
    const openButton = document.getElementById("menuToggle");
    const closeButton = document.getElementById("menuClose");

    if (!sidebar || !overlay || !openButton || !closeButton) return;

    function setSidebarOpen(open) {
        sidebar.classList.toggle("open", open);
        overlay.hidden = !open;
        openButton.setAttribute("aria-expanded", String(open));
        document.body.style.overflow = open ? "hidden" : "";
    }

    openButton.addEventListener("click", () => setSidebarOpen(true));
    closeButton.addEventListener("click", () => setSidebarOpen(false));
    overlay.addEventListener("click", () => setSidebarOpen(false));

    document.addEventListener("keydown", event => {
        if (event.key === "Escape") setSidebarOpen(false);
    });
})();
