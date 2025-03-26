import React, { useEffect, useState } from 'react';
import { useNavigate } from 'react-router-dom';

const ProfilePage = () => {
  const navigate = useNavigate();
  const [user, setUser] = useState(null);
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
  const [message, setMessage] = useState(null);

  // Password change states
  const [passwordData, setPasswordData] = useState({
    currentPassword: '',
    newPassword: '',
    confirmPassword: ''
  });
  const [passwordErrors, setPasswordErrors] = useState({
    currentPassword: '',
    newPassword: '',
    confirmPassword: ''
  });

  // Forgot password states
  const [forgotEmail, setForgotEmail] = useState('');
  const [verificationCode, setVerificationCode] = useState('');
  const [newPasswordData, setNewPasswordData] = useState({
    newPassword: '',
    confirmPassword: ''
  });
  const [forgotStep, setForgotStep] = useState(1);

  useEffect(() => {
    if (message) {
      const timer = setTimeout(() => setMessage(null), 3000);
      return () => clearTimeout(timer);
    }
  }, [message]);

  useEffect(() => {
    const fetchUserData = async () => {
      try {
        const token = localStorage.getItem('authToken');
        if (!token) {
          throw new Error('Nincs token elmentve! Jelentkezz be újra.');
        }

        const response = await fetch(process.env.REACT_APP_API_BASE_URL + `/UserAccounts/GetOneUserData/${localStorage.getItem('username')}`, {
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

      const response = await fetch(process.env.REACT_APP_API_BASE_URL + `/Guests/GetGuestData/${username}`, {
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

  const handleChangePassword = async (e) => {
    e.preventDefault();
    setPasswordErrors({ currentPassword: '', newPassword: '', confirmPassword: '' }); // Reset errors

    try {
      const token = localStorage.getItem('authToken');
      const username = localStorage.getItem('username');
      const passwordRegex = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[!@#$%^&*])[A-Za-z\d!@#$%^&*]{8,}$/;

      if (!token || !username) {
        throw new Error('Hiányzó autentikációs adatok');
      }

      if (passwordData.newPassword !== passwordData.confirmPassword) {
        setPasswordErrors((prev) => ({
          ...prev,
          confirmPassword: 'Az új jelszavak nem egyeznek!',
        }));
        return;
      }

      if (!passwordRegex.test(passwordData.newPassword)) {
        setPasswordErrors((prev) => ({
          ...prev,
          newPassword: 'Az új jelszónak legalább 8 karakterből kell állnia, és tartalmaznia kell kisbetűt, nagybetűt, számot és speciális karaktert (!@#$%^&*?)!',
        }));
        return;
      }

      const response = await fetch(process.env.REACT_APP_API_BASE_URL + `/UserAccounts/Newpasswithknownpass/${username}`, {
        method: 'PUT',
        headers: {
          'Authorization': `Bearer ${token}`,
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          OldPassword: passwordData.currentPassword,
          Password: passwordData.newPassword,
        }),
      });

      const data = await response.json();

      if (!response.ok) {
        if (data.message.includes('jelenlegi jelszó')) {
          setPasswordErrors((prev) => ({
            ...prev,
            currentPassword: 'Helytelen jelenlegi jelszó!',
          }));
        }
        throw new Error(data.message || `Hiba a jelszó változtatásakor: ${response.status}`);
      }

      setMessage({ type: 'success', text: 'Jelszó sikeresen megváltoztatva!' });
      setPasswordData({
        currentPassword: '',
        newPassword: '',
        confirmPassword: '',
      });
    } catch (err) {
      setMessage({ type: 'error', text: err.message });
    }
  };

  const handleForgotPasswordStep1 = async (e) => {
    e.preventDefault();
    try {
      const response = await fetch(process.env.REACT_APP_API_BASE_URL + `/UserAccounts/ForgotPasswordsendemail/${forgotEmail}`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
      });

      if (!response.ok) {
        throw new Error('Hiba történt a jelszó visszaállítási kérelem során');
      }

      setMessage({
        type: 'success',
        text: 'Ellenőrizze email fiókját a 6 számjegyű kódért!'
      });
      setForgotStep(2);
    } catch (err) {
      setMessage({ type: 'error', text: err.message });
    }
  };

  const handleForgotPasswordStep2 = async (e) => {
    e.preventDefault();
    try {
      const response = await fetch(process.env.REACT_APP_API_BASE_URL + '/UserAccounts/VerifyTheforgotpass', {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          Email: forgotEmail,
          Code: verificationCode
        }),
      });

      if (!response.ok) {
        throw new Error('Hibás ellenőrző kód vagy email!');
      }

      setMessage({ type: 'success', text: 'Kód sikeresen ellenőrizve!' });
      setForgotStep(3);
    } catch (err) {
      setMessage({ type: 'error', text: err.message });
    }
  };

  const handleForgotPasswordStep3 = async (e) => {
    e.preventDefault();
    try {
      if (newPasswordData.newPassword !== newPasswordData.confirmPassword) {
        setMessage({ type: 'error', text: 'Az új jelszavak nem egyeznek!' });
        return;
      }

      const response = await fetch(process.env.REACT_APP_API_BASE_URL + '/UserAccounts/SetNewPassword', {
        method: 'PUT',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({
          Email: forgotEmail,
          Password: newPasswordData.newPassword
        }),
      });

      const data = await response.json();

      if (!response.ok) {
        throw new Error(data.message || 'Hiba történt az új jelszó beállításakor');
      }

      setMessage({ type: 'success', text: 'Jelszó sikeresen visszaállítva! Jelentkezzen be az új jelszóval.' });
      setForgotEmail('');
      setVerificationCode('');
      setNewPasswordData({ newPassword: '', confirmPassword: '' });
      setForgotStep(1);
    } catch (err) {
      setMessage({ type: 'error', text: err.message });
    }
  };

  const handleDeleteAccount = async () => {
    // ... (unchanged)
  };

  const handleAddGuest = async (e) => {
    // ... (unchanged)
  };

  const handleDeleteGuest = async (guestId) => {
    // ... (unchanged)
  };

  const handleEditGuest = (guest) => {
    // ... (unchanged)
  };

  const handleUpdateGuest = async (e) => {
    // ... (unchanged)
  };

  const handleLogout = () => {
    localStorage.removeItem('authToken');
    localStorage.removeItem('username');
    setUser(null);
    navigate('/');
  };

  if (loading) {
    return <div className="bg-blue-50 min-h-screen p-6 sm:p-8">Betöltés...</div>;
  }

  return (
    <div className="bg-blue-50 min-h-screen p-6 sm:p-8 relative">
      {message && (
        <div
          className={`fixed top-4 right-4 p-4 rounded-lg shadow-lg text-white ${message.type === 'success' ? 'bg-green-500' : 'bg-red-500'}`}
        >
          {message.text}
        </div>
      )}

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

        {error && <p className="text-red-600 mb-4">{error}</p>}

        {user && (
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
                    <img src={user.profileImage} alt="Profilkép" className="w-full h-full object-cover" />
                  </div>
                </div>
                <h3 className="text-lg sm:text-xl font-semibold text-blue-800 mb-1">{user.username}</h3>
                <p className="text-xs sm:text-sm text-blue-600 mb-4">Regisztrált: {user.dateCreated}</p>
              </div>
              <div className="space-y-4">
                <div className="border-t border-blue-100 pt-4">
                  <label className="block text-sm font-medium mb-1 text-blue-800">Felhasználónév</label>
                  <div className="px-3 sm:px-4 py-2 bg-blue-50 rounded-lg text-blue-700 text-sm sm:text-base">
                    {user.username}
                  </div>
                </div>
                <div>
                  <label className="block text-sm font-medium mb-1 text-blue-800">Email cím</label>
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
                  <h2 className="text-xl sm:text-2xl font-semibold text-blue-800">Elmentett vendég adatok</h2>
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
                {guests.length > 0 ? (
                  guests.map((guest) => (
                    <div
                      key={guest.guestId || Math.random()}
                      className="border border-blue-100 rounded-lg p-3 sm:p-4 hover:border-blue-300 transition-all bg-blue-50/50 hover:shadow-md"
                    >
                      <h3 className="font-semibold mb-2 text-blue-800">
                        {guest.firstName} {guest.lastName}
                      </h3>
                      <p className="text-xs sm:text-sm text-blue-700 mb-1">
                        Email: {guest.email || 'Nincs megadva'}
                      </p>
                      <p className="text-xs sm:text-sm text-blue-700 mb-3">
                        Telefon: {guest.phoneNumber || 'Nincs megadva'}
                      </p>
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
        )}

        {user && (
          <>
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
                  <form className="space-y-4" onSubmit={handleChangePassword}>
                    <div>
                      <label className="block text-sm font-medium mb-1 text-blue-800" htmlFor="current-password">
                        Jelenlegi jelszó
                      </label>
                      <input
                        type="password"
                        id="current-password"
                        className="w-full px-3 sm:px-4 py-2 border border-blue-200 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-all"
                        placeholder="Jelenlegi jelszó"
                        value={passwordData.currentPassword}
                        onChange={(e) => setPasswordData({ ...passwordData, currentPassword: e.target.value })}
                        required
                      />
                      {passwordErrors.currentPassword && (
                        <p className="text-red-600 text-xs mt-1">{passwordErrors.currentPassword}</p>
                      )}
                    </div>
                    <div>
                      <label className="block text-sm font-medium mb-1 text-blue-800" htmlFor="new-password">
                        Új jelszó
                      </label>
                      <input
                        type="password"
                        id="new-password"
                        className="w-full px-3 sm:px-4 py-2 border border-blue-200 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-all"
                        placeholder="Új jelszó"
                        value={passwordData.newPassword}
                        onChange={(e) => setPasswordData({ ...passwordData, newPassword: e.target.value })}
                        required
                      />
                      {passwordErrors.newPassword && (
                        <p className="text-red-600 text-xs mt-1">{passwordErrors.newPassword}</p>
                      )}
                    </div>
                    <div>
                      <label className="block text-sm font-medium mb-1 text-blue-800" htmlFor="confirm-password">
                        Új jelszó megerősítése
                      </label>
                      <input
                        type="password"
                        id="confirm-password"
                        className="w-full px-3 sm:px-4 py-2 border border-blue-200 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-all"
                        placeholder="Új jelszó megerősítése"
                        value={passwordData.confirmPassword}
                        onChange={(e) => setPasswordData({ ...passwordData, confirmPassword: e.target.value })}
                        required
                      />
                      {passwordErrors.confirmPassword && (
                        <p className="text-red-600 text-xs mt-1">{passwordErrors.confirmPassword}</p>
                      )}
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
                    {forgotStep === 1 && 'Adja meg az email címét a jelszó visszaállításához.'}
                    {forgotStep === 2 && 'Adja meg az emailben kapott 6 számjegyű kódot.'}
                    {forgotStep === 3 && 'Adja meg az új jelszót.'}
                  </p>
                  {forgotStep === 1 && (
                    <form className="space-y-4" onSubmit={handleForgotPasswordStep1}>
                      <div>
                        <label className="block text-sm font-medium mb-1 text-blue-800" htmlFor="reset-email">
                          Email cím
                        </label>
                        <input
                          type="email"
                          id="reset-email"
                          className="w-full px-3 sm:px-4 py-2 border border-blue-200 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-all"
                          placeholder="Email cím"
                          value={forgotEmail}
                          onChange={(e) => setForgotEmail(e.target.value)}
                          required
                        />
                      </div>
                      <button
                        type="submit"
                        className="w-full md:w-auto bg-indigo-600 text-white px-4 sm:px-6 py-2 rounded-lg hover:bg-indigo-700 transform hover:scale-105 transition-all duration-200"
                      >
                        Kód küldése
                      </button>
                    </form>
                  )}
                  {forgotStep === 2 && (
                    <form className="space-y-4" onSubmit={handleForgotPasswordStep2}>
                      <div>
                        <label className="block text-sm font-medium mb-1 text-blue-800" htmlFor="verification-code">
                          Ellenőrző kód
                        </label>
                        <input
                          type="text"
                          id="verification-code"
                          className="w-full px-3 sm:px-4 py-2 border border-blue-200 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-all"
                          placeholder="6 számjegyű kód"
                          value={verificationCode}
                          onChange={(e) => setVerificationCode(e.target.value)}
                          maxLength="6"
                          required
                        />
                      </div>
                      <button
                        type="submit"
                        className="w-full md:w-auto bg-indigo-600 text-white px-4 sm:px-6 py-2 rounded-lg hover:bg-indigo-700 transform hover:scale-105 transition-all duration-200"
                      >
                        Kód ellenőrzése
                      </button>
                    </form>
                  )}
                  {forgotStep === 3 && (
                    <form className="space-y-4" onSubmit={handleForgotPasswordStep3}>
                      <div>
                        <label className="block text-sm font-medium mb-1 text-blue-800" htmlFor="new-password-forgot">
                          Új jelszó
                        </label>
                        <input
                          type="password"
                          id="new-password-forgot"
                          className="w-full px-3 sm:px-4 py-2 border border-blue-200 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-all"
                          placeholder="Új jelszó"
                          value={newPasswordData.newPassword}
                          onChange={(e) => setNewPasswordData({ ...newPasswordData, newPassword: e.target.value })}
                          required
                        />
                      </div>
                      <div>
                        <label className="block text-sm font-medium mb-1 text-blue-800" htmlFor="confirm-password-forgot">
                          Új jelszó megerősítése
                        </label>
                        <input
                          type="password"
                          id="confirm-password-forgot"
                          className="w-full px-3 sm:px-4 py-2 border border-blue-200 rounded-lg focus:ring-2 focus:ring-indigo-500 focus:border-indigo-500 transition-all"
                          placeholder="Új jelszó megerősítése"
                          value={newPasswordData.confirmPassword}
                          onChange={(e) => setNewPasswordData({ ...newPasswordData, confirmPassword: e.target.value })}
                          required
                        />
                      </div>
                      <button
                        type="submit"
                        className="w-full md:w-auto bg-indigo-600 text-white px-4 sm:px-6 py-2 rounded-lg hover:bg-indigo-700 transform hover:scale-105 transition-all duration-200"
                      >
                        Jelszó beállítása
                      </button>
                    </form>
                  )}
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
                    <p className="font-medium text-red-600 mb-4 text-sm sm:text-base">
                      Biztosan törölni szeretné a fiókját? Ez a művelet nem visszavonható.
                    </p>
                    <div className="mb-4">
                      <label className="block text-sm font-medium mb-1 text-blue-800" htmlFor="confirm-delete">
                        Írja be a jelszavát a megerősítéshez:
                      </label>
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
          </>
        )}

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
            </div>
          </div>
        )}
      </div>
    </div>
  );
};

export default ProfilePage;