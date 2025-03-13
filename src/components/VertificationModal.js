import axios from 'axios';
import React, { useState } from 'react';

function VerificationModal({ email, onClose, onSuccess }) {
    const [code, setCode] = useState('');  // A kód változóban történő tárolása, stringként
    const [errorMessage, setErrorMessage] = useState('');  // Hibaüzenet
    const [successMessage, setSuccessMessage] = useState('');  // Sikerüzenet

    const handleVerify = async () => {
        setErrorMessage(null);
        setSuccessMessage(null);

        // Kód érvényesítése
        if (!code) {
            setErrorMessage('Kérjük, adja meg a 2FA kódot!');
            return;
        }

        try {
            console.log(email + ",")
            console.log(String(code) + ",")
            // A kód és email stringként történő elküldése
            const response = await axios.post('https://localhost:7047/UserAccounts/Verify2FA', 
                {
                    email: email,  // Email
                    code: String(code) // A kódot stringként küldjük
                }
            );

            console.log(response);

            // Ha a válasz státuszkódja 200
            if (response.status === 200) {
                setSuccessMessage('Sikeres aktiválás, mostmár bejelentkezhetsz!');
                onSuccess();  // A sikeres aktiválás után visszatérünk
            } else {
                setErrorMessage(response.data.message || 'Hiba történt a kód ellenőrzésekor!');
            }
        } catch (error) {
            setErrorMessage('Hálózati hiba történt. Próbáld újra később!');
        }
    };

    return (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
            <div className="bg-white rounded-lg shadow-xl p-8 max-w-md w-full relative">
                <div className="space-y-6">
                    <div className="text-center">
                        <h3 className="text-2xl font-bold text-gray-900 mb-2">2FA Kód</h3>
                        <p className="text-gray-600">Kérjük add meg az emailedre küldött ellenőrző kódot</p>
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

                    <div>
                        <label className="block text-sm font-medium text-gray-700 mb-1">
                            6 számjegyű kód
                        </label>
                        <input
                            type="text"
                            className="w-full px-4 py-2 border border-gray-300 rounded-lg focus:ring-2 focus:ring-blue-500 focus:border-blue-500 text-center text-xl"
                            value={code}
                            onChange={(e) => setCode(e.target.value)}
                            maxLength="6"
                            placeholder="••••••"
                        />
                    </div>

                    <button
                        onClick={handleVerify}
                        className="w-full bg-gradient-to-r from-blue-900 to-blue-800 text-white py-2 px-4 rounded-lg hover:from-blue-800 hover:to-blue-700 transition-all duration-200"
                    >
                        Kód ellenőrzése
                    </button>
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

export default VerificationModal;
