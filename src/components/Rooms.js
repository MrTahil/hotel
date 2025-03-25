import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import LoginModal from './LoginModal';
import RegisterModal from './RegisterModal';

function RoomCard() {
  // Állapotok
  const [rooms, setRooms] = useState([]);
  const [filteredRooms, setFilteredRooms] = useState([]);
  const [guestCount, setGuestCount] = useState('Bármennyi');
  const [checkInDate, setCheckInDate] = useState('');
  const [checkOutDate, setCheckOutDate] = useState('');
  const [selectedRoomTypes, setSelectedRoomTypes] = useState([]);
  const [selectedAmenities, setSelectedAmenities] = useState([]);
  const [priceRange, setPriceRange] = useState([0, 500000]);
  const [allRoomTypes, setAllRoomTypes] = useState([]);
  const [allAmenities, setAllAmenities] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);
  const [showFilters, setShowFilters] = useState(false);
  const [showLoginModal, setShowLoginModal] = useState(false);
  const [showRegisterModal, setShowRegisterModal] = useState(false);
  
  const navigate = useNavigate();

  // Szobák betöltése
  useEffect(() => {
    const fetchRooms = async () => {
      setIsLoading(true);
      setError(null);
      try {
        const response = await fetch('https://localhost:7047/Rooms/GetRoomWith');
        if (!response.ok) throw new Error('Hálózati hiba történt');
        const data = await response.json();
        setRooms(data);
        setFilteredRooms(data);
        
        // Szobatípusok és kényelmi szolgáltatások kinyerése
        const types = [...new Set(data.map(room => room.roomType))];
        const amenities = [...new Set(data.flatMap(room => room.amenities || []))];
        setAllRoomTypes(types);
        setAllAmenities(amenities);
      } catch (error) {
        console.error('Szobák betöltése sikertelen:', error);
        setError(error.message);
      } finally {
        setIsLoading(false);
      }
    };
    fetchRooms();
  }, []);

  // Dátumkezelés
  const getTodayDate = () => new Date().toISOString().split('T')[0];
  
  const getMinCheckOutDate = () => {
    if (!checkInDate) return getTodayDate();
    const checkIn = new Date(checkInDate);
    checkIn.setDate(checkIn.getDate() + 2);
    return checkIn.toISOString().split('T')[0];
  };

  const handleCheckInChange = (e) => {
    const newDate = e.target.value;
    setCheckInDate(newDate);
    // Ne módosítsuk automatikusan a távozási dátumot
  };

  const handleCheckOutChange = (e) => {
    setCheckOutDate(e.target.value);
  };

  // Szűrők kezelése
  const handleRoomTypeChange = (type) => {
    setSelectedRoomTypes(prev => 
      prev.includes(type) ? prev.filter(t => t !== type) : [...prev, type]
    );
  };

  const handleAmenityChange = (amenity) => {
    setSelectedAmenities(prev => 
      prev.includes(amenity) ? prev.filter(a => a !== amenity) : [...prev, amenity]
    );
  };

  const handlePriceChange = (e, index) => {
    const value = parseInt(e.target.value) || 0;
    const newRange = [...priceRange];
    newRange[index] = value;
    setPriceRange(newRange.sort((a, b) => a - b));
  };

  // Szűrés alkalmazása
  const applyFilters = async () => {
    setIsLoading(true);
    setError(null);
    
    try {
      let result = [...rooms];

      // 1. Dátum alapú szűrés (ha van megadva dátum)
      if (checkInDate && checkOutDate) {
        try {
          const response = await fetch('https://localhost:7047/Rooms/Searchwithparams', {
            method: 'POST', // PUT helyett POST
            headers: {
              'Content-Type': 'application/json',
            },
            body: JSON.stringify({
              CheckInDate: checkInDate,
              CheckOutDate: checkOutDate,
              GuestNumber: guestCount === 'Bármennyi' ? 0 : parseInt(guestCount),
            }),
          });

          if (!response.ok) throw new Error('Hiba a dátum alapú szűrésben');
          result = await response.json();
        } catch (err) {
          console.warn('API szűrés sikertelen, lokális szűrés alkalmazva', err);
          // Ha nem sikerül az API hívás, folytatjuk a lokális szűréssel
        }
      }

      // 2. Lokális szűrések
      // Vendégek száma
      if (guestCount !== 'Bármennyi') {
        const numGuests = parseInt(guestCount);
        result = result.filter(room => room.capacity >= numGuests);
      }

      // Szobatípus
      if (selectedRoomTypes.length > 0) {
        result = result.filter(room => selectedRoomTypes.includes(room.roomType));
      }

      // Kényelmi szolgáltatások
      if (selectedAmenities.length > 0) {
        result = result.filter(room => 
          selectedAmenities.every(amenity => room.amenities?.includes(amenity))
        );
      }

      // Ár tartomány
      result = result.filter(room => 
        room.pricePerNight >= priceRange[0] && room.pricePerNight <= priceRange[1]
      );

      setFilteredRooms(result);
    } catch (error) {
      console.error('Szűrési hiba:', error);
      setError('Hiba történt a szűrés során');
    } finally {
      setIsLoading(false);
      setShowFilters(false);
    }
  };

  // Foglalás gomb
  const handleBookingClick = (roomId, room) => {
    if (!localStorage.getItem('authToken')) {
      setShowLoginModal(true);
    } else {
      navigate(`/Foglalas/${roomId}`, { 
        state: { 
          room,
          checkInDate: checkInDate || '', 
          checkOutDate: checkOutDate || '' 
        } 
      });
    }
  };

  // Szűrők alaphelyzetbe
  const resetFilters = () => {
    setSelectedRoomTypes([]);
    setSelectedAmenities([]);
    setPriceRange([0, 500000]);
    setGuestCount('Bármennyi');
    setCheckInDate('');
    setCheckOutDate('');
    setFilteredRooms(rooms);
  };

  // Bejelentkezési modal kezelése
  const handleCloseLogin = () => setShowLoginModal(false);
  const handleCloseRegister = () => setShowRegisterModal(false);
  const switchToRegister = () => {
    setShowLoginModal(false);
    setShowRegisterModal(true);
  };
  const switchToLogin = () => {
    setShowRegisterModal(false);
    setShowLoginModal(true);
  };

  return (
    <main className="bg-gradient-to-b from-blue-50 to-indigo-50 min-h-screen pt-8 md:pt-16">
      <section className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        {/* Szűrő gomb */}
        <div className="flex justify-between items-center mb-6">
  <div className="relative inline-block">
    <h2 className="text-3xl font-bold text-gray-800 relative z-10">
      <span className="bg-gradient-to-r from-blue-600 to-indigo-600 bg-clip-text text-transparent">
        Szobák
      </span>
    </h2>
    <div className="absolute bottom-0 left-0 w-full h-2 bg-blue-100 rounded-full z-0"></div>
  </div>
  <button
    onClick={() => setShowFilters(!showFilters)}
    className="flex items-center gap-2 px-4 py-2 bg-white rounded-lg shadow-sm text-gray-700 hover:bg-gray-50 transition-colors duration-200"
  >
    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 4v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z"/>
    </svg>
    Szűrők
  </button>
</div>

        {/* Szűrő panel */}
        {showFilters && (
          <div className="bg-white rounded-2xl shadow-xl p-6 mb-8 ring-1 ring-gray-100/50">
            <div className="grid grid-cols-1 md:grid-cols-3 gap-6">
              {/* Dátum és vendégek */}
              <div className="space-y-4">
                <h3 className="font-semibold text-gray-700">Dátum és vendégek</h3>
                <div className="space-y-2">
                  <label className="block text-sm font-medium text-gray-600">Érkezés</label>
                  <input
                    type="date"
                    className="w-full px-4 py-2 rounded-lg border border-gray-300"
                    value={checkInDate}
                    onChange={handleCheckInChange}
                    min={getTodayDate()}
                  />
                </div>
                <div className="space-y-2">
                  <label className="block text-sm font-medium text-gray-600">Távozás</label>
                  <input
                    type="date"
                    className="w-full px-4 py-2 rounded-lg border border-gray-300"
                    value={checkOutDate}
                    onChange={handleCheckOutChange}
                    min={getMinCheckOutDate()}
                    disabled={!checkInDate}
                  />
                </div>
                <div className="space-y-2">
                  <label className="block text-sm font-medium text-gray-600">Vendégek száma</label>
                  <select
                    className="w-full px-4 py-2 rounded-lg border border-gray-300"
                    value={guestCount}
                    onChange={(e) => setGuestCount(e.target.value)}
                  >
                    <option value="Bármennyi">Bármennyi</option>
                    {[1, 2, 3, 4, 5, 6].map(num => (
                      <option key={num} value={num}>{num} fő</option>
                    ))}
                  </select>
                </div>
              </div>

              {/* Szobatípusok */}
              <div className="space-y-4">
                <h3 className="font-semibold text-gray-700">Szobatípus</h3>
                <div className="grid grid-cols-1 gap-2">
                  {allRoomTypes.map(type => (
                    <label key={type} className="flex items-center space-x-2">
                      <input
                        type="checkbox"
                        checked={selectedRoomTypes.includes(type)}
                        onChange={() => handleRoomTypeChange(type)}
                        className="rounded text-blue-600"
                      />
                      <span>{type}</span>
                    </label>
                  ))}
                </div>
              </div>

              {/* További szűrők */}
              <div className="space-y-4">
                <h3 className="font-semibold text-gray-700">Kényelmi szolgáltatások</h3>
                <div className="grid grid-cols-1 gap-2">
                  {allAmenities.map(amenity => (
                    <label key={amenity} className="flex items-center space-x-2">
                      <input
                        type="checkbox"
                        checked={selectedAmenities.includes(amenity)}
                        onChange={() => handleAmenityChange(amenity)}
                        className="rounded text-blue-600"
                      />
                      <span>{amenity}</span>
                    </label>
                  ))}
                </div>

                <div className="pt-4">
                  <h3 className="font-semibold text-gray-700 mb-2">Ár tartomány (Ft/éj)</h3>
                  <div className="flex items-center space-x-4">
                    <input
                      type="number"
                      value={priceRange[0]}
                      onChange={(e) => handlePriceChange(e, 0)}
                      className="w-24 px-3 py-2 border border-gray-300 rounded-lg"
                      min="0"
                      placeholder="Minimum"
                    />
                    <span>-</span>
                    <input
                      type="number"
                      value={priceRange[1]}
                      onChange={(e) => handlePriceChange(e, 1)}
                      className="w-24 px-3 py-2 border border-gray-300 rounded-lg"
                      min={priceRange[0]}
                      placeholder="Maximum"
                    />
                  </div>
                </div>
              </div>
            </div>

            <div className="flex justify-between mt-6">
              <button
                onClick={resetFilters}
                className="px-4 py-2 text-gray-600 hover:text-gray-800"
              >
                Szűrők törlése
              </button>
              <div className="space-x-3">
                <button
                  onClick={() => setShowFilters(false)}
                  className="px-4 py-2 border border-gray-300 rounded-lg hover:bg-gray-50"
                >
                  Mégse
                </button>
                <button
                  onClick={applyFilters}
                  disabled={isLoading}
                  className="px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:bg-blue-400"
                >
                  {isLoading ? 'Keresés...' : 'Szűrés alkalmazása'}
                </button>
              </div>
            </div>
          </div>
        )}

        {/* Hibaüzenet */}
        {error && (
          <div className="bg-red-100 border-l-4 border-red-500 text-red-700 p-4 mb-6 rounded">
            <p>{error}</p>
          </div>
        )}

        {/* Betöltés */}
        {isLoading ? (
          <div className="flex justify-center items-center py-24">
            <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-blue-500"></div>
          </div>
        ) : (
          /* Szobák megjelenítése */
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-8 pb-16">
            {filteredRooms.length > 0 ? (
              filteredRooms.map(room => (
                <RoomItem 
                  key={room.roomId} 
                  room={room} 
                  onBookingClick={handleBookingClick}
                />
              ))
            ) : (
              <NoRoomsAvailable />
            )}
          </div>
        )}
      </section>

      {/* Modális ablakok */}
      {showLoginModal && (
        <LoginModal 
          onClose={handleCloseLogin} 
          switchToRegister={switchToRegister} 
        />
      )}

      {showRegisterModal && (
        <RegisterModal 
          onClose={handleCloseRegister} 
          switchToLogin={switchToLogin} 
        />
      )}
    </main>
  );
}

// Segédkomponens egy szoba megjelenítéséhez
function RoomItem({ room, onBookingClick }) {
  return (
    <div className="group bg-white rounded-2xl shadow-xl hover:shadow-2xl overflow-hidden transform transition-all duration-300 ease-out hover:-translate-y-2 flex flex-col">
      <div className="relative h-56 bg-gray-200 overflow-hidden flex-shrink-0">
        <img 
          src={room.images || 'https://via.placeholder.com/300'} 
          className="w-full h-full object-cover transform group-hover:scale-105 transition-all duration-500" 
          alt={room.roomType}
        />
        <div className="absolute inset-0 bg-gradient-to-t from-black/20 via-transparent to-transparent" />
        <span className="absolute top-4 right-4 bg-blue-800 text-white px-3 py-1 rounded-full text-sm font-semibold shadow-lg backdrop-blur-sm bg-opacity-90">
          {room.pricePerNight ? `${room.pricePerNight.toLocaleString('hu-HU')} Ft` : 'Ár igénylés'}
        </span>
      </div>

      <div className="p-5 flex flex-col flex-grow">
        <h3 className="text-xl font-extrabold text-gray-900 mb-2">{room.roomType}</h3>
        <p className="text-gray-600 line-clamp-2 mb-4">{room.description || 'Nincs leírás megadva'}</p>
        
        <RoomDetails room={room} />
        
        {/* Javított foglalás gomb */}
        <div className="mt-auto pt-4">
          <button
            className="w-full bg-blue-700 text-white px-6 py-3 rounded-xl font-semibold transition-colors flex items-center justify-center gap-2 group/button shadow-lg hover:bg-blue-800"
            onClick={() => onBookingClick(room.roomId, room)}
          >
            Foglalás
            <svg className="w-4 h-4 transition-transform group-hover/button:translate-x-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M17 8l4 4m0 0l-4 4m4-4H3"/>
            </svg>
          </button>
        </div>
      </div>
    </div>
  );
}

// Szoba részletek
function RoomDetails({ room }) {
  return (
    <div className="space-y-2.5 text-gray-600">
      <div className="flex items-center gap-2">
        <svg className="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6"/>
        </svg>
        <span>Szoba #{room.roomNumber}</span>
      </div>
      
      <div className="flex items-center gap-2">
        <svg className="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"/>
        </svg>
        <span>{room.capacity} fő</span>
      </div>

      <div className="flex items-center gap-2">
        <svg className="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"/>
        </svg>
        <span>{room.floorNumber}. emelet</span>
      </div>

      {room.amenities?.length > 0 && (
        <div className="pt-2">
          <div className="flex flex-wrap gap-1">
            {room.amenities.slice(0, 3).map(amenity => (
              <span key={amenity} className="text-xs bg-blue-100 text-blue-800 px-2 py-1 rounded">
                {amenity}
              </span>
            ))}
            {room.amenities.length > 3 && (
              <span className="text-xs bg-gray-100 text-gray-800 px-2 py-1 rounded">
                +{room.amenities.length - 3} további
              </span>
            )}
          </div>
        </div>
      )}
    </div>
  );
}

// Nincs elérhető szoba üzenet
function NoRoomsAvailable() {
  return (
    <div className="col-span-full py-24 text-center">
      <div className="max-w-md mx-auto space-y-4">
        <svg className="w-16 h-16 mx-auto text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
        </svg>
        <h3 className="text-2xl font-bold text-gray-800">Nincsenek elérhető szobák</h3>
        <p className="text-gray-600">Próbáljon meg más szűrőfeltételeket megadni!</p>
      </div>
    </div>
  );
}

export default RoomCard;