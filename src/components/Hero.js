import React, { useState, useEffect, useRef } from 'react';
import '../styles/Hero.css';
import { Link, useNavigate } from 'react-router-dom';

function Hero() {
    const [rooms, setRooms] = useState([]);
    const [currentIndex, setCurrentIndex] = useState(0);
    const [transitionEnabled, setTransitionEnabled] = useState(true);
    const [programs, setPrograms] = useState([]);
    const navigate = useNavigate();
    const sliderRef = useRef(null);

    useEffect(() => {
        // Szobák lekérése
        fetch(process.env.REACT_APP_API_BASE_URL + '/Rooms/GetRoomWith')
            .then(response => response.json())
            .then(data => {
                const availableRooms = data
                    .filter(room => room.status?.toLowerCase() !== 'unavailable')
                    .map(room => ({
                        id: room.roomId,
                        name: room.roomType,
                        description: room.description,
                        image: room.images || 'https://via.placeholder.com/300',
                        price: room.pricePerNight
                    }));
                setRooms(availableRooms);
            })
            .catch(error => console.error('Error fetching rooms:', error));

        // Események lekérése
        fetch(process.env.REACT_APP_API_BASE_URL + `/Events/Geteents`)
            .then(response => response.json())
            .then(data => {
                const formattedPrograms = data.map(event => ({
                    id: event.eventId,
                    name: event.eventName,
                    description: event.description,
                    images: event.images || 'https://via.placeholder.com/300',
                    schedule: event.eventDate,
                    organizerName: event.organizerName,
                    contactInfo: event.contactInfo,
                    location: event.location,
                    capacity: event.capacity,
                    price: event.price,
                }));
                setPrograms(formattedPrograms.slice(0, 3));
            })
            .catch(error => console.error('Error fetching events:', error));
    }, []);

    const truncateDescription = (description, maxLength) => {
        return description && description.length > maxLength 
            ? description.substring(0, maxLength) + '...' 
            : description || 'Nincs leírás';
    };

    const handleRoomClick = (roomId) => {
        const token = localStorage.getItem('authToken');
        if (token) {
            navigate(`/szobak/${roomId}`);
        } else {
            navigate('/login');
        }
    };

    // Dinamikus elmozdulás kiszámítása a képernyőméret alapján
    const getSlideOffset = () => {
        const width = window.innerWidth;
        if (width < 640) {
            // Mobil nézet: teljes szélesség (100%) + középre igazítás
            const containerWidth = sliderRef.current ? sliderRef.current.offsetWidth : width;
            const cardWidth = containerWidth * 0.8; // w-4/5 = 80%
            return (containerWidth - cardWidth) / 2 - (currentIndex * cardWidth);
        }
        if (width < 768) return currentIndex * 50;  // sm nézet: 50%
        return currentIndex * 33.33;                // md+ nézet: 33.33%
    };

    // Körkörös navigáció balra
    const handleSlideLeft = () => {
        setCurrentIndex(prev => {
            const newIndex = prev - 1;
            if (newIndex < 0) {
                setTransitionEnabled(false);
                setTimeout(() => {
                    setTransitionEnabled(true);
                }, 0);
                return rooms.length - 1;
            }
            return newIndex;
        });
    };

    // Körkörös navigáció jobbra
    const handleSlideRight = () => {
        setCurrentIndex(prev => {
            const newIndex = prev + 1;
            if (newIndex >= rooms.length) {
                setTransitionEnabled(false);
                setTimeout(() => {
                    setTransitionEnabled(true);
                }, 0);
                return 0;
            }
            return newIndex;
        });
    };

    // Többszörösen duplikált szobák a zökkenőmentes végtelen csúszáshoz
    const duplicatedRooms = [...rooms, ...rooms, ...rooms]; // Háromszoros duplikáció

    return (
        <div>
            <header className="bg-gradient-to-b from-blue-900 to-blue-800 text-white py-12 md:py-24">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                    <div className="text-center">
                        <h1 className="text-3xl md:text-5xl font-bold mb-4 md:mb-6">Üdvözöljük a Luxus Szállodában</h1>
                        <p className="text-lg md:text-xl text-blue-200 max-w-3xl mx-auto mb-8 md:mb-12">
                            Fedezze fel a kényelem és elegancia új dimenzióját szállodánkban
                        </p>
                        <Link to="/szobak">
                            <button className="bg-white text-blue-900 px-6 md:px-8 py-3 md:py-4 rounded-full font-semibold hover:bg-blue-50 transition-all duration-300 transform hover:scale-105 shadow-lg">
                                Foglaljon Most
                            </button>
                        </Link>
                    </div>
                </div>
            </header>

            <main className="bg-gradient-to-b from-blue-100 via-blue-50 to-white min-h-screen pt-8 md:pt-16">
                <section className="py-8 md:py-16">
                    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                        <div className="text-center mb-8 md:mb-16">
                            <h1 className="text-3xl md:text-5xl font-bold text-blue-900 mb-4 md:mb-6">Luxus Szálloda</h1>
                            <p className="text-lg md:text-xl text-blue-700 max-w-3xl mx-auto">
                                Fedezze fel a kényelem és elegancia új dimenzióját szállodánkban!
                            </p>
                        </div>
                        
                        {/* Szobák csúszkája */}
                        <div className="relative max-w-5xl mx-auto mb-8 md:mb-16">
                            <button 
                                onClick={handleSlideLeft} 
                                className="absolute left-0 top-1/2 transform -translate-y-1/2 bg-white p-2 rounded-full shadow-md text-blue-900 z-10"
                            >
                                ←
                            </button>
                            <div className="overflow-hidden">
                                <div 
                                    ref={sliderRef}
                                    className={`flex ${transitionEnabled ? 'transition-transform duration-500' : ''} gap-6`}
                                    style={{ 
                                        transform: window.innerWidth < 640 
                                            ? `translateX(${getSlideOffset()}px)` 
                                            : `translateX(-${getSlideOffset()}%)`
                                    }}
                                >
                                    {duplicatedRooms.map((room, index) => (
                                        <div 
                                            key={`${room.id}-${index}`} 
                                            className="flex-shrink-0 w-4/5 sm:w-1/2 md:w-1/3 mx-auto"
                                            onClick={() => handleRoomClick(room.id)}
                                        >
                                            <div className="bg-white rounded-xl shadow-lg overflow-hidden cursor-pointer transform hover:scale-105 transition-all duration-300">
                                                <div
                                                    className="h-48 bg-cover bg-center"
                                                    style={{ backgroundImage: `url(${room.image})` }}
                                                ></div>
                                                <div className="p-4">
                                                    <h3 className="text-lg font-bold text-blue-900 mb-2">{room.name}</h3>
                                                    <p className="text-sm text-blue-700 mb-2">{truncateDescription(room.description, 50)}</p>
                                                    <p className="text-blue-600 text-sm">Ár: {room.price} Ft/éj</p>
                                                </div>
                                            </div>
                                        </div>
                                    ))}
                                </div>
                            </div>
                            <button 
                                onClick={handleSlideRight} 
                                className="absolute right-0 top-1/2 transform -translate-y-1/2 bg-white p-2 rounded-full shadow-md text-blue-900 z-10"
                            >
                                →
                            </button>
                        </div>

                        {/* Események szekció */}
                        <h2 className="text-2xl md:text-4xl font-bold text-center text-blue-900 mb-8 md:mb-16">Felkapott Események</h2>
                        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-6 md:gap-8">
                            {programs.map((program) => (
                                <div key={program.id} className="bg-white rounded-xl shadow-lg overflow-hidden transform hover:scale-105 transition-all duration-300">
                                    <div className="h-48 bg-cover bg-center" style={{ backgroundImage: `url(${program.images})` }}></div>
                                    <div className="p-4">
                                        <h3 className="text-lg font-bold text-blue-900 mb-2">{program.name}</h3>
                                        <p className="text-sm text-blue-700 mb-2">{truncateDescription(program.description, 50)}</p>
                                        <p className="text-blue-600 text-sm">Időpont: {new Date(program.schedule).toLocaleDateString('hu-HU')}</p>
                                        <Link to="/programok">
                                            <button className="mt-4 w-full bg-blue-800 text-white px-4 py-2 rounded-lg font-semibold hover:bg-blue-700 transition-colors text-sm">
                                                Részletek
                                            </button>
                                        </Link>
                                    </div>
                                </div>
                            ))}
                        </div>
                    </div>
                </section>
            </main>
        </div>
    );
}

export default Hero;