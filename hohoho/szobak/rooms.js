// Szoba részletek kezelése
document.addEventListener("DOMContentLoaded", function() {
    const detailsBtns = document.querySelectorAll(".details-btn");
    const roomPopup = document.querySelector("#room-popup");
    const roomDescription = document.querySelector("#room-description");
    const closePopupBtn = document.querySelector(".close-popup");

    // Részletek megjelenítése
    detailsBtns.forEach(button => {
        button.addEventListener("click", function() {
            const roomId = this.parentElement.id;
            const roomData = getRoomDetails(roomId);
            showPopup(roomData);
        });
    });

    // Popup megjelenítése
    function showPopup(data) {
        roomDescription.textContent = data.description;
        roomPopup.style.display = "flex";
    }

    // Popup bezárása
    closePopupBtn.addEventListener("click", function() {
        roomPopup.style.display = "none";
    });

    // Szoba részletek
    function getRoomDetails(roomId) {
        const rooms = {
            "room-1": {
                description: "A Standard Szoba ideális választás a pihenéshez. Két felnőtt számára kényelmes helyet biztosít, modern felszereltséggel."
            },
            "room-2": {
                description: "A Family Suite egy tágas, kényelmes családi szoba, amely két különálló hálószobával és saját fürdőszobával rendelkezik."
            },
            "room-3": {
                description: "A Deluxe Szoba a legmagasabb kényelmet biztosít, luxus felszereltséggel és saját terasszal, panorámás kilátással."
            }
        };

        return rooms[roomId];
    }
});
