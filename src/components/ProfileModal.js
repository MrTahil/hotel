import React, { useEffect, useState } from 'react';

const ProfilePage = () => {
  const [user, setUser] = useState({
    username: '',
    email: '',
    profileImage: 'https://randomuser.me/api/portraits/men/42.jpg',
    registrationDate: '2023. szeptember 15.',
    userId: 0,
    password: '',
    role: '',
    refreshToken: '',
    refreshTokenExpiryTime: '',
    status: '',
    dateCreated: '',
    lastLogin: '',
    dateUpdated: '',
    notes: '',
    authenticationcode: '',
    authenticationexpire: '',
  });

  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);

  useEffect(() => {
    const fetchUserData = async () => {
      try {
        
        // 1. Token lekérése
        const token = localStorage.getItem('authToken');
        console.log('Token értéke:', token); // Debug
        if (!token) {
          throw new Error('Nincs token elmentve! Jelentkezz be újra.');
        }
            console.log(token)
        // 2. Kérés küldése
        const response = await fetch(`https://localhost:7047/UserAccounts/GetOneUserData/${localStorage.getItem('username')}`, {
          method: 'GET',
          headers: {
            'Authorization': `Bearer ${token}`,
            'Content-Type': 'application/json',
          },
        });

        // 3. Hibakezelés
        if (response.status === 401) {
          throw new Error('Token érvénytelen vagy lejárt!');
        }

        if (!response.ok) {
          throw new Error(`HTTP hiba! Státusz: ${response.status}`);
        }

        // 4. Adatok feldolgozása
        const data = await response.json();
        console.log('Szerver válasza:', data);

        setUser(prev => ({
          ...prev,
          username: data.username,
          email: data.email,
          userId: data.userId,
          role: data.role,
          dateCreated: new Date(data.dateCreated).toLocaleDateString('hu-HU'),
          // ... egyéb mezők
        }));

      } catch (error) {
        console.error('Hiba történt:', error.message);
        setError(error.message);
        
        // Átirányítás bejelentkezésre
        if (error.message.includes('token') || error.message.includes('401')) {
          window.location.href = '/login';
        }
      } finally {
        setLoading(false);
      }
    };

    fetchUserData();
  }, []);

  if (loading) {
    return (
      <div className="bg-blue-50 min-h-screen p-6 sm:p-8 flex items-center justify-center">
        <div className="text-blue-800 text-xl">Adatok betöltése...</div>
      </div>
    );
  }

  if (error) {
    return (
      <div className="bg-blue-50 min-h-screen p-6 sm:p-8 flex items-center justify-center">
        <div className="text-red-600 text-xl">{error}</div>
      </div>
    );
  }

  return (
    <div className="bg-blue-50 min-h-screen p-6 sm:p-8">
      <div className="max-w-4xl mx-auto">
        {/* Fejléc */}
        <div className="flex flex-col sm:flex-row sm:items-center mb-6 gap-4">
          <button
            onClick={() => window.history.back()}
            className="flex items-center bg-blue-600 text-white px-4 py-2 rounded-lg hover:bg-blue-700 transition-colors"
          >
            <span className="material-symbols-outlined mr-2">arrow_back</span>
            Vissza a főoldalra
          </button>
          <h1 className="text-2xl sm:text-3xl font-bold text-blue-800">Profil kezelése</h1>
        </div>

        {/* Profil adatok */}
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-2 gap-6 md:gap-8 mb-8">
          {/* Bal oldali panel */}
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

          {/* Jobb oldali panel - Elmentett vendégek */}
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
              <button className="flex items-center justify-center bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transform hover:scale-105 transition-all duration-200 w-full md:w-auto">
                <span className="material-symbols-outlined mr-2">person_add</span>
                Vendég hozzáadása
              </button>
            </div>
            
            {/* Vendég lista */}
            <div className="space-y-4">
              <div className="border border-blue-100 rounded-lg p-3 sm:p-4 hover:border-blue-300 transition-all bg-blue-50/50 hover:shadow-md">
                <h3 className="font-semibold mb-2 text-blue-800">Nagy Zoltán</h3>
                <p className="text-xs sm:text-sm text-blue-700 mb-1">Email: nagy.zoltan@example.com</p>
                <p className="text-xs sm:text-sm text-blue-700 mb-3">Telefon: +36 30 123 4567</p>
                <div className="flex flex-wrap gap-2">
                  <button className="px-3 py-1 bg-blue-100 text-blue-600 rounded hover:bg-blue-200 transition-colors flex items-center text-sm">
                    <span className="material-symbols-outlined text-sm mr-1">edit</span>
                    Szerkesztés
                  </button>
                  <button className="px-3 py-1 bg-red-100 text-red-600 rounded hover:bg-red-200 transition-colors flex items-center text-sm">
                    <span className="material-symbols-outlined text-sm mr-1">delete</span>
                    Törlés
                  </button>
                </div>
              </div>
            </div>
          </div>
        </div>

        {/* További szekciók (jelszó kezelés, fiók törlés) maradnak változatlanok */}
      </div>
    </div>
  );
};

export default ProfilePage;