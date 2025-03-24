import React, { useState, useEffect } from 'react';
import '../styles/Hero.css';
import { Link } from 'react-router-dom';

function Hero() {
    const [programs, setPrograms] = useState([]);

    useEffect(() => {
        // Az adatok lekérése a szerverről
        fetch('https://localhost:7047/Events/Geteents')
            .then(response => response.json())
            .then(data => {
                const formattedPrograms = data.map(event => ({
                    id: event.eventId,
                    name: event.eventName,
                    description: event.description,
                    images: event.images || 'https://via.placeholder.com/300', // Alapértelmezett kép, ha nincs megadva
                    schedule: event.eventDate,
                    organizerName: event.organizerName,
                    contactInfo: event.contactInfo,
                    location: event.location,
                    capacity: event.capacity,
                    price: event.price,
                }));
                // Az első 3 program kiválasztása a "Felkapott Események" szekcióhoz
                setPrograms(formattedPrograms.slice(0, 3));
            })
            .catch(error => console.error('Hiba történt az adatok lekérésekor:', error));
    }, []);

    // Funkció a leírás levágásához
    const truncateDescription = (description, maxLength) => {
        if (description && description.length > maxLength) {
            return description.substring(0, maxLength) + '...';
        }
        return description || 'Nincs leírás';
    };

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
                                Fedezze fel a kényelem és elegancia új dimenzióját
                            </p>
                        </div>
                        <Link to="/szobak">
                            <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4 md:gap-8 mb-8 md:mb-16">
                                <div className="bg-white rounded-xl shadow-lg overflow-hidden transform hover:scale-105 transition-all duration-300">
                                    <div
                                        className="h-48 md:h-64 bg-cover bg-center"
                                        style={{ backgroundImage: `url('https://images.unsplash.com/photo-1611892440504-42a792e24d32?ixlib=rb-4.0.3')` }}
                                    ></div>
                                    <div className="p-4 md:p-6">
                                        <h3 className="text-xl md:text-2xl font-bold text-blue-900 mb-2">Deluxe Szoba</h3>
                                        <p className="text-blue-700">Luxus és kényelem találkozása</p>
                                    </div>
                                </div>

                                <div className="bg-white rounded-xl shadow-lg overflow-hidden transform hover:scale-105 transition-all duration-300">
                                    <div
                                        className="h-48 md:h-64 bg-cover bg-center"
                                        style={{ backgroundImage: `url('https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?ixlib=rb-4.0.3')` }}
                                    ></div>
                                    <div className="p-4 md:p-6">
                                        <h3 className="text-xl md:text-2xl font-bold text-blue-900 mb-2">Páros Szoba</h3>
                                        <p className="text-blue-700">Páratlan elegancia</p>
                                    </div>
                                </div>

                                <div className="bg-white rounded-xl shadow-lg overflow-hidden transform hover:scale-105 transition-all duration-300">
                                    <div
                                        className="h-48 md:h-64 bg-cover bg-center"
                                        style={{ backgroundImage: `url('https://images.unsplash.com/photo-1578683010236-d716f9a3f461?ixlib=rb-4.0.3')` }}
                                    ></div>
                                    <div className="p-4 md:p-6">
                                        <h3 className="text-xl md:text-2xl font-bold text-blue-900 mb-2">Királyi Lakosztály</h3>
                                        <p className="text-blue-700">A tökéletesség új szintje</p>
                                    </div>
                                </div>
                            </div>
                        </Link>

                        <h2 className="text-2xl md:text-4xl font-bold text-center text-blue-900 mb-8 md:mb-16">Felkapott Események</h2>
                        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4 md:gap-8">
                            {programs.length > 0 ? (
                                programs.map((program) => (
                                    <div
                                        key={program.id}
                                        className="bg-white rounded-xl shadow-lg overflow-hidden transform hover:scale-105 transition-all duration-300"
                                    >
                                        <div
                                            className="h-48 md:h-64 bg-cover bg-center"
                                            style={{ backgroundImage: `url(${program.images})` }}
                                        ></div>
                                        <div className="p-4 md:p-6">
                                            <h3 className="text-xl md:text-2xl font-bold text-blue-900 mb-2">{program.name}</h3>
                                            <p className="text-blue-700 mb-2">{truncateDescription(program.description, 50)}</p>
                                            <p className="text-blue-600 text-sm">Időpont: {new Date(program.schedule).toLocaleDateString('hu-HU')}</p>
                                            <Link to="/programok">
                                                <button className="mt-4 w-full bg-blue-800 text-white px-4 py-2 rounded-lg font-semibold hover:bg-blue-700 transition-colors">
                                                    Részletek
                                                </button>
                                            </Link>
                                        </div>
                                    </div>
                                ))
                            ) : (
                                <p className="text-center text-blue-700 col-span-full">Jelenleg nincsenek felkapott események.</p>
                            )}
                        </div>
                    </div>
                </section>
            </main>
        </div>
    );
}

export default Hero;