import React, { useState, useEffect } from "react";
import "../styles/serprog.css";

function Programs() {
  const [programs, setPrograms] = useState([]);
  const [selectedProgram, setSelectedProgram] = useState(null);
  const [numberOfTickets, setNumberOfTickets] = useState(1);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);

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
    const fetchPrograms = async () => {
      try {
        setIsLoading(true);
        const response = await fetch(process.env.REACT_APP_API_BASE_URL + "/Events/Geteents");
        if (!response.ok) {
          throw new Error(`Hiba történt az adatok lekérésekor: ${response.status}`);
        }
        const data = await response.json();
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
        setPrograms(formattedPrograms);
      } catch (err) {
        console.error("Hiba történt az adatok lekérésekor:", err);
        setError(err.message);
      } finally {
        setIsLoading(false);
      }
    };

    fetchPrograms();
  }, []);

  const openModal = (program) => {
    setSelectedProgram(program);
    setNumberOfTickets(1);
  };

  const closeModal = () => {
    setSelectedProgram(null);
  };

  const truncateDescription = (description, maxLength) => {
    if (!description) return "";
    if (description.length > maxLength) {
      return description.substring(0, maxLength) + "...";
    }
    return description;
  };

  if (isLoading) {
    return (
      <div className="min-h-screen bg-blue-50 flex items-center justify-center">
        <div className="text-center">
          <div className="w-16 h-16 border-4 border-blue-500 border-t-transparent rounded-full animate-spin mx-auto"></div>
          <p className="mt-4 text-lg text-blue-800">Programok betöltése...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen bg-blue-50 flex items-center justify-center px-4">
        <div className="bg-white p-6 rounded-lg shadow-lg max-w-md w-full text-center">
          <div className="text-red-500 mb-4">
            <svg xmlns="http://www.w3.org/2000/svg" className="h-12 w-12 mx-auto" fill="none" viewBox="0 0 24 24" stroke="currentColor">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z" />
            </svg>
          </div>
          <h2 className="text-xl font-bold text-gray-800 mb-2">Hiba történt</h2>
          <p className="text-gray-600 mb-4">{error}</p>
          <button 
            onClick={() => window.location.reload()}
            className="bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors"
          >
            Újrapróbálkozás
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-blue-50 py-6 px-4 sm:px-6 lg:px-8">
      <div className="max-w-7xl mx-auto">
        <h1 className="text-3xl sm:text-4xl font-bold text-blue-800 text-center mb-8 sm:mb-10">
          Programok
        </h1>
        
        {programs.length === 0 ? (
          <div className="text-center py-10">
            <p className="text-gray-600 text-lg">Jelenleg nincsenek elérhető programok.</p>
          </div>
        ) : (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-6">
            {programs.map((program) => (
              <div
                key={program.id}
                className="bg-white rounded-lg shadow-md overflow-hidden hover:shadow-lg transition-shadow duration-300"
              >
                <div className="relative h-48 sm:h-56">
                  <img
                    src={program.images}
                    alt={program.name}
                    className="w-full h-full object-cover"
                    onError={(e) => {
                      e.target.src = "../img/default_imgage.png";
                    }}
                  />
                  <div className="absolute top-2 right-2 bg-blue-600 text-white text-xs font-semibold px-2 py-1 rounded-full">
                    {program.capacity} férőhely
                  </div>
                </div>
                <div className="p-4 sm:p-5">
                  <h2 className="text-xl sm:text-2xl font-semibold text-blue-800 mb-2 line-clamp-2">
                    {program.name}
                  </h2>
                  <p className="text-gray-600 text-sm mb-4 line-clamp-3">
                    {truncateDescription(program.description, 100)}
                  </p>
                  <div className="flex justify-between items-center">
                    <span className="text-blue-600 font-bold">
                      {program.price ? `${program.price.toLocaleString("hu-HU")} Ft` : "Ingyenes"}
                    </span>
                    <button
                      onClick={() => openModal(program)}
                      className="bg-blue-600 text-white px-4 py-2 rounded-lg text-sm sm:text-base hover:bg-blue-700 transition-colors"
                    >
                      Részletek
                    </button>
                  </div>
                </div>
              </div>
            ))}
          </div>
        )}
      </div>

      {selectedProgram && (
        <div className="fixed inset-0 bg-black bg-opacity-60 flex items-center justify-center p-4 z-50 overflow-y-auto">
          <div className="bg-white rounded-xl shadow-xl w-full max-w-4xl mx-2 sm:mx-4 max-h-[90vh] overflow-y-auto">
            <div className="bg-blue-600 text-white p-4 sm:p-6 flex justify-between items-center sticky top-0 z-10">
              <h2 className="text-xl sm:text-2xl font-bold">{selectedProgram.name}</h2>
              <button
                onClick={closeModal}
                className="text-white hover:text-blue-200 focus:outline-none"
                aria-label="Bezárás"
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
            <div className="p-4 sm:p-6 bg-blue-50">
              <div className="flex flex-col md:flex-row gap-6">
                <div className="md:w-1/2">
                  <img
                    src={selectedProgram.images}
                    alt={selectedProgram.name}
                    className="w-full h-48 sm:h-64 object-cover rounded-lg shadow-md"
                    onError={(e) => {
                      e.target.src = "../img/default_imgage.png";
                    }}
                  />
                </div>
                <div className="md:w-1/2 space-y-3 sm:space-y-4 text-gray-800">
                  <div>
                    <span className="font-semibold text-blue-800 block">Időpont:</span>
                    {new Date(selectedProgram.schedule).toLocaleString("hu-HU")}
                  </div>
                  <div>
                    <span className="font-semibold text-blue-800 block">Szervező:</span>
                    {selectedProgram.organizerName || "N/A"}
                  </div>
                  <div>
                    <span className="font-semibold text-blue-800 block">Elérhetőség:</span>
                    {selectedProgram.contactInfo || "N/A"}
                  </div>
                  <div>
                    <span className="font-semibold text-blue-800 block">Helyszín:</span>
                    {selectedProgram.location || "N/A"}
                  </div>
                  <div>
                    <span className="font-semibold text-blue-800 block">Férőhelyek:</span>
                    {selectedProgram.capacity || "N/A"}
                  </div>
                  <div>
                    <span className="font-semibold text-blue-800 block">Ár / Fő:</span>
                    {selectedProgram.price
                      ? `${selectedProgram.price.toLocaleString("hu-HU")} Ft`
                      : "Ingyenes"}
                  </div>
                </div>
              </div>
              
              <div className="mt-6">
                <span className="font-semibold text-blue-800 block mb-2">Leírás:</span>
                <p className="text-gray-700 bg-white p-3 rounded-lg">
                  {selectedProgram.description || "Nincs elérhető leírás."}
                </p>
              </div>
              
              <div className="mt-6">
                <label className="block text-sm font-semibold text-blue-800 mb-2">
                  Jegyek száma
                </label>
                <input
                  type="number"
                  min="1"
                  max={selectedProgram.capacity}
                  value={numberOfTickets}
                  onChange={(e) => {
                    const value = parseInt(e.target.value);
                    if (value > 0 && value <= selectedProgram.capacity) {
                      setNumberOfTickets(value);
                    }
                  }}
                  className="w-full p-2 border border-blue-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:outline-none"
                />
                <div className="text-sm text-gray-500 mt-1">
                  Maximálisan {selectedProgram.capacity} jegyet foglalhat
                </div>
              </div>
            </div>
            <div className="bg-blue-100 p-4 sm:p-6 flex flex-col sm:flex-row justify-end space-y-3 sm:space-y-0 sm:space-x-4">
              <button
                onClick={handleBooking}
                className="bg-blue-600 text-white px-4 py-2 rounded-lg font-semibold hover:bg-blue-800 transition-colors w-full sm:w-auto"
              >
                Foglalás
              </button>
              <button
                onClick={closeModal}
                className="bg-gray-600 text-white px-4 py-2 rounded-lg font-semibold hover:bg-gray-800 transition-colors w-full sm:w-auto"
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