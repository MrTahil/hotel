import React, { useState } from 'react';
import { NavLink, useNavigate } from 'react-router-dom';
import { FontAwesomeIcon } from "@fortawesome/react-fontawesome";
import { faUser } from "@fortawesome/free-regular-svg-icons";
import RegisterModal from './RegisterModal';
import LoginModal from './LoginModal';
import ProfileModal from './ProfileModal';
import { Link } from 'react-router-dom';

function Navbar() {
    const navigate = useNavigate();
    const [activeModal, setActiveModal] = useState(null);
    const [menuOpen, setMenuOpen] = useState(false);
    const [isMobileMenuOpen, setIsMobileMenuOpen] = useState(false);
    const [showProfileModal, setShowProfileModal] = useState(false);

    // Minden rendereléskor ellenőrizzük a localStorage-ot
    const user = localStorage.getItem('username');

    const closeModal = () => setActiveModal(null);

    const handleLogout = () => {
        localStorage.removeItem('authToken');
        localStorage.removeItem('username');
        setMenuOpen(false); // Menü bezárása
        setShowProfileModal(false);
        navigate('/'); // Visszatérés a főoldalra
    };

    const handleMenuItemClick = () => {
        setMenuOpen(false); // Menü bezárása kattintáskor
    };

    return (
        <>
            <nav className="sticky top-0 bg-gradient-to-r from-blue-900 to-blue-800 shadow-lg w-full z-50">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex justify-between items-center">
                    <div className="flex items-center space-x-4">
                        <img
                            src="../logo.png"
                            alt="Logó"
                            className="h-9 rounded-lg shadow-lg"
                            style={{
                                maskImage: "radial-gradient(circle, rgba(0,0,0,1) 60%, rgba(0,0,0,0) 100%)",
                                WebkitMaskImage: "radial-gradient(circle, rgba(0,0,0,1) 60%, rgba(0,0,0,0) 100%)",
                            }}
                        />
                        <button
                            onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
                            className="md:hidden text-white hover:text-blue-200 transition-colors"
                        >
                            {isMobileMenuOpen ? (
                                <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                                </svg>
                            ) : (
                                <svg className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                                    <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M4 6h16M4 12h16M4 18h16" />
                                </svg>
                            )}
                        </button>
                    </div>

                    <div className="hidden md:flex space-x-8">
                        <NavLink to="/" className={({ isActive }) => `text-white hover:text-blue-200 transition-colors ${isActive ? 'text-blue-200' : ''}`}>
                            Kezdőlap
                        </NavLink>
                        <NavLink to="/szobak" className={({ isActive }) => `text-white hover:text-blue-200 transition-colors ${isActive ? 'text-blue-200' : ''}`}>
                            Szobák
                        </NavLink>
                        <NavLink to="/szolgaltatasok" className={({ isActive }) => `text-white hover:text-blue-200 transition-colors ${isActive ? 'text-blue-200' : ''}`}>
                            Szolgáltatások
                        </NavLink>
                        <NavLink to="/programok" className={({ isActive }) => `text-white hover:text-blue-200 transition-colors ${isActive ? 'text-blue-200' : ''}`}>
                            Programok
                        </NavLink>
                        <NavLink to="/rolunk" className={({ isActive }) => `text-white hover:text-blue-200 transition-colors ${isActive ? 'text-blue-200' : ''}`}>
                            Rólunk
                        </NavLink>
                    </div>

                    <div className="relative">
                        <button
                            onClick={() => setMenuOpen(!menuOpen)}
                            className="flex items-center justify-center w-10 h-10 rounded-full bg-white/10 hover:bg-white/20 transition-colors"
                        >
                            <FontAwesomeIcon icon={faUser} className="text-white text-xl" />
                        </button>
                        {menuOpen && (
                            <div className="absolute right-0 mt-2 w-48 bg-white rounded-lg shadow-lg py-2">
                                {user ? (
                                    <>
                                        <Link to="/profile" onClick={handleMenuItemClick}>
                                            <button className="block w-full text-left px-4 py-2 hover:bg-blue-50 text-gray-700">
                                                Profil megnyitása
                                            </button>
                                        </Link>
                                        <button
                                            onClick={() => {
                                                handleLogout();
                                                handleMenuItemClick();
                                            }}
                                            className="block w-full text-left px-4 py-2 hover:bg-blue-50 text-gray-700"
                                        >
                                            Kijelentkezés
                                        </button>
                                    </>
                                ) : (
                                    <>
                                        <button
                                            onClick={() => {
                                                setActiveModal('login');
                                                handleMenuItemClick();
                                            }}
                                            className="block w-full text-left px-4 py-2 hover:bg-blue-50 text-gray-700"
                                        >
                                            Bejelentkezés
                                        </button>
                                        <button
                                            onClick={() => {
                                                setActiveModal('register');
                                                handleMenuItemClick();
                                            }}
                                            className="block w-full text-left px-4 py-2 hover:bg-blue-50 text-gray-700"
                                        >
                                            Regisztráció
                                        </button>
                                    </>
                                )}
                            </div>
                        )}
                    </div>
                </div>

                {isMobileMenuOpen && (
                    <div className="md:hidden absolute top-full left-0 w-full bg-gradient-to-r from-blue-900 to-blue-800 z-50">
                        <div className="px-4 py-2 space-y-2">
                            <NavLink
                                to="/"
                                className={({ isActive }) => `block text-white hover:text-blue-200 py-2 px-4 ${isActive ? 'text-blue-200' : ''}`}
                                onClick={() => setIsMobileMenuOpen(false)}
                            >
                                Kezdőlap
                            </NavLink>
                            <NavLink
                                to="/szobak"
                                className={({ isActive }) => `block text-white hover:text-blue-200 py-2 px-4 ${isActive ? 'text-blue-200' : ''}`}
                                onClick={() => setIsMobileMenuOpen(false)}
                            >
                                Szobák
                            </NavLink>
                            <NavLink
                                to="/szolgaltatasok"
                                className={({ isActive }) => `block text-white hover:text-blue-200 py-2 px-4 ${isActive ? 'text-blue-200' : ''}`}
                                onClick={() => setIsMobileMenuOpen(false)}
                            >
                                Szolgáltatások
                            </NavLink>
                            <NavLink
                                to="/programok"
                                className={({ isActive }) => `block text-white hover:text-blue-200 py-2 px-4 ${isActive ? 'text-blue-200' : ''}`}
                                onClick={() => setIsMobileMenuOpen(false)}
                            >
                                Programok
                            </NavLink>
                            <NavLink
                                to="/rolunk"
                                className={({ isActive }) => `block text-white hover:text-blue-200 py-2 px-4 ${isActive ? 'text-blue-200' : ''}`}
                                onClick={() => setIsMobileMenuOpen(false)}
                            >
                                Rólunk
                            </NavLink>
                        </div>
                    </div>
                )}
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
                    setUser={() => {}}
                />
            )}

            {showProfileModal && (
                <ProfileModal
                    user={user}
                    onClose={() => setShowProfileModal(false)}
                    onLogout={handleLogout}
                />
            )}
        </>
    );
}

export default Navbar;