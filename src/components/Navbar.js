import React, { useState, useEffect, useRef } from 'react';
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
    const menuRef = useRef(null);

    const user = localStorage.getItem('username');
    const token = localStorage.getItem('authToken');

    // Function to check if token is expired
    const isTokenExpired = () => {
        if (!token) return true; // No token means it's "expired"
        try {
            const decodedToken = JSON.parse(atob(token.split('.')[1])); // Decode JWT payload
            const currentTime = Math.floor(Date.now() / 1000); // Current time in seconds
            return decodedToken.exp < currentTime; // Check if expiration time is in the past
        } catch (error) {
            console.error('Error decoding token:', error);
            return true; // If token is invalid, treat it as expired
        }
    };

    const closeModal = () => setActiveModal(null);

    const handleLogout = () => {
        localStorage.removeItem('authToken');
        localStorage.removeItem('username');
        setMenuOpen(false);
        setShowProfileModal(false);
        navigate('/');
    };

    const handleMenuItemClick = () => {
        setMenuOpen(false);
    };

    const handleProfileClick = () => {
        if (isTokenExpired()) {
            setActiveModal('login'); // Redirect to login if token is expired
            setMenuOpen(false);
        } else {
            navigate('/profile'); // Navigate to profile if token is valid
            handleMenuItemClick();
        }
    };

    useEffect(() => {
        const handleClickOutside = (event) => {
            if (menuRef.current && !menuRef.current.contains(event.target)) {
                setMenuOpen(false);
            }
        };
        document.addEventListener('click', handleClickOutside);
        return () => {
            document.removeEventListener('click', handleClickOutside);
        };
    }, []);

    return (
        <>
            <nav className="sticky top-0 bg-gradient-to-r from-blue-900 via-blue-800 to-blue-700 shadow-xl w-full z-50 transition-all duration-300">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 py-4 flex justify-between items-center">
                    <div className="flex items-center space-x-4">
                        <Link to="/" onClick={() => setIsMobileMenuOpen(false)}>
                            <img
                                src="../logo.png"
                                alt="Logó"
                                className="h-10 rounded-full shadow-lg transform hover:scale-105 transition-transform duration-300 cursor-pointer"
                                style={{
                                    maskImage: "radial-gradient(circle, rgba(0,0,0,1) 70%, rgba(0,0,0,0) 100%)",
                                    WebkitMaskImage: "radial-gradient(circle, rgba(0,0,0,1) 70%, rgba(0,0,0,0) 100%)",
                                }}
                            />
                        </Link>
                        <button
                            onClick={() => setIsMobileMenuOpen(!isMobileMenuOpen)}
                            className="md:hidden text-white hover:text-blue-300 transition-colors duration-200 p-1 rounded-full hover:bg-white/10"
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

                    <div className="hidden md:flex space-x-6">
                        <NavLink
                            to="/"
                            className={({ isActive }) =>
                                `relative text-white hover:text-blue-300 hover:bg-white/10 px-4 py-2 rounded-lg transition-all duration-300 ease-in-out ${
                                    isActive ? 'text-blue-300 bg-white/20' : ''
                                }`
                            }
                        >
                            Kezdőlap
                        </NavLink>
                        <NavLink
                            to="/szobak"
                            className={({ isActive }) =>
                                `relative text-white hover:text-blue-300 hover:bg-white/10 px-4 py-2 rounded-lg transition-all duration-300 ease-in-out ${
                                    isActive ? 'text-blue-300 bg-white/20' : ''
                                }`
                            }
                        >
                            Szobák
                        </NavLink>
                        <NavLink
                            to="/szolgaltatasok"
                            className={({ isActive }) =>
                                `relative text-white hover:text-blue-300 hover:bg-white/10 px-4 py-2 rounded-lg transition-all duration-300 ease-in-out ${
                                    isActive ? 'text-blue-300 bg-white/20' : ''
                                }`
                            }
                        >
                            Szolgáltatások
                        </NavLink>
                        <NavLink
                            to="/programok"
                            className={({ isActive }) =>
                                `relative text-white hover:text-blue-300 hover:bg-white/10 px-4 py-2 rounded-lg transition-all duration-300 ease-in-out ${
                                    isActive ? 'text-blue-300 bg-white/20' : ''
                                }`
                            }
                        >
                            Programok
                        </NavLink>
                        <NavLink
                            to="/rolunk"
                            className={({ isActive }) =>
                                `relative text-white hover:text-blue-300 hover:bg-white/10 px-4 py-2 rounded-lg transition-all duration-300 ease-in-out ${
                                    isActive ? 'text-blue-300 bg-white/20' : ''
                                }`
                            }
                        >
                            Rólunk
                        </NavLink>
                    </div>

                    <div className="relative" ref={menuRef}>
                        <button
                            onClick={() => setMenuOpen(!menuOpen)}
                            className="flex items-center justify-center w-12 h-12 rounded-full bg-white/10 hover:bg-white/20 transition-all duration-300 transform hover:scale-110 hover:shadow-lg group"
                        >
                            <FontAwesomeIcon
                                icon={faUser}
                                className="text-white text-xl group-hover:text-blue-300 transition-colors duration-300"
                            />
                        </button>
                        {menuOpen && (
                            <div className="absolute right-0 mt-3 w-56 bg-white rounded-xl shadow-xl py-3 transform origin-top-right animate-[fadeIn_0.2s_ease-out] border border-blue-100">
                                {user && token && !isTokenExpired() ? (
                                    <>
                                        <button
                                            onClick={handleProfileClick}
                                            className="block w-full text-left px-5 py-3 hover:bg-blue-50 text-gray-700 transition-colors duration-200 flex items-center"
                                        >
                                            <span className="material-symbols-outlined mr-2 text-blue-600">person</span>
                                            Profil megnyitása
                                        </button>
                                        <button
                                            onClick={() => {
                                                handleLogout();
                                                handleMenuItemClick();
                                            }}
                                            className="block w-full text-left px-5 py-3 hover:bg-blue-50 text-gray-700 transition-colors duration-200 flex items-center"
                                        >
                                            <span className="material-symbols-outlined mr-2 text-red-600">logout</span>
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
                                            className="block w-full text-left px-5 py-3 hover:bg-blue-50 text-gray-700 transition-colors duration-200 flex items-center"
                                        >
                                            <span className="material-symbols-outlined mr-2 text-green-600">login</span>
                                            Bejelentkezés
                                        </button>
                                        <button
                                            onClick={() => {
                                                setActiveModal('register');
                                                handleMenuItemClick();
                                            }}
                                            className="block w-full text-left px-5 py-3 hover:bg-blue-50 text-gray-700 transition-colors duration-200 flex items-center"
                                        >
                                            <span className="material-symbols-outlined mr-2 text-blue-600">person_add</span>
                                            Regisztráció
                                        </button>
                                    </>
                                )}
                            </div>
                        )}
                    </div>
                </div>

                {isMobileMenuOpen && (
                    <div className="md:hidden absolute top-full left-0 w-full bg-gradient-to-r from-blue-900 via-blue-800 to-blue-700 shadow-lg z-50 animate-[slideDown_0.3s_ease-out]">
                        <div className="px-4 py-4 space-y-3">
                            <NavLink
                                to="/"
                                className={({ isActive }) =>
                                    `block text-white hover:text-blue-300 py-3 px-5 rounded-lg hover:bg-white/10 transition-all duration-200 ${isActive ? 'text-blue-300 bg-white/20' : ''}`
                                }
                                onClick={() => setIsMobileMenuOpen(false)}
                            >
                                Kezdőlap
                            </NavLink>
                            <NavLink
                                to="/szobak"
                                className={({ isActive }) =>
                                    `block text-white hover:text-blue-300 py-3 px-5 rounded-lg hover:bg-white/10 transition-all duration-200 ${isActive ? 'text-blue-300 bg-white/20' : ''}`
                                }
                                onClick={() => setIsMobileMenuOpen(false)}
                            >
                                Szobák
                            </NavLink>
                            <NavLink
                                to="/szolgaltatasok"
                                className={({ isActive }) =>
                                    `block text-white hover:text-blue-300 py-3 px-5 rounded-lg hover:bg-white/10 transition-all duration-200 ${isActive ? 'text-blue-300 bg-white/20' : ''}`
                                }
                                onClick={() => setIsMobileMenuOpen(false)}
                            >
                                Szolgáltatások
                            </NavLink>
                            <NavLink
                                to="/programok"
                                className={({ isActive }) =>
                                    `block text-white hover:text-blue-300 py-3 px-5 rounded-lg hover:bg-white/10 transition-all duration-200 ${isActive ? 'text-blue-300 bg-white/20' : ''}`
                                }
                                onClick={() => setIsMobileMenuOpen(false)}
                            >
                                Programok
                            </NavLink>
                            <NavLink
                                to="/rolunk"
                                className={({ isActive }) =>
                                    `block text-white hover:text-blue-300 py-3 px-5 rounded-lg hover:bg-white/10 transition-all duration-200 ${isActive ? 'text-blue-300 bg-white/20' : ''}`
                                }
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

            {showProfileModal && !isTokenExpired() && (
                <ProfileModal
                    user={user}
                    onClose={() => setShowProfileModal(false)}
                    onLogout={handleLogout}
                />
            )}

            <style jsx global>{`
                @keyframes slideDown {
                    from {
                        opacity: 0;
                        transform: translateY(-10px);
                    }
                    to {
                        opacity: 1;
                        transform: translateY(0);
                    }
                }
                @keyframes fadeIn {
                    from {
                        opacity: 0;
                    }
                    to {
                        opacity: 1;
                    }
                }
            `}</style>
        </>
    );
}

export default Navbar;