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
            const response = await fetch('https://localhost:7047/UserAccounts/Login', {
                method: 'POST',
                headers: { 'Content-Type': 'application/json' },
                body: JSON.stringify({ username, password })
            });

            if (!response.ok) {
                throw new Error('Hibás felhasználónév vagy jelszó');
            }

            const data = await response.json();
            console.log('Sikeres bejelentkezés:', data);
            onClose();
            localStorage.setItem('authToken', data.token);
            localStorage.setItem('username', username);
            setUser(username);
        } catch (error) {
            setError(error.message);
        }
    };

    return (
        <div className="modal-backdrop">
            <div className="modal">
                <h2 className='title'>Bejelentkezés</h2>
                <p className="message">Jelentkezz be, böngéssz, foglalj és utazz szabadon!</p>
                {error && <p className="error-message">{error}</p>}
                <form onSubmit={handleLogin}>
                    <label>
                        <span>Felhasználónév</span>
                        <input type="text" value={username} onChange={(e) => setUsername(e.target.value)} required />
                    </label>
                    <label>
                        <span>Jelszó</span>
                        <input type="password" value={password} onChange={(e) => setPassword(e.target.value)} required />
                    </label>
                    <button type="submit">Bejelentkezés</button>
                    <p className="signin">
                        Nincs fiókod? <a href="#" onClick={(e) => { e.preventDefault(); switchToRegister(); }}>Regisztrálj!</a>
                    </p>
                </form>
                <button className="close-btn" onClick={onClose}>Bezárás</button>
            </div>
        </div>
    );
}

export default LoginModal;
