import React, { useState, useEffect } from "react";
import { useLocation, useNavigate, useParams } from "react-router-dom";

export const Foglalas = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const { room } = location.state || {};
  const { id } = useParams();

  useEffect(() => {
    const token = localStorage.getItem('authToken');
    if (!token) {
      alert("Kérjük, jelentkezzen be a foglalás megkezdéséhez!");
      navigate("/"); // Átirányítás a bejelentkezés oldalra
    }
  }, [navigate]);

  const [additionalGuests, setAdditionalGuests] = useState(0);
  const [savedGuests, setSavedGuests] = useState([]);
  const [mainGuest, setMainGuest] = useState(null);
  const [amenities, setAmenities] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);


  const [checkInDate, setCheckInDate] = useState("");
  const [checkOutDate, setCheckOutDate] = useState("");
  const [paymentMethod, setPaymentMethod] = useState("");
  const [bookingError, setBookingError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);

  const handleBooking = async () => {
    setIsSubmitting(true);
    setBookingError("");
    
    try {
      // Alap validációk
      if (!checkInDate || !checkOutDate) {
        throw new Error("Kérjük töltsd ki mindkét dátum mezőt");
      }

      if (new Date(checkOutDate) <= new Date(checkInDate)) {
        throw new Error("A kijelentkezés dátuma a bejelentkezés után kell legyen");
      }

      if (!paymentMethod) {
        throw new Error("Válassz fizetési módot");
      }

      const totalGuests = additionalGuests + 1; // Fővendég + többi

      const bookingData = {
        GuestId: mainGuest,
        CheckInDate: new Date(checkInDate),
        CheckOutDate: new Date(checkOutDate),
        NumberOfGuests: totalGuests,
        PaymentMethod: paymentMethod
      };

      const token = localStorage.getItem('authToken');
      const response = await fetch(`https://localhost:7047/Bookings/New_Booking/${id}`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`
        },
        body: JSON.stringify(bookingData)
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || "Hiba a foglalás során");
      }

      // Sikeres foglalás
      navigate("/", {
        state: {
          success: true,
          message: "Foglalásodat rögzítettük! Hamarosan emailt kapsz visszaigazolással."
        }
      });

    } catch (error) {
      console.error("Foglalási hiba:", error);
      setBookingError(error.message);
    } finally {
      setIsSubmitting(false);
    }
  };

  // Dátum validációkhoz segédfüggvények
  const getMinCheckoutDate = () => {
    if (!checkInDate) return "";
    const minDate = new Date(checkInDate);
    minDate.setDate(minDate.getDate() + 2);
    return minDate.toISOString().split("T")[0];
  };







  // Vendégadatok lekérése
  useEffect(() => {
    const fetchGuests = async () => {
      const username = localStorage.getItem('username');
      const token = localStorage.getItem('authToken');
      
      try {
        const response = await fetch(
          `https://localhost:7047/Guests/GetGuestData/${username}`,
          {
            headers: { 'Authorization': `Bearer ${token}` },
          }
        );
        
        if (!response.ok) throw new Error('Nem sikerült lekérni a vendégeket');
        
        const data = await response.json();
        
        // Ha az adat nem tömb, akkor tömbbé alakítjuk
        const guestsArray = Array.isArray(data) ? data : [data];
        
        setSavedGuests(guestsArray); // Tömb beállítása
        if (guestsArray.length > 0) setMainGuest(guestsArray[0].guestId);
      } catch (error) {
        console.error('Hiba a vendégek lekérésekor:', error);
        setSavedGuests([]); // Üres tömb beállítása hiba esetén
      }
    };
  
    fetchGuests();
  }, []);

  // Kényelmi szolgáltatások
  useEffect(() => {
    const fetchAmenities = async () => {
      setLoading(true);
      try {
        const response = await fetch(`https://localhost:7047/Amenities/GetAmenitiesForRoom/${id}`);
        if (!response.ok) throw new Error("Hiba a kényelmi szolgáltatások lekérdezése során.");
        setAmenities(await response.json());
      } catch (error) {
        setError(error.message);
      } finally {
        setLoading(false);
      }
    };

    if (room) fetchAmenities();
  }, [room, id]);

  const calculateGuestType = (dateOfBirth) => {
    const birthDate = new Date(dateOfBirth);
    const age = new Date().getFullYear() - birthDate.getFullYear();
    
    if (age < 3) return '0-3';
    if (age < 18) return 'gyermekkoru';
    if (age < 26) return 'diak';
    if (age >= 65) return 'idoskoru';
    return 'felnott';
  };

  const calculateTotalPrice = () => {
    if (!room?.pricePerNight) return 0;
    
    const basePrice = room.pricePerNight;
    const selectedGuest = savedGuests.find(g => g.guestId === mainGuest);
    const guestType = selectedGuest ? calculateGuestType(selectedGuest.dateOfBirth) : 'felnott';
    
    const discounts = {
      '0-3': 0.95,
      'gyermekkoru': 0.8,
      'diak': 0.4,
      'idoskoru': 0.1,
      'felnott': 0
    };

    const mainGuestPrice = basePrice * (1 - (discounts[guestType] || 0));
    const additionalPrice = additionalGuests * basePrice;
    
    return mainGuestPrice + additionalPrice;
  };



  return (
 
      <div id="webcrumbs">
      <div className="w-full max-w-[1200px] mx-auto bg-gradient-to-b from-blue-50 to-blue-100 font-sans">
        <section className="py-6 md:py-10 px-4 md:px-8">
          <div className="flex flex-col lg:flex-row gap-6">
            {/* Bal oldali tartalom (2/3 széles) */}
            <div className="w-full lg:w-2/3">
              <h2 className="text-3xl md:text-4xl font-bold text-blue-800 mb-4 md:mb-6">
                {room.roomType || "Deluxe Panoráma Szoba"}
              </h2>

              {/* Szoba kép */}
              <div className="bg-white rounded-xl overflow-hidden shadow-lg mb-6 transform hover:scale-[1.01] transition-transform duration-300">
                <div className="relative h-[300px] md:h-[400px]">
                  <div className="absolute inset-0 flex">
                    <img
                      src={
                        room.images ||
                        "https://images.unsplash.com/photo-1566665797739-1674de7a421a?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=2574&q=80"
                      }
                      alt="Deluxe szoba"
                      className="w-full h-full object-cover"
                    />
                  </div>
                  <div className="absolute bottom-0 left-0 right-0 bg-gradient-to-t from-blue-900/80 to-transparent p-4">
                    <span className="text-white font-semibold text-lg">
                      Gyönyörű kilátás a városra
                    </span>
                  </div>
                </div>
              </div>

              {/* További képek (grid reszponzív) */}
              <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-4 mb-6">
                <img
                  src="https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1770&q=80"
                  alt="Fürdőszoba"
                  className="w-full h-48 object-cover rounded-lg shadow-md hover:shadow-xl transition-shadow duration-300"
                />
                <img
                  src="https://images.unsplash.com/photo-1618773928121-c32242e63f39?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1770&q=80"
                  alt="Ágy"
                  className="w-full h-48 object-cover rounded-lg shadow-md hover:shadow-xl transition-shadow duration-300"
                />
                <img
                  src="https://images.unsplash.com/photo-1540518614846-7eded433c457?ixlib=rb-4.0.3&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&w=1771&q=80"
                  alt="Kilátás"
                  className="w-full h-48 object-cover rounded-lg shadow-md hover:shadow-xl transition-shadow duration-300"
                />
              </div>

              {/* Szoba leírása */}
              <div className="bg-white rounded-xl p-4 md:p-6 shadow-lg mb-6">
                <h3 className="text-2xl font-bold text-blue-700 mb-4">
                  Szoba leírása
                </h3>
                <p className="text-gray-700 mb-4">
                  {room.description ||
                    "A 45 négyzetméteres Deluxe Panoráma Szobánk modern eleganciát kínál gyönyörű panorámával a városra. A kényelmet a luxus matracokkal felszerelt king-size ágy biztosítja, míg a tágas fürdőszoba esőzuhannyal és prémium piperecikkekkel várja."}
                </p>
                <div className="grid grid-cols-1 sm:grid-cols-2 gap-4 mt-6">
                  {loading && <p>Kényelmi szolgáltatások betöltése...</p>}
                  {error && <p className="text-red-500">{error}</p>}
                  {!loading && !error && amenities.length === 0 && (
                    <p>Nincsenek elérhető kényelmi szolgáltatások.</p>
                  )}
                  {!loading &&
                    !error &&
                    amenities.map((amenity, index) => (
                      <div key={index} className="flex items-center gap-2">
                        <span className="material-symbols-outlined text-blue-600">
                          {amenity.icon || "check"}
                        </span>
                        <span>{amenity.amenityName}</span>
                      </div>
                    ))}
                </div>
              </div>
            </div>

            {/* Jobb oldali űrlap (1/3 széles) */}
            <div className="w-full lg:w-1/3">
              {/* Alap foglaló (reszponzív dropdown) */}
              <div className="bg-white rounded-xl p-4 md:p-6 shadow-lg mb-6 border-t-4 border-blue-600">
                <h3 className="text-2xl font-bold text-blue-700 mb-4">Alap foglaló</h3>
                <div className="mb-4 relative">
                  <select
                    value={mainGuest || ""}
                    onChange={(e) => setMainGuest(Number(e.target.value))}
                    className="w-full p-2 md:p-3 lg:p-4 text-sm md:text-base lg:text-lg pl-4 pr-10 border border-gray-300 rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500 transition-all duration-200 appearance-none bg-white dark:bg-gray-800 dark:border-gray-700 dark:text-white cursor-pointer"
                  >
                    <option value="" disabled className="text-gray-400">
                      Válassz egy vendéget...
                    </option>
                    {savedGuests.map((guest) => (
                      <option
                        key={guest.guestId}
                        value={guest.guestId}
                        className="p-2 hover:bg-blue-50 rounded-lg transition-colors duration-200 dark:hover:bg-gray-700"
                      >
                        {guest.firstName} {guest.lastName}
                        {guest.dateOfBirth &&
                          ` (${new Date(guest.dateOfBirth).getFullYear()})`}
                      </option>
                    ))}
                    <option
                      value="new"
                      className="bg-green-50 text-green-800 hover:bg-green-100 p-2 rounded-lg transition-colors duration-200 dark:bg-green-800 dark:text-green-100 dark:hover:bg-green-700"
                    >
                      Új vendég hozzáadása
                    </option>
                  </select>
                  <div className="pointer-events-none absolute inset-y-0 right-0 flex items-center px-3 text-gray-500 dark:text-gray-400">
                    <svg
                      className="w-5 h-5"
                      fill="none"
                      stroke="currentColor"
                      viewBox="0 0 24 24"
                      xmlns="http://www.w3.org/2000/svg"
                    >
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth={2}
                        d="M19 9l-7 7-7-7"
                      />
                    </svg>
                  </div>
                </div>
              </div>

              {/* További vendégek (csak ha a szoba kapacitása > 1) */}
              {room.capacity > 1 && (
                <div className="bg-white rounded-xl p-4 md:p-6 shadow-lg mb-6 border-t-4 border-blue-600">
                  <h3 className="text-2xl font-bold text-blue-700 mb-4">
                    További vendégek
                  </h3>
                  <div className="mb-4">
                    <input
                      type="number"
                      min="0"
                      max={room.capacity - 1}
                      value={additionalGuests}
                      onChange={(e) =>
                        setAdditionalGuests(Math.min(e.target.value, room.capacity - 1))
                      }
                      className="w-full p-2 md:p-3 lg:p-4 text-sm md:text-base lg:text-lg border border-gray-300 rounded-lg shadow-sm focus:outline-none focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                    />
                    <p className="text-sm text-gray-500 mt-2">
                      Maximum {room.capacity - 1} fő
                    </p>
                  </div>
                </div>
              )}

<div className="bg-white rounded-xl p-4 md:p-6 shadow-lg border-t-4 border-blue-600 mb-6">
                <h3 className="text-2xl font-bold text-blue-700 mb-4">Időpontok</h3>
                
                <div className="space-y-4">
                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Bejelentkezés
                    </label>
                    <input
                      type="date"
                      min={new Date().toISOString().split("T")[0]}
                      value={checkInDate}
                      onChange={(e) => setCheckInDate(e.target.value)}
                      className="w-full p-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                    />
                  </div>

                  <div>
                    <label className="block text-sm font-medium text-gray-700 mb-1">
                      Kijelentkezés
                    </label>
                    <input
                      type="date"
                      min={getMinCheckoutDate()}
                      value={checkOutDate}
                      onChange={(e) => setCheckOutDate(e.target.value)}
                      className="w-full p-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                      disabled={!checkInDate}
                    />
                  </div>
                </div>
              </div>

              {/* Fizetési mód */}
              <div className="bg-white rounded-xl p-4 md:p-6 shadow-lg border-t-4 border-blue-600 mb-6">
                <h3 className="text-2xl font-bold text-blue-700 mb-4">Fizetési mód</h3>
                <select
                  value={paymentMethod}
                  onChange={(e) => setPaymentMethod(e.target.value)}
                  className="w-full p-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500"
                >
                  <option value="">Válassz fizetési módot</option>
                  <option value="Bankkártya">Bankkártya</option>
                  <option value="Fizetés érkezéskor">Fizetés érkezéskor</option>
                  <option value="Átutalás">Átutalás</option>
                </select>
              </div>

              {/* Összegzés és hibaüzenet */}
              <div className="bg-white rounded-xl p-4 md:p-6 shadow-lg border-t-4 border-blue-600 space-y-4 mb-6">
                {bookingError && (
                  <div className="p-3 bg-red-100 text-red-700 rounded-lg">
                    {bookingError}
                  </div>
                )}

                <div className="space-y-2">
                  <div className="flex justify-between">
                    <span className="text-gray-700">Alapár/éj:</span>
                    <span className="font-semibold">{room.pricePerNight} Ft</span>
                  </div>
                  <div className="flex justify-between">
                    <span className="text-gray-700">Éjszakák száma:</span>
                    <span className="font-semibold">
                      {checkInDate && checkOutDate ? 
                        Math.ceil(
                          (new Date(checkOutDate) - new Date(checkInDate)) / (1000 * 3600 * 24)
                        ) : 0}
                    </span>
                  </div>
                  <div className="flex justify-between text-lg font-bold">
                    <span>Összesen:</span>
                    <span className="text-blue-800">
                      {calculateTotalPrice()} Ft
                    </span>
                  </div>
                </div>

                <button
                  onClick={handleBooking}
                  disabled={isSubmitting}
                  className={`w-full py-3 rounded-lg font-bold transition-colors ${
                    isSubmitting 
                      ? "bg-gray-400 cursor-not-allowed" 
                      : "bg-blue-600 hover:bg-blue-700 text-white"
                  }`}
                >
                  {isSubmitting ? "Feldolgozás..." : "Foglalás megerősítése"}
                </button>
              </div>

              {/* További szolgáltatások */}
              <div className="bg-white rounded-xl p-4 md:p-6 shadow-lg">
                <h3 className="text-2xl font-bold text-blue-700 mb-4">
                  További szolgáltatások
                </h3>
                <ul className="space-y-3">
                  <li className="flex items-center gap-2">
                    <span className="material-symbols-outlined text-blue-600">
                      spa
                    </span>
                    <span>Wellness részleg használata</span>
                  </li>
                  <li className="flex items-center gap-2">
                    <span className="material-symbols-outlined text-blue-600">
                      fitness_center
                    </span>
                    <span>Fitness terem</span>
                  </li>
                  <li className="flex items-center gap-2">
                    <span className="material-symbols-outlined text-blue-600">
                      restaurant
                    </span>
                    <span>Gourmet étterem</span>
                  </li>
                  <li className="flex items-center gap-2">
                    <span className="material-symbols-outlined text-blue-600">
                      directions_car
                    </span>
                    <span>Parkolás</span>
                  </li>
                  <li className="flex items-center gap-2">
                    <span className="material-symbols-outlined text-blue-600">
                      iron
                    </span>
                    <span>Mosodai szolgáltatás</span>
                  </li>
                </ul>
              </div>
            </div>
          </div>
        </section>
      </div>
    </div>
  );
};

export default Foglalas;