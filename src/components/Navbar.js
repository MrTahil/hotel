import React, { useState, useEffect } from 'react';
import { NavLink } from 'react-router-dom';
import RegisterModal from './RegisterModal';
import LoginModal from './LoginModal';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faUser } from "@fortawesome/free-regular-svg-icons";






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
            <nav className="sticky top-0 bg-gradient-to-r from-blue-900 to-blue-800 shadow-lg w-full z-50">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4">
                    <div className="flex justify-between items-center">
                        <div className="flex items-center">
                            <NavLink 
                                to="/" 
                                className="text-white text-xl md:text-2xl font-bold cursor-pointer hover:text-blue-200 transition-colors"
                            >
                                Hmz
                            </NavLink>
                        </div>
                        
                        <div className="flex items-center space-x-4 md:space-x-8">
                            <div className="hidden md:flex space-x-8">
                                <NavLink 
                                    to="/" 
                                    className={({ isActive }) => 
                                        `text-white hover:text-blue-200 transition-colors ${isActive ? 'text-blue-200' : ''}`
                                    }
                                >
                                    Kezdőlap
                                </NavLink>
                                <NavLink 
                                    to="/szobak" 
                                    className={({ isActive }) => 
                                        `text-white hover:text-blue-200 transition-colors ${isActive ? 'text-blue-200' : ''}`
                                    }
                                >
                                    Szobák
                                </NavLink>
                                <NavLink 
                                    to="/szolgaltatasok" 
                                    className={({ isActive }) => 
                                        `text-white hover:text-blue-200 transition-colors ${isActive ? 'text-blue-200' : ''}`
                                    }
                                >
                                    Szolgáltatások
                                </NavLink>
                                <NavLink 
                                    to="/programok" 
                                    className={({ isActive }) => 
                                        `text-white hover:text-blue-200 transition-colors ${isActive ? 'text-blue-200' : ''}`
                                    }
                                >
                                    Programok
                                </NavLink>
                                <NavLink 
                                    to="/rolunk" 
                                    className={({ isActive }) => 
                                        `text-white hover:text-blue-200 transition-colors ${isActive ? 'text-blue-200' : ''}`
                                    }
                                >
                                    Rólunk
                                </NavLink>
                                <NavLink 
                                    to="/kapcsolat" 
                                    className={({ isActive }) => 
                                        `text-white hover:text-blue-200 transition-colors ${isActive ? 'text-blue-200' : ''}`
                                    }
                                >
                                    Kapcsolat
                                </NavLink>
                            </div>

                            <details className="relative">
                                <summary className="list-none cursor-pointer">
                                    <button className="flex items-center justify-center w-10 h-10 rounded-full bg-white/10 hover:bg-white/20 transition-colors">
                                    <FontAwesomeIcon icon={faUser} className="text-white text-xl" />
                                    </button>
                                </summary>
                                <div className="absolute right-0 mt-2 w-48 bg-white rounded-lg shadow-lg py-2">
                                    {user ? (
                                        <>
                                            <div className="px-4 py-2 text-gray-700">{user}</div>
                                            <button
                                                onClick={handleLogout}
                                                className="block w-full text-left px-4 py-2 hover:bg-blue-50 text-gray-700"
                                            >
                                                Kijelentkezés
                                            </button>
                                        </>
                                    ) : (
                                        <>
                                            <button
                                                onClick={() => setActiveModal('login')}
                                                className="block w-full text-left px-4 py-2 hover:bg-blue-50 text-gray-700"
                                            >
                                                Bejelentkezés
                                            </button>
                                            <button
                                                onClick={() => setActiveModal('register')}
                                                className="block w-full text-left px-4 py-2 hover:bg-blue-50 text-gray-700"
                                            >
                                                Regisztráció
                                            </button>
                                        </>
                                    )}
                                </div>
                            </details>
                        </div>
                    </div>
                </div>
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