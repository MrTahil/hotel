import React, { useState, useEffect } from 'react';

function RoomCard() {
  const [rooms, setRooms] = useState([]);

  useEffect(() => {
    const fetchRooms = async () => {
      try {
        const response = await fetch('https://localhost:7047/Rooms/GetRoomWith');
        const data = await response.json();
        console.log(data); // Ellenőrizd a szerver válaszát
        setRooms(data);
      } catch (error) {
        console.error('Error fetching rooms:', error);
      }
    };
    fetchRooms();
  }, []);

  // Szűrd le az elérhető szobákat (kis- és nagybetűérzékenység nélkül)
  const availableRooms = rooms.filter(room => room.status.toLowerCase() === 'available');

  return (
    <main className="bg-gradient-to-b from-blue-100 via-blue-50 to-white min-h-screen pt-8 md:pt-16">
      <section className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <form className="bg-white rounded-xl shadow-lg p-6 md:p-8 mb-8 md:mb-16">
          <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
            <div className="space-y-2">
              <label className="block text-sm font-medium text-gray-700">Bejelentkezés</label>
              <input type="date" className="w-full rounded-lg border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500" />
            </div>
            <div className="space-y-2">
              <label className="block text-sm font-medium text-gray-700">Kijelentkezés</label>
              <input type="date" className="w-full rounded-lg border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500" />
            </div>
            <div className="space-y-2">
              <label className="block text-sm font-medium text-gray-700">Vendégek száma</label>
              <select className="w-full rounded-lg border-gray-300 shadow-sm focus:border-blue-500 focus:ring-blue-500">
                <option>1 fő</option>
                <option>2 fő</option>
                <option>3 fő</option>
                <option>4 fő</option>
              </select>
            </div>
            <div className="flex items-end">
              <button className="w-full bg-blue-800 text-white px-6 py-3 rounded-lg font-semibold hover:bg-blue-700 transition-colors shadow-lg">
                Keresés
              </button>
            </div>
          </div>
        </form>

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-6">
          {availableRooms.length > 0 ? (
            availableRooms.map((room) => (
              <div key={room.room_id || room.room_number || Math.random()} className="bg-white rounded-xl shadow-lg overflow-hidden transform hover:scale-105 transition-all duration-300">
                <div className="h-48 bg-gray-200">
                  <img src={room.images || 'default-image.jpg'} alt={room.room_type || 'No image available'} className="w-full h-full object-cover" />
                </div>
                <div className="p-4">
                  <h3 className="text-xl font-bold text-blue-900 mb-2">{room.roomType || 'Not Available'}</h3>
                  <p className="text-blue-700 mb-4">{room.description || 'No description available'}</p>
                  <div className="space-y-1">
                    <p><strong>Szobaszám:</strong> {room.roomNumber || 'N/A'}</p>
                    <p><strong>Befogadó képesség:</strong> {room.capacity ? `${room.capacity} fő` : 'N/A'}</p>
                    <p><strong>Ár / Éj:</strong> {room.pricePerNight ? `${room.pricePerNight} Ft` : 'N/A'}</p>
                    <p><strong>Státusz:</strong> {room.status || 'No data'}</p>
                    <p><strong>Emelet:</strong> {room.floorNumber || 'N/A'}</p>
                    <p><strong>Kényelmi szolgáltatások:</strong> {room.amenities || 'No data'}</p>
                  </div>
                  <button className="w-full bg-blue-800 text-white px-4 py-2 rounded-lg font-semibold hover:bg-blue-700 transition-colors mt-4">
                    Részletek
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