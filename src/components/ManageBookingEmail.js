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

  if (loading) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center p-4">
        <div className="text-blue-600 text-lg md:text-xl animate-pulse">
          Betöltés...
        </div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="min-h-screen bg-gray-50 flex items-center justify-center p-4">
        <div className="text-red-600 text-sm md:text-base bg-red-50 p-4 rounded-lg border border-red-200 max-w-md w-full">
          <h2 className="font-bold mb-2">Hiba történt</h2>
          {error}
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-gray-50 p-3 sm:p-4 md:p-6 lg:p-8">
      <div className="max-w-4xl mx-auto bg-white rounded-xl shadow-lg overflow-hidden">
        {/* Fejléc szekció */}
        <div className="bg-gradient-to-r from-blue-800 to-blue-900 text-white p-4 sm:p-6">
          <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
            <h1 className="text-2xl sm:text-3xl font-bold">Foglalás részletei</h1>
            <div className="flex flex-col sm:flex-row gap-2 sm:gap-4 w-full sm:w-auto">
              <button
                onClick={() => (window.location.href = 'https://hmzrt.eu')}
                className="w-full sm:w-auto bg-white/10 hover:bg-white/20 text-white px-4 py-2 rounded-lg transition-colors text-sm sm:text-base flex items-center justify-center"
              >
                <span className="material-icons-outlined mr-2">arrow_back</span>
                Vissza a főoldalra
              </button>
              <button
                onClick={() => setShowDeleteModal(true)}
                className="w-full sm:w-auto bg-red-500 hover:bg-red-600 text-white px-4 py-2 rounded-lg transition-colors text-sm sm:text-base flex items-center justify-center"
              >
                <span className="material-icons-outlined mr-2">delete</span>
                Foglalás törlése
              </button>
            </div>
          </div>
        </div>

        {/* Fő tartalom */}
        <div className="p-4 sm:p-6 space-y-6">
          {/* Vendég és szoba információk grid */}
          <div className="grid grid-cols-1 md:grid-cols-2 gap-4 sm:gap-6">
            {/* Vendég adatok kártya */}
            <div className="bg-blue-50 rounded-xl p-4 sm:p-5">
              <h2 className="text-lg sm:text-xl font-semibold text-blue-900 mb-4 flex items-center">
                <span className="material-icons-outlined mr-2">person</span>
                Vendég adatai
              </h2>
              <div className="space-y-3">
                <p className="text-sm sm:text-base text-gray-700">
                  <span className="font-medium">Név:</span>{' '}
                  {booking.firstName} {booking.lastName}
                </p>
                <p className="text-sm sm:text-base text-gray-700">
                  <span className="font-medium">Vendégek száma:</span>{' '}
                  {booking.numberOfGuests} fő
                </p>
              </div>
            </div>

            {/* Szoba adatok kártya */}
            <div className="bg-blue-50 rounded-xl p-4 sm:p-5">
              <h2 className="text-lg sm:text-xl font-semibold text-blue-900 mb-4 flex items-center">
                <span className="material-icons-outlined mr-2">hotel</span>
                Szoba adatai
              </h2>
              <div className="space-y-3">
                <p className="text-sm sm:text-base text-gray-700">
                  <span className="font-medium">Szobaszám:</span>{' '}
                  {booking.roomNumber}
                </p>
                <p className="text-sm sm:text-base text-gray-700">
                  <span className="font-medium">Emelet:</span>{' '}
                  {booking.floorNumber}
                </p>
                <p className="text-sm sm:text-base text-gray-700">
                  <span className="font-medium">Szoba típusa:</span>{' '}
                  {booking.roomType}
                </p>
              </div>
            </div>
          </div>

          {/* Foglalás részletei kártya */}
          <div className="bg-gradient-to-r from-blue-50 to-blue-100 rounded-xl p-4 sm:p-5">
            <h2 className="text-lg sm:text-xl font-semibold text-blue-900 mb-4 flex items-center">
              <span className="material-icons-outlined mr-2">event</span>
              Foglalás részletei
            </h2>
            <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
              <div className="space-y-3">
                <p className="text-sm sm:text-base text-gray-700">
                  <span className="font-medium">Foglalás dátuma:</span>{' '}
                  {new Date(booking.bookingDate).toLocaleDateString('hu-HU')}
                </p>
                <p className="text-sm sm:text-base text-gray-700">
                  <span className="font-medium">Érkezés:</span>{' '}
                  {new Date(booking.checkInDate).toLocaleDateString('hu-HU')}
                </p>
                <p className="text-sm sm:text-base text-gray-700">
                  <span className="font-medium">Távozás:</span>{' '}
                  {new Date(booking.checkOutDate).toLocaleDateString('hu-HU')}
                </p>
              </div>
              <div className="space-y-3">
                <p className="text-sm sm:text-base text-gray-700">
                  <span className="font-medium">Fizetési státusz:</span>{' '}
                  <span className={`px-2 py-1 rounded-full text-xs ${
                    booking.paymentStatus === 'Paid' 
                      ? 'bg-green-100 text-green-800' 
                      : 'bg-yellow-100 text-yellow-800'
                  }`}>
                    {booking.paymentStatus}
                  </span>
                </p>
                <p className="text-sm sm:text-base text-gray-700">
                  <span className="font-medium">Teljes ár:</span>{' '}
                  <span className="text-lg font-semibold text-blue-900">
                    {booking.totalPrice.toLocaleString('hu-HU')} Ft
                  </span>
                </p>
              </div>
            </div>
          </div>
        </div>
      </div>

      {/* Törlés megerősítő modal */}
      {showDeleteModal && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50 p-4">
          <div className="bg-white rounded-xl shadow-xl max-w-md w-full p-4 sm:p-6">
            <h3 className="text-xl font-bold text-gray-900 mb-4 flex items-center">
              <span className="material-icons-outlined text-red-600 mr-2">warning</span>
              Foglalás törlése
            </h3>
            <p className="text-gray-600 mb-6">
              Biztosan törölni szeretné ezt a foglalást? Ez a művelet nem
              vonható vissza.
            </p>
            <div className="flex flex-col sm:flex-row justify-end gap-2 sm:gap-4">
              <button
                onClick={() => setShowDeleteModal(false)}
                className="w-full sm:w-auto px-4 py-2 bg-gray-200 text-gray-800 rounded-lg 
                hover:bg-gray-300 transition-colors text-sm sm:text-base"
              >
                Mégsem
              </button>
              <button
                onClick={handleDelete}
                className="w-full sm:w-auto px-4 py-2 bg-red-600 text-white rounded-lg 
                hover:bg-red-700 transition-colors text-sm sm:text-base"
              >
                Törlés megerősítése
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}