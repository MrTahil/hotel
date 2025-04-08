import React, { useState, useEffect, useRef } from 'react';
import '../styles/Hero.css';
import { Link, useNavigate } from 'react-router-dom';

function Hero() {
    const [rooms, setRooms] = useState([]);
    const [currentIndex, setCurrentIndex] = useState(0);
    const [transitionEnabled, setTransitionEnabled] = useState(true);
    const [programs, setPrograms] = useState([]);
    const [loading, setLoading] = useState(true);
    const [selectedFeature, setSelectedFeature] = useState(null);
    const navigate = useNavigate();
    const sliderRef = useRef(null);
    const containerRef = useRef(null);

    useEffect(() => {
        fetch(process.env.REACT_APP_API_BASE_URL + '/Rooms/GetRoomWith')
            .then(response => {
                if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
                return response.text();
            })
            .then(data => {
                if (!Array.isArray(data)) {
                    console.error('Expected an array from API, got:', data);
                    setRooms([]);
                    return;
                }
                const availableRooms = data
                    .filter(room => room.status?.toLowerCase() !== 'unavailable')
                    .map(room => ({
                        id: room.roomId,
                        name: room.roomType,
                        description: room.description,
                        image: room.images || 'https://via.placeholder.com/300',
                        price: room.pricePerNight,
                        amenities: room.amenities || ['Wi-Fi', 'TV', 'Minibar']
                    }));
                setRooms(availableRooms);
            })
            .catch(error => {
                console.error('Error fetching rooms:', error);
                setRooms([]);
            })
            .finally(() => setLoading(false));

        fetch(process.env.REACT_APP_API_BASE_URL + `/Events/Geteents`)
            .then(response => {
                if (!response.ok) throw new Error(`HTTP error! status: ${response.status}`);
                return response.text();
            })
            .then(data => {
                if (!Array.isArray(data)) {
                    console.error('Expected an array from API, got:', data);
                    setPrograms([]);
                    return;
                }
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
            .catch(error => {
                console.error('Error fetching events:', error);
                setPrograms([]);
            });
    }, []);

    const truncateDescription = (description, maxLength) => {
        return description && description.length > maxLength
            ? description.substring(0, maxLength) + '...'
            : description || 'Nincs leírás';
    };

    const handleRoomClick = (roomId) => {
        const token = localStorage.getItem('authToken');
        if (token) navigate(`/szobak/${roomId}`);
        else navigate('/login');
    };

    const getSlideOffset = () => {
        if (!containerRef.current) return 0;

        const containerWidth = containerRef.current.offsetWidth;
        const cardWidth = calculateCardWidth(containerWidth);
        return -(currentIndex * cardWidth) + (containerWidth - cardWidth) / 2;
    };

    const calculateCardWidth = (containerWidth) => {
        const width = window.innerWidth;
        if (width < 640) {
            return containerWidth * 0.9;
        } else if (width < 1024) {
            return containerWidth / 2;
        } else {
            return containerWidth / 3;
        }
    };

    const handleSlideLeft = () => {
        setCurrentIndex((prev) => {
            const newIndex = prev - 1;
            if (newIndex < 0) {
                setTransitionEnabled(false);
                setTimeout(() => {
                    setTransitionEnabled(true);
                    setCurrentIndex(rooms.length - 1);
                }, 0);
                return rooms.length - 1;
            }
            return newIndex;
        });
    };

    const handleSlideRight = () => {
        setCurrentIndex((prev) => {
            const newIndex = prev + 1;
            if (newIndex >= rooms.length) {
                setTransitionEnabled(false);
                setTimeout(() => {
                    setTransitionEnabled(true);
                    setCurrentIndex(0);
                }, 0);
                return 0;
            }
            return newIndex;
        });
    };

    const duplicatedRooms = rooms.length > 0 ? [...rooms, ...rooms, ...rooms] : [];

    const features = [
        {
            icon: 'spa',
            title: 'Wellness & Spa',
            shortDesc: 'Pihenjen modern spa részlegünkben.',
            detailedDesc: {
                description: 'Fedezze fel a tökéletes kikapcsolódást modern wellness és spa részlegünkben, ahol a legújabb technológiák és hagyományos relaxációs módszerek találkoznak. Élvezze a tágas szaunák, gőzkabinok és jakuzzik kényelmét, miközben a professzionális masszőrök által kínált exkluzív kezelések széles választékából válogathat.',
                facilities: ['Finn szauna', 'Infraszauna', 'Gőzkabin', 'Jakuzzi', 'Aromaterápiás szoba', 'Relaxációs zóna'],
                openingHours: 'Hétfőtől vasárnapig: 8:00 - 20:00',
                pricing: 'Alapszolgáltatások ingyenesek a vendégek számára, masszázsok 5000 Ft/30 perctől',
                additionalInfo: 'Kérjük, előre foglaljon időpontot a masszázsokra. Fürdőruhát és papucsot biztosítunk.',
            },
        },
        {
            icon: 'restaurant',
            title: 'Gourmet Étterem',
            shortDesc: 'Élvezze díjnyertes fogásainkat.',
            detailedDesc: {
                description: 'Lépjen be gasztronómiai csodák világába gourmet éttermünkben, ahol a legfinomabb alapanyagokból készült, díjnyertes fogások várják Önt. Séfjeink a helyi és nemzetközi konyha legjavát ötvözik, hogy minden egyes étel egyedi élményt nyújtson.',
                menuHighlights: ['Szezonális degusztációs menü', 'Frissen fogott tengeri halak', 'Házi készítésű desszertek', 'Vegán és gluténmentes opciók'],
                openingHours: 'Hétfőtől vasárnapig: 7:00 - 22:00',
                pricing: 'Főételek 5000 Ft-tól 30.000 Ft-ig, borok poháronként 1200 Ft-tól',
                additionalInfo: 'Asztalfoglalás ajánlott, különösen hétvégeken.',
            },
        },
        {
            icon: 'pool',
            title: 'Medence',
            shortDesc: 'Frissüljön fel stílusos medencénkben.',
            detailedDesc: {
                description: 'Merüljön el a luxusban stílusos medencénkben, amely a szálloda egyik ékköve. A kristálytiszta vizű, fűtött beltéri és kültéri medencék egész évben elérhetőek, így bármikor élvezheti a víz nyugtató hatását.',
                facilities: ['Beltéri fűtött medence', 'Kültéri infinity medence', 'Gyermekmedence', 'Medence melletti bár'],
                openingHours: 'Hétfőtől vasárnapig: 6:00 - 22:00',
                pricing: 'Ingyenes a vendégek számára',
                additionalInfo: 'Törölközőt biztosítunk, a medence melletti bár 10:00-tól üzemel.',
            },
        },
        {
            icon: 'wifi',
            title: 'Ingyenes Wi-Fi',
            shortDesc: 'Gyors kapcsolat mindenhol.',
            detailedDesc: {
                description: 'Maradjon mindig kapcsolatban ingyenes, nagy sebességű Wi-Fi hálózatunkkal, amely a szálloda minden szegletében elérhető. Legyen szó munkáról, videóhívásokról vagy egyszerű böngészésről, nálunk a stabil és gyors internetkapcsolat alapfelszereltség.',
                facilities: ['100 Mbps sebesség', 'Több eszköz támogatása', '24/7 technikai segítség'],
                openingHours: 'Folyamatosan elérhető',
                pricing: 'Ingyenes minden vendég számára',
                additionalInfo: 'Jelszó a recepción kérhető.',
            },
        },
    ];

    const openDetails = (feature) => {
        setSelectedFeature(feature);
    };

    const closeDetails = () => {
        setSelectedFeature(null);
    };

    if (loading) {
        return (
            <div className="flex items-center justify-center h-screen bg-blue-50">
                <p className="text-blue-900 text-xl font-semibold">Betöltés...</p>
            </div>
        );
    }

    return (
        <div className="font-sans">
            <header className="relative h-screen flex items-center justify-center text-white overflow-hidden">
                <video
                    autoPlay
                    loop
                    muted
                    className="absolute inset-0 w-full h-full object-cover z-0 opacity-70"
                    src="https://assets.mixkit.co/videos/preview/mixkit-luxury-hotel-room-1201-large.mp4"
                />
                <div className="absolute inset-0 bg-blue-900 z-10"></div>
                <div className="relative z-20 text-center px-4">
                    <h1 class="animate-bounce text-4xl md:text-6xl font-bold mb-6">Üdvözöljük az álomnyaralás kezdetén</h1>
                    <p class="text-lg md:text-2xl max-w-3xl mx-auto mb-8 text-blue-100">
                        Fedezze fel a tökéletes pihenés és kiváló szolgáltatások harmonikus világát
                    </p>
                    <Link to="/szobak">
                        <button className="bg-blue-600 text-white px-8 py-4 rounded-lg font-semibold text-lg hover:bg-blue-700 shadow-lg">
                            Foglaljon Most
                        </button>
                    </Link>
                    <div className="absolute bottom-10 left-10 animate-bounce z-30">
                        <button
                            onClick={() => {
                                const mainSection = document.querySelector('main');
                                mainSection.scrollIntoView({ behavior: 'smooth' });
                            }}
                            className="text-white hover:text-blue-200 transition-colors"
                            aria-label="Görgés lefele (bal)"
                        >
                            <svg
                                xmlns="http://www.w3.org/2000/svg"
                                className="h-10 w-10"
                                fill="none"
                                viewBox="0 0 24 24"
                                stroke="currentColor"
                            >
                                <path
                                    strokeLinecap="round"
                                    strokeLinejoin="round"
                                    strokeWidth={2}
                                    d="M19 14l-7 7m0 0l-7-7m7 7V3"
                                />
                            </svg>
                            <span className="sr-only">Görgés lefele</span>
                        </button>
                    </div>

                    <div className="absolute bottom-10 right-10 animate-bounce z-30">
                        <button
                            onClick={() => {
                                const mainSection = document.querySelector('main');
                                mainSection.scrollIntoView({ behavior: 'smooth' });
                            }}
                            className="text-white hover:text-blue-200 transition-colors"
                            aria-label="Görgés lefele (jobb)"
                        >
                            <svg
                                xmlns="http://www.w3.org/2000/svg"
                                className="h-10 w-10"
                                fill="none"
                                viewBox="0 0 24 24"
                                stroke="currentColor"
                            >
                                <path
                                    strokeLinecap="round"
                                    strokeLinejoin="round"
                                    strokeWidth={2}
                                    d="M19 14l-7 7m0 0l-7-7m7 7V3"
                                />
                            </svg>
                            <span className="sr-only">Görgés lefele</span>
                        </button>
                    </div>
                </div>
            </header>

            <main className="bg-blue-50">
                <section className="py-16">
                    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
                        <h2 className="text-3xl md:text-5xl font-bold text-blue-900 mb-6">Miért Válasszon Minket?</h2>
                        <p className="text-lg text-blue-700 max-w-3xl mx-auto mb-12">
                            Kivételes szolgáltatások és modern dizájn minden vendégünk számára.
                        </p>
                        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-4 gap-8">
                            {features.map((feature, idx) => (
                                <div
                                    key={idx}
                                    className="bg-white p-6 rounded-lg shadow-md hover:shadow-lg transition-shadow duration-300 flex flex-col items-center"
                                >
                                    <span className="material-symbols-outlined text-4xl text-blue-600 mb-4">
                                        {feature.icon}
                                    </span>
                                    <h3 className="text-xl font-semibold text-blue-900 mb-2">{feature.title}</h3>
                                    <p className="text-blue-700 mb-4">{feature.shortDesc}</p>
                                    <button
                                        onClick={() => openDetails(feature)}
                                        className="mt-auto bg-blue-600 text-white px-4 py-2 rounded-lg font-semibold hover:bg-blue-700 transition-colors shadow-sm"
                                    >
                                        Részletek
                                    </button>
                                </div>
                            ))}
                        </div>
                    </div>
                </section>

                {selectedFeature && (
                    <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
                        <div className="bg-white rounded-lg shadow-xl p-6 max-w-lg w-full mx-4 max-h-[80vh] overflow-y-auto">
                            <div className="flex justify-between items-center mb-4">
                                <h3 className="text-2xl font-bold text-blue-900">{selectedFeature.title}</h3>
                                <button
                                    onClick={closeDetails}
                                    className="text-gray-600 hover:text-gray-800"
                                >
                                    <svg
                                        xmlns="http://www.w3.org/2000/svg"
                                        className="h-6 w-6"
                                        fill="none"
                                        viewBox="0 0 24 24"
                                        stroke="currentColor"
                                    >
                                        <path
                                            strokeLinecap="round"
                                            strokeLinejoin="round"
                                            strokeWidth={2}
                                            d="M6 18L18 6M6 6l12 12"
                                        />
                                    </svg>
                                </button>
                            </div>
                            <p className="text-gray-700 mb-4">{selectedFeature.detailedDesc.description}</p>
                            <div className="space-y-4">
                                {selectedFeature.detailedDesc.facilities || selectedFeature.detailedDesc.menuHighlights ? (
                                    <div>
                                        <h4 className="text-lg font-semibold text-blue-800">
                                            {selectedFeature.title === 'Gourmet Étterem' ? 'Menü kiemelések' : 'Szolgáltatások'}
                                        </h4>
                                        <ul className="list-disc pl-5 text-gray-600">
                                            {(selectedFeature.detailedDesc.facilities || selectedFeature.detailedDesc.menuHighlights).map((item, idx) => (
                                                <li key={idx}>{item}</li>
                                            ))}
                                        </ul>
                                    </div>
                                ) : null}

                                {selectedFeature.detailedDesc.openingHours && (
                                    <div>
                                        <h4 className="text-lg font-semibold text-blue-800">Nyitvatartás</h4>
                                        <p className="text-gray-600">{selectedFeature.detailedDesc.openingHours}</p>
                                    </div>
                                )}

                                {selectedFeature.detailedDesc.pricing && (
                                    <div>
                                        <h4 className="text-lg font-semibold text-blue-800">Árak</h4>
                                        <p className="text-gray-600">{selectedFeature.detailedDesc.pricing}</p>
                                    </div>
                                )}

                                {selectedFeature.detailedDesc.additionalInfo && (
                                    <div>
                                        <h4 className="text-lg font-semibold text-blue-800">További információ</h4>
                                        <p className="text-gray-600">{selectedFeature.detailedDesc.additionalInfo}</p>
                                    </div>
                                )}
                            </div>
                            <button
                                onClick={closeDetails}
                                className="mt-6 w-full bg-blue-600 text-white px-4 py-2 rounded-lg font-semibold hover:bg-blue-700 transition-colors"
                            >
                                Bezárás
                            </button>
                        </div>
                    </div>
                )}

                <section className="py-16 bg-white">
                    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                        <h2 className="text-3xl md:text-5xl font-bold text-blue-900 text-center mb-12">Szobáink</h2>
                        {rooms.length === 0 ? (
                            <p className="text-center text-blue-700">Jelenleg nincsenek elérhető szobák.</p>
                        ) : (
                            <div className="relative" ref={containerRef}>
                                <button
                                    onClick={handleSlideLeft}
                                    className="absolute left-0 top-1/2 transform -translate-y-1/2 bg-blue-600 text-white p-3 rounded-full shadow-lg hover:bg-blue-700 z-10"
                                >
                                    <span className="material-symbols-outlined">chevron_left</span>
                                </button>
                                <div className="overflow-hidden">
                                    <div
                                        ref={sliderRef}
                                        className={`flex ${transitionEnabled ? 'transition-transform duration-500 ease-in-out' : ''} gap-6`}
                                        style={{ transform: `translateX(${getSlideOffset()}px)` }}
                                    >
                                        {duplicatedRooms.map((room, index) => (
                                            <div
                                                key={`${room.id}-${index}`}
                                                className="flex-shrink-0 w-[90%] sm:w-1/2 lg:w-1/3 px-2"
                                                onClick={() => handleRoomClick(room.id)}
                                            >
                                                <div className="bg-white rounded-lg shadow-md overflow-hidden cursor-pointer">
                                                    <div
                                                        className="h-64 bg-cover bg-center"
                                                        style={{ backgroundImage: `url(${room.image})` }}
                                                    ></div>
                                                    <div className="p-6">
                                                        <h3 className="text-xl font-bold text-blue-900 mb-2">{room.name}</h3>
                                                        <p className="text-sm text-blue-700 mb-4">{truncateDescription(room.description, 80)}</p>
                                                        <p className="text-blue-600 font-semibold mb-4">Ár: {room.price} Ft/éj</p>
                                                        <ul className="text-sm text-blue-600 space-y-1">
                                                            {room.amenities.map((amenity, i) => (
                                                                <li key={i}>• {amenity}</li>
                                                            ))}
                                                        </ul>
                                                        <button className="mt-4 w-full bg-blue-600 text-white py-2 rounded-lg hover:bg-blue-700">Foglalás</button>
                                                    </div>
                                                </div>
                                            </div>
                                        ))}
                                    </div>
                                </div>
                                <button
                                    onClick={handleSlideRight}
                                    className="absolute right-0 top-1/2 transform -translate-y-1/2 bg-blue-600 text-white p-3 rounded-full shadow-lg hover:bg-blue-700 z-10"
                                >
                                    <span className="material-symbols-outlined">chevron_right</span>
                                </button>
                            </div>
                        )}
                    </div>
                </section>

                <section className="py-16 bg-blue-50">
                    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                        <h2 className="text-3xl md:text-5xl font-bold text-blue-900 text-center mb-12">Események</h2>
                        {programs.length === 0 ? (
                            <p className="text-center text-blue-700">Jelenleg nincsenek elérhető események.</p>
                        ) : (
                            <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-8">
                                {programs.map((program) => (
                                    <div key={program.id} className="bg-white rounded-lg shadow-md overflow-hidden">
                                        <div className="h-64 bg-cover bg-center" style={{ backgroundImage: `url(${program.images})` }}></div>
                                        <div className="p-6">
                                            <h3 className="text-xl font-bold text-blue-900 mb-2">{program.name}</h3>
                                            <p className="text-sm text-blue-700 mb-4">{truncateDescription(program.description, 80)}</p>
                                            <p className="text-blue-600 mb-2">Időpont: {new Date(program.schedule).toLocaleDateString('hu-HU')}</p>
                                            <p className="text-blue-600 mb-4">Helyszín: {program.location}</p>
                                            <Link to="/programok">
                                                <button className="w-full bg-blue-800 text-white py-2 rounded-lg font-semibold hover:bg-blue-900">Részletek</button>
                                            </Link>
                                        </div>
                                    </div>
                                ))}
                            </div>
                        )}
                    </div>
                </section>

                <section className="py-16 bg-white">
                    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8 text-center">
                        <h2 className="text-3xl md:text-5xl font-bold text-blue-900 mb-12">Vendégeink Véleménye</h2>
                        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
                            {[
                                { name: 'Kovács Anna', text: 'Kiváló kiszolgálás, modern és tiszta szobák.' },
                                { name: 'Nagy Péter', text: 'A design és a kényelem lenyűgöző, ajánlom mindenkinek!' },
                                { name: 'Szabó Eszter', text: 'Elegáns környezet, tökéletes pihenés.' }
                            ].map((testimonial, idx) => (
                                <div key={idx} className="bg-blue-50 p-6 rounded-lg shadow-md">
                                    <p className="text-blue-700 italic mb-4">"{testimonial.text}"</p>
                                    <p className="text-blue-900 font-semibold">{testimonial.name}</p>
                                </div>
                            ))}
                        </div>
                    </div>
                </section>

                <section className="py-16 bg-blue-50">
                    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
                            <div className="bg-white rounded-lg shadow-md p-6 text-center">
                                <span className="material-symbols-outlined text-4xl text-blue-600 mb-4">hotel</span>
                                <h3 className="text-xl font-bold text-blue-900 mb-4">Foglaljon Most!</h3>
                                <p className="text-blue-700 mb-6">Tapasztalja meg a modern luxust nálunk.</p>
                                <Link to="/szobak">
                                    <button className="bg-blue-600 text-white px-6 py-2 rounded-lg font-semibold hover:bg-blue-700">Foglalás</button>
                                </Link>
                            </div>
                            <div className="bg-white rounded-lg shadow-md p-6 text-center">
                                <span className="material-symbols-outlined text-4xl text-blue-600 mb-4">location_on</span>
                                <h3 className="text-xl font-bold text-blue-900 mb-4">Kapcsolat</h3>
                                <p className="text-blue-700">3526 Miskolc, Palóczy László utca 3</p>
                                <p className="text-blue-700">Telefon: +36 12 345 6789</p>
                                <p className="text-blue-700">Email: hmzrtkando@gmail.com</p>
                            </div>
                            <div className="bg-white rounded-lg shadow-md p-6 text-center">
                                <span className="material-symbols-outlined text-4xl text-blue-600 mb-4">schedule</span>
                                <h3 className="text-xl font-bold text-blue-900 mb-4">Nyitvatartás</h3>
                                <p className="text-blue-700">Recepció: 0-24</p>
                                <p className="text-blue-700">Spa: 8:00 - 20:00</p>
                                <p className="text-blue-700">Étterem: 7:00 - 22:00</p>
                            </div>
                        </div>
                        <div className="mt-12 text-center">
                            <h3 className="text-xl font-bold text-blue-900 mb-4">Gyors Linkek</h3>
                            <div className="flex justify-center space-x-6">
                                <Link to="/szobak" className="text-blue-600 hover:text-blue-800">Szobák</Link>
                                <Link to="/programok" className="text-blue-600 hover:text-blue-800">Programok</Link>
                                <Link to="/rolunk" className="text-blue-600 hover:text-blue-800">Kapcsolat</Link>
                            </div>
                        </div>
                    </div>
                </section>
            </main>
        </div>
    );
}

export default Hero;