import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import LoginModal from './LoginModal';
import RegisterModal from './RegisterModal';

function RoomCard() {
  const [rooms, setRooms] = useState([]);
  const [filteredRooms, setFilteredRooms] = useState([]);
  const [guestCount, setGuestCount] = useState('Bármennyi');
  const [dateRange, setDateRange] = useState({ startDate: '', endDate: '' });
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
  const [bookedDatesByRoom, setBookedDatesByRoom] = useState({});
  const [applyDateFilter, setApplyDateFilter] = useState(false);

  const navigate = useNavigate();

  useEffect(() => {
    const fetchRoomsAndBookings = async () => {
      setIsLoading(true);
      try {
        const roomsResponse = await fetch(`${process.env.REACT_APP_API_BASE_URL}${process.env.REACT_APP_API_ROOMS_URL}`);
        if (!roomsResponse.ok) throw new Error(process.env.REACT_APP_ERROR_NETWORK);
        const roomsData = await roomsResponse.text();

        const token = localStorage.getItem('authToken');
        const bookingsPromises = roomsData.map(async room => {
          try {
            const response = await fetch(
              `${process.env.REACT_APP_API_BASE_URL}/Bookings/GetBookedDates/${room.roomId}`,
              { headers: token ? { "Authorization": `Bearer ${token}` } : {} }
            );
            return response.ok ? await response.text() : [];
          } catch (err) {
            console.warn(`Failed to fetch booked dates for room ${room.roomId}:`, err.message);
            return [];
          }
        });

        const bookingsData = await Promise.all(bookingsPromises);
        const bookedDatesMap = bookingsData.reduce((acc, dates, index) => {
          acc[roomsData[index].roomId] = dates;
          return acc;
        }, {});

        setBookedDatesByRoom(bookedDatesMap);
        setRooms(roomsData);
        setFilteredRooms(roomsData);

        const types = [...new Set(roomsData.map(room => room.roomType))];
        const amenities = [...new Set(roomsData.flatMap(room => room.amenities || []))];
        const floors = [...new Set(roomsData.map(room => room.floorNumber))].sort((a, b) => a - b);

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

    fetchRoomsAndBookings();
  }, []);

  const hasDateConflict = (roomId, startDate, endDate) => {
    if (!startDate || !endDate) return false;

    const bookings = bookedDatesByRoom[roomId] || [];
    const newStart = new Date(startDate);
    const newEnd = new Date(endDate);

    return bookings.some(booking => {
      const existingStart = new Date(booking.checkInDate);
      const existingEnd = new Date(booking.checkOutDate);
      return newStart < existingEnd && newEnd > existingStart;
    });
  };

  const getTodayDate = () => new Date().toISOString().split('T')[0];

  const getMinEndDate = (start) => {
    if (!start) return getTodayDate();
    const minDate = new Date(start);
    minDate.setDate(minDate.getDate() + 1);
    return minDate.toISOString().split('T')[0];
  };

  const handleDateChange = (e) => {
    const { name, value } = e.target;
    setDateRange(prev => {
      const newRange = { ...prev, [name]: value };

      if (name === 'startDate' && value && prev.endDate && value >= prev.endDate) {
        const minEnd = new Date(value);
        minEnd.setDate(minEnd.getDate() + 1);
        newRange.endDate = minEnd.toISOString().split('T')[0];
      }

      return newRange;
    });
  };

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

  const handlePriceChange = (e) => {
    const value = e.target.value === '' ? '' : parseInt(e.target.value) || 0;
    const newMax = value === '' ? 500000 : Math.min(Math.max(value, 0), 500000);
    setPriceRange([0, newMax]);
  };

  const applyFilters = () => {
    setIsLoading(true);
    try {
      let filtered = [...rooms];

      if (applyDateFilter && dateRange.startDate && dateRange.endDate) {
        filtered = filtered.filter(room =>
          !hasDateConflict(room.roomId, dateRange.startDate, dateRange.endDate)
        );
      }

      filtered = applyNonDateFilters(filtered);
      setFilteredRooms(filtered);
    } catch (error) {
      console.error('Filtering error:', error);
      setError('Hiba történt a szűrés során');
    } finally {
      setIsLoading(false);
      setShowFilters(false);
    }
  };

  const applyNonDateFilters = (roomsToFilter) => {
    let filtered = [...roomsToFilter];

    if (guestCount !== 'Bármennyi') {
      const numGuests = parseInt(guestCount);
      filtered = filtered.filter(room => room.capacity >= numGuests);
    }

    if (selectedRoomTypes.length > 0) {
      filtered = filtered.filter(room => selectedRoomTypes.includes(room.roomType));
    }

    if (selectedFloors.length > 0) {
      filtered = filtered.filter(room => selectedFloors.includes(room.floorNumber));
    }

    if (selectedAmenities.length > 0) {
      filtered = filtered.filter(room =>
        selectedAmenities.every(amenity => room.amenities?.includes(amenity))
      );
    }

    filtered = filtered.filter(room => room.pricePerNight <= priceRange[1]);

    return filtered;
  };

  const handleBookingClick = (roomId, room) => {
    if (!localStorage.getItem('authToken')) {
      setShowLoginModal(true);
    } else {
      navigate(`/Foglalas/${roomId}`, {
        state: {
          room,
          checkInDate: dateRange.startDate || '',
          checkOutDate: dateRange.endDate || ''
        }
      });
    }
  };

  const resetFilters = () => {
    setSelectedRoomTypes([]);
    setSelectedAmenities([]);
    setSelectedFloors([]);
    setPriceRange([0, 500000]);
    setGuestCount('Bármennyi');
    setDateRange({ startDate: '', endDate: '' });
    setApplyDateFilter(false);
    setFilteredRooms(rooms);
  };

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
        <div className="mb-6">
          <div className="relative inline-block mb-4">
            <h2 className=" animate-bounce text-3xl font-bold text-gray-800 relative z-10">
              <span className="bg-gradient-to-r from-blue-600 to-indigo-600 bg-clip-text text-transparent">
                Szobák
              </span>
            </h2>
            <div className="absolute bottom-0 left-0 w-full h-2 bg-blue-100 rounded-full z-0"></div>
          </div>

          <div className="w-full bg-white rounded-xl shadow-md p-4 mb-6">
            <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Érkezés</label>
                <input
                  type="date"
                  name="startDate"
                  className="w-full px-4 py-2 rounded-lg border border-gray-300 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  value={dateRange.startDate}
                  onChange={handleDateChange}
                  min={getTodayDate()}
                />
              </div>
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">Távozás</label>
                <input
                  type="date"
                  name="endDate"
                  className="w-full px-4 py-2 rounded-lg border border-gray-300 focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  value={dateRange.endDate}
                  onChange={handleDateChange}
                  min={getMinEndDate(dateRange.startDate)}
                  disabled={!dateRange.startDate}
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
              <div className="flex items-end gap-2">
                <button
                  onClick={applyFilters}
                  className="w-full px-4 py-2 bg-blue-600 text-white rounded-lg hover:bg-blue-700 transition-colors flex items-center justify-center gap-2"
                >
                  <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z" />
                  </svg>
                  Keresés
                </button>
              </div>
            </div>

            <div className="mt-3 flex items-center">
              <input
                type="checkbox"
                id="applyDateFilter"
                checked={applyDateFilter}
                onChange={() => setApplyDateFilter(!applyDateFilter)}
                className="h-4 w-4 text-blue-600 rounded border-gray-300"
                disabled={!dateRange.startDate || !dateRange.endDate}
              />
              <label htmlFor="applyDateFilter" className="ml-2 text-sm text-gray-700">
                Szűrés a kiválasztott dátumokra
              </label>
            </div>
          </div>

          <div className="flex justify-between items-center">
            <button
              onClick={() => setShowFilters(!showFilters)}
              className="flex items-center gap-2 px-4 py-2 bg-white rounded-lg shadow-sm text-gray-700 hover:bg-gray-50 transition-colors duration-200 border border-gray-200"
            >
              <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M3 4a1 1 0 011-1h16a1 1 0 011 1v2.586a1 1 0 01-.293.707l-6.414 6.414a1 1 0 00-.293.707V17l-4 4v-6.586a1 1 0 00-.293-.707L3.293 7.293A1 1 0 013 6.586V4z" />
              </svg>
              Szűrők {showFilters ? 'bezárása' : 'megnyitása'}
            </button>

            {(selectedRoomTypes.length > 0 || selectedAmenities.length > 0 || selectedFloors.length > 0 || priceRange[1] < 500000 || applyDateFilter) && (
              <div className="mt-3 md:mt-0">
                <div className="text-sm text-gray-600 mb-1">Aktív szűrők:</div>
                <div className="flex flex-wrap gap-2 overflow-x-auto py-1 -mx-2 px-2">
                  {selectedRoomTypes.map(type => (
                    <span key={type} className="bg-blue-100 text-blue-800 px-2 py-1 rounded-full text-xs whitespace-nowrap">
                      {type}
                    </span>
                  ))}
                  {selectedFloors.map(floor => (
                    <span key={floor} className="bg-green-100 text-green-800 px-2 py-1 rounded-full text-xs whitespace-nowrap">
                      {floor}. emelet
                    </span>
                  ))}
                  {selectedAmenities.map(amenity => (
                    <span key={amenity} className="bg-yellow-100 text-yellow-800 px-2 py-1 rounded-full text-xs whitespace-nowrap">
                      {amenity}
                    </span>
                  ))}
                  {priceRange[1] < 500000 && (
                    <span className="bg-purple-100 text-purple-800 px-2 py-1 rounded-full text-xs whitespace-nowrap">
                      Max {priceRange[1].toLocaleString('hu-HU')} Ft
                    </span>
                  )}
                  {applyDateFilter && dateRange.startDate && dateRange.endDate && (
                    <span className="bg-orange-100 text-orange-800 px-2 py-1 rounded-full text-xs whitespace-nowrap">
                      {new Date(dateRange.startDate).toLocaleDateString('hu-HU')} - {new Date(dateRange.endDate).toLocaleDateString('hu-HU')}
                    </span>
                  )}
                </div>
              </div>
            )}
          </div>
        </div>

        {showFilters && (
          <div className="bg-white rounded-2xl shadow-xl p-6 mb-8 border border-gray-200 fixed md:relative inset-0 md:inset-auto z-50 md:z-auto overflow-y-auto">
            <div className="md:hidden sticky top-0 bg-white pb-4 border-b border-gray-200">
              <div className="flex justify-between items-center">
                <h2 className="text-xl font-bold text-gray-800">Szűrők</h2>
                <button
                  onClick={() => setShowFilters(false)}
                  className="p-2 text-gray-500 hover:text-gray-700"
                >
                  <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M6 18L18 6M6 6l12 12" />
                  </svg>
                </button>
              </div>
            </div>

            <div className="max-h-[80vh] md:max-h-none overflow-y-auto touch-pan-y">
              <div className="grid grid-cols-1 md:grid-cols-2 gap-6 pt-4 md:pt-0">
                <div className="space-y-3">
                  <h3 className="font-medium text-gray-800 flex items-center gap-2">
                    <svg className="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
                    </svg>
                    Szobatípus
                  </h3>
                  <div className="grid grid-cols-2 gap-2">
                    {allRoomTypes.map(type => (
                      <label key={type} className="flex items-center gap-2 text-sm">
                        <input
                          type="checkbox"
                          checked={selectedRoomTypes.includes(type)}
                          onChange={() => handleRoomTypeChange(type)}
                          className="h-4 w-4 text-blue-600 rounded border-gray-300"
                        />
                        {type}
                      </label>
                    ))}
                  </div>
                </div>

                <div className="space-y-3">
                  <h3 className="font-medium text-gray-800 flex items-center gap-2">
                    <svg className="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
                    </svg>
                    Emelet
                  </h3>
                  <div className="flex flex-wrap gap-2">
                    {allFloors.map(floor => (
                      <button
                        key={floor}
                        onClick={() => handleFloorChange(floor)}
                        className={`px-3 py-1 text-sm rounded-md ${selectedFloors.includes(floor)
                          ? 'bg-blue-100 text-blue-800 font-medium'
                          : 'bg-gray-100 text-gray-700 hover:bg-gray-200'
                          }`}
                      >
                        {floor}.
                      </button>
                    ))}
                  </div>
                </div>

                <div className="space-y-3">
                  <h3 className="font-medium text-gray-800 flex items-center gap-2">
                    <svg className="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M20 7l-8-4-8 4m16 0l-8 4m8-4v10l-8 4m0-10L4 7m8 4v10M4 7v10l8 4" />
                    </svg>
                    Kényelem
                  </h3>
                  <div className="grid grid-cols-2 gap-2 max-h-40 overflow-y-auto pr-2">
                    {allAmenities.map(amenity => (
                      <label key={amenity} className="flex items-center gap-2 text-sm">
                        <input
                          type="checkbox"
                          checked={selectedAmenities.includes(amenity)}
                          onChange={() => handleAmenityChange(amenity)}
                          className="h-4 w-4 text-blue-600 rounded border-gray-300"
                        />
                        <span className="truncate">{amenity}</span>
                      </label>
                    ))}
                  </div>
                </div>

                <div className="space-y-3">
                  <h3 className="font-medium text-gray-800 flex items-center gap-2">
                    <svg className="w-5 h-5 text-blue-600" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M12 8c-1.657 0-3 .895-3 2s1.343 2 3 2 3 .895 3 2-1.343 2-3 2m0-8c1.11 0 2.08.402 2.599 1M12 8V7m0 1v8m0 0v1m0-1c-1.11 0-2.08-.402-2.599-1M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
                    </svg>
                    Ár (Ft/éj)
                  </h3>
                  <div className="space-y-2">
                    <div className="flex gap-2">
                      <input
                        type="number"
                        value={priceRange[1] === 500000 ? '' : priceRange[1]}
                        onChange={handlePriceChange}
                        className="w-full px-3 py-2 border border-gray-300 rounded-md text-sm"
                        placeholder="Maximum"
                      />
                    </div>
                    <input
                      type="range"
                      min="0"
                      max="500000"
                      step="1000"
                      value={priceRange[1]}
                      onChange={handlePriceChange}
                      className="w-full"
                    />
                  </div>
                </div>
              </div>

              <div className="md:hidden sticky bottom-0 bg-white pt-4 border-t border-gray-200">
                <div className="flex flex-col sm:flex-row justify-between gap-3">
                  <button
                    onClick={resetFilters}
                    className="px-4 py-2 text-sm text-gray-700 font-medium flex items-center gap-2 hover:bg-gray-100 rounded-md"
                  >
                    Szűrők törlése
                  </button>
                  <div className="flex gap-2">
                    <button
                      onClick={() => setShowFilters(false)}
                      className="px-4 py-2 border border-gray-300 text-sm text-gray-700 rounded-md hover:bg-gray-50 flex-1"
                    >
                      Mégse
                    </button>
                    <button
                      onClick={applyFilters}
                      className="px-4 py-2 bg-blue-600 text-white text-sm rounded-md hover:bg-blue-700 flex-1"
                    >
                      Alkalmaz
                    </button>
                  </div>
                </div>
              </div>
            </div>

            <div className="hidden md:flex flex-col sm:flex-row justify-between gap-3 mt-6 pt-4 border-t border-gray-200">
              <button
                onClick={resetFilters}
                className="px-4 py-2 text-sm text-gray-700 font-medium flex items-center gap-2 hover:bg-gray-100 rounded-md"
              >
                Szűrők törlése
              </button>
              <div className="flex gap-2">
                <button
                  onClick={() => setShowFilters(false)}
                  className="px-4 py-2 border border-gray-300 text-sm text-gray-700 rounded-md hover:bg-gray-50"
                >
                  Mégse
                </button>
                <button
                  onClick={applyFilters}
                  className="px-4 py-2 bg-blue-600 text-white text-sm rounded-md hover:bg-blue-700"
                >
                  Alkalmaz
                </button>
              </div>
            </div>
          </div>
        )}

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
                  isAvailable={!applyDateFilter || !dateRange.startDate || !dateRange.endDate ||
                    !hasDateConflict(room.roomId, dateRange.startDate, dateRange.endDate)}
                  checkInDate={dateRange.startDate}
                  checkOutDate={dateRange.endDate}
                />
              ))
            ) : (
              <NoRoomsAvailable />
            )}
          </div>
        )}
      </section>

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

function RoomItem({ room, onBookingClick, isAvailable, checkInDate, checkOutDate }) {
  return (
    <div className={`group bg-white rounded-xl shadow-md hover:shadow-lg overflow-hidden transform transition-all duration-300 ease-out hover:-translate-y-1 flex flex-col h-full ${!isAvailable ? 'opacity-70' : ''
      }`}>
      <div className="relative h-56 bg-gray-200 overflow-hidden flex-shrink-0">
        <img
          src={room.images || process.env.REACT_APP_DEFAULT_ROOM_IMAGE}
          className="w-full h-full object-cover transform group-hover:scale-105 transition-all duration-500"
          alt={room.roomType}
          loading="lazy"
        />
        <div className="absolute inset-0 bg-gradient-to-t from-black/20 via-transparent to-transparent" />
        <span className="absolute top-4 right-4 bg-blue-800 text-white px-3 py-1 rounded-full text-sm font-semibold shadow-lg backdrop-blur-sm bg-opacity-90">
          {room.pricePerNight ? `${room.pricePerNight.toLocaleString('hu-HU')} Ft / Éjszaka` : 'Ár igénylés'}
        </span>
        {!isAvailable && checkInDate && checkOutDate && (
          <div className="absolute bottom-0 left-0 right-0 bg-red-600 text-white text-center py-1 text-sm font-medium">
            Foglalt: {new Date(checkInDate).toLocaleDateString('hu-HU')} - {new Date(checkOutDate).toLocaleDateString('hu-HU')}
          </div>
        )}
      </div>

      <div className="p-5 flex flex-col flex-grow">
        <h3 className="text-xl font-extrabold text-gray-900 mb-2">{room.roomType || 'Deluxe szoba'}</h3>
        <p className="text-gray-600 line-clamp-2 mb-4">{room.description || 'Nincs elérhető leírás'}</p>

        <RoomDetails room={room} />

        <div className="mt-auto pt-4">
          <button
            className={`w-full px-6 py-3 rounded-lg font-medium transition-colors flex items-center justify-center gap-2 group/button shadow-md ${isAvailable
              ? 'bg-blue-600 text-white hover:bg-blue-700'
              : 'bg-gray-400 text-gray-700 cursor-not-allowed'
              }`}
            onClick={() => isAvailable && onBookingClick(room.roomId, room)}
            disabled={!isAvailable}
          >
            {isAvailable ? (
              <>
                Foglalás
                <svg className="w-4 h-4 transition-transform group-hover/button:translate-x-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M17 8l4 4m0 0l-4 4m4-4H3" />
                </svg>
              </>
            ) : (
              'Foglalt'
            )}
          </button>
        </div>
      </div>
    </div>
  );
}

function RoomDetails({ room }) {
  return (
    <div className="space-y-3 text-gray-600">
      <div className="flex items-center gap-3">
        <svg className="w-5 h-5 text-blue-600 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M3 12l2-2m0 0l7-7 7 7M5 10v10a1 1 0 001 1h3m10-11l2 2m-2-2v10a1 1 0 01-1 1h-3m-6 0a1 1 0 001-1v-4a1 1 0 011-1h2a1 1 0 011 1v4a1 1 0 001 1m-6 0h6" />
        </svg>
        <span>Szoba #{room.roomNumber}</span>
      </div>

      <div className="flex items-center gap-3">
        <svg className="w-5 h-5 text-blue-600 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z" />
        </svg>
        <span>{room.capacity} fő</span>
      </div>

      <div className="flex items-center gap-3">
        <svg className="w-5 h-5 text-blue-600 flex-shrink-0" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
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

function NoRoomsAvailable() {
  return (
    <div className="col-span-full py-24 text-center">
      <div className="max-w-md mx-auto space-y-4">
        <svg className="w-16 h-16 mx-auto text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z" />
        </svg>
        <h3 className="text-2xl font-bold text-gray-800">Nincsenek elérhető szobák</h3>
        <p className="text-gray-600">Próbáljon meg más szűrőfeltételeket megadni!</p>
      </div>
    </div>
  );
}

export default RoomCard;