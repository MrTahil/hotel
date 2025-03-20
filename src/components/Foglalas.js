import React, { useState, useEffect } from "react";
import { useLocation, useNavigate, useParams } from "react-router-dom";

export const Foglalas = () => {
  const location = useLocation();
  const navigate = useNavigate();
  const { room } = location.state || {};
  const { id } = useParams();

  const [guests, setGuests] = useState(1);
  const [additionalGuests, setAdditionalGuests] = useState(0);
  const [guestTypes, setGuestTypes] = useState([]);
  const [mainGuestType, setMainGuestType] = useState("felnott");
  const [amenities, setAmenities] = useState([]); // Kényelmi szolgáltatások
  const [loading, setLoading] = useState(false); // Betöltési állapot
  const [error, setError] = useState(null); // Hibaállapot

  

  useEffect(() => {
    const handleBeforeUnload = (e) => {
      e.preventDefault();
      e.returnValue = "";
      return "Biztosan elhagyja az oldalt? A foglalás nem lesz mentve.";
    };

    window.addEventListener("beforeunload", handleBeforeUnload);

    return () => {
      window.removeEventListener("beforeunload", handleBeforeUnload);
    };
  }, []);
 
  // Fetch a kényelmi szolgáltatásokhoz
  useEffect(() => {
    const fetchAmenities = async () => {
        setLoading(true);
        setError(null);
        try {
          console.log("Received room data:", room);
          const response = await fetch(
            `https://localhost:7047/Amenities/GetAmenitiesForRoom/${id}`
          );
          if (!response.ok) {
            throw new Error("Hiba a kényelmi szolgáltatások lekérdezése során.");
          }
          
          const data = await response.json();
          
          console.log("Lekért kényelmi szolgáltatások:", data); // Konzolos 
          setAmenities(data);
        } catch (error) {
          console.error("Hiba a fetch során:", error);
          setError(error.message);
        } finally {
          setLoading(false);
        }
      
    };
    
    fetchAmenities();
  }, [room]);

  if (!room) {
    return (
      <div className="p-6 text-center text-red-500">
        Nincs kiválasztott szoba a foglaláshoz.
      </div>
    );
  }

  const handleGuestTypeChange = (index, event) => {
    const newGuestTypes = [...guestTypes];
    newGuestTypes[index] = event.target.value;
    setGuestTypes(newGuestTypes);
  };

  const handleAdditionalGuestsChange = (event) => {
    const value = Math.min(event.target.value, room.capacity - guests);
    setAdditionalGuests(value);
    const newGuestTypes = new Array(value).fill("");
    setGuestTypes(newGuestTypes);
  };

  const calculateDiscount = (type) => {
    switch (type) {
      case "0-3":
        return 0.95;
      case "gyermekkoru":
        return 0.8;
      case "diak":
        return 0.4;
      case "idoskoru":
        return 0.1;
      default:
        return 0;
    }
  };

  const calculateTotalPrice = () => {
    const basePrice = room.pricePerNight || 36000;
    const mainGuestDiscount = calculateDiscount(mainGuestType);
    const mainGuestPrice = basePrice * (1 - mainGuestDiscount);

    const additionalGuestsPrice = guestTypes.reduce((acc, type) => {
      const discount = calculateDiscount(type);
      return acc + basePrice * (1 - discount);
    }, 0);

    return mainGuestPrice + additionalGuestsPrice;
  };

  const handleMainGuestTypeChange = (event) => {
    setMainGuestType(event.target.value);
  };

  const handleBooking = () => {
    const totalPrice = calculateTotalPrice();
    alert(`Foglalás sikeres! Összesen fizetendő: ${totalPrice} Ft`);
    navigate("/");
  };

  return (
    <div id="webcrumbs">
      <div className="w-[1200px] bg-gradient-to-b from-blue-50 to-blue-100 font-sans">
        <section className="py-10 px-8">
          <div className="flex gap-8">
            <div className="w-2/3">
              <h2 className="text-4xl font-bold text-blue-800 mb-6">
                {room.roomType || "Deluxe Panoráma Szoba"}
              </h2>
              <div className="bg-white rounded-xl overflow-hidden shadow-lg mb-8 transform hover:scale-[1.01] transition-transform duration-300">
                <div className="relative h-[400px]">
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
              <div className="grid grid-cols-3 gap-4 mb-8">
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
              <div className="bg-white rounded-xl p-6 shadow-lg mb-8">
                <h3 className="text-2xl font-bold text-blue-700 mb-4">
                  Szoba leírása
                </h3>
                <p className="text-gray-700 mb-4">
                  {room.description ||
                    "A 45 négyzetméteres Deluxe Panoráma Szobánk modern eleganciát kínál gyönyörű panorámával a városra. A kényelmet a luxus matracokkal felszerelt king-size ágy biztosítja, míg a tágas fürdőszoba esőzuhannyal és prémium piperecikkekkel várja."}
                </p>
                <div className="grid grid-cols-2 gap-4 mt-6">
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
          {amenity.icon || "check"} {/* Alapértelmezett ikon, ha nincs megadva */}
        </span>
        <span>{amenity.amenityName}</span> {/* Kényelmi szolgáltatás neve */}
      </div>
    ))}
</div>
              </div>
            </div>
            <div className="w-1/3">
              <div className="bg-white rounded-xl p-6 shadow-lg mb-6 border-t-4 border-blue-600">
                <h3 className="text-2xl font-bold text-blue-700 mb-4">
                  Alap foglaló kora
                </h3>
                <div className="mb-4">
                  <select
                    value={mainGuestType}
                    onChange={handleMainGuestTypeChange}
                    className="w-full p-2 border border-gray-300 rounded-lg"
                  >
                    <option value="0-3">0-3 éves</option>
                    <option value="gyermekkoru">Gyermekkorú</option>
                    <option value="diak">Diák</option>
                    <option value="felnott">Felnőtt</option>
                    <option value="idoskoru">Időskorú</option>
                  </select>
                </div>
              </div>

              {room.capacity > 1 && (
                <div className="bg-white rounded-xl p-6 shadow-lg mb-6 border-t-4 border-blue-600">
                  <h3 className="text-2xl font-bold text-blue-700 mb-4">
                    További fők hozzáadása
                  </h3>
                  <div className="mb-4">
                    <p className="text-gray-700">További fők</p>
                    <input
                      type="number"
                      min="0"
                      max={room.capacity - guests}
                      value={additionalGuests}
                      onChange={handleAdditionalGuestsChange}
                      className="w-full p-2 border border-gray-300 rounded-lg"
                    />
                    <p className="text-sm text-gray-500 mt-2">
                      Maximalizálva: {room.capacity - guests} fő
                    </p>
                  </div>
                  {Array.from({ length: additionalGuests }).map((_, index) => (
                    <div key={index} className="mb-4">
                      <p className="text-gray-700">Vendég {index + 1}</p>
                      <select
                        value={guestTypes[index] || ""}
                        onChange={(event) => handleGuestTypeChange(index, event)}
                        className="w-full p-2 border border-gray-300 rounded-lg"
                      >
                        <option value="">Válasszon vendég típust</option>
                        <option value="0-3">0-3 éves</option>
                        <option value="gyermekkoru">Gyermekkorú</option>
                        <option value="diak">Diák</option>
                        <option value="felnott">Felnőtt</option>
                        <option value="idoskoru">Időskorú</option>
                      </select>
                    </div>
                  ))}
                </div>
              )}

              <div className="bg-white rounded-xl p-6 shadow-lg mb-6 border-t-4 border-blue-600">
                <h3 className="text-2xl font-bold text-blue-700 mb-4">
                  Foglalási információk
                </h3>
                <div className="mb-4">
                  <p className="text-gray-700">Alapár</p>
                  <p className="text-2xl font-bold text-blue-800">
                    {room.pricePerNight
                      ? `${room.pricePerNight} Ft / éjszaka`
                      : "36.000 Ft / éjszaka"}
                  </p>
                </div>
                <div className="mb-4">
                  <p className="text-gray-700">Összesen fizetendő</p>
                  <p className="text-2xl font-bold text-blue-800">
                    {calculateTotalPrice()} Ft
                  </p>
                </div>
                <div className="mb-6">
                  <p className="text-sm text-gray-500">
                    Az ár tartalmazza az adókat és a reggeli ellátást.
                  </p>
                </div>
                <button
                  onClick={handleBooking}
                  className="w-full bg-blue-600 text-white py-3 rounded-lg font-bold hover:bg-blue-700 transform hover:scale-[1.02] transition-all duration-300 shadow-md hover:shadow-lg"
                >
                  Lefoglalom
                </button>
              </div>

              <div className="bg-white rounded-xl p-6 shadow-lg mb-6">
                <h3 className="text-2xl font-bold text-blue-700 mb-4">
                  Aktuális ajánlatok
                </h3>
                <details className="mb-4 group">
                  <summary className="flex justify-between items-center p-3 bg-blue-50 rounded-lg cursor-pointer hover:bg-blue-100 transition-colors duration-200">
                    <span className="font-semibold">Romantikus hétvége</span>
                    <span className="material-symbols-outlined transform group-open:rotate-180 transition-transform duration-300">
                      expand_more
                    </span>
                  </summary>
                  <div className="p-3 border-t border-blue-100">
                    <p className="text-gray-700 mb-2">
                      2 éjszaka áráért 3 éjszakát foglalhat hétvégén, pezsgővel
                      és romantikus vacsorával.
                    </p>
                    <p className="font-semibold text-blue-700">Kedvezmény: 33%</p>
                  </div>
                </details>
                <details className="mb-4 group">
                  <summary className="flex justify-between items-center p-3 bg-blue-50 rounded-lg cursor-pointer hover:bg-blue-100 transition-colors duration-200">
                    <span className="font-semibold">Hosszú tartózkodás</span>
                    <span className="material-symbols-outlined transform group-open:rotate-180 transition-transform duration-300">
                      expand_more
                    </span>
                  </summary>
                  <div className="p-3 border-t border-blue-100">
                    <p className="text-gray-700 mb-2">
                      5+ éjszaka foglalása esetén jelentős kedvezmény és ingyenes
                      transzfer a repülőtérről.
                    </p>
                    <p className="font-semibold text-blue-700">Kedvezmény: 20%</p>
                  </div>
                </details>
                <details className="group">
                  <summary className="flex justify-between items-center p-3 bg-blue-50 rounded-lg cursor-pointer hover:bg-blue-100 transition-colors duration-200">
                    <span className="font-semibold">Előfoglalási kedvezmény</span>
                    <span className="material-symbols-outlined transform group-open:rotate-180 transition-transform duration-300">
                      expand_more
                    </span>
                  </summary>
                  <div className="p-3 border-t border-blue-100">
                    <p className="text-gray-700 mb-2">
                      60 nappal érkezés előtti foglalás esetén extra kedvezmény és
                      ingyenes korai bejelentkezés.
                    </p>
                    <p className="font-semibold text-blue-700">Kedvezmény: 15%</p>
                  </div>
                </details>
              </div>

              <div className="bg-white rounded-xl p-6 shadow-lg">
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