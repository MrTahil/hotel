import React, { useState } from 'react';
import '../styles/Modal.css';
import VerificationModal from '../components/VertificationModal';

function RegisterModal({ onClose, switchToLogin }) {
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [errorMessage, setErrorMessage] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const [showVerification, setShowVerification] = useState(false);

  // Email validáció
  const validateEmail = (email) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  };

  // Jelszó validáció
  const validatePassword = (password) => {
    const passwordRegex = /^(?=.*[A-Z])(?=.*[\W_]).{6,}$/;
    return passwordRegex.test(password);
  };

  // Felhasználónév validáció
  const validateUsername = (username) => {
    const usernameRegex = /^[a-zA-Z0-9_]{3,20}$/;
    return usernameRegex.test(username);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setErrorMessage('');
    setSuccessMessage('');

    if (!username.trim()) {
      setErrorMessage('A felhasználónév nem lehet üres!');
      return;
    }

    if (!validateUsername(username)) {
      setErrorMessage('A felhasználónév csak betűket, számokat és aláhúzást tartalmazhat, és 3-20 karakter hosszú kell legyen!');
      return;
    }

    if (!validateEmail(email)) {
      setErrorMessage('Érvénytelen e-mail cím formátum!');
      return;
    }

    if (!validatePassword(password)) {
      setErrorMessage('A jelszónak legalább 6 karakter hosszúnak kell lennie, tartalmaznia kell legalább egy nagybetűt és egy speciális karaktert!');
      return;
    }

    if (password !== confirmPassword) {
      setErrorMessage('A megadott jelszavak nem egyeznek!');
      return;
    }

    try {
      const response = await fetch(`${process.env.REACT_APP_API_BASE_URL}/UserAccounts/Register`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ UserName: username, email, password }),
      });

      const data = await response.json();
      console.log('Szerver válasz:', data);

      if (response.ok) {
        setSuccessMessage('Sikeres regisztráció! Kérlek ellenőrizd az emailed és add meg a kódot!');
        localStorage.setItem('username', username);
        localStorage.setItem('email', email);
        setTimeout(() => setShowVerification(true), 2000);
      } else {
        let errorMsg = 'Ismeretlen hiba történt a regisztráció során!';
        if (data.errors && data.errors.UserName) {
          errorMsg = data.errors.UserName[0];
        } else if (data.message) {
          errorMsg = data.message;
        }
        setErrorMessage(errorMsg);
      }
    } catch (error) {
      setErrorMessage('Hálózati hiba történt. Kérjük próbáld újra később!');
      console.error('Hiba részletei:', error);
    }
  };

  const handleVerificationSuccess = () => {
    setShowVerification(false);
    switchToLogin();
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg shadow-xl p-8 max-w-md w-full relative">
        {showVerification ? (
          <VerificationModal email={email} onClose={onClose} onSuccess={handleVerificationSuccess} />
        ) : (
          <form className="space-y-6" onSubmit={handleSubmit}>
            <div className="text-center">
              <h3 className="text-2xl font-bold text-gray-900 mb-2">Regisztráció</h3>
              <p className="text-gray-600">Regisztrálj, böngéssz, foglalj és utazz szabadon!</p>
            </div>

            {errorMessage && (
              <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
                {errorMessage}
              </div>
            )}

            {successMessage && (
              <div className="bg-blue-100 border border-blue-400 text-blue-700 px-4 py-3 rounded">
                {successMessage}
              </div>
            )}

            <div className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Felhasználónév
                </label>
                <input
                  required
                  type="text"
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  value={username}
                  onChange={(e) => setUsername(e.target.value)}
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Email
                </label>
                <input
                  required
                  type="email"
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  value={email}
                  onChange={(e) => setEmail(e.target.value)}
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Jelszó
                </label>
                <input
                  required
                  type="password"
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                />
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Jelszó megerősítése
                </label>
                <input
                  required
                  type="password"
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  value={confirmPassword}
                  onChange={(e) => setConfirmPassword(e.target.value)}
                />
              </div>
            </div>

            <button
              type="submit"
              className="w-full bg-gradient-to-r from-blue-900 to-blue-800 text-white py-2 px-4 rounded-lg hover:from-blue-800 hover:to-blue-700 transition-all duration-200"
            >
              Regisztráció
            </button>

            <p className="text-center text-sm text-gray-600">
              Már van fiókod?{' '}
              <button
                type="button"
                onClick={switchToLogin}
                className="text-blue-800 hover:text-blue-700 font-semibold"
              >
                Jelentkezz be!
              </button>
            </p>
          </form>
        )}

        <button
          onClick={onClose}
          className="absolute top-4 right-4 text-gray-500 hover:text-gray-700"
        >
          <svg className="w-6 h-6" fill="none" stroke="currentColor" viewBox="0 0 24 24">
            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
          </svg>
        </button>
      </div>
    </div>
  );
}

export default RegisterModal;