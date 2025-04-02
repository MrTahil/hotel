import React, { useState, useEffect } from "react";
import "../styles/serprog.css";

function Programs() {
  const [programs, setPrograms] = useState([]);
  const [selectedProgram, setSelectedProgram] = useState(null);
  const [numberOfTickets, setNumberOfTickets] = useState(1);

  const fetchGuestData = async () => {
    try {
      const token = localStorage.getItem("authToken");
      const username = localStorage.getItem("username");
      if (!token || !username) {
        throw new Error("Nincs token vagy felhasználónév elmentve! Jelentkezz be.");
      }
      const response = await fetch(
        `${process.env.REACT_APP_API_BASE_URL}/Guests/GetGuestData/${username}`,
        {
          method: "GET",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );
      if (!response.ok) {
        throw new Error(`Nem sikerült lekérni a vendég adatait: ${response.status}`);
      }
      const data = await response.json();
      return data;
    } catch (err) {
      console.error("Hiba a vendég adatok lekérdezésekor:", err.message);
      return null;
    }
  };

  const handleBooking = async () => {
    try {
      const token = localStorage.getItem("authToken");
      if (!token) {
        throw new Error("Nincs token elmentve! Jelentkezz be újra.");
      }
      const guestData = await fetchGuestData();
      if (!guestData || guestData.length === 0) {
        throw new Error("Nincs vendég adat a felhasználóhoz!");
      }
      const guestId = guestData[0].guestId;
      const payload = {
        GuestId: guestId,
        NumberOfTickets: numberOfTickets,
        Status: "Foglalt",
        PaymentStatus: "Fizetésre vár",
        Notes: "",
      };
      const response = await fetch(
        `${process.env.REACT_APP_API_BASE_URL}/Feedback/NewEvenBooking/${selectedProgram.id}`,
        {
          method: "POST",
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
          body: JSON.stringify(payload),
        }
      );
      if (!response.ok) {
        throw new Error(`Hiba a foglalás során: ${response.status}`);
      }
      alert("Foglalás sikeres!");
      closeModal();
    } catch (err) {
      console.error("Hiba a foglalás közben:", err.message);
      alert(err.message);
    }
  };

  useEffect(() => {
    fetch(process.env.REACT_APP_API_BASE_URL + "/Events/Geteents")
      .then((response) => response.json())
      .then((data) => {
        const formattedPrograms = data.map((event) => ({
          id: event.eventId,
          name: event.eventName,
          description: event.description,
          images: event.images || "../img/default_imgage.png",
          schedule: event.eventDate,
          organizerName: event.organizerName,
          contactInfo: event.contactInfo,
          location: event.location,
          capacity: event.capacity,
          price: event.price,
        }));
        console.log(data);
        setPrograms(formattedPrograms);
      })
      .catch((error) =>
        console.error("Hiba történt az adatok lekérésekor:", error)
      );
  }, []);

  const openModal = (program) => {
    setSelectedProgram(program);
    setNumberOfTickets(1);
  };

  const closeModal = () => {
    setSelectedProgram(null);
  };

  const truncateDescription = (description, maxLength) => {
    if (description.length > maxLength) {
      return description.substring(0, maxLength) + "...";
    }
    return description;
  };

  return (
    <div className="min-h-screen bg-blue-50 py-10 px-4 sm:px-6 lg:px-8">
      <h1 className="text-4xl font-bold text-blue-800 text-center mb-10 animate-bounce">
        Programok
      </h1>
      <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-8 max-w-6xl mx-auto">
        {programs.map((program) => (
          <div
            key={program.id}
            className="bg-white rounded-xl shadow-lg overflow-hidden hover:shadow-2xl hover:scale-105 transform animate-fade-in"
          >
            <div className="relative">
              <img
                src={program.images}
                alt={program.name}
                className="w-full h-56 object-cover"
              />
              <div className="absolute top-2 right-2 bg-blue-600 text-white text-xs font-semibold px-2 py-1 rounded-full">
                {program.capacity} férőhely
              </div>
            </div>
            <div className="p-5">
              <h2 className="text-2xl font-semibold text-blue-800 mb-2">
                {program.name}
              </h2>
              <p className="text-gray-600 text-sm mb-4">
                {truncateDescription(program.description, 70)}
              </p>
              <button
                onClick={() => openModal(program)}
                className="w-full bg-blue-600 text-white py-2 rounded-lg font-semibold hover:bg-blue-800 hover:shadow-lg"
              >
                Részletek
              </button>
            </div>
          </div>
        ))}
      </div>

      {selectedProgram && (
        <div className="fixed inset-0 bg-black bg-opacity-60 flex items-center justify-center p-4 z-50 animate-fade-in">
          <div className="bg-white rounded-2xl shadow-2xl max-w-3xl w-full mx-4 overflow-hidden">
            <div className="bg-blue-600 text-white p-6 flex justify-between items-center">
              <h2 className="text-2xl font-bold">{selectedProgram.name}</h2>
              <button
                onClick={closeModal}
                className="text-white hover:text-blue-200 focus:outline-none"
              >
                <svg
                  className="w-6 h-6"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    strokeWidth="2"
                    d="M6 18L18 6M6 6l12 12"
                  />
                </svg>
              </button>
            </div>
            <div className="p-6 bg-blue-50">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
                <div>
                  <img
                    src={selectedProgram.images}
                    alt={selectedProgram.name}
                    className="w-full h-64 object-cover rounded-lg shadow-md"
                  />
                </div>
                <div className="space-y-4 text-gray-800">
                  <p>
                    <span className="font-semibold text-blue-800">Időpont:</span>{" "}
                    {new Date(selectedProgram.schedule).toLocaleString("hu-HU")}
                  </p>
                  <p>
                    <span className="font-semibold text-blue-800">Szervező:</span>{" "}
                    {selectedProgram.organizerName}
                  </p>
                  <p>
                    <span className="font-semibold text-blue-800">Elérhetőség:</span>{" "}
                    {selectedProgram.contactInfo}
                  </p>
                  <p>
                    <span className="font-semibold text-blue-800">Helyszín:</span>{" "}
                    {selectedProgram.location}
                  </p>
                  <p>
                    <span className="font-semibold text-blue-800">Férőhelyek:</span>{" "}
                    {selectedProgram.capacity}
                  </p>
                  <p>
                    <span className="font-semibold text-blue-800">Ár / Fő:</span>{" "}
                    {selectedProgram.price
                      ? `${selectedProgram.price.toLocaleString("hu-HU")} Ft`
                      : "N/A"}
                  </p>
                  <p>
                    <span className="font-semibold text-blue-800">Leírás:</span>{" "}
                    {selectedProgram.description}
                  </p>
                  <div>
                    <label className="block text-sm font-semibold text-blue-800 mb-2">
                      Jegyek száma
                    </label>
                    <input
                      type="number"
                      min="1"
                      max={selectedProgram.capacity}
                      value={numberOfTickets}
                      onChange={(e) => setNumberOfTickets(parseInt(e.target.value))}
                      className="w-full p-2 border border-blue-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none"
                    />
                  </div>
                </div>
              </div>
            </div>
            <div className="bg-blue-100 p-6 flex justify-end space-x-4">
              <button
                onClick={handleBooking}
                className="bg-blue-600 text-white px-6 py-2 rounded-lg font-semibold hover:bg-blue-800 hover:shadow-lg"
              >
                Foglalás
              </button>
              <button
                onClick={closeModal}
                className="bg-red-600 text-white px-6 py-2 rounded-lg font-semibold hover:bg-red-800 hover:shadow-lg"
              >
                Bezárás
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}

export default Programs;