import React, { useState, useEffect } from 'react';

function RoomCard() {
  const [rooms, setRooms] = useState([]);
  const [guestCount, setGuestCount] = useState('Bármennyi');
  const [filteredRooms, setFilteredRooms] = useState([]);

  useEffect(() => {
    const fetchRooms = async () => {
      try {
        const response = await fetch('https://localhost:7047/Rooms/GetRoomWith');
        const data = await response.json();
        console.log(data); // Ellenőrizd a szerver válaszát
        setRooms(data);
        setFilteredRooms(data.filter(room => room.status.toLowerCase() === 'available'));
      } catch (error) {
        console.error('Error fetching rooms:', error);
      }
    };
    fetchRooms();
  }, []);

  const handleSearch = () => {
    setFilteredRooms(
      guestCount === 'Bármennyi' 
        ? rooms.filter(room => room.status.toLowerCase() === 'available')
        : rooms.filter(room => room.status.toLowerCase() === 'available' && room.capacity === parseInt(guestCount))
    );
  };

  const getRoomImage = (roomType) => {
    switch (roomType.toLowerCase()) {
      case 'deluxe': return '../img/deluxe_room.png';
      case 'standard': return '../img/standard_room.png';
      case 'suite': return '../img/suite_room.png';
      case 'single': return '../img/single_room.png';
      case 'family': return '../img/family_room.png';
      case 'queen': return '../img/queen_room.png';
      case 'king': return '../img/king_room.png';
      default: return '../img/default_image.png';
    }
  };

  return (
    <main className="bg-gradient-to-b from-blue-100 via-blue-50 to-white min-h-screen pt-8 md:pt-16">
      <section className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <form className="bg-white rounded-xl shadow-lg p-6 md:p-8 mb-8 md:mb-16" onSubmit={(e) => e.preventDefault()}>
          <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
            <div className="space-y-2">
              <label className="block text-sm font-medium text-gray-700">Érkezés</label>
              <input type="date" className="w-full rounded-lg border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500" />
            </div>
            <div className="space-y-2">
              <label className="block text-sm font-medium text-gray-700">Távozás</label>
              <input type="date" className="w-full rounded-lg border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500" />
            </div>
            <div className="space-y-2">
              <label className="block text-sm font-medium text-gray-700">Vendégek száma</label>
              <select 
                className="w-full rounded-lg border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500" 
                value={guestCount} 
                onChange={(e) => setGuestCount(e.target.value)}
              >
                <option value="Bármennyi">Bármennyi</option>
                <option value="1">1 fő</option>
                <option value="2">2 fő</option>
                <option value="3">3 fő</option>
                <option value="4">4 fő</option>
                <option value="5">5 fő</option>
              </select>
            </div>
            <div className="flex items-end">
              <button 
                className="w-full bg-blue-800 text-white px-6 py-3 rounded-lg font-semibold hover:bg-blue-700 transition-colors shadow-lg"
                onClick={handleSearch}
              >
                Keresés
              </button>
            </div>
          </div>
        </form>

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
          {filteredRooms.length > 0 ? (
            filteredRooms.map((room) => (
              <div key={room.room_id || room.room_number || Math.random()} className="bg-white rounded-xl shadow-lg overflow-hidden transform hover:scale-105 transition-all duration-300">
                <div className="h-48 bg-gray-200">
                  <img src={getRoomImage(room.roomType)} alt={room.roomType || 'No image available'} className="w-full h-full object-cover" />
                </div>
                <div className="p-4">
                  <h3 className="text-xl font-bold text-blue-900 mb-2">{room.roomType || 'Not Available'}</h3>
                  <p className="text-blue-700 mb-4">{room.description || 'No description available'}</p>
                  <div className="space-y-1">
                    <p><strong>Szobaszám:</strong> {room.roomNumber || 'N/A'}</p>
                    <p><strong>Befogadó képesség:</strong> {room.capacity ? `${room.capacity} fő` : 'N/A'}</p>
                    <p><strong>Ár / Éj / Fő:</strong> {room.pricePerNight ? `${room.pricePerNight} Ft` : 'N/A'}</p>
                    <p><strong>Státusz:</strong> {room.status || 'No data'}</p>
                    <p><strong>Emelet:</strong> {room.floorNumber || 'N/A'}</p>
                    <p><strong>Kényelmi szolgáltatások:</strong> {room.amenities || 'No data'}</p>
                  </div>
                  <button className="w-full bg-blue-800 text-white px-4 py-2 rounded-lg font-semibold hover:bg-blue-700 transition-colors mt-4">
                    Foglalás
                  </button>
                </div>
              </div>
            ))
          ) : (
            <p className="text-center text-gray-600 col-span-full">Nincsenek elérhető szobák.</p>
          )}
        </div>
      </section>
    </main>
  );
}

export default RoomCard;