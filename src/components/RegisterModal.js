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

    const validateEmail = (email) => {
        const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
        return emailRegex.test(email);
    };

    const validatePassword = (password) => {
        const passwordRegex = /^(?=.*[A-Z])(?=.*[\W_]).{6,}$/;
        return passwordRegex.test(password);
    };

    const handleSubmit = async (e) => {
        e.preventDefault();
        setErrorMessage('');

        if (!username) {
            setErrorMessage('A felhasználónév nem lehet üres!');
            return;
        }

        if (!validateEmail(email)) {
            setErrorMessage('Érvénytelen e-mail cím!');
            return;
        }

        if (!validatePassword(password)) {
            setErrorMessage('A jelszónak legalább 6 karakterből kell állnia, tartalmaznia kell egy nagybetűt és egy speciális karaktert!');
            return;
        }

        if (password !== confirmPassword) {
            setErrorMessage('A jelszavak nem egyeznek!');
            return;
        }

        try {
            const response = await fetch('https://localhost:7047/UserAccounts/Register', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ username, email, password }),
            });

            const data = await response.json();

            if (response.ok) {
                setSuccessMessage('Sikeres regisztráció! Kérlek ellenőrizd az emailed és add meg a kódot!');
                setTimeout(() => setShowVerification(true), 2000);
            } else {
                setErrorMessage(data.message || 'Hiba történt a regisztráció során!');
            }
        } catch (error) {
            setErrorMessage('Hálózati hiba történt. Próbáld újra később!');
        }
    };

    const handleVerificationSuccess = () => {
        setShowVerification(false);
        switchToLogin();
    };

    return (
        <div className="modal-backdrop">
            <div className="modal">
                {showVerification ? (
                    <VerificationModal email={email} onClose={onClose} onSuccess={handleVerificationSuccess} />
                ) : (
                    <form className="form" onSubmit={handleSubmit}>
                        <p className="title">Regisztráció</p>
                        <p className="message">Regisztrálj, böngéssz, foglalj és utazz szabadon!</p>
                        {errorMessage && <p className="error-message">{errorMessage}</p>}
                        {successMessage && <p className="success-message">{successMessage}</p>}
                        <label>
                            <input required type="text" className="input" value={username} onChange={(e) => setUsername(e.target.value)} />
                            <span>Felhasználónév</span>
                        </label>
                        <label>
                            <input required type="email" className="input" value={email} onChange={(e) => setEmail(e.target.value)} />
                            <span>Email</span>
                        </label>
                        <label>
                            <input required type="password" className="input" value={password} onChange={(e) => setPassword(e.target.value)} />
                            <span>Jelszó</span>
                        </label>
                        <label>
                            <input required type="password" className="input" value={confirmPassword} onChange={(e) => setConfirmPassword(e.target.value)} />
                            <span>Jelszó megerősítése</span>
                        </label>
                        <button className="submit" type="submit">Regisztráció</button>
                        <p className="signin">
                            Már van egy fiókod? <a href="#" onClick={(e) => { e.preventDefault(); switchToLogin(); }}>Jelentkezz be!</a>
                        </p>
                    </form>
                )}
                <button className="close-btn" onClick={onClose}>Bezárás</button>
            </div>
        </div>
    );
}

export default RegisterModal;
