import React, { useState, useEffect } from 'react';
import { NavLink } from 'react-router-dom';
import RegisterModal from './RegisterModal';
import LoginModal from './LoginModal';
import '../styles/Navbar.css';

function Navbar() {
    const [activeModal, setActiveModal] = useState(null);
    const [user, setUser] = useState(null);

    const closeModal = () => setActiveModal(null);

    useEffect(() => {
        const loggedUser = localStorage.getItem('username');
        if (loggedUser) {
            setUser(loggedUser);
        }
    }, []);

    const handleLogout = () => {
        localStorage.removeItem('authToken');
        localStorage.removeItem('username');
        setUser(null);
    };

    return (
        <>
            <nav className="navbar">
                <ul className="flexnav">
                    <li>
                        <NavLink style={({ isActive }) => ({
                            backgroundColor: isActive ? "#A9D6E5" : "",
                            transform: isActive ? "scale(1.05)" : ""
                        })} to="/">Kezdőlap</NavLink>
                    </li>
                    <li>
                        <NavLink style={({ isActive }) => ({
                            backgroundColor: isActive ? "#A9D6E5" : "",
                            transform: isActive ? "scale(1.05)" : ""
                        })} to="/szobak">Szobák</NavLink>
                    </li>
                    <li>
                        <NavLink style={({ isActive }) => ({
                            backgroundColor: isActive ? "#A9D6E5" : "",
                            transform: isActive ? "scale(1.05)" : ""
                        })} to="/szolgaltatasok">Szolgáltatások</NavLink>
                    </li>
                    <li>
                        <NavLink style={({ isActive }) => ({
                            backgroundColor: isActive ? "#A9D6E5" : "",
                            transform: isActive ? "scale(1.05)" : ""
                        })} to="/programok">Programok</NavLink>
                    </li>
                    <li>
                        <NavLink style={({ isActive }) => ({
                            backgroundColor: isActive ? "#A9D6E5" : "",
                            transform: isActive ? "scale(1.05)" : ""
                        })} to="/rolunk">Rólunk</NavLink>
                    </li>
                    <li>
                        <NavLink style={({ isActive }) => ({
                            backgroundColor: isActive ? "#A9D6E5" : "",
                            transform: isActive ? "scale(1.05)" : ""
                        })} to="/kapcsolat">Kapcsolat</NavLink>
                    </li>
                    <div className="auth-buttons">
                        {user ? (
                            <li className="user-name">
                                <span>{user}</span>
                                <div className="logout-dropdown">
                                    <button onClick={handleLogout}>Kijelentkezés</button>
                                </div>
                            </li>
                        ) : (
                            <>
                                <li>
                                    <button onClick={() => setActiveModal('login')}>Bejelentkezés</button>
                                </li>
                                <li>
                                    <button onClick={() => setActiveModal('register')}>Regisztráció</button>
                                </li>
                            </>
                        )}
                    </div>
                </ul>
            </nav>

            {activeModal === 'register' && (
                <RegisterModal 
                    onClose={closeModal} 
                    switchToLogin={() => setActiveModal('login')} 
                />
            )}

            {activeModal === 'login' && (
                <LoginModal 
                    onClose={closeModal} 
                    switchToRegister={() => setActiveModal('register')} 
                    setUser={setUser} 
                />
            )}
        </>
    );
}

export default Navbar;
