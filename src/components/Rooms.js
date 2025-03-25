import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import LoginModal from './LoginModal';
import RegisterModal from './RegisterModal';

function RoomCard() {
  // States
  const [rooms, setRooms] = useState([]);
  const [filteredRooms, setFilteredRooms] = useState([]);
  const [guestCount, setGuestCount] = useState('Bármennyi');
  const [checkInDate, setCheckInDate] = useState('');
  const [checkOutDate, setCheckOutDate] = useState('');
  const [selectedRoomTypes, setSelectedRoomTypes] = useState([]);
  const [selectedAmenities, setSelectedAmenities] = useState([]);
  const [selectedFloors, setSelectedFloors] = useState([]);
  const [priceRange, setPriceRange] = useState([0, 500000]);
  const [allRoomTypes, setAllRoomTypes] = useState([]);
  const [allAmenities, setAllAmenities] = useState([]);
  const [allFloors, setAllFloors] = useState([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState(null);
  const [showFilters, setShowFilters] = useState(false);
  const [showLoginModal, setShowLoginModal] = useState(false);
  const [showRegisterModal, setShowRegisterModal] = useState(false);
  
  const navigate = useNavigate();

  // Load rooms
  useEffect(() => {
    const fetchRooms = async () => {
      setIsLoading(true);
      setError(null);
      try {
        const response = await fetch(`${process.env.REACT_APP_API_BASE_URL}${process.env.REACT_APP_API_ROOMS_URL}`);
        if (!response.ok) throw new Error(process.env.REACT_APP_ERROR_NETWORK);
        const data = await response.json();
        setRooms(data);
        setFilteredRooms(data);
        
        // Extract unique values
        const types = [...new Set(data.map(room => room.roomType))];
        const amenities = [...new Set(data.flatMap(room => room.amenities || []))];
        const floors = [...new Set(data.map(room => room.floorNumber))].sort((a, b) => a - b);
        setAllRoomTypes(types);
        setAllAmenities(amenities);
        setAllFloors(floors);
      } catch (error) {
        console.error('Failed to load rooms:', error);
        setError(error.message);
      } finally {
        setIsLoading(false);
      }
    };
    fetchRooms();
  }, []);

  // Date handling
  const getTodayDate = () => new Date().toISOString().split('T')[0];
  
  const getMinCheckOutDate = () => {
    if (!checkInDate) return getTodayDate();
    const checkIn = new Date(checkInDate);
    checkIn.setDate(checkIn.getDate() + 1); // Minimum 1 night stay
    return checkIn.toISOString().split('T')[0];
  };

  const handleCheckInChange = (e) => {
    const newDate = e.target.value;
    setCheckInDate(newDate);
    // Reset checkout date if it's before new checkin
    if (checkOutDate && newDate > checkOutDate) {
      setCheckOutDate('');
    }
  };

  const handleCheckOutChange = (e) => {
    setCheckOutDate(e.target.value);
  };

  // Filter handlers
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

  const handleFloorChange = (floor) => {
    setSelectedFloors(prev =>
      prev.includes(floor) ? prev.filter(f => f !== floor) : [...prev, floor]
    );
  };

  const handlePriceChange = (e, index) => {
    const value = parseInt(e.target.value) || 0;
    const newRange = [...priceRange];
    newRange[index] = value;
    setPriceRange(newRange.sort((a, b) => a - b));
  };

  // Apply filters
  const applyFilters = async () => {
    setIsLoading(true);
    setError(null);
    
    try {
      let result = [...rooms];

      // 1. Date-based filtering (if dates are provided)
      if (checkInDate && checkOutDate) {
        try {
          const response = await fetch(`${process.env.REACT_APP_API_BASE_URL}${process.env.REACT_APP_API_SEARCH_ROOMS}`, {
            method: 'POST',
            headers: {
              'Content-Type': 'application/json',
            },
            body: JSON.stringify({
              CheckInDate: checkInDate,
              CheckOutDate: checkOutDate,
              GuestNumber: guestCount === 'Bármennyi' ? 0 : parseInt(guestCount),
            }),
          });

          if (!response.ok) throw new Error('Error in date-based filtering');
          result = await response.json();
        } catch (err) {
          console.warn('API filtering failed, applying local filters', err);
        }
      }

      // 2. Local filters
      // Guest count
      if (guestCount !== 'Bármennyi') {
        const numGuests = parseInt(guestCount);
        result = result.filter(room => room.capacity >= numGuests);
      }

      // Room type
      if (selectedRoomTypes.length > 0) {
        result = result.filter(room => selectedRoomTypes.includes(room.roomType));
      }

      // Floor
      if (selectedFloors.length > 0) {
        result = result.filter(room => selectedFloors.includes(room.floorNumber));
      }

      // Amenities
      if (selectedAmenities.length > 0) {
        result = result.filter(room => 
          selectedAmenities.every(amenity => room.amenities?.includes(amenity))
        );
      }

      // Price range
      result = result.filter(room => 
        room.pricePerNight >= priceRange[0] && room.pricePerNight <= priceRange[1]
      );

      setFilteredRooms(result);
    } catch (error) {
      console.error('Filtering error:', error);
      setError(process.env.REACT_APP_ERROR_NETWORK);
    } finally {
      setIsLoading(false);
      setShowFilters(false);
    }
  };

  // Booking button
  const handleBookingClick = (roomId, room) => {
    if (!localStorage.getItem(process.env.REACT_APP_AUTH_TOKEN_KEY)) {
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

  // Reset filters
  const resetFilters = () => {
    setSelectedRoomTypes([]);
    setSelectedAmenities([]);
    setSelectedFloors([]);
    setPriceRange([0, 500000]);
    setGuestCount('Bármennyi');
    setFilteredRooms(rooms);
  };

  // Modal handlers
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
        {/* Header */}
        <div className="mb-6">
          <div className="relative inline-block mb-4">
            <h2 className="text-3xl font-bold text-gray-800 relative z-10">
              <span className="bg-gradient-to-r from-blue-600 to-indigo-600 bg-clip-text text-transparent">
                Szobák
              </span>
            </h2>
            <div className="absolute bottom-0 left-0 w-full h-2 bg-blue-100 rounded-full z-0"></div>
          </div>
          
          {/* Full-width date picker */}
          <div className="w-full bg-white rounded-xl shadow-md p-4 mb-6">
            <div className="grid grid-cols-1 md:grid-cols-3 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">{process.env.REACT_APP_DATE_LABEL} Érkezés</label>
                <input
                  type="date"
                  className="w-full px-4 py-2 rounded-lg border border-gray-300 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  value={checkInDate}
                  onChange={handleCheckInChange}
                  min={getTodayDate()}
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">{process.env.REACT_APP_DATE_LABEL} Távozás</label>
                <input
                  type="date"
                  className="w-full px-4 py-2 rounded-lg border border-gray-300 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  value={checkOutDate}
                  onChange={handleCheckOutChange}
                  min={getMinCheckOutDate()}
                  disabled={!checkInDate}
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Vendégek száma</label>
                <select
                  className="w-full px-4 py-2 rounded-lg border border-gray-300 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
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
          </div>

          {/* Filter button and active filters */}
          <div className="flex justify-between items-center">
            <button
              onClick={() => setShowFilters(!showFilters)}
              className="flex items-center gap-2 px-4 py-2 bg-white rounded-lg shadow-sm text-gray-700 hover:bg-gray-50 transition-colors duration-200 border border-gray-200"
            >
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 4v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z"/>
              </svg>
              Szűrők {showFilters ? 'bezárása' : 'megnyitása'}
            </button>

            {/* Active filters indicator */}
            {(selectedRoomTypes.length > 0 || selectedAmenities.length > 0 || selectedFloors.length > 0 || priceRange[0] > 0 || priceRange[1] < 500000) && (
              <div className="flex items-center gap-2 text-sm text-gray-600">
                <span>Aktív szűrők:</span>
                <div className="flex flex-wrap gap-2">
                  {selectedRoomTypes.map(type => (
                    <span key={type} className="bg-blue-100 text-blue-800 px-2 py-1 rounded-full text-xs">
                      {type}
                    </span>
                  ))}
                  {selectedFloors.map(floor => (
                    <span key={floor} className="bg-green-100 text-green-800 px-2 py-1 rounded-full text-xs">
                      {floor}. emelet
                    </span>
                  ))}
                  {priceRange[0] > 0 && (
                    <span className="bg-purple-100 text-purple-800 px-2 py-1 rounded-full text-xs">
                      Min {priceRange[0]} Ft
                    </span>
                  )}
                  {priceRange[1] < 500000 && (
                    <span className="bg-purple-100 text-purple-800 px-2 py-1 rounded-full text-xs">
                      Max {priceRange[1]} Ft
                    </span>
                  )}
                </div>
              </div>
            )}
          </div>
        </div>

        {/* Enhanced Filter Panel */}
        {showFilters && (
          <div className="bg-white rounded-2xl shadow-xl p-6 mb-8 border border-gray-200">
            <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
              {/* Room types - Enhanced */}
              <div className="space-y-4">
                <div className="flex items-center gap-3">
                  <div className="w-8 h-8 rounded-lg bg-blue-50 flex items-center justify-center">
                    <svg className="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6"/>
                    </svg>
                  </div>
                  <h3 className="font-semibold text-lg text-gray-800">Szobatípus</h3>
                </div>
                <div className="space-y-3 pl-11">
                  {allRoomTypes.map(type => (
                    <label key={type} className="flex items-center gap-3 p-2 hover:bg-gray-50 rounded-lg transition-colors cursor-pointer">
                      <input
                        type="checkbox"
                        checked={selectedRoomTypes.includes(type)}
                        onChange={() => handleRoomTypeChange(type)}
                        className="h-5 w-5 text-blue-600 rounded border-gray-300 focus:ring-blue-500"
                      />
                      <span className="text-gray-700">{type}</span>
                    </label>
                  ))}
                </div>
              </div>

              {/* Floors & Amenities - Side by side on larger screens */}
              <div className="grid grid-cols-1 sm:grid-cols-2 gap-8">
                {/* Floors - Enhanced */}
                <div className="space-y-4">
                  <div className="flex items-center gap-3">
                    <div className="w-8 h-8 rounded-lg bg-blue-50 flex items-center justify-center">
                      <svg className="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"/>
                      </svg>
                    </div>
                    <h3 className="font-semibold text-lg text-gray-800">Emelet</h3>
                  </div>
                  <div className="grid grid-cols-2 gap-3 pl-11">
                    {allFloors.map(floor => (
                      <label key={floor} className="flex items-center gap-3 p-2 hover:bg-gray-50 rounded-lg transition-colors cursor-pointer">
                        <input
                          type="checkbox"
                          checked={selectedFloors.includes(floor)}
                          onChange={() => handleFloorChange(floor)}
                          className="h-5 w-5 text-blue-600 rounded border-gray-300 focus:ring-blue-500"
                        />
                        <span className="text-gray-700">{floor}. emelet</span>
                      </label>
                    ))}
                  </div>
                </div>

                {/* Amenities - Enhanced */}
                <div className="space-y-4">
                  <div className="flex items-center gap-3">
                    <div className="w-8 h-8 rounded-lg bg-blue-50 flex items-center justify-center">
                      <svg className="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4"/>
                      </svg>
                    </div>
                    <h3 className="font-semibold text-lg text-gray-800">Kényelem</h3>
                  </div>
                  <div className="space-y-3 pl-11 max-h-48 overflow-y-auto pr-2 custom-scrollbar">
                    {allAmenities.map(amenity => (
                      <label key={amenity} className="flex items-center gap-3 p-2 hover:bg-gray-50 rounded-lg transition-colors cursor-pointer">
                        <input
                          type="checkbox"
                          checked={selectedAmenities.includes(amenity)}
                          onChange={() => handleAmenityChange(amenity)}
                          className="h-5 w-5 text-blue-600 rounded border-gray-300 focus:ring-blue-500"
                        />
                        <span className="text-gray-700 text-sm">{amenity}</span>
                      </label>
                    ))}
                  </div>
                </div>
              </div>

              {/* Price range - Enhanced */}
              <div className="space-y-4">
                <div className="flex items-center gap-3">
                  <div className="w-8 h-8 rounded-lg bg-blue-50 flex items-center justify-center">
                    <svg className="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
                    </svg>
                  </div>
                  <h3 className="font-semibold text-lg text-gray-800">Ár tartomány (Ft/éj)</h3>
                </div>
                <div className="space-y-6 pl-11">
                  <div className="flex items-center gap-4">
                    <div className="flex-1">
                      <label className="block text-sm text-gray-500 mb-1">Minimum</label>
                      <input
                        type="number"
                        value={priceRange[0]}
                        onChange={(e) => handlePriceChange(e, 0)}
                        className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-blue-500 focus:border-blue-500"
                        min="0"
                        max={priceRange[1]}
                        placeholder="0"
                      />
                    </div>
                    <div className="flex-1">
                      <label className="block text-sm text-gray-500 mb-1">Maximum</label>
                      <input
                        type="number"
                        value={priceRange[1]}
                        onChange={(e) => handlePriceChange(e, 1)}
                        className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-blue-500 focus:border-blue-500"
                        min={priceRange[0]}
                        max="500000"
                        placeholder="500000"
                      />
                    </div>
                  </div>
                  
                  {/* Enhanced Range Slider */}
                  <div className="px-2 pt-4">
                    <div className="relative h-4">
                      {/* Track */}
                      <div className="absolute top-1/2 left-0 right-0 h-1 bg-gray-200 rounded-full transform -translate-y-1/2"></div>
                      {/* Active range */}
                      <div 
                        className="absolute top-1/2 h-1 bg-blue-500 rounded-full transform -translate-y-1/2"
                        style={{
                          left: `${(priceRange[0] / 500000) * 100}%`,
                          right: `${100 - (priceRange[1] / 500000) * 100}%`
                        }}
                      ></div>
                      {/* Thumbs */}
                      <input
                        type="range"
                        min="0"
                        max="500000"
                        step="1000"
                        value={priceRange[0]}
                        onChange={(e) => handlePriceChange(e, 0)}
                        className="absolute top-1/2 left-0 w-full h-full opacity-0 cursor-pointer transform -translate-y-1/2 z-10"
                      />
                      <div 
                        className="absolute top-1/2 w-4 h-4 bg-white border-2 border-blue-500 rounded-full transform -translate-y-1/2 -translate-x-1/2 shadow cursor-pointer"
                        style={{ left: `${(priceRange[0] / 500000) * 100}%` }}
                      ></div>
                      
                      <input
                        type="range"
                        min="0"
                        max="500000"
                        step="1000"
                        value={priceRange[1]}
                        onChange={(e) => handlePriceChange(e, 1)}
                        className="absolute top-1/2 left-0 w-full h-full opacity-0 cursor-pointer transform -translate-y-1/2 z-10"
                      />
                      <div 
                        className="absolute top-1/2 w-4 h-4 bg-white border-2 border-blue-500 rounded-full transform -translate-y-1/2 -translate-x-1/2 shadow cursor-pointer"
                        style={{ left: `${(priceRange[1] / 500000) * 100}%` }}
                      ></div>
                    </div>
                    <div className="flex justify-between text-xs text-gray-500 mt-2">
                      <span>0 Ft</span>
                      <span>500 000 Ft</span>
                    </div>
                  </div>
                </div>
              </div>
            </div>

            {/* Action Buttons - Enhanced */}
            <div className="flex flex-col sm:flex-row justify-between items-center gap-4 mt-8 pt-6 border-t border-gray-200">
              <button
                onClick={resetFilters}
                className="px-6 py-3 text-gray-700 hover:text-gray-900 font-medium flex items-center gap-2 hover:bg-gray-100 rounded-lg transition-colors w-full sm:w-auto justify-center"
              >
                <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M4 4v5h.582m15.356 2A8.001 8.001 0 004.582 9m0 0H9m11 11v-5h-.581m0 0a8.003 8.003 0 01-15.357-2m15.357 2H15"/>
                </svg>
                Összes szűrő törlése
              </button>
              <div className="flex gap-3 w-full sm:w-auto">
                <button
                  onClick={() => setShowFilters(false)}
                  className="px-6 py-3 border border-gray-300 text-gray-700 rounded-lg hover:bg-gray-50 transition-colors font-medium w-full sm:w-auto"
                >
                  Mégse
                </button>
                <button
                  onClick={applyFilters}
                  disabled={isLoading}
                  className="px-8 py-3 bg-blue-600 text-white rounded-lg hover:bg-blue-700 disabled:bg-blue-400 transition-colors font-medium flex items-center justify-center gap-2 shadow-md hover:shadow-lg w-full sm:w-auto"
                >
                  {isLoading ? (
                    <>
                      <svg className="animate-spin h-5 w-5 text-white" xmlns="http://www.w3.org/2000/svg" fill="none" viewBox="0 0 24 24">
                        <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4"></circle>
                        <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z"></path>
                      </svg>
                      Folyamatban...
                    </>
                  ) : (
                    <>
                      Szűrés alkalmazása
                    </>
                  )}
                </button>
              </div>
            </div>
          </div>
        )}

        {/* Error message */}
        {error && (
          <div className="bg-red-50 border-l-4 border-red-500 text-red-700 p-4 mb-6 rounded-lg flex items-start">
            <svg className="w-5 h-5 mr-2 flex-shrink-0" fill="currentColor" viewBox="0 0 20 20">
              <path fillRule="evenodd" d="M10 18a8 8 0 100-16 8 8 0 000 16zM8.707 7.293a1 1 0 00-1.414 1.414L8.586 10l-1.293 1.293a1 1 0 101.414 1.414L10 11.414l1.293 1.293a1 1 0 001.414-1.414L11.414 10l1.293-1.293a1 1 0 00-1.414-1.414L10 8.586 8.707 7.293z" clipRule="evenodd" />
            </svg>
            <div>
              <h3 className="font-medium">Hiba történt</h3>
              <p className="text-sm">{error}</p>
            </div>
          </div>
        )}

        {/* Rooms display */}
        {isLoading ? (
          <div className="flex justify-center items-center py-24">
            <div className="animate-spin rounded-full h-12 w-12 border-t-2 border-b-2 border-blue-500"></div>
          </div>
        ) : (
          <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-6 pb-16">
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

      {/* Modals */}
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

// Room item component
function RoomItem({ room, onBookingClick }) {
  return (
    <div className="group bg-white rounded-xl shadow-md hover:shadow-lg overflow-hidden transform transition-all duration-300 ease-out hover:-translate-y-1 flex flex-col h-full">
      <div className="relative h-56 bg-gray-200 overflow-hidden flex-shrink-0">
        <img 
          src={room.images || process.env.REACT_APP_DEFAULT_ROOM_IMAGE} 
          className="w-full h-full object-cover transform group-hover:scale-105 transition-all duration-500" 
          alt={room.roomType}
          loading="lazy"
        />
        <div className="absolute inset-0 bg-gradient-to-t from-black/20 via-transparent to-transparent" />
        <span className="absolute top-4 right-4 bg-blue-800 text-white px-3 py-1 rounded-full text-sm font-semibold shadow-lg backdrop-blur-sm bg-opacity-90">
          {room.pricePerNight ? `${room.pricePerNight.toLocaleString('hu-HU')} ${process.env.REACT_APP_PRICE_FORMAT}` : 'Ár igénylés'}
        </span>
      </div>

      <div className="p-5 flex flex-col flex-grow">
        <h3 className="text-xl font-extrabold text-gray-900 mb-2">{room.roomType || process.env.REACT_APP_DEFAULT_ROOM_NAME}</h3>
        <p className="text-gray-600 line-clamp-2 mb-4">{room.description || process.env.REACT_APP_NO_DESCRIPTION_TEXT}</p>
        
        <RoomDetails room={room} />
        
        <div className="mt-auto pt-4">
          <button
            className="w-full bg-blue-600 text-white px-6 py-3 rounded-lg font-medium transition-colors flex items-center justify-center gap-2 group/button shadow-md hover:bg-blue-700"
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

// Room details component
function RoomDetails({ room }) {
  return (
    <div className="space-y-3 text-gray-600">
      <div className="flex items-center gap-3">
        <svg className="w-5 h-5 text-blue-600 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6"/>
        </svg>
        <span>Szoba #{room.roomNumber}</span>
      </div>
      
      <div className="flex items-center gap-3">
        <svg className="w-5 h-5 text-blue-600 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"/>
        </svg>
        <span>{room.capacity} fő</span>
      </div>

      <div className="flex items-center gap-3">
        <svg className="w-5 h-5 text-blue-600 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4"/>
        </svg>
        <span>{room.floorNumber}. emelet</span>
      </div>

      {room.amenities?.length > 0 && (
        <div className="pt-2">
          <div className="flex flex-wrap gap-2">
            {room.amenities.slice(0, 3).map(amenity => (
              <span key={amenity} className="text-xs bg-blue-100 text-blue-800 px-2 py-1 rounded-full">
                {amenity}
              </span>
            ))}
            {room.amenities.length > 3 && (
              <span className="text-xs bg-gray-100 text-gray-800 px-2 py-1 rounded-full">
                +{room.amenities.length - 3} további
              </span>
            )}
          </div>
        </div>
      )}
    </div>
  );
}

// No rooms available component
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