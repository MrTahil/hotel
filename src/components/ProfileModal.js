import React, { useEffect, useState } from 'react';

const ProfilePage = () => {
  const [user, setUser] = useState({
    username: '',
    email: '',
    profileImage: 'https://randomuser.me/api/portraits/men/42.jpg',
    registrationDate: '2023. szeptember 15.',
  });

  useEffect(() => {
    // Felhasználó adatainak betöltése a localStorage-ből
    const username = localStorage.getItem('username');
    const email = localStorage.getItem('email');
    console.log('Username:', username); // Ellenőrzés
    console.log('Email:', email); // Ellenőrzés

    if (username) {
      setUser((prevUser) => ({
        ...prevUser, // Megőrizzük a már meglévő mezőket
        username,
        email: email || 'Nincs email cím', // Ha az email null, akkor egy alapértelmezett értéket állítunk be
      }));
    } else {
      console.error('Nincs username a localStorage-ben!');
    }
  }, []);

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
        </div>

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
                  <img src={user.profileImage} alt="Profilkép" className="w-full h-full object-cover" />
                </div>
                <div className="absolute inset-0 flex items-center justify-center opacity-0 group-hover:opacity-100 transition-opacity duration-300">
                  <label htmlFor="profile-upload" className="bg-blue-600/80 text-white p-2 rounded-full cursor-pointer hover:bg-blue-700 transition-colors">
                    <span className="material-symbols-outlined">photo_camera</span>
                    <input type="file" id="profile-upload" className="hidden" accept="image/*" />
                  </label>
                </div>
              </div>
              
              <h3 className="text-lg sm:text-xl font-semibold text-blue-800 mb-1">{user.username}</h3>
              <p className="text-xs sm:text-sm text-blue-600 mb-4">Regisztrált: {user.registrationDate}</p>
              
              <button className="text-blue-600 hover:text-blue-800 flex items-center transition-colors text-sm">
                <span className="material-symbols-outlined mr-1 text-sm">edit</span>
                Profil szerkesztése
              </button>
            </div>
            
            <div className="space-y-4">
              <div className="border-t border-blue-100 pt-4">
                <label className="block text-sm font-medium mb-1 text-blue-800">Felhasználónév</label>
                <div className="px-3 sm:px-4 py-2 bg-blue-50 rounded-lg text-blue-700 text-sm sm:text-base">{user.username}</div>
              </div>
              
              <div>
                <label className="block text-sm font-medium mb-1 text-blue-800">Email cím</label>
                <div className="px-3 sm:px-4 py-2 bg-blue-50 rounded-lg text-blue-700 text-sm sm:text-base break-all">{user.email}</div>
              </div>
              
              <button
                type="button"
                className="mt-2 w-full bg-blue-600 text-white px-4 sm:px-6 py-2 rounded-lg hover:bg-blue-700 transition-colors"
              >
                Adatok szerkesztése
              </button>
            </div>
          </div>

          {/* Jobb oldali panel */}
          <div className="bg-white p-4 sm:p-6 rounded-lg shadow-md hover:shadow-xl transition-shadow duration-300 border border-blue-100">
            <div className="flex flex-col gap-3 mb-4">
              <div className="flex items-center">
                <span className="material-symbols-outlined text-3xl sm:text-4xl mr-3 sm:mr-4 bg-cyan-100 p-2 sm:p-3 rounded-full text-cyan-600">
                  bookmark
                </span>
                <h2 className="text-xl sm:text-2xl font-semibold text-blue-800">Elmentett vendég adatok</h2>
              </div>
              <button className="flex items-center justify-center bg-green-600 text-white px-4 py-2 rounded-lg hover:bg-green-700 transform hover:scale-105 transition-all duration-200 w-full md:w-auto">
                <span className="material-symbols-outlined mr-2">person_add</span>
                Vendég hozzáadása
              </button>
            </div>
            
            <div className="border border-blue-100 rounded-lg p-3 sm:p-4 mb-4 hover:border-blue-300 transition-all bg-blue-50/50 hover:shadow-md">
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
            
            <div className="border border-blue-100 rounded-lg p-3 sm:p-4 hover:border-blue-300 transition-all bg-blue-50/50 hover:shadow-md">
              <h3 className="font-semibold mb-2 text-blue-800">Kiss Éva</h3>
              <p className="text-xs sm:text-sm text-blue-700 mb-1">Email: kiss.eva@example.com</p>
              <p className="text-xs sm:text-sm text-blue-700 mb-3">Telefon: +36 20 987 6543</p>
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
        
        {/* Jelszó kezelése szekció */}
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
                  className="w-full md:w-auto bg-indigo-600 text-white px-4 sm:px-6 py-2 rounded-lg hover:bg-indigo-700 transform hover:scale-105 transition-all duration-200"
                >
                  Visszaállítási link küldése
                </button>
              </form>
            </div>
          </details>
        </div>
        
        {/* Fiók törlés szekció */}
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
              <summary className="list-none cursor-pointer">
                <button className="px-4 sm:px-5 py-2 bg-white text-red-600 border border-red-300 rounded-lg hover:bg-red-100 transition-colors flex items-center">
                  <span className="material-symbols-outlined mr-2">delete_forever</span>
                  Fiók törlése
                </button>
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
                  />
                </div>
                <button
                  type="button"
                  className="w-full md:w-auto bg-red-600 text-white px-4 sm:px-6 py-2 rounded-lg hover:bg-red-700 transform hover:scale-105 transition-all duration-200"
                >
                  Végleges törlés
                </button>
              </div>
            </details>
          </div>
        </div>
      </div>
    </div>
  );
};

export default ProfilePage;