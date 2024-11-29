// Szoba részletek kezelése
document.addEventListener("DOMContentLoaded", function() {
    const roomList = document.querySelector(".room-list");
    const roomPopup = document.querySelector("#room-popup");
    const roomDescription = document.querySelector("#room-description");
    const closePopupBtn = document.querySelector(".close-popup");

    // Fetch a szobaadatokat
    fetch('C:\Users\Tanulo\Desktop\Hotel-Web\hohoho\szobak\rooms.json')
        .then(response => response.json())
        .then(rooms => {
            rooms.forEach(room => {
                // Szoba kártya létrehozása
                const roomCard = document.createElement("div");
                roomCard.classList.add("room-card");
                roomCard.id = room.id;

                roomCard.innerHTML = `
                    <img src="${room.image}" alt="${room.name}">
                    <h2>${room.name}</h2>
                    <p class="description">${room.description}</p>
                    <div class="small-images">
                        ${room.smallImages.map(image => `<img src="${image}" alt="Szoba extra">`).join('')}
                    </div>
                    <p class="price">Ár: ${room.pricePerNight}</p>
                    <p class="capacity">Férőhely: ${room.capacity}</p>
                    <button class="details-btn">További részletek</button>
                `;
                roomList.appendChild(roomCard);

                // Szoba részletek gomb kezelése
                const detailsBtn = roomCard.querySelector(".details-btn");
                detailsBtn.addEventListener("click", function() {
                    showPopup(room);
                });
            });
        })
        .catch(error => console.error('Hiba történt a szobaadatok betöltésekor:', error));

    // Popup megjelenítése
    function showPopup(room) {
        roomDescription.innerHTML = `
            <h2>${room.name}</h2>
            <p>${room.description}</p>
            <p><strong>Ár: ${room.pricePerNight}</strong></p>
            <p><strong>Férőhely: ${room.capacity}</strong></p>
            <div class="small-images">
                ${room.smallImages.map(image => `<img src="${image}" alt="Szoba extra">`).join('')}
            </div>
        `;
        roomPopup.style.display = "flex";
    }

    // Popup bezárása
    closePopupBtn.addEventListener("click", function() {
        roomPopup.style.display = "none";
    });
});
