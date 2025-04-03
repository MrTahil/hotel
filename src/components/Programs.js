import React, { useState, useEffect } from "react";

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
      console.log("Booking payload:", payload, "Event ID:", selectedProgram.id);
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
        const response = await fetch(`${process.env.REACT_APP_API_BASE_URL}/Events/Geteents`);
        if (!response.ok) {
          throw new Error(`Hiba történt az adatok lekérésekor: ${response.status}`);
        }
        const data = await response.json();
        console.log("Raw API response:", data); // Debugging: Log raw response

        const formattedPrograms = data.map((event) => ({
          id: event.eventId ?? event.EventId,
          name: event.eventName ?? event.EventName ?? "Ismeretlen esemény",
          description: event.description ?? event.Description ?? "",
          images: event.images ?? event.Images ?? "../img/default_imgage.png",
          schedule: event.eventDate ?? event.EventDate ?? null,
          organizerName: event.organizerName ?? event.OrganizerName ?? "",
          contactInfo: event.contactInfo ?? event.ContactInfo ?? "",
          location: event.location ?? event.Location ?? "",
          capacity: event.capacity ?? event.Capacity ?? 0,
          price: event.price ?? event.Price ?? 0,
          leftToGet: event.leftToGet ?? (event.capacity - (event.eventbookings?.reduce((sum, booking) => sum + booking.numberOfTickets, 0) || 0))
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
    console.log("Selected program:", program);
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
      <div className="min-h-screen bg-gradient-to-br from-blue-100 to-indigo-200 flex items-center justify-center">
        <div className="text-center">
          <div className="w-20 h-20 border-8 border-indigo-500 border-t-transparent rounded-full animate-spin mx-auto"></div>
          <p className="mt-6 text-2xl font-semibold text-indigo-800 animate-pulse">Programok betöltése...</p>
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen bg-gradient-to-br from-blue-100 to-indigo-200 flex items-center justify-center px-4">
        <div className="bg-white p-8 rounded-2xl shadow-2xl max-w-lg w-full text-center transform transition-all hover:scale-105">
          <div className="text-red-600 mb-6">
            <svg
              xmlns="http://www.w3.org/2000/svg"
              className="h-16 w-16 mx-auto"
              fill="none"
              viewBox="0 0 24 24"
              stroke="currentColor"
            >
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth={2}
                d="M12 9v2m0 4h.01m-6.938 4h13.856c1.54 0 2.502-1.667 1.732-3L13.732 4c-.77-1.333-2.694-1.333-3.464 0L3.34 16c-.77 1.333.192 3 1.732 3z"
              />
            </svg>
          </div>
          <h2 className="text-3xl font-extrabold text-gray-900 mb-4">Hiba történt</h2>
          <p className="text-gray-700 mb-6 text-lg">{error}</p>
          <button
            onClick={() => window.location.reload()}
            className="bg-indigo-600 text-white px-6 py-3 rounded-full text-lg font-semibold hover:bg-indigo-700 transition-all duration-300 transform hover:scale-105"
          >
            Újrapróbálkozás
          </button>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gradient-to-br from-blue-100 to-indigo-200 py-8 px-4 sm:px-6 lg:px-8">
      <div className="max-w-7xl mx-auto">
        <h1 className="text-4xl sm:text-5xl font-extrabold text-indigo-800 text-center mb-10 animate-bounce tracking-tight">
          Programok
        </h1>

        {programs.length === 0 ? (
          <div className="text-center py-12">
            <p className="text-xl text-gray-700 font-medium">Jelenleg nincsenek elérhető programok.</p>
          </div>
        ) : (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-8">
            {programs.map((program) => (
              <div
                key={program.id}
                className="bg-white rounded-2xl shadow-xl overflow-hidden transform transition-all hover:scale-105 hover:shadow-2xl"
              >
                <div className="relative h-56 sm:h-64">
                  <img
                    src={program.images}
                    alt={program.name}
                    className="w-full h-full object-cover transition-transform duration-300 hover:scale-110"
                    onError={(e) => {
                      e.target.src = "../img/default_imgage.png";
                    }}
                  />
                  <div className="absolute top-3 right-3 bg-indigo-600 text-white text-sm font-bold px-3 py-1 rounded-full shadow-md">
                    {program.leftToGet} hely maradt
                  </div>
                </div>
                <div className="p-6">
                  <h2 className="text-2xl font-bold text-indigo-800 mb-3 line-clamp-2 leading-tight">
                    {program.name}
                  </h2>
                  <p className="text-gray-600 text-base mb-4 line-clamp-3">
                    {truncateDescription(program.description, 100)}
                  </p>
                  <div className="flex justify-between items-center">
                    <span className="text-indigo-600 font-extrabold text-lg">
                      {program.price ? `${program.price.toLocaleString("hu-HU")} Ft/fő` : "Ingyenes"}
                    </span>
                    <button
                      onClick={() => openModal(program)}
                      className="bg-indigo-600 text-white px-5 py-2 rounded-full font-semibold hover:bg-indigo-700 transition-all duration-300 transform hover:scale-105"
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
        <div className="fixed inset-0 bg-black bg-opacity-70 flex items-center justify-center p-4 z-50 overflow-y-auto">
          <div className="bg-white rounded-3xl shadow-2xl w-full max-w-5xl mx-2 sm:mx-4 max-h-[90vh] overflow-y-auto transform transition-all scale-95 animate-fade-in">
            <div className="bg-gradient-to-r from-indigo-600 to-blue-600 text-white p-6 flex justify-between items-center sticky top-0 z-10">
              <h2 className="text-2xl sm:text-3xl font-extrabold tracking-tight">{selectedProgram.name}</h2>
              <button
                onClick={closeModal}
                className="text-white hover:text-indigo-200 focus:outline-none transition-colors duration-200"
                aria-label="Bezárás"
              >
                <svg
                  className="w-8 h-8"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12" />
                </svg>
              </button>
            </div>
            <div className="p-6 bg-gradient-to-br from-blue-50 to-indigo-50">
              <div className="flex flex-col md:flex-row gap-8">
                <div className="md:w-1/2">
                  <img
                    src={selectedProgram.images}
                    alt={selectedProgram.name}
                    className="w-full h-64 sm:h-80 object-cover rounded-2xl shadow-lg transition-transform duration-300 hover:scale-105"
                    onError={(e) => {
                      e.target.src = "../img/default_imgage.png";
                    }}
                  />
                </div>
                <div className="md:w-1/2 space-y-5 text-gray-800">
                  <div>
                    <span className="font-bold text-indigo-700 text-lg block">Időpont:</span>
                    <p className="text-gray-700">
                      {selectedProgram.schedule
                        ? new Date(selectedProgram.schedule).toLocaleString("hu-HU")
                        : "N/A"}
                    </p>
                  </div>
                  <div>
                    <span className="font-bold text-indigo-700 text-lg block">Szervező:</span>
                    <p className="text-gray-700">{selectedProgram.organizerName || "N/A"}</p>
                  </div>
                  <div>
                    <span className="font-bold text-indigo-700 text-lg block">Elérhetőség:</span>
                    <p className="text-gray-700">{selectedProgram.contactInfo || "N/A"}</p>
                  </div>
                  <div>
                    <span className="font-bold text-indigo-700 text-lg block">Helyszín:</span>
                    <p className="text-gray-700">{selectedProgram.location || "N/A"}</p>
                  </div>
                  <div>
                    <span className="font-bold text-indigo-700 text-lg block">Férőhelyek:</span>
                    <p className="text-gray-700">{selectedProgram.capacity || "N/A"} (Még {selectedProgram.leftToGet} hely van)</p>
                  </div>
                  <div>
                    <span className="font-bold text-indigo-700 text-lg block">Ár / Fő:</span>
                    <p className="text-gray-700">
                      {selectedProgram.price
                        ? `${selectedProgram.price.toLocaleString("hu-HU")} Ft`
                        : "Ingyenes"}
                    </p>
                  </div>
                </div>
              </div>

              <div className="mt-8">
                <span className="font-bold text-indigo-700 text-lg block mb-3">Leírás:</span>
                <p className="text-gray-700 bg-white p-4 rounded-xl shadow-inner">
                  {selectedProgram.description || "Nincs elérhető leírás."}
                </p>
              </div>

              <div className="mt-8">
                <label className="block text-lg font-bold text-indigo-700 mb-3">Jegyek száma</label>
                <input
                  type="number"
                  min="1"
                  max={selectedProgram.leftToGet}
                  value={numberOfTickets}
                  onChange={(e) => {
                    const value = parseInt(e.target.value);
                    if (value > 0 && value <= selectedProgram.leftToGet) {
                      setNumberOfTickets(value);
                    }
                  }}
                  className="w-full p-3 border-2 border-indigo-300 rounded-xl focus:ring-4 focus:ring-indigo-400 focus:border-indigo-500 outline-none transition-all duration-200"
                />
                <div className="text-sm text-gray-600 mt-2">
                  Maximálisan {selectedProgram.leftToGet} jegyet foglalhat
                </div>
                {selectedProgram.price > 0 && (
                  <div className="mt-4 text-xl font-bold text-indigo-800">
                    Összesen: {(selectedProgram.price * numberOfTickets).toLocaleString("hu-HU")} Ft
                  </div>
                )}
              </div>
            </div>
            <div className="bg-indigo-100 p-6 flex flex-col sm:flex-row justify-end space-y-4 sm:space-y-0 sm:space-x-6">
              <button
                onClick={handleBooking}
                className="bg-indigo-600 text-white px-6 py-3 rounded-full font-bold text-lg hover:bg-indigo-700 transition-all duration-300 transform hover:scale-105 shadow-md"
              >
                Foglalás
              </button>
              <button
                onClick={closeModal}
                className="bg-gray-600 text-white px-6 py-3 rounded-full font-bold text-lg hover:bg-gray-700 transition-all duration-300 transform hover:scale-105 shadow-md"
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