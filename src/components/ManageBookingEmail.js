import React, { useState, useEffect } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { jwtDecode } from 'jwt-decode';

export default function ManageBookingEmail() {
  const { bookingId } = useParams();
  const navigate = useNavigate();
  const [booking, setBooking] = useState(null);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [showDeleteModal, setShowDeleteModal] = useState(false);

  useEffect(() => {
    const fetchBookingData = async () => {
      try {
        const token = localStorage.getItem('authToken');
        if (!token) {
          throw new Error('Nincs bejelentkezve! Kérjük, jelentkezzen be.');
        }

        const decodedToken = jwtDecode(token);
        const userId = decodedToken.sub;

        const response = await fetch(
          `${process.env.REACT_APP_API_BASE_URL}/Bookings/GetEmailBooking/${bookingId}`,
          {
            method: 'POST',
            headers: {
              Authorization: `Bearer ${token}`,
              'Content-Type': 'application/json',
            },
            body: JSON.stringify({ userId: parseInt(userId) }),
          }
        );

        if (!response.ok) {
          throw new Error('Nem sikerült betölteni a foglalás adatait');
        }

        const data = await response.json();
        setBooking(data[0]);
      } catch (err) {
        setError(err.message);
      } finally {
        setLoading(false);
      }
    };

    fetchBookingData();
  }, [bookingId]);

  const handleDelete = async () => {
    try {
      const token = localStorage.getItem('authToken');
      const response = await fetch(
        `${process.env.REACT_APP_API_BASE_URL}/Bookings/DeleteBooking/${bookingId}`,
        {
          method: 'DELETE',
          headers: {
            Authorization: `Bearer ${token}`,
          },
        }
      );

      if (response.ok) {
        window.location.href = 'https://hmzrt.eu';
      } else {
        throw new Error('Nem sikerült törölni a foglalást');
      }
    } catch (err) {
      setError(err.message);
    }
  };

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-blue-600">Betöltés...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center">
        <div className="text-red-600">Hiba: {error}</div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 p-6">
      <div className="max-w-4xl mx-auto bg-white rounded-xl shadow-lg p-8">
        <div className="flex justify-between items-center mb-8">
          <h1 className="text-3xl font-bold text-gray-900">Foglalás részletei</h1>
          <div className="flex gap-4">
            <button
              onClick={() => (window.location.href = 'https://hmzrt.eu')}
              className="bg-blue-600 text-white px-6 py-2 rounded-lg hover:bg-blue-700 
              transition-colors"
            >
              Vissza a főoldalra
            </button>
            <button
              onClick={() => setShowDeleteModal(true)}
              className="bg-red-600 text-white px-6 py-2 rounded-lg hover:bg-red-700 
              transition-colors"
            >
              Foglalás törlése
            </button>
          </div>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 gap-8">
          <div className="space-y-4">
            <h2 className="text-xl font-semibold text-gray-800">Vendég adatai</h2>
            <div className="bg-gray-50 p-4 rounded-lg">
              <p className="text-gray-700">
                <span className="font-medium">Név:</span> {booking.firstName}{' '}
                {booking.lastName}
              </p>
              <p className="text-gray-700">
                <span className="font-medium">Vendégek száma:</span>{' '}
                {booking.numberOfGuests}
              </p>
            </div>
          </div>

          <div className="space-y-4">
            <h2 className="text-xl font-semibold text-gray-800">Szoba adatai</h2>
            <div className="bg-gray-50 p-4 rounded-lg">
              <p className="text-gray-700">
                <span className="font-medium">Szobaszám:</span>{' '}
                {booking.roomNumber}
              </p>
              <p className="text-gray-700">
                <span className="font-medium">Emelet:</span>{' '}
                {booking.floorNumber}
              </p>
              <p className="text-gray-700">
                <span className="font-medium">Szoba típusa:</span>{' '}
                {booking.roomType}
              </p>
            </div>
          </div>

          <div className="space-y-4 md:col-span-2">
            <h2 className="text-xl font-semibold text-gray-800">
              Foglalás részletei
            </h2>
            <div className="bg-gray-50 p-4 rounded-lg grid grid-cols-1 md:grid-cols-2 gap-4">
              <div>
                <p className="text-gray-700">
                  <span className="font-medium">Foglalás dátuma:</span>{' '}
                  {new Date(booking.bookingDate).toLocaleDateString('hu-HU')}
                </p>
                <p className="text-gray-700">
                  <span className="font-medium">Érkezés:</span>{' '}
                  {new Date(booking.checkInDate).toLocaleDateString('hu-HU')}
                </p>
                <p className="text-gray-700">
                  <span className="font-medium">Távozás:</span>{' '}
                  {new Date(booking.checkOutDate).toLocaleDateString('hu-HU')}
                </p>
              </div>
              <div>
                <p className="text-gray-700">
                  <span className="font-medium">Fizetési státusz:</span>{' '}
                  {booking.paymentStatus}
                </p>
                <p className="text-gray-700">
                  <span className="font-medium">Teljes ár:</span>{' '}
                  {booking.totalPrice.toLocaleString('hu-HU')} Ft
                </p>
              </div>
            </div>
          </div>
        </div>

        {showDeleteModal && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center 
          justify-center z-50">
            <div className="bg-white p-6 rounded-lg max-w-md w-full">
              <h3 className="text-xl font-bold text-gray-900 mb-4">
                Foglalás törlése
              </h3>
              <p className="text-gray-600 mb-6">
                Biztosan törölni szeretné ezt a foglalást? Ez a művelet nem
                vonható vissza.
              </p>
              <div className="flex justify-end gap-4">
                <button
                  onClick={() => setShowDeleteModal(false)}
                  className="px-4 py-2 bg-gray-200 text-gray-800 rounded-lg 
                  hover:bg-gray-300"
                >
                  Mégsem
                </button>
                <button
                  onClick={handleDelete}
                  className="px-4 py-2 bg-red-600 text-white rounded-lg 
                  hover:bg-red-700"
                >
                  Törlés megerősítése
                </button>
              </div>
            </div>
          </div>
        )}
      </div>
    </div>
  );
}
