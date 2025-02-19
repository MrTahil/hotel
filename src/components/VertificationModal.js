import React, { useState } from 'react';
import '../styles/Modal.css';

function VerificationModal({ email, onClose, onSuccess }) {
    const [code, setCode] = useState('');
    const [errorMessage, setErrorMessage] = useState('');

    const handleVerify = async (e) => {
        e.preventDefault();
        setErrorMessage('');

        try {
            const response = await fetch('https://localhost:7047/UserAccounts/Verify2FA', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ email: email, code: code }) // JSON helyes küldése
            });

            const data = await response.json();

            if (response.ok) {
                onSuccess(); // Ha sikeres, akkor továbblép
            } else {
                setErrorMessage(data.message || 'Helytelen kód, próbáld újra!');
            }
        } catch (error) {
            setErrorMessage('Hálózati hiba történt. Próbáld újra később!');
        }
    };

    return (
        <div className="modal-backdrop">
            <div className="modal">
                <p className="title">Email hitelesítés</p>
                <p className="message">Írd be az emailben kapott kódot.</p>
                {errorMessage && <p className="error-message">{errorMessage}</p>}
                <form onSubmit={handleVerify}>
                    <label>
                        <input
                            type="text"
                            className="input"
                            value={code}
                            onChange={(e) => setCode(e.target.value)}
                            required
                        />
                        <span>Hitelesítő kód</span>
                    </label>
                    <button className="submit" type="submit">Megerősítés</button>
                </form>
                <button className="close-btn" onClick={onClose}>Bezárás</button>
            </div>
        </div>
    );
}

export default VerificationModal;
