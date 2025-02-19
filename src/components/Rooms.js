import React, { useState, useEffect } from 'react';
import '../styles/Rooms.css';

function RoomCard() {
  const [rooms, setRooms] = useState([]);

  useEffect(() => {
    const fetchRooms = async () => {
      try {
        const response = await fetch('https://localhost:7047/Rooms/GetRoomWith');
        const data = await response.json();
        console.log(response.json)
        setRooms(data);
      } catch (error) {
        console.error('Error fetching rooms:', error);
      }
    };
    fetchRooms();
  }, []);

  return (
    <div className="room-cards">
      {rooms.map((room) => (
        <div className="card" key={room.room_id || room.room_number || Math.random()}>
          <div className="card-image">
            <img src={room.images || 'default-image.jpg'} alt={room.room_type || 'No image available'} />
          </div>
          <div className="card-info">
            <h2>{room.roomType || 'Not Available'}</h2>
            <p><strong>Room Number:</strong> {room.roomNumber || 'N/A'}</p>
            <p><strong>Capacity:</strong> {room.capacity ? `${room.capacity} persons` : 'N/A'}</p>
            <p><strong>Price per Night:</strong> {room.pricePerNight ? `${room.pricePerNight} Ft` : 'N/A'}</p>
            <p><strong>Status:</strong> {room.status || 'No data'}</p>
            <p><strong>Floor:</strong> {room.floorNumber || 'N/A'}</p>
            <p><strong>Description:</strong> {room.description || 'No description available'}</p>
            <p><strong>Amenities:</strong> {room.amenities || 'No data'}</p>
          </div>
          <div className="card-footer">
          </div>
        </div>
      ))}
    </div>
  );
}

export default RoomCard;
