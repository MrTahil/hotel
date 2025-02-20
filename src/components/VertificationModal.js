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
                    maxLength="6"  // Maximum hosszúság, amit elfogadunk
                    placeholder="Adja meg a kódot"
                />
            </label>
            <button onClick={handleVerify}>Kód ellenőrzése</button>
            <button className="close-btn" onClick={onClose}>Bezárás</button>
        </div>
    );
}

export default VerificationModal;
