import React from 'react';
import '../styles/Hero.css';
import { Link } from 'react-router-dom';

function Hero() {
    return (
        <div>
            <header className="bg-gradient-to-b from-blue-900 to-blue-800 text-white py-12 md:py-24">
                <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                    <div className="text-center">
                        <h1 className="text-3xl md:text-5xl font-bold mb-4 md:mb-6">Üdvözöljük a Luxus Szállodában</h1>
                        <p className="text-lg md:text-xl text-blue-200 max-w-3xl mx-auto mb-8 md:mb-12">Fedezze fel a kényelem és elegancia új dimenzióját szállodánkban</p>
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
                            <p className="text-lg md:text-xl text-blue-700 max-w-3xl mx-auto">Fedezze fel a kényelem és elegancia új dimenzióját</p>
                        </div>
                    <Link to='/szobak'>
                        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-4 md:gap-8 mb-8 md:mb-16">
                            <div className="bg-white rounded-xl shadow-lg overflow-hidden transform hover:scale-105 transition-all duration-300">
                                <div className="h-48 md:h-64 bg-cover bg-center" style={{backgroundImage: `url('https://images.unsplash.com/photo-1611892440504-42a792e24d32?ixlib=rb-4.0.3')`}}></div>
                                <div className="p-4 md:p-6">
                                    <h3 className="text-xl md:text-2xl font-bold text-blue-900 mb-2">Deluxe Szoba</h3>
                                    <p className="text-blue-700">Luxus és kényelem találkozása</p>
                                </div>
                            </div>
                            
                            <div className="bg-white rounded-xl shadow-lg overflow-hidden transform hover:scale-105 transition-all duration-300">
                                <div className="h-48 md:h-64 bg-cover bg-center" style={{backgroundImage: `url('https://images.unsplash.com/photo-1582719478250-c89cae4dc85b?ixlib=rb-4.0.3')`}}></div>
                                <div className="p-4 md:p-6">
                                    <h3 className="text-xl md:text-2xl font-bold text-blue-900 mb-2">Páros Szoba</h3>
                                    <p className="text-blue-700">Páratlan elegancia</p>
                                </div>
                            </div>
                            
                            <div className="bg-white rounded-xl shadow-lg overflow-hidden transform hover:scale-105 transition-all duration-300">
                                <div className="h-48 md:h-64 bg-cover bg-center" style={{backgroundImage: `url('https://images.unsplash.com/photo-1578683010236-d716f9a3f461?ixlib=rb-4.0.3')`}}></div>
                                <div className="p-4 md:p-6">
                                    <h3 className="text-xl md:text-2xl font-bold text-blue-900 mb-2">Királyi Lakosztály</h3>
                                    <p className="text-blue-700">A tökéletesség új szintje</p>
                                </div>
                            </div>
                        </div>
                        </Link>
                        <h2 className="text-2xl md:text-4xl font-bold text-center text-blue-900 mb-8 md:mb-16">Felkapott Események</h2>
                        <div className="relative h-[300px] md:h-[500px] rounded-2xl overflow-hidden shadow-xl">
                            <div className="absolute inset-0 bg-gradient-to-r from-blue-900/80 to-blue-800/80 z-10"></div>
                            <div className="absolute inset-0">
                                <div className="w-full h-full">
                                    <div className="relative w-full h-full slider">
                                        <div className="absolute inset-0 w-full h-full bg-cover bg-center" style={{backgroundImage: `url('https://images.unsplash.com/photo-1587825140708-dfaf72ae4b04?ixlib=rb-4.0.3')`}}></div>
                                        <div className="absolute inset-0 w-full h-full bg-cover bg-center" style={{backgroundImage:`url('https://images.unsplash.com/photo-1492684223066-81342ee5ff30?ixlib=rb-4.0.3')`}}></div>
                                        <div className="absolute inset-0 w-full h-full bg-cover bg-center" style={{backgroundImage: `url('https://images.unsplash.com/photo-1519167758481-83f550bb49b3?ixlib=rb-4.0.3')`}}></div>
                                    </div>
                                </div>
                            </div>
                            <div className="absolute inset-0 z-20 flex items-center justify-center">
                                <div className="text-center p-4 md:p-8">
                                    <h3 className="text-2xl md:text-4xl font-bold text-white mb-2 md:mb-4">Nyári Gasztro Fesztivál</h3>
                                    <p className="text-lg md:text-xl text-blue-100 mb-4 md:mb-8 max-w-2xl mx-auto">Fedezze fel a helyi és nemzetközi konyha remekeit luxus környezetben.</p>
                                    <button className="bg-white text-blue-800 px-6 md:px-8 py-3 md:py-4 rounded-full font-semibold hover:bg-blue-50 transition-all duration-300 transform hover:scale-105 shadow-lg">
                                        Részletek
                                    </button>
                                </div>
                            </div>
                        </div>
                    </div>
                </section>
            </main>
        </div>
    );
}

export default Hero;