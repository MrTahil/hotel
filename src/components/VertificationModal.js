import React, { useState } from 'react';

function VerificationModal({ email, onClose, onSuccess }) {
    const [code, setCode] = useState('');
    const [errorMessage, setErrorMessage] = useState('');
    const [successMessage, setSuccessMessage] = useState('');

    const handleVerify = async () => {
        try {
            const response = await fetch('https://localhost:7047/UserAccounts/Verify2FA', {
                method: 'POST', // Post metódus
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email, code }) // A megfelelő formátum
            });
            

            const data = await response.json();

            if (response.ok) {
                setSuccessMessage('Sikeres aktiválás, mostmár bejelentkezhetsz!');
                onSuccess();
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
                    value={code}
                    onChange={(e) => setCode(e.target.value)}
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
