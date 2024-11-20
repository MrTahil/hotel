// Navigációs menü interakció
document.addEventListener("DOMContentLoaded", function() {
    const menuButton = document.querySelector(".menu-button");
    const nav = document.querySelector(".flexnav");

    // Menü megjelenítése mobil nézetben
    menuButton.addEventListener("click", function() {
        nav.classList.toggle("show");
    });

    // Hover események a legördülő menükhöz
    const menuItems = document.querySelectorAll(".flexnav > li");

    menuItems.forEach(item => {
        item.addEventListener("mouseenter", function() {
            const dropdown = item.querySelector("ul");
            if (dropdown) {
                dropdown.style.display = "block";
                dropdown.classList.add("show-dropdown");
            }
        });

        item.addEventListener("mouseleave", function() {
            const dropdown = item.querySelector("ul");
            if (dropdown) {
                dropdown.style.display = "none";
                dropdown.classList.remove("show-dropdown");
            }
        });
    });

    // Ha a felhasználó az egérrel a legördülő menün kívülre megy, elrejti a legördülő menüt
    document.addEventListener("click", function(event) {
        const isMenuClicked = nav.contains(event.target);
        if (!isMenuClicked) {
            nav.classList.remove("show");
        }
    });
});
