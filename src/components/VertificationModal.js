import React, { useState } from 'react';

function VerificationModal({ email, onClose, onSuccess }) {
    const [code, setCode] = useState('');  // A kód változóban történő tárolása
    const [errorMessage, setErrorMessage] = useState('');  // Hibaüzenet
    const [successMessage, setSuccessMessage] = useState('');  // Sikerüzenet

    const handleVerify = async () => {
        // Kód érvényesítése
        if (!code) {
            setErrorMessage('Kérjük, adja meg a 2FA kódot!');
            return;
        }

        try {
            // Az elküldött kódot tároljuk változóban, itt elküldjük a backendre
            const response = await fetch('https://localhost:7047/UserAccounts/Verify2FA', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email, code })  // A változóban lévő kódot elküldjük
            });

            const data = await response.json();

            // Ha a válasz sikeres
            if (response.ok) {
                setSuccessMessage('Sikeres aktiválás, mostmár bejelentkezhetsz!');
                onSuccess();  // A sikeres aktiválás után visszatérünk
            } else {
                setErrorMessage(data.message || 'Hiba történt a kód ellenőrzésekor!');
            }
        } catch (error) {
            setErrorMessage('Hálózati hiba történt. Próbáld újra később!');
        }
    };

    return (
        <div className="verification-modal">
            <h2>2FA Kód</h2>
            {errorMessage && <p className="error-message">{errorMessage}</p>}
            {successMessage && <p className="success-message">{successMessage}</p>}
            <label>
                <input
                    type="text"
                    className="input"
                    value={code}  // A felhasználó által beírt kódot tároljuk
                    onChange={(e) => setCode(e.target.value)}  // Frissítjük a kód változót
                    maxLength="6"
                    placeholder="Adja meg a kódot"
                />
            </label>
            <button onClick={handleVerify}>Kód ellenőrzése</button>
            <button className="close-btn" onClick={onClose}>Bezárás</button>
        </div>
    );
}

export default VerificationModal;
