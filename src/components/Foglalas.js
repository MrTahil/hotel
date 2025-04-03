import React, { useState, useEffect } from "react";
import { useLocation, useNavigate, useParams } from "react-router-dom";

export const Foglalas = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const { room, checkInDate: initialCheckInDate, checkOutDate: initialCheckOutDate } = location.state || {};
  const { id } = useParams();

  const [additionalGuests, setAdditionalGuests] = useState(0);
  const [savedGuests, setSavedGuests] = useState([]);
  const [mainGuest, setMainGuest] = useState(null);
  const [amenities, setAmenities] = useState([]);
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState(null);
  const [checkInDate, setCheckInDate] = useState(initialCheckInDate || "");
  const [checkOutDate, setCheckOutDate] = useState(initialCheckOutDate || "");
  const [paymentMethod, setPaymentMethod] = useState("");
  const [bookingError, setBookingError] = useState("");
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [showGuestModal, setShowGuestModal] = useState(false);
  const [isRoomAvailable, setIsRoomAvailable] = useState(true);
  const [availabilityMessage, setAvailabilityMessage] = useState("");
  const [bookedDates, setBookedDates] = useState([]);
  const [guestData, setGuestData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phoneNumber: '',
    address: '',
    city: '',
    country: '',
    dateOfBirth: '',
    gender: '',
  });
  const [successMessage, setSuccessMessage] = useState("");

  useEffect(() => {
    const fetchBookedDates = async () => {
      try {
        const token = localStorage.getItem('authToken');
        const response = await fetch(
          `${process.env.REACT_APP_API_BASE_URL}/Bookings/GetBookedDates/${id}`,
          { headers: { "Authorization": `Bearer ${token}` } }
        );

        if (!response.ok) throw new Error("Hiba a foglalt dátumok lekérésekor");
        const data = await response.json();
        setBookedDates(data);
      } catch (error) {
        console.error("Hiba a foglalt dátumoknál:", error);
        setBookedDates([]);
      }
    };

    fetchBookedDates();
  }, [id]);

  useEffect(() => {
    const token = localStorage.getItem('authToken');
    if (!token) {
      alert("Kérjük, jelentkezzen be a foglalás megkezdéséhez!");
      navigate("/");
    }
  }, [navigate]);

  const fetchGuests = async () => {
    const username = localStorage.getItem('username');
    const token = localStorage.getItem('authToken');

    try {
      const response = await fetch(
        `${process.env.REACT_APP_API_BASE_URL}/Guests/GetGuestData/${username}`,
        { headers: { 'Authorization': `Bearer ${token}` } }
      );

      if (!response.ok) throw new Error('Nem sikerült lekérni a vendégeket');
      const data = await response.json();
      const guestsArray = Array.isArray(data) ? data : [data];

      setSavedGuests(guestsArray);
      if (guestsArray.length > 0 && !mainGuest) setMainGuest(guestsArray[0].guestId);
    } catch (error) {
      console.error('Hiba a vendégek lekérésekor:', error);
      setSavedGuests([]);
    }
  };

  useEffect(() => {
    fetchGuests();
  }, []);

  const checkRoomAvailability = () => {
    if (!checkInDate || !checkOutDate) return;

    const newCheckIn = new Date(checkInDate);
    const newCheckOut = new Date(checkOutDate);

    if (newCheckOut <= newCheckIn) {
      setIsRoomAvailable(false);
      setAvailabilityMessage("A kijelentkezés dátuma a bejelentkezés után kell legyen");
      return;
    }

    const hasConflict = bookedDates.some(booking => {
      const existingCheckIn = new Date(booking.checkInDate);
      const existingCheckOut = new Date(booking.checkOutDate);
      return newCheckIn < existingCheckOut && newCheckOut > existingCheckIn;
    });

    setIsRoomAvailable(!hasConflict);
    setAvailabilityMessage(
      !hasConflict
        ? "A szoba elérhető a megadott időpontban."
        : "Sajnos a szoba már foglalt erre az időpontra."
    );
  };

  useEffect(() => {
    checkRoomAvailability();
  }, [checkInDate, checkOutDate]);

  const handleBooking = async () => {
    setIsSubmitting(true);
    setBookingError("");

    try {
      if (!mainGuest) {
        throw new Error("Kérjük, válasszon ki egy fő foglalót, vagy adjon hozzá egy új vendéget a folytatáshoz.");
      }
      if (!checkInDate || !checkOutDate) {
        throw new Error("Kérjük, töltsd ki mindkét dátum mezőt!");
      }
      if (new Date(checkOutDate) <= new Date(checkInDate)) {
        throw new Error("A kijelentkezés dátuma a bejelentkezés után kell legyen!");
      }
      if (!paymentMethod) {
        throw new Error("Kérjük, válassz fizetési módot!");
      }
      if (!isRoomAvailable) {
        throw new Error("A szoba már foglalt erre az időpontra!");
      }

      const totalGuests = additionalGuests + 1;
      const bookingData = {
        GuestId: mainGuest,
        CheckInDate: new Date(checkInDate),
        CheckOutDate: new Date(checkOutDate),
        NumberOfGuests: totalGuests,
        PaymentMethod: paymentMethod,
      };

      const token = localStorage.getItem("authToken");
      const response = await fetch(`${process.env.REACT_APP_API_BASE_URL}/Bookings/New_Booking/${id}`, {
        method: "POST",
        headers: {
          "Content-Type": "application/json",
          "Authorization": `Bearer ${token}`,
        },
        body: JSON.stringify(bookingData),
      });

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || "Hiba történt a foglalás során.");
      }

      navigate("/", {
        state: {
          success: true,
          message: "Foglalásodat rögzítettük! Hamarosan emailt kapsz visszaigazolással.",
        },
      });
    } catch (error) {
      console.error("Foglalási hiba:", error);
      setBookingError(error.message);
    } finally {
      setIsSubmitting(false);
    }
  };

  const getMinCheckoutDate = () => {
    if (!checkInDate) return "";
    const minDate = new Date(checkInDate);
    minDate.setDate(minDate.getDate() + 2);
    return minDate.toISOString().split("T")[0];
  };

  useEffect(() => {
    const fetchAmenities = async () => {
      setLoading(true);
      try {
        const response = await fetch(`${process.env.REACT_APP_API_BASE_URL}/Amenities/GetAmenitiesForRoom/${id}`);
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

  const calculateTotalPrice = () => {
    if (!room?.pricePerNight || !checkInDate || !checkOutDate) return 0;

    const nights = Math.ceil((new Date(checkOutDate) - new Date(checkInDate)) / (1000 * 3600 * 24));
    return (additionalGuests + 1) * room.pricePerNight * nights;
  };

  const handleAddGuest = async (e) => {
    e.preventDefault();
    setError(null);

    try {
      const token = localStorage.getItem('authToken');
      const username = localStorage.getItem('username');

      if (!token) {
        throw new Error('Nincs token elmentve! Jelentkezz be újra.');
      }
      if (!username) {
        throw new Error('Nincs felhasználónév elmentve! Jelentkezz be újra.');
      }

      const userResponse = await fetch(
        `${process.env.REACT_APP_API_BASE_URL}/UserAccounts/GetOneUserData/${username}`,
        {
          headers: { 'Authorization': `Bearer ${token}` },
        }
      );

      if (!userResponse.ok) {
        const errorText = await userResponse.text();
        throw new Error(`Nem sikerült lekérni a felhasználói adatokat: ${errorText}`);
      }

      const userData = await userResponse.json();
      const userId = userData.userId;

      if (!userId) {
        throw new Error('Felhasználói azonosító nem található!');
      }

      if (!guestData.firstName || !guestData.lastName || !guestData.dateOfBirth) {
        throw new Error('A vezetéknév, keresztnév és születési dátum megadása kötelező!');
      }

      const payload = {
        ...guestData,
        userId: userId,
        dateOfBirth: guestData.dateOfBirth ? new Date(guestData.dateOfBirth).toISOString() : null,
      };

      const response = await fetch(`${process.env.REACT_APP_API_BASE_URL}/Guests/Addnewguest`, {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        const contentType = response.headers.get('Content-Type');
        let errorData;

        if (contentType && contentType.includes('application/json')) {
          errorData = await response.json();
          throw new Error(errorData.message || `Hiba a vendég hozzáadása során: ${response.status}`);
        } else {
          const errorText = await response.text();
          throw new Error(errorText || `Hiba a vendég hozzáadása során: ${response.status}`);
        }
      }

      const text = await response.text();
      let newGuest;

      const contentType = response.headers.get('Content-Type');
      if (contentType && contentType.includes('application/json') && text) {
        newGuest = JSON.parse(text);
      } else {
        newGuest = {
          guestId: Date.now(),
          ...guestData,
        };
      }

      const updatedGuests = [...savedGuests, newGuest];
      setSavedGuests(updatedGuests);
      setMainGuest(newGuest.guestId);
      setShowGuestModal(false);
      setGuestData({
        firstName: '',
        lastName: '',
        email: '',
        phoneNumber: '',
        address: '',
        city: '',
        country: '',
        dateOfBirth: '',
        gender: '',
      });
      setError(null);
      await fetchGuests();
      setSuccessMessage('Vendég sikeresen hozzáadva!');
      setTimeout(() => setSuccessMessage(''), 3000);
    } catch (err) {
      console.error('Hiba a vendég hozzáadása közben:', err.message);
      setError(err.message);
    }
  };

  return (
    <div id="webcrumbs">
      <div className="w-full max-w-[1200px] mx-auto bg-gradient-to-b from-blue-50 to-blue-100 font-sans">
        <section className="py-6 md:py-10 px-4 md:px-8 relative">
          {successMessage && (
            <div className="fixed top-4 right-4 z-50 bg-green-500 text-white px-4 py-2 rounded-lg shadow-lg animate-fade-in">
              {successMessage}
            </div>
          )}
          <div className="flex flex-col lg:flex-row gap-6">
            <div className="w-full lg:w-2/3">
              <h2 className="text-3xl md:text-4xl font-bold text-blue-800 mb-4 md:mb-6 flex items-center gap-4">
                <button
                  onClick={() => navigate("/szobak")}
                  className="p-2 rounded-full hover:bg-blue-100 transition-colors"
                  title="Vissza a szobákhoz"
                  aria-label="Vissza"
                >
                  <svg xmlns="http://www.w3.org/2000/svg" className="h-8 w-8 text-blue-600" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M10 19l-7-7m0 0l7-7m-7 7h18" />
                  </svg>
                </button>
                {room?.roomType || "Deluxe Panoráma Szoba"}
              </h2>

              <div className="bg-white rounded-xl overflow-hidden shadow-lg mb-6 transform hover:scale-[1.01] transition-transform duration-300">
                <div className="relative h-[300px] md:h-[400px]">
                  <div className="absolute inset-0 flex">
                    <img
                      src={
                        room?.images ||
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

              <div className="bg-white rounded-xl p-4 md:p-6 shadow-lg mb-6">
                <h3 className="text-2xl font-bold text-blue-700 mb-4">
                  Szoba leírása
                </h3>
                <p className="text-gray-700 mb-4">
                  {room?.description ||
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

              <div className="bg-white rounded-xl p-4 md:p-6 shadow-lg mb-6 border-t-4 border-teal-500">
                <h3 className="text-2xl font-bold text-teal-700 mb-4 flex items-center">
                  <span className="material-symbols-outlined mr-2">comment</span>
                  Vendégvélemények
                </h3>
                {room?.reviews && room.reviews.length > 0 ? (
                  <div className="space-y-6">
                    {room.reviews.map((review) => (
                      <div key={review.id} className="border-b border-gray-200 pb-4">
                        <div className="flex items-center justify-between mb-2">
                          <div className="text-yellow-400 flex">
                            {[...Array(5)].map((_, i) => (
                              <span
                                key={i}
                                className={i < review.rating ? "text-yellow-400" : "text-gray-300"}
                              >
                                ★
                              </span>
                            ))}
                          </div>
                          <span className="text-sm text-gray-500">
                            {new Date(review.reviewDate).toLocaleDateString('hu-HU')}
                          </span>
                        </div>
                        <p className="text-gray-700 mb-2">{review.comment}</p>
                        {review.response && (
                          <div className="bg-teal-50 p-3 rounded-lg mt-2">
                            <p className="text-sm font-medium text-teal-800">Válasz a szállásadótól:</p>
                            <p className="text-gray-700">{review.response}</p>
                          </div>
                        )}
                      </div>
                    ))}
                  </div>
                ) : (
                  <p className="text-gray-700 italic">Még nincsenek vélemények ehhez a szobához.</p>
                )}
              </div>
            </div>

            <div className="w-full lg:w-1/3">
              <div className="bg-white rounded-xl p-4 md:p-6 shadow-lg mb-6 border-t-4 border-blue-600">
                <h3 className="text-2xl font-bold text-blue-700 mb-4">Alap foglaló</h3>
                <div className="mb-4 relative">
                  <select
                    value={mainGuest || ""}
                    onChange={(e) => {
                      if (e.target.value === "new") {
                        setShowGuestModal(true);
                      } else {
                        setMainGuest(Number(e.target.value));
                      }
                    }}
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

              {room?.capacity > 1 && (
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
                  {availabilityMessage && (
                    <p className={`text-sm ${isRoomAvailable ? "text-green-600" : "text-red-600"}`}>
                      {availabilityMessage}
                    </p>
                  )}
                </div>
              </div>

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

              <div className="bg-white rounded-xl p-4 md:p-6 shadow-lg border-t-4 border-blue-600 space-y-4 mb-6">
                {bookingError && (
                  <div className="p-3 bg-red-100 text-red-700 rounded-lg">
                    {bookingError}
                  </div>
                )}
                <div className="space-y-2">
                  <div className="flex justify-between">
                    <span className="text-gray-700">Alapár / Éjszaka:</span>
                    <span className="font-semibold">{room?.pricePerNight} Ft</span>
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
                  disabled={isSubmitting || !isRoomAvailable}
                  className={`w-full py-3 rounded-lg font-bold transition-colors ${isSubmitting || !isRoomAvailable
                    ? "bg-gray-400 cursor-not-allowed"
                    : "bg-blue-600 hover:bg-blue-700 text-white"
                    }`}
                >
                  {isSubmitting ? "Feldolgozás..." : "Foglalás megerősítése"}
                </button>
              </div>

              <div className="bg-white rounded-xl p-4 md:p-6 shadow-lg">
                <h3 className="text-2xl font-bold text-blue-700 mb-4">
                  További szolgáltatások
                </h3>
                <ul className="space-y-3">
                  <li className="flex items-center gap-2">
                    <span className="material-symbols-outlined text-blue-600">spa</span>
                    <span>Wellness részleg használata</span>
                  </li>
                  <li className="flex items-center gap-2">
                    <span className="material-symbols-outlined text-blue-600">fitness_center</span>
                    <span>Fitness terem</span>
                  </li>
                  <li className="flex items-center gap-2">
                    <span className="material-symbols-outlined text-blue-600">restaurant</span>
                    <span>Gourmet étterem</span>
                  </li>
                  <li className="flex items-center gap-2">
                    <span className="material-symbols-outlined text-blue-600">directions_car</span>
                    <span>Parkolás</span>
                  </li>
                  <li className="flex items-center gap-2">
                    <span className="material-symbols-outlined text-blue-600">iron</span>
                    <span>Mosodai szolgáltatás</span>
                  </li>
                </ul>
              </div>
            </div>
          </div>
        </section>
      </div>

      {showGuestModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white p-4 sm:p-6 rounded-lg shadow-lg w-full max-w-lg sm:max-w-2xl">
            <h3 className="text-xl font-semibold text-blue-800 mb-4">
              Új vendég hozzáadása
            </h3>
            <form onSubmit={handleAddGuest} className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-blue-800">Vezetéknév</label>
                  <input
                    type="text"
                    value={guestData.lastName}
                    onChange={(e) => setGuestData({ ...guestData, lastName: e.target.value })}
                    className="w-full p-2 border border-blue-200 rounded-lg"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-blue-800">Keresztnév</label>
                  <input
                    type="text"
                    value={guestData.firstName}
                    onChange={(e) => setGuestData({ ...guestData, firstName: e.target.value })}
                    className="w-full p-2 border border-blue-200 rounded-lg"
                    required
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-blue-800">Email</label>
                  <input
                    type="email"
                    value={guestData.email}
                    onChange={(e) => setGuestData({ ...guestData, email: e.target.value })}
                    className="w-full p-2 border border-blue-200 rounded-lg"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-blue-800">Telefonszám</label>
                  <input
                    type="text"
                    value={guestData.phoneNumber}
                    onChange={(e) => setGuestData({ ...guestData, phoneNumber: e.target.value })}
                    className="w-full p-2 border border-blue-200 rounded-lg"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-blue-800">Születési dátum</label>
                  <input
                    type="date"
                    value={guestData.dateOfBirth}
                    onChange={(e) => setGuestData({ ...guestData, dateOfBirth: e.target.value })}
                    className="w-full p-2 border border-blue-200 rounded-lg"
                    required
                  />
                </div>
              </div>
              <div className="space-y-4">
                <div>
                  <label className="block text-sm font-medium text-blue-800">Cím</label>
                  <input
                    type="text"
                    value={guestData.address}
                    onChange={(e) => setGuestData({ ...guestData, address: e.target.value })}
                    className="w-full p-2 border border-blue-200 rounded-lg"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-blue-800">Város</label>
                  <input
                    type="text"
                    value={guestData.city}
                    onChange={(e) => setGuestData({ ...guestData, city: e.target.value })}
                    className="w-full p-2 border border-blue-200 rounded-lg"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-blue-800">Ország</label>
                  <input
                    type="text"
                    value={guestData.country}
                    onChange={(e) => setGuestData({ ...guestData, country: e.target.value })}
                    className="w-full p-2 border border-blue-200 rounded-lg"
                  />
                </div>
                <div>
                  <label className="block text-sm font-medium text-blue-800">Nem</label>
                  <select
                    value={guestData.gender}
                    onChange={(e) => setGuestData({ ...guestData, gender: e.target.value })}
                    className="w-full p-2 border border-blue-200 rounded-lg"
                  >
                    <option value="">Válasszon</option>
                    <option value="Férfi">Férfi</option>
                    <option value="Nő">Nő</option>
                    <option value="Egyéb">Egyéb</option>
                  </select>
                </div>
              </div>
              <div className="sm:col-span-2 flex gap-2 justify-end mt-4">
                <button
                  type="submit"
                  className="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transition-colors"
                >
                  Hozzáadás
                </button>
                <button
                  type="button"
                  onClick={() => {
                    setShowGuestModal(false);
                    setGuestData({
                      firstName: '',
                      lastName: '',
                      email: '',
                      phoneNumber: '',
                      address: '',
                      city: '',
                      country: '',
                      dateOfBirth: '',
                      gender: '',
                    });
                  }}
                  className="bg-gray-600 text-white px-4 py-2 rounded-lg hover:bg-gray-700 transition-colors"
                >
                  Mégse
                </button>
              </div>
            </form>
            {error && <p className="text-red-600 mt-2">{error}</p>}
          </div>
        </div>
      )}
    </div>
  );
};

export default Foglalas;