import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import LoginModal from './LoginModal';
import RegisterModal from './RegisterModal';

function RoomCard() {
  const [rooms, setRooms] = useState([]);
  const [guestCount, setGuestCount] = useState('Bármennyi');
  const [checkInDate, setCheckInDate] = useState('');
  const [checkOutDate, setCheckOutDate] = useState('');
  const [filteredRooms, setFilteredRooms] = useState([]);
  const [showLoginModal, setShowLoginModal] = useState(false);
  const [showRegisterModal, setShowRegisterModal] = useState(false);
  const navigate = useNavigate();

  useEffect(() => {
    const fetchRooms = async () => {
      try {
        const response = await fetch('https://localhost:7047/Rooms/GetRoomWith');
        const data = await response.json();
        console.log(data);
        setRooms(data);
        setFilteredRooms(data.filter(room => room.status.toLowerCase() !== null));
      } catch (error) {
        console.error('Error fetching rooms:', error);
      }
    };
    fetchRooms();
  }, []);

  const handleSearch = async () => {
    try {
      const response = await fetch('https://localhost:7047/Rooms/Searchwithparams', {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          CheckInDate: checkInDate,
          CheckOutDate: checkOutDate,
          GuestNumber: guestCount === 'Bármennyi' ? 0 : parseInt(guestCount),
        }),
      });
      const data = await response.json();
      setFilteredRooms(data);
    } catch (error) {
      console.error('Error searching rooms:', error);
    }
  };

  const handleBookingClick = (roomId, room) => {
    const token = localStorage.getItem('authToken');
    if (!token) {
      setShowLoginModal(true); // Login modál megjelenítése
    } else {
      // Átadjuk a checkInDate és checkOutDate értékeket a Foglalas oldalnak
      navigate(`/Foglalas/${roomId}`, { 
        state: { 
          room, 
          checkInDate, 
          checkOutDate 
        } 
      });
    }
  };

  const handleCloseLogin = () => {
    setShowLoginModal(false);
  };

  const handleCloseRegister = () => {
    setShowRegisterModal(false);
  };

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
        <form
          className="bg-white rounded-2xl shadow-xl p-6 md:p-8 mb-8 md:mb-16 ring-1 ring-gray-100/50 hover:ring-blue-200/50 transition-all duration-500"
          onSubmit={(e) => e.preventDefault()}
        >
          <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
            {[['Érkezés', checkInDate, setCheckInDate], ['Távozás', checkOutDate, setCheckOutDate]].map(([label, value, setter]) => (
              <div className="space-y-2 relative" key={label}>
                <label className="block text-sm font-medium text-gray-600">{label}</label>
                <div className="relative">
                  <input
                    type="date"
                    className="w-full pl-10 pr-4 py-3 rounded-xl border-0 bg-gray-50/70 shadow-sm text-gray-700 font-medium focus:ring-2 focus:ring-blue-500 focus:bg-white transition-all"
                    value={value}
                    onChange={(e) => setter(e.target.value)}
                  />
                  <svg className="w-5 h-5 absolute left-3 top-3.5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M8 7V3m8 4V3m-9 8h10M5 21h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v12a2 2 0 002 2z"/>
                  </svg>
                </div>
              </div>
            ))}

            <div className="space-y-2">
              <label className="block text-sm font-medium text-gray-600">Vendégek száma</label>
              <div className="relative">
                <select
                  className="w-full pl-10 pr-4 py-3 appearance-none rounded-xl border-0 bg-gray-50/70 shadow-sm text-gray-700 font-medium focus:ring-2 focus:ring-blue-500 focus:bg-white transition-all"
                  value={guestCount}
                  onChange={(e) => setGuestCount(e.target.value)}
                >
                  <option value="Bármennyi">Bármennyi</option>
                  <option value="1">1 fő</option>
                  <option value="2">2 fő</option>
                  <option value="3">3 fő</option>
                  <option value="4">4 fő</option>
                  <option value="5">5 fő</option>
                  <option value="6">6 fő</option>
                </select>
                <svg className="w-5 h-5 absolute left-3 top-3.5 text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M16 7a4 4 0 11-8 0 4 4 0 018 0zM12 14a7 7 0 00-7 7h14a7 7 0 00-7-7z"/>
                </svg>
              </div>
            </div>

            <div className="flex items-end">
              <button
                className="w-full bg-gradient-to-br from-blue-600 to-indigo-700 text-white px-6 py-4 rounded-xl font-bold hover:shadow-xl hover:scale-[1.02] active:scale-95 transition-all duration-200 shadow-lg flex items-center justify-center gap-2"
                onClick={handleSearch}
              >
                <svg className="w-5 h-5 text-white" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M21 21l-6-6m2-5a7 7 0 11-14 0 7 7 0 0114 0z"/>
                </svg>
                Keresés
              </button>
            </div>
          </div>
        </form>

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 xl:grid-cols-4 gap-8 pb-16">
          {filteredRooms.length > 0 ? (
            filteredRooms.map((room) => (
              <div 
                key={room.roomId} 
                className="group bg-white rounded-2xl shadow-xl hover:shadow-2xl overflow-hidden transform transition-all duration-300 ease-out hover:-translate-y-2 flex flex-col"
              >
                <div className="relative h-56 bg-gray-200 overflow-hidden flex-shrink-0">
                  <img 
                    src={room.images} 
                    className="w-full h-full object-cover transform group-hover:scale-105 transition-all duration-500" 
                    alt={room.roomType}
                  />
                  <div className="absolute inset-0 bg-gradient-to-t from-black/20 via-transparent to-transparent" />
                  <span className="absolute top-4 right-4 bg-blue-800/90 text-white px-3 py-1 rounded-full text-sm font-semibold shadow-sm">
                    {room.pricePerNight ? `${room.pricePerNight.toLocaleString()} Ft` : 'N/A'}
                  </span>
                </div>

                <div className="p-5 flex flex-col flex-grow relative pb-16">
                  <h3 className="text-xl font-extrabold text-gray-900 mb-2">{room.roomType}</h3>
                  <p className="text-gray-600 line-clamp-2 mb-4">{room.description}</p>
                  
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
                    <span>
                      <br></br>
                      <br></br>
                    </span>
                  </div>

                  <div className="absolute bottom-5 left-5 right-5">
                    <button
                      className="w-full bg-blue-700/90 text-white px-6 py-3 rounded-xl font-semibold bg-blue-800 transition-colors flex items-center justify-center gap-2 group/button shadow-lg"
                      onClick={() => handleBookingClick(room.roomId, room)}
                    >
                      Foglalás
                      <svg className="w-4 h-4 transition-transform group-hover/button:translate-x-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M17 8l4 4m0 0l-4 4m4-4H3"/>
                      </svg>
                    </button>
                  </div>
                </div>
              </div>
            ))
          ) : (
            <div className="col-span-full py-24 text-center">
              <div className="max-w-md mx-auto space-y-4">
                <svg className="w-16 h-16 mx-auto text-gray-400" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M9.172 16.172a4 4 0 015.656 0M9 10h.01M15 10h.01M21 12a9 9 0 11-18 0 9 9 0 0118 0z"/>
                </svg>
                <h3 className="text-2xl font-bold text-gray-800">Nincsenek elérhető szobák</h3>
                <p className="text-gray-600">Próbáljon meg más szűrőfeltételeket megadni!</p>
              </div>
            </div>
          )}
        </div>
      </section>

      {showLoginModal && (
        <LoginModal 
          onClose={handleCloseLogin} 
          switchToRegister={switchToRegister} 
          setUser={(username) => console.log('User set:', username)}
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

export default RoomCard;