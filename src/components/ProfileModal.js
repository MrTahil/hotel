import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';

const ProfilePage = () => {
  const navigate = useNavigate();
  const [user, setUser] = useState(null); // Inicializálás null-ra
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [deletePassword, setDeletePassword] = useState('');
  const [showGuestModal, setShowGuestModal] = useState(false);
  const [guests, setGuests] = useState([]);
  const [guestData, setGuestData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    phoneNumber: '',
    address: '',
    city: '',
    country: '',
    dateOfBirth: '',
    gender: '',
  });
  const [editGuestId, setEditGuestId] = useState(null);

  useEffect(() => {
    const fetchUserData = async () => {
      try {
        const token = localStorage.getItem('authToken');
        if (!token) {
          throw new Error('Nincs token elmentve! Jelentkezz be újra.');
        }

        const response = await fetch(`https://localhost:7047/UserAccounts/GetOneUserData/${localStorage.getItem('username')}`, {
          method: 'GET',
          headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json',
          },
        });

        if (response.status === 401) {
          throw new Error('Token érvénytelen vagy lejárt!');
        }

        if (!response.ok) {
          throw new Error(`HTTP hiba! Státusz: ${response.status}`);
        }

        const data = await response.json();
        setUser({
          username: data.username,
          email: data.email,
          profileImage: data.profileImage || 'https://randomuser.me/api/portraits/men/42.jpg',
          userId: data.userId,
          role: data.role,
          dateCreated: new Date(data.dateCreated).toLocaleDateString('hu-HU'),
        });
      } catch (error) {
        console.error('Hiba történt:', error.message);
        setError(error.message);
        if (error.message.includes('token') || error.message.includes('401')) {
          navigate('/login');
        }
      } finally {
        setLoading(false);
      }
    };

    fetchUserData();
  }, [navigate]);

  const fetchGuests = async () => {
    try {
      setLoading(true);
      const token = localStorage.getItem('authToken');
      const username = localStorage.getItem('username');
      if (!token || !username) {
        setError('Hiányzó autentikációs adatok');
        return;
      }

      const response = await fetch(`https://localhost:7047/Guests/GetGuestData/${username}`, {
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        if (response.status === 404) {
          setGuests([]);
          return;
        }
        throw new Error(`Nem sikerült lekérni a vendégeket: ${response.status}`);
      }

      const data = await response.json();
      setGuests(Array.isArray(data) ? data : [data]);
    } catch (err) {
      console.error('Hiba a vendéglista lekérésekor:', err.message);
      setError(err.message);
      setGuests([]);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchGuests();
  }, []);

  const handleDeleteAccount = async () => {
    try {
      const token = localStorage.getItem('authToken');
      if (!token) {
        throw new Error('Nincs token elmentve! Jelentkezz be újra.');
      }

      const username = localStorage.getItem('username');
      if (!username) {
        throw new Error('Nincs felhasználónév elmentve!');
      }

      const response = await fetch(`https://localhost:7047/UserAccounts/DeleteUserByUsername/${username}`, {
        method: 'DELETE',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ Password: deletePassword }),
      });

      if (response.status === 401) {
        throw new Error('Token érvénytelen vagy lejárt!');
      }

      if (response.status === 404) {
        throw new Error('Felhasználó nem található!');
      }

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData.message || `HTTP hiba! Státusz: ${response.status}`);
      }

      localStorage.removeItem('authToken');
      localStorage.removeItem('username');
      navigate('/');
    } catch (error) {
      console.error('Hiba történt a fiók törlése közben:', error.message);
      setError(error.message);
    }
  };

  const handleAddGuest = async (e) => {
    e.preventDefault();
    try {
      const token = localStorage.getItem('authToken');
      if (!token) {
        throw new Error('Nincs token elmentve! Jelentkezz be újra.');
      }

      if (!user?.userId) {
        throw new Error('Felhasználói azonosító nem található!');
      }

      if (!guestData.firstName || !guestData.lastName || !guestData.dateOfBirth) {
        throw new Error('A vezetéknév, keresztnév és születési dátum megadása kötelező!');
      }

      const payload = {
        ...guestData,
        userId: user.userId,
        dateOfBirth: guestData.dateOfBirth ? new Date(guestData.dateOfBirth).toISOString() : null,
      };

      const response = await fetch('https://localhost:7047/Guests/Addnewguest', {
        method: 'POST',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Hiba a vendég hozzáadása során: ${errorText}`);
      }

      const responseText = await response.text();
      if (responseText.includes('Sikeres')) {
        await fetchGuests();
        setShowGuestModal(false);
        setGuestData({
          firstName: '',
          lastName: '',
          email: '',
          phoneNumber: '',
          address: '',
          city: '',
          country: '',
          dateOfBirth: '',
          gender: '',
        });
        setError(null);
        alert('Vendég sikeresen hozzáadva!');
      } else {
        throw new Error(`Váratlan szerver válasz: ${responseText}`);
      }
    } catch (err) {
      console.error('Hiba a vendég hozzáadása közben:', err.message);
      setError(err.message);
    }
  };

  const handleDeleteGuest = async (guestId) => {
    try {
      const token = localStorage.getItem('authToken');
      if (!token) {
        throw new Error('Nincs token elmentve! Jelentkezz be újra.');
      }

      const response = await fetch(`https://localhost:7047/Guests/DeleteGuest/${guestId}`, {
        method: 'DELETE',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Hiba a vendég törlése során: ${errorText}`);
      }

      await fetchGuests();
      alert('Vendég sikeresen törölve!');
    } catch (err) {
      console.error('Hiba a vendég törlése közben:', err.message);
      setError(err.message);
    }
  };

  const handleEditGuest = (guest) => {
    setGuestData({
      firstName: guest.firstName,
      lastName: guest.lastName,
      email: guest.email || '',
      phoneNumber: guest.phoneNumber || '',
      address: guest.address || '',
      city: guest.city || '',
      country: guest.country || '',
      dateOfBirth: guest.dateOfBirth ? new Date(guest.dateOfBirth).toISOString().split('T')[0] : '',
      gender: guest.gender || '',
    });
    setEditGuestId(guest.guestId);
    setShowGuestModal(true);
  };

  const handleUpdateGuest = async (e) => {
    e.preventDefault();
    try {
      const token = localStorage.getItem('authToken');
      if (!token) {
        throw new Error('Nincs token elmentve! Jelentkezz be újra.');
      }

      if (!editGuestId) {
        throw new Error('Nincs kiválasztott vendég a szerkesztéshez!');
      }

      const payload = {
        ...guestData,
        dateOfBirth: guestData.dateOfBirth ? new Date(guestData.dateOfBirth).toISOString() : null,
      };

      const response = await fetch(`https://localhost:7047/Guests/UpdateGuest/${editGuestId}`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(payload),
      });

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(`Hiba a vendég módosítása során: ${errorText}`);
      }

      const responseText = await response.text();
      if (responseText.includes('Sikeres')) {
        await fetchGuests();
        setShowGuestModal(false);
        setGuestData({
          firstName: '',
          lastName: '',
          email: '',
          phoneNumber: '',
          address: '',
          city: '',
          country: '',
          dateOfBirth: '',
          gender: '',
        });
        setEditGuestId(null);
        setError(null);
        alert('Vendég sikeresen módosítva!');
      } else {
        throw new Error(`Váratlan szerver válasz: ${responseText}`);
      }
    } catch (err) {
      console.error('Hiba a vendég módosítása közben:', err.message);
      setError(err.message);
    }
  };

  // Kijelentkezés funkció
  const handleLogout = () => {
    localStorage.removeItem('authToken');
    localStorage.removeItem('username');
    setUser(null); // Felhasználói állapot nullázása
    navigate('/'); // Átirányítás a főoldalra
  };

  if (loading) {
    return <div className="bg-blue-50 min-h-screen p-6 sm:p-8">Betöltés...</div>;
  }

  if (error) {
    return <div className="bg-blue-50 min-h-screen p-6 sm:p-8">Hiba: {error}</div>;
  }

  if (!user) {
    return null; // Ha nincs user (pl. kijelentkezés után), ne rendereljen semmit
  }

  return (
    <div className="bg-blue-50 min-h-screen p-6 sm:p-8">
      <div className="max-w-4xl mx-auto">
        <div className="flex flex-col sm:flex-row sm:items-center mb-6 gap-4">
          <button
            onClick={() => window.history.back()}
            className="flex items-center bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors"
          >
            <span className="material-symbols-outlined mr-2">arrow_back</span>
            Vissza a főoldalra
          </button>
          <h1 className="text-2xl sm:text-3xl font-bold text-blue-800">Profil kezelése</h1>
          <button
            onClick={handleLogout}
            className="flex items-center bg-red-600 text-white px-4 py-2 rounded-lg hover:bg-red-700 transition-colors"
          >
            <span className="material-symbols-outlined mr-2">logout</span>
            Kijelentkezés
          </button>
        </div>

        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-2 gap-6 md:gap-8 mb-8">
          <div className="bg-white p-4 sm:p-6 rounded-lg shadow-md hover:shadow-xl transition-shadow duration-300 border border-blue-100">
            <div className="flex items-center mb-4">
              <span className="material-symbols-outlined text-3xl sm:text-4xl mr-3 sm:mr-4 bg-blue-100 p-2 sm:p-3 rounded-full text-blue-600">
                person
              </span>
              <h2 className="text-xl sm:text-2xl font-semibold text-blue-800">Profilom</h2>
            </div>
            
            <div className="flex flex-col items-center mb-6">
              <div className="relative group mb-4">
                <div className="w-24 h-24 sm:w-32 sm:h-32 rounded-full overflow-hidden border-4 border-blue-100 group-hover:border-blue-300 transition-all duration-300">
                  <img 
                    src={user.profileImage} 
                    alt="Profilkép" 
                    className="w-full h-full object-cover"
                  />
                </div>
              </div>
              
              <h3 className="text-lg sm:text-xl font-semibold text-blue-800 mb-1">
                {user.username}
              </h3>
              <p className="text-xs sm:text-sm text-blue-600 mb-4">
                Regisztrált: {user.dateCreated}
              </p>
            </div>
            
            <div className="space-y-4">
              <div className="border-t border-blue-100 pt-4">
                <label className="block text-sm font-medium mb-1 text-blue-800">
                  Felhasználónév
                </label>
                <div className="px-3 sm:px-4 py-2 bg-blue-50 rounded-lg text-blue-700 text-sm sm:text-base">
                  {user.username}
                </div>
              </div>
              
              <div>
                <label className="block text-sm font-medium mb-1 text-blue-800">
                  Email cím
                </label>
                <div className="px-3 sm:px-4 py-2 bg-blue-50 rounded-lg text-blue-700 text-sm sm:text-base break-all">
                  {user.email}
                </div>
              </div>
            </div>
          </div>

          <div className="bg-white p-4 sm:p-6 rounded-lg shadow-md hover:shadow-xl transition-shadow duration-300 border border-blue-100">
            <div className="flex flex-col gap-3 mb-4">
              <div className="flex items-center">
                <span className="material-symbols-outlined text-3xl sm:text-4xl mr-3 sm:mr-4 bg-cyan-100 p-2 sm:p-3 rounded-full text-cyan-600">
                  bookmark
                </span>
                <h2 className="text-xl sm:text-2xl font-semibold text-blue-800">
                  Elmentett vendég adatok
                </h2>
              </div>
              <button
                onClick={() => setShowGuestModal(true)}
                className="flex items-center justify-center bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transform hover:scale-105 transition-all duration-200 w-full md:w-auto"
              >
                <span className="material-symbols-outlined mr-2">person_add</span>
                Vendég hozzáadása
              </button>
            </div>
            
            <div className="space-y-4">
              {loading ? (
                <p className="text-blue-600">Betöltés...</p>
              ) : error ? (
                <p className="text-red-600">Hiba: {error}</p>
              ) : guests.length > 0 ? (
                guests.map((guest) => (
                  <div
                    key={guest.guestId || Math.random()}
                    className="border border-blue-100 rounded-lg p-3 sm:p-4 hover:border-blue-300 transition-all bg-blue-50/50 hover:shadow-md"
                  >
                    <h3 className="font-semibold mb-2 text-blue-800">
                      {guest.firstName} {guest.lastName}
                    </h3>
                    <p className="text-xs sm:text-sm text-blue-700 mb-1">Email: {guest.email || 'Nincs megadva'}</p>
                    <p className="text-xs sm:text-sm text-blue-700 mb-3">Telefon: {guest.phoneNumber || 'Nincs megadva'}</p>
                    <div className="flex flex-wrap gap-2">
                      <button
                        onClick={() => handleEditGuest(guest)}
                        className="px-3 py-1 bg-blue-100 text-blue-600 rounded hover:bg-blue-200 transition-colors flex items-center text-sm"
                      >
                        <span className="material-symbols-outlined text-sm mr-1">edit</span>
                        Szerkesztés
                      </button>
                      <button
                        onClick={() => handleDeleteGuest(guest.guestId)}
                        className="px-3 py-1 bg-red-100 text-red-600 rounded hover:bg-red-200 transition-colors flex items-center text-sm"
                      >
                        <span className="material-symbols-outlined text-sm mr-1">delete</span>
                        Törlés
                      </button>
                    </div>
                  </div>
                ))
              ) : (
                <p className="text-blue-700">Nincsenek mentett vendégek.</p>
              )}
            </div>
          </div>
        </div>

        <div className="bg-white p-4 sm:p-6 rounded-lg shadow-md mb-6 sm:mb-8 hover:shadow-xl transition-shadow duration-300 border border-blue-100">
          <div className="flex items-center mb-4">
            <span className="material-symbols-outlined text-3xl sm:text-4xl mr-3 sm:mr-4 bg-indigo-100 p-2 sm:p-3 rounded-full text-indigo-600">
              lock
            </span>
            <h2 className="text-xl sm:text-2xl font-semibold text-blue-800">Jelszó kezelése</h2>
          </div>

          <details className="mb-4 sm:mb-6 group">
            <summary className="list-none flex items-center justify-between cursor-pointer p-3 sm:p-4 bg-blue-50 rounded-lg hover:bg-blue-100 transition-colors">
              <span className="font-medium text-blue-800">Jelszó megváltoztatása</span>
              <span className="material-symbols-outlined transition-transform group-open:rotate-180 text-blue-600">
                expand_more
              </span>
            </summary>
            <div className="p-3 sm:p-4 border-t border-blue-100 animate-[fadeIn_0.2s_ease-in-out]">
              <form className="space-y-4">
                <div>
                  <label className="block text-sm font-medium mb-1 text-blue-800" htmlFor="current-password">Jelenlegi jelszó</label>
                  <input
                    type="password"
                    id="current-password"
                    className="w-full px-3 sm:px-4 py-2 border border-blue-200 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-all"
                    placeholder="Jelenlegi jelszó"
                  />
                </div>
                
                <div>
                  <label className="block text-sm font-medium mb-1 text-blue-800" htmlFor="new-password">Új jelszó</label>
                  <input
                    type="password"
                    id="new-password"
                    className="w-full px-3 sm:px-4 py-2 border border-blue-200 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-all"
                    placeholder="Új jelszó"
                  />
                </div>
                
                <div>
                  <label className="block text-sm font-medium mb-1 text-blue-800" htmlFor="confirm-password">Új jelszó megerősítése</label>
                  <input
                    type="password"
                    id="confirm-password"
                    className="w-full px-3 sm:px-4 py-2 border border-blue-200 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-all"
                    placeholder="Új jelszó megerősítése"
                  />
                </div>
                
                <button
                  type="submit"
                  className="w-full md:w-auto bg-indigo-600 text-white px-4 sm:px-6 py-2 rounded-lg hover:bg-indigo-700 transform hover:scale-105 transition-all duration-200"
                >
                  Jelszó megváltoztatása
                </button>
              </form>
            </div>
          </details>
          
          <details className="group">
            <summary className="list-none flex items-center justify-between cursor-pointer p-3 sm:p-4 bg-blue-50 rounded-lg hover:bg-blue-100 transition-colors">
              <span className="font-medium text-blue-800">Elfelejtett jelszó</span>
              <span className="material-symbols-outlined transition-transform group-open:rotate-180 text-blue-600">
                expand_more
              </span>
            </summary>
            <div className="p-3 sm:p-4 border-t border-blue-100 animate-[fadeIn_0.2s_ease-in-out]">
              <p className="mb-4 text-xs sm:text-sm text-blue-700">
                Adja meg az email címét, és küldünk egy linket, amellyel visszaállíthatja a jelszavát.
              </p>
              <form className="space-y-4">
                <div>
                  <label className="block text-sm font-medium mb-1 text-blue-800" htmlFor="reset-email">Email cím</label>
                  <input
                    type="email"
                    id="reset-email"
                    className="w-full px-3 sm:px-4 py-2 border border-blue-200 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-all"
                    placeholder="Email cím"
                  />
                </div>
                
                <button
                  type="submit"
                  className="w-full md:w-auto bg-indigo-600 text-white px-4 sm:px-6 py-2 rounded-lg hover:bg-indigo-700 transform hover:scale-105 transitionomba-all duration-200"
                >
                  Visszaállítási link küldése
                </button>
              </form>
            </div>
          </details>
        </div>
        
        <div className="border-t border-blue-200 pt-6 sm:pt-8 mt-6 sm:mt-8">
          <div className="bg-red-50 p-4 sm:p-6 rounded-lg border border-red-200">
            <h2 className="text-lg sm:text-xl font-semibold text-red-600 mb-2 flex items-center">
              <span className="material-symbols-outlined mr-2">warning</span>
              Fiók törlése
            </h2>
            <p className="text-xs sm:text-sm text-blue-700 mb-4">
              A fiók törlése végleges művelet, és nem vonható vissza. Az összes adat, beleértve a mentett vendég adatokat is, véglegesen törlődni fog.
            </p>
            <details className="group">
              <summary className="list-none cursor-pointer flex items-center">
                <span className="material-symbols-outlined mr-2">delete_forever</span>
                <span>Fiók törlése</span>
              </summary>
              <div className="mt-4 p-3 sm:p-4 bg-white border border-red-200 rounded-lg animate-[fadeIn_0.2s_ease-in-out]">
                <p className="font-medium text-red-600 mb-4 text-sm sm:text-base">Biztosan törölni szeretné a fiókját? Ez a művelet nem visszavonható.</p>
                <div className="mb-4">
                  <label className="block text-sm font-medium mb-1 text-blue-800" htmlFor="confirm-delete">Írja be a jelszavát a megerősítéshez:</label>
                  <input
                    type="password"
                    id="confirm-delete"
                    className="w-full px-3 sm:px-4 py-2 border border-blue-200 rounded-lg focus:ring-2 focus:ring-red-500 focus:border-red-500 transition-all"
                    placeholder="Jelszó"
                    value={deletePassword}
                    onChange={(e) => setDeletePassword(e.target.value)}
                  />
                </div>
                <button
                  type="button"
                  className="w-full md:w-auto bg-red-600 text-white px-4 sm:px-6 py-2 rounded-lg hover:bg-red-700 transform hover:scale-105 transition-all duration-200"
                  onClick={handleDeleteAccount}
                >
                  Végleges törlés
                </button>
              </div>
            </details>
          </div>
        </div>

        {showGuestModal && (
          <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <div className="bg-white p-4 sm:p-6 rounded-lg shadow-lg w-full max-w-lg sm:max-w-2xl">
              <h3 className="text-xl font-semibold text-blue-800 mb-4">
                {editGuestId ? 'Vendég szerkesztése' : 'Új vendég hozzáadása'}
              </h3>
              <form onSubmit={editGuestId ? handleUpdateGuest : handleAddGuest} className="grid grid-cols-1 sm:grid-cols-2 gap-4">
                <div className="space-y-4">
                  <div>
                    <label className="block text-sm font-medium text-blue-800">Vezetéknév</label>
                    <input
                      type="text"
                      value={guestData.lastName}
                      onChange={(e) => setGuestData({ ...guestData, lastName: e.target.value })}
                      className="w-full p-2 border border-blue-200 rounded-lg"
                      required
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-blue-800">Keresztnév</label>
                    <input
                      type="text"
                      value={guestData.firstName}
                      onChange={(e) => setGuestData({ ...guestData, firstName: e.target.value })}
                      className="w-full p-2 border border-blue-200 rounded-lg"
                      required
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-blue-800">Email</label>
                    <input
                      type="email"
                      value={guestData.email}
                      onChange={(e) => setGuestData({ ...guestData, email: e.target.value })}
                      className="w-full p-2 border border-blue-200 rounded-lg"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-blue-800">Telefonszám</label>
                    <input
                      type="text"
                      value={guestData.phoneNumber}
                      onChange={(e) => setGuestData({ ...guestData, phoneNumber: e.target.value })}
                      className="w-full p-2 border border-blue-200 rounded-lg"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-blue-800">Születési dátum</label>
                    <input
                      type="date"
                      value={guestData.dateOfBirth}
                      onChange={(e) => setGuestData({ ...guestData, dateOfBirth: e.target.value })}
                      className="w-full p-2 border border-blue-200 rounded-lg"
                      required
                    />
                  </div>
                </div>
                <div className="space-y-4">
                  <div>
                    <label className="block text-sm font-medium text-blue-800">Cím</label>
                    <input
                      type="text"
                      value={guestData.address}
                      onChange={(e) => setGuestData({ ...guestData, address: e.target.value })}
                      className="w-full p-2 border border-blue-200 rounded-lg"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-blue-800">Város</label>
                    <input
                      type="text"
                      value={guestData.city}
                      onChange={(e) => setGuestData({ ...guestData, city: e.target.value })}
                      className="w-full p-2 border border-blue-200 rounded-lg"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-blue-800">Ország</label>
                    <input
                      type="text"
                      value={guestData.country}
                      onChange={(e) => setGuestData({ ...guestData, country: e.target.value })}
                      className="w-full p-2 border border-blue-200 rounded-lg"
                    />
                  </div>
                  <div>
                    <label className="block text-sm font-medium text-blue-800">Nem</label>
                    <select
                      value={guestData.gender}
                      onChange={(e) => setGuestData({ ...guestData, gender: e.target.value })}
                      className="w-full p-2 border border-blue-200 rounded-lg"
                    >
                      <option value="">Válasszon</option>
                      <option value="Férfi">Férfi</option>
                      <option value="Nő">Nő</option>
                      <option value="Egyéb">Kalács</option>
                    </select>
                  </div>
                </div>
                <div className="sm:col-span-2 flex gap-2 justify-end mt-4">
                  <button
                    type="submit"
                    className="bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700"
                  >
                    {editGuestId ? 'Mentés' : 'Hozzáadás'}
                  </button>
                  <button
                    type="button"
                    onClick={() => {
                      setShowGuestModal(false);
                      setEditGuestId(null);
                      setGuestData({
                        firstName: '',
                        lastName: '',
                        email: '',
                        phoneNumber: '',
                        address: '',
                        city: '',
                        country: '',
                        dateOfBirth: '',
                        gender: '',
                      });
                    }}
                    className="bg-gray-600 text-white px-4 py-2 rounded-lg hover:bg-gray-700"
                  >
                    Mégse
                  </button>
                </div>
              </form>
              {error && <p className="text-red-600 mt-2">{error}</p>}
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default ProfilePage;