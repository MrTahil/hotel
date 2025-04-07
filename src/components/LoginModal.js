import React, { useState } from 'react';
import '../styles/Modal.css';

function LoginModal({ onClose, switchToRegister, setUser }) {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  const handleLogin = async (e) => {
    e.preventDefault();
    setError('');

    try {
      const response = await fetch(process.env.REACT_APP_API_BASE_URL+'/UserAccounts/Login', {
        method: 'POST',
        headers: { 'Content-Type': 'application/json' },
        body: JSON.stringify({ username, password }),
      });

      if (!response.ok) {
        throw new Error('Hibás felhasználónév vagy jelszó');
      }

      const data = await response.json();
      console.log('Sikeres bejelentkezés:', data);
      onClose();
      localStorage.setItem('authToken', data.accessToken);
      console.log(data.accessToken)
      localStorage.setItem('username', username);
      localStorage.setItem('email', data.email);
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
              <input
              placeholder='Ezt neked kell tudni...'
                type="password"
                className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500"
                value={password}
                onChange={(e) => setPassword(e.target.value)}
                required
              />
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