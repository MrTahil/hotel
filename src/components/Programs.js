import React, { useState, useEffect } from "react";
import axios from "axios";

function Programs() {
  const [programs, setPrograms] = useState([]);
  const [selectedProgram, setSelectedProgram] = useState(null);
  const [numberOfTickets, setNumberOfTickets] = useState(1);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState(null);
  const [bookingError, setBookingError] = useState(null);
  const [bookingSuccess, setBookingSuccess] = useState(null);

  const fetchGuestData = async () => {
    try {
      const token = localStorage.getItem("authToken");
      const username = localStorage.getItem("username");
      if (!token || !username) {
        throw new Error("Nincs token vagy felhasználónév elmentve! Jelentkezz be.");
      }
      const response = await axios.get(
        `${process.env.REACT_APP_API_BASE_URL}/Guests/GetGuestData/${username}`,
        {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );
      return response.data;
    } catch (err) {
      console.error("Hiba a vendég adatok lekérdezésekor:", err.message);
      return null;
    }
  };

  const handleBooking = async () => {
    try {
      setBookingError(null);
      setBookingSuccess(null);
      const token = localStorage.getItem("authToken");
      if (!token) {
        throw new Error("Nincs bejelentkezve, kérem jelentkezzen be vagy regisztráljon!");
      }
      const guestData = await fetchGuestData();
      if (!guestData || guestData.length === 0) {
        setBookingError("Nincs vendégadat a foglaláshoz! Kérlek, jelentkezz be vagy regisztrálj vendégként.");
        return;
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
      const response = await axios.post(
        `${process.env.REACT_APP_API_BASE_URL}/Feedback/NewEvenBooking/${selectedProgram.id}`,
        payload,
        {
          headers: {
            Authorization: `Bearer ${token}`,
            "Content-Type": "application/json",
          },
        }
      );
      setPrograms((prevPrograms) =>
        prevPrograms.map((program) =>
          program.id === selectedProgram.id
            ? { ...program, leftToGet: program.leftToGet - numberOfTickets }
            : program
        )
      );
      setSelectedProgram((prev) => ({
        ...prev,
        leftToGet: prev.leftToGet - numberOfTickets,
      }));
      setBookingSuccess("Foglalás sikeres! A modal hamarosan bezárul.");
      setTimeout(() => closeModal(), 2000);
    } catch (err) {
      console.error("Hiba a foglalás közben:", err.message);
      setBookingError(err.message);
    }
  };

  useEffect(() => {
    const fetchPrograms = async () => {
      try {
        setIsLoading(true);
        const response = await axios.get(`${process.env.REACT_APP_API_BASE_URL}/Events/Geteents`);
        const data = response.data;
        console.log("Raw API response:", data);

        const formattedPrograms = data.map((event) => ({
          id: event.eventId,
          name: event.eventName ?? "Ismeretlen esemény",
          description: event.description ?? "",
          images: event.images ?? "../img/default_imgage.png",
          schedule: event.eventDate ?? null,
          organizerName: event.organizerName ?? "Nincs megadva",
          contactInfo: event.contactInfo ?? "",
          location: event.location ?? "",
          capacity: event.capacity ?? 0,
          price: event.price ?? 0,
          leftToGet: event.lefttoGet ?? 0,
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
    setBookingError(null);
    setBookingSuccess(null);
  };

  const closeModal = () => {
    setSelectedProgram(null);
    setBookingError(null);
    setBookingSuccess(null);
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
    <div className="min-h-screen bg-gradient-to-br from-indigo-200 via-blue-100 to-purple-200 py-8 px-4 sm:px-6 lg:px-8 relative overflow-hidden">
      <div className="absolute inset-0 bg-[radial-gradient(ellipse_at_center,_var(--tw-gradient-stops))] from-indigo-300/20 via-transparent to-transparent pointer-events-none"></div>
      <div className="max-w-7xl mx-auto relative z-10">
        <h1 className="animate-bounce text-4xl sm:text-5xl font-extrabold text-indigo-600 text-center mb-12 drop-shadow-lg animate-fade-in py-2">
          Programok
        </h1>

        {programs.length === 0 ? (
          <div className="text-center py-12">
            <p className="text-xl text-gray-700 font-medium bg-white/80 p-4 rounded-lg shadow-md">Jelenleg nincsenek elérhető programok.</p>
          </div>
        ) : (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-8">
            {programs.map((program) => (
              <div
                key={program.id}
                className="bg-white rounded-xl shadow-lg overflow-hidden transform transition-all hover:-translate-y-2 hover:shadow-xl hover:rotate-1"
              >
                <div className="relative h-56 sm:h-64 group">
                  <img
                    src={program.images}
                    alt={program.name}
                    className="w-full h-full object-cover transition-transform duration-500 group-hover:scale-105"
                    onError={(e) => {
                      e.target.src = "../img/default_imgage.png";
                    }}
                  />
                  <div className="absolute inset-0 bg-indigo-500/10 opacity-0 group-hover:opacity-100 transition-opacity duration-300"></div>
                  <div className="absolute top-3 right-3 bg-indigo-500 text-white text-xs font-bold px-2 py-1 rounded-full shadow-md animate-pulse">
                    {program.leftToGet} hely
                  </div>
                </div>
                <div className="p-5 bg-gradient-to-b from-white to-indigo-50">
                  <h2 className="text-xl font-semibold text-indigo-700 mb-2 line-clamp-2 leading-tight">
                    {program.name}
                  </h2>
                  <p className="text-gray-600 text-sm mb-4 line-clamp-2">{truncateDescription(program.description, 80)}</p>
                  <div className="flex justify-between items-center">
                    <span className="text-indigo-600 font-bold text-lg">
                      {program.price ? `${program.price.toLocaleString("hu-HU")} Ft/fő` : "Ingyenes"}
                    </span>
                    <button
                      onClick={() => openModal(program)}
                      className="bg-gradient-to-r from-indigo-600 to-blue-500 text-white px-4 py-2 rounded-full font-medium hover:bg-gradient-to-l hover:from-blue-600 hover:to-indigo-700 transition-all duration-300 transform hover:scale-105"
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
        <div className="fixed inset-0 bg-black bg-opacity-60 flex items-center justify-center p-2 sm:p-4 z-50">
          <div className="bg-white rounded-xl shadow-2xl w-full max-w-lg flex flex-col h-auto max-h-[95vh] overflow-y-auto animate-[fadeIn_0.3s_ease-out]">
            <div className="bg-gradient-to-r from-indigo-600 to-blue-600 text-white p-3 flex justify-between items-center flex-shrink-0 rounded-t-xl">
              <h2 className="text-lg font-semibold truncate">{selectedProgram.name}</h2>
              <button
                onClick={closeModal}
                className="text-white hover:text-indigo-200 transition-colors duration-200"
                aria-label="Bezárás"
              >
                <svg
                  className="w-5 h-5"
                  fill="none"
                  stroke="currentColor"
                  viewBox="0 0 24 24"
                  xmlns="http://www.w3.org/2000/svg"
                >
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12" />
                </svg>
              </button>
            </div>

            <div className="flex flex-col p-3 bg-gradient-to-br from-indigo-50 to-blue-50">
              <div className="w-full flex-shrink-0 mb-3">
                <div className="relative w-full h-32 rounded-lg overflow-hidden shadow-md">
                  <img
                    src={selectedProgram.images}
                    alt={selectedProgram.name}
                    className="w-full h-full object-cover transition-transform duration-300 hover:scale-105"
                    onError={(e) => {
                      e.target.src = "../img/default_imgage.png";
                    }}
                  />
                </div>
              </div>

              <div className="space-y-2 text-sm text-gray-700">
                <div className="flex items-start">
                  <span className="font-semibold text-indigo-600 w-20 flex-shrink-0">Időpont:</span>
                  <p className="flex-1">
                    {selectedProgram.schedule
                      ? new Date(selectedProgram.schedule).toLocaleString("hu-HU")
                      : "N/A"}
                  </p>
                </div>
                <div className="flex items-start">
                  <span className="font-semibold text-indigo-600 w-20 flex-shrink-0">Szervező:</span>
                  <p className="flex-1">{selectedProgram.organizerName}</p>
                </div>
                <div className="flex items-start">
                  <span className="font-semibold text-indigo-600 w-20 flex-shrink-0">Helyszín:</span>
                  <p className="flex-1">{selectedProgram.location || "N/A"}</p>
                </div>
                <div className="flex items-start">
                  <span className="font-semibold text-indigo-600 w-20 flex-shrink-0">Férőhely:</span>
                  <p className="flex-1">
                    {selectedProgram.capacity || "N/A"} (Még {selectedProgram.leftToGet} hely)
                  </p>
                </div>
                <div className="flex items-start">
                  <span className="font-semibold text-indigo-600 w-20 flex-shrink-0">Ár / Fő:</span>
                  <p className="flex-1">
                    {selectedProgram.price
                      ? `${selectedProgram.price.toLocaleString("hu-HU")} Ft`
                      : "Ingyenes"}
                  </p>
                </div>
                <div className="flex items-start">
                  <span className="font-semibold text-indigo-600 w-20 flex-shrink-0">Leírás:</span>
                  <p className="flex-1 line-clamp-2">
                    {selectedProgram.description || "Nincs elérhető leírás."}
                  </p>
                </div>
              </div>

              {selectedProgram.leftToGet > 0 && !bookingSuccess ? (
                <div className="mt-3">
                  <label className="block text-sm font-semibold text-indigo-600 mb-1">Jegyek száma</label>
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
                    className="w-full p-2 border border-indigo-300 rounded-md focus:ring-2 focus:ring-indigo-400 focus:border-indigo-500 outline-none text-sm bg-white/80"
                  />
                  <p className="text-xs text-gray-500 mt-1">
                    Max: {selectedProgram.leftToGet} jegy
                  </p>
                  {selectedProgram.price > 0 && (
                    <p className="mt-2 text-sm font-semibold text-indigo-700">
                      Összesen: {(selectedProgram.price * numberOfTickets).toLocaleString("hu-HU")} Ft
                    </p>
                  )}
                </div>
              ) : selectedProgram.leftToGet === 0 ? (
                <div className="mt-3 text-center text-red-600 font-semibold">
                  Nincs több szabad hely erre a programra!
                </div>
              ) : null}

              {bookingError && (
                <div className="mt-3 bg-red-100 border border-red-400 text-red-700 px-4 py-2 rounded-md text-sm">
                  {bookingError}
                </div>
              )}

              {bookingSuccess && (
                <div className="mt-3 bg-green-100 border border-green-400 text-green-700 px-4 py-2 rounded-md text-sm flex items-center">
                  <svg
                    className="w-5 h-5 mr-2"
                    fill="none"
                    stroke="currentColor"
                    viewBox="0 0 24 24"
                    xmlns="http://www.w3.org/2000/svg"
                  >
                    <path
                      strokeLinecap="round"
                      strokeLinejoin="round"
                      strokeWidth="2"
                      d="M5 13l4 4L19 7"
                    />
                  </svg>
                  {bookingSuccess}
                </div>
              )}
            </div>

            <div className="bg-indigo-100 p-3 flex flex-col sm:flex-row justify-end gap-2 flex-shrink-0 rounded-b-xl">
              <button
                onClick={handleBooking}
                className={`bg-gradient-to-r from-indigo-600 to-blue-500 text-white px-3 py-1 rounded-md font-semibold transition-all duration-300 text-sm shadow-md hover:shadow-lg ${
                  selectedProgram.leftToGet === 0 || bookingSuccess
                    ? "opacity-50 cursor-not-allowed"
                    : "hover:from-blue-600 hover:to-indigo-700"
                }`}
                disabled={selectedProgram.leftToGet === 0 || bookingSuccess}
              >
                Foglalás
              </button>
              <button
                onClick={closeModal}
                className="bg-gradient-to-r from-gray-500 to-gray-600 text-white px-3 py-1 rounded-md font-semibold hover:from-gray-600 hover:to-gray-700 transition-all duration-300 text-sm shadow-md hover:shadow-lg"
              >
                Bezárás
              </button>
            </div>
          </div>
        </div>
      )}

      <style>
        {`
          @keyframes fadeIn {
            from {
              opacity: 0;
              transform: scale(0.95);
            }
            to {
              opacity: 1;
              transform: scale(1);
            }
          }
        `}
      </style>
    </div>
  );
}

export default Programs;