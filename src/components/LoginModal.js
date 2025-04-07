import React, { useState } from 'react';
import '../styles/Modal.css';

function LoginModal({ onClose, switchToRegister, setUser }) {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [showPassword, setShowPassword] = useState(false); // New state for password visibility

  const minPasswordLength = 8; // A minimális jelszóhossz

  const handleLogin = async (e) => {
    e.preventDefault();
    setError('');

    // Jelszó hosszának ellenőrzése
    if (password.length < minPasswordLength) {
      setError(`A jelszó túl rövid. Legalább ${minPasswordLength} karakter hosszú kell legyen.`);
      return;
    }

    try {
      const response = await fetch(`${process.env.REACT_APP_API_BASE_URL}/UserAccounts/Login`, {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ UserName: username, Password: password }), // Adjusted to match backend DTO
      });

      if (!response.ok) {
        throw new Error('Hibás felhasználónév vagy jelszó');
      }

      const data = await response.json();
      console.log('Sikeres bejelentkezés:', data);
      onClose();
      localStorage.setItem('authToken', data.accessToken);
      localStorage.setItem('username', username);
      // Note: Backend doesn't return email in Login response, so this line may need adjustment
      // localStorage.setItem('email', data.email); // Commented out unless backend provides email
      setUser(username);
    } catch (error) {
      setError(error.message);
    }
  };

  return (
    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
      <div className="bg-white rounded-lg shadow-xl p-8 max-w-md w-full relative">
        <div className="space-y-6">
          <div className="text-center">
            <h3 className="text-2xl font-bold text-gray-900 mb-2">Bejelentkezés</h3>
            <p className="text-gray-600">Jelentkezz be, böngéssz, foglalj és utazz szabadon!</p>
          </div>

          {error && (
            <div className="bg-red-100 border border-red-400 text-red-700 px-4 py-3 rounded">
              {error}
            </div>
          )}

          <form className="space-y-4" onSubmit={handleLogin}>
            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Felhasználónév
              </label>
              <input
                type="text"
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                value={username}
                onChange={(e) => setUsername(e.target.value)}
                required
              />
            </div>

            <div>
              <label className="block text-sm font-medium text-gray-700 mb-1">
                Jelszó
              </label>
              <div className="relative">
                <input
                  placeholder="Ezt neked kell tudni..."
                  type={showPassword ? 'text' : 'password'}
                  className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                  value={password}
                  onChange={(e) => setPassword(e.target.value)}
                  required
                />
                <button
                  type="button"
                  onClick={() => setShowPassword(!showPassword)}
                  className="absolute right-3 top-1/2 transform -translate-y-1/2 text-gray-500 hover:text-blue-600 focus:outline-none"
                >
                  {showPassword ? (
                    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M13.875 18.825A10.05 10.05 0 0112 19c-4.478 0-8.268-2.943-9.543-7a9.97 9.97 0 011.563-3.029m5.858.908a3 3 0 114.243 4.243M9.878 9.878l4.242 4.242M9.88 9.88l-3.29-3.29m7.532 7.532l3.29 3.29M3 3l3.59 3.59m0 0A9.953 9.953 0 0112 5c4.478 0 8.268 2.943 9.543 7a10.025 10.025 0 01-4.132 5.411m0 0L21 21" />
                    </svg>
                  ) : (
                    <svg className="w-5 h-5" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M15 12a3 3 0 11-6 0 3 3 0 016 0z" />
                      <path strokeLinecap="round" strokeLinejoin="round" strokeWidth="2" d="M2.458 12C3.732 7.943 7.523 5 12 5c4.478 0 8.268 2.943 9.542 7-1.274 4.057-5.064 7-9.542 7-4.477 0-8.268-2.943-9.542-7z" />
                    </svg>
                  )}
                </button>
              </div>
            </div>

            <button
              type="submit"
              className="w-full bg-gradient-to-r from-blue-900 to-blue-800 text-white py-2 px-4 rounded-lg hover:from-blue-800 hover:to-blue-700 transition-all duration-200"
            >
              Bejelentkezés
            </button>
          </form>

          <p className="text-center text-sm text-gray-600">
            Nincs fiókod?{' '}
            <button
              type="button"
              onClick={switchToRegister}
              className="text-blue-800 hover:text-blue-700 font-semibold"
            >
              Regisztrálj!
            </button>
          </p>
        </div>

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

export default LoginModal;