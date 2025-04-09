import React, { useState, useEffect } from 'react';
import '../styles/Modal.css';
import VerificationModal from '../components/VertificationModal';
import axios from 'axios';

function RegisterModal({ onClose, switchToLogin }) {
  const [username, setUsername] = useState('');
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [confirmPassword, setConfirmPassword] = useState('');
  const [errorMessage, setErrorMessage] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const [showVerification, setShowVerification] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);
  const [isLoading, setIsLoading] = useState(false);

  const validateEmail = (email) => {
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    return emailRegex.test(email);
  };

  const validateUsername = (username) => {
    const usernameRegex = /^[a-zA-Z0-9_áéíóöőúüűÁÉÍÓÖŐÚÜŰ]{3,20}$/;
    return usernameRegex.test(username);
  };

  const validatePassword = (password) => {
    const passwordRegex = /^(?=.*[A-Z])(?=.*[!@#$%^&*()_+\-=\[\]{}|;:,.<>?/~`]).{8,}$/;
    return passwordRegex.test(password);
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setErrorMessage('');
    setSuccessMessage('');
    setIsLoading(true);

    if (!username.trim()) {
      setErrorMessage('A felhasználónév nem lehet üres!');
      setIsLoading(false);
      return;
    }

    if (!validateUsername(username)) {
      setErrorMessage('A felhasználónév túl rövid vagy csak betűket (beleértve ékezeteseket is), számokat és aláhúzást tartalmazhat, és 3-20 karakter hosszú kell legyen!');
      setIsLoading(false);
      return;
    }

    if (!validateEmail(email)) {
      setErrorMessage('Érvénytelen e-mail cím formátum!');
      setIsLoading(false);
      return;
    }

    if (!validatePassword(password)) {
      setErrorMessage('A jelszónak legalább 8 karakter hosszúnak kell lennie, és tartalmaznia kell legalább egy nagybetűt és egy speciális karaktert!');
      setIsLoading(false);
      return;
    }

    if (password !== confirmPassword) {
      setErrorMessage('A megadott jelszavak nem egyeznek!');
      setIsLoading(false);
      return;
    }

    try {
      const response = await axios.post(
        `${process.env.REACT_APP_API_BASE_URL}/UserAccounts/Register`,
        { UserName: username, Email: email, Password: password },
        { headers: { 'Content-Type': 'application/json' } }
      );

      console.log('Szerver válasz:', response.data, 'Státusz:', response.status);

      if (response.status === 201) {
        setSuccessMessage('Sikeres regisztráció! Hamarosan átirányítunk az ellenőrzéshez...');
        localStorage.setItem('username', username);
        localStorage.setItem('email', email);
        setTimeout(() => {
          setShowVerification(true);
          setIsLoading(false);
        }, 3000); // Show success message for 3 seconds before verification
      } else {
        let errorMsg = 'Ismeretlen hiba történt a regisztráció során!';
        if (response.status === 400) {
          if (response.data.includes('Név vagy Email már használatban van')) {
            errorMsg = 'A felhasználónév vagy email már foglalt!';
          } else {
            errorMsg = response.data || 'Hibás kérés!';
          }
        } else if (response.status === 500) {
          errorMsg = response.data || 'Szerverhiba történt!';
        }
        setErrorMessage(errorMsg);
        setIsLoading(false);
      }
    } catch (error) {
      setErrorMessage('Hálózati hiba történt. Kérjük próbáld újra később!');
      console.error('Hiba részletei:', error);
      setIsLoading(false);
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
        ) : successMessage ? (
          <div className="text-center py-8">
            <svg 
              className="w-16 h-16 text-green-500 mx-auto mb-4 animate-bounce" 
              fill="none" 
              stroke="currentColor" 
              viewBox="0 0 24 24"
            >
              <path 
                strokeLinecap="round" 
                strokeLinejoin="round" 
                strokeWidth="2" 
                d="M9 12l2 2 4-4m6 2a9 9 0 11-18 0 9 9 0 0118 0z" 
              />
            </svg>
            <h3 className="text-2xl font-bold text-green-600 mb-4">Sikeres regisztráció!</h3>
            <p className="text-gray-600 mb-4">
              {successMessage}
            </p>
            <div className="flex justify-center">
              <svg 
                className="animate-spin h-5 w-5 text-blue-500" 
                viewBox="0 0 24 24"
              >
                <circle 
                  className="opacity-25" 
                  cx="12" 
                  cy="12" 
                  r="10" 
                  stroke="currentColor" 
                  strokeWidth="4" 
                  fill="none" 
                />
                <path 
                  className="opacity-75" 
                  fill="currentColor" 
                  d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" 
                />
              </svg>
            </div>
          </div>
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

            <div className="space-y-4">
              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Felhasználónév
                </label>
                <input
                  placeholder="PetőfiSanyi"
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
                  placeholder="example@gmail.com"
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
                <div className="relative">
                  <input
                    placeholder="Ezt jól jegyezd meg!"
                    required
                    type={showPassword ? 'text' : 'password'}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                    value={password}
                    onChange={(e) => setPassword(e.target.value)}
                  />
                  <button
                    type="button"
                    onClick={() => setShowPassword(!showPassword)}
                    className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-500 hover:text-blue-600 focus:outline-none"
                  >
                    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      {showPassword ? (
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532l3.29 3.29M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0 01-4.132 5.411m0 0L21 21" />
                      ) : (
                        <>
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542-7z" />
                        </>
                      )}
                    </svg>
                  </button>
                </div>
              </div>

              <div>
                <label className="block text-sm font-medium text-gray-700 mb-1">
                  Jelszó megerősítése
                </label>
                <div className="relative">
                  <input
                    placeholder="És ne felejtsd el..."
                    required
                    type={showConfirmPassword ? 'text' : 'password'}
                    className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                    value={confirmPassword}
                    onChange={(e) => setConfirmPassword(e.target.value)}
                  />
                  <button
                    type="button"
                    onClick={() => setShowConfirmPassword(!showConfirmPassword)}
                    className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-500 hover:text-blue-600 focus:outline-none"
                  >
                    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      {showConfirmPassword ? (
                        <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532l3.29 3.29M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0 01-4.132 5.411m0 0L21 21" />
                      ) : (
                        <>
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                          <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542-7z" />
                        </>
                      )}
                    </svg>
                  </button>
                </div>
              </div>
            </div>

            <button
              type="submit"
              className="w-full bg-gradient-to-r from-blue-900 to-blue-800 text-white py-2 px-4 rounded-lg hover:from-blue-800 hover:to-blue-700 transition-all duration-200 flex items-center justify-center"
              disabled={isLoading}
            >
              {isLoading ? (
                <>
                  <svg className="animate-spin h-5 w-5 mr-2 text-white" viewBox="0 0 24 24">
                    <circle className="opacity-25" cx="12" cy="12" r="10" stroke="currentColor" strokeWidth="4" fill="none" />
                    <path className="opacity-75" fill="currentColor" d="M4 12a8 8 0 018-8V0C5.373 0 0 5.373 0 12h4zm2 5.291A7.962 7.962 0 014 12H0c0 3.042 1.135 5.824 3 7.938l3-2.647z" />
                  </svg>
                  Feldolgozás...
                </>
              ) : (
                'Regisztráció'
              )}
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