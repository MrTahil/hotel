import React, { useState, useEffect } from 'react';
import '../styles/Hero.css';
import { Link, useNavigate } from 'react-router-dom';
import axios from 'axios';

function Hero() {
    const [rooms, setRooms] = useState([]);
    const [programs, setPrograms] = useState([]);
    const [loading, setLoading] = useState(true);
    const [selectedFeature, setSelectedFeature] = useState(null);
    const navigate = useNavigate();

    useEffect(() => {
        const fetchRooms = async () => {
            try {
                const response = await axios.get(process.env.REACT_APP_API_BASE_URL + '/Rooms/GetRoomWith');
                if (response.status !== 200) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }

                const data = response.data;

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
            } catch (error) {
                console.error('Error fetching rooms:', error);
                setRooms([]);
            } finally {
                setLoading(false);
            }
        };

        const fetchEvents = async () => {
            try {
                const response = await axios.get(process.env.REACT_APP_API_BASE_URL + `/Events/Geteents`);
                if (response.status !== 200) {
                    throw new Error(`HTTP error! status: ${response.status}`);
                }

                const data = response.data;

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
            } catch (error) {
                console.error('Error fetching events:', error);
                setPrograms([]);
            }
        };

        fetchRooms();
        fetchEvents();
    }, []);

    const truncateDescription = (description, maxLength) => {
        return description && description.length > maxLength
            ? description.substring(0, maxLength) + '...'
            : description || 'Nincs leírás';
    };

    const handleRoomClick = () => {
        const token = localStorage.getItem('authToken');
        if (token) navigate('/szobak');
        else navigate('/login');
    };

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

    const testimonials = [
        {
            name: "Kovács Anna",
            role: "Üzleti utas",
            content: "A legjobb szálloda, ahol valaha megfordultam! A személyzet figyelmes, a szoba hihetetlenül kényelmes volt, és a wellness részleg tökéletes volt a munka utáni relaxáláshoz.",
            rating: 5,
            image: "https://randomuser.me/api/portraits/women/43.jpg"
        },
        {
            name: "Ardelean Milán",
            role: "Honeymoon utas",
            content: "Tökéletes választás volt a nászútunkra! A kilátás a tengerre lenyűgöző, a séf különleges vacsorát készített nekünk (bundáskenyér) , és az egész élmény varázslatos volt. Annyi a kár hogy xbox helyett ps-t kellett vona a tvk-hez adni. TT!",
            rating: 5,
            image: "https://th.bing.com/th/id/OIP.3H2ZzNxqIihEaOOlCg5jFAHaE_?rs=1&pid=ImgDetMain"
        },
        {
            name: "Tóth Eszter",
            role: "Családos anyuka",
            content: "A gyerekek imádták a medencét és a játékteret, míg mi felnőttek élveztük a spa kezeléseket. Mindenki megtalálta a számára való szórakozást!",
            rating: 4,
            image: "https://randomuser.me/api/portraits/women/65.jpg"
        }
    ];

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
                    <h1 className="animate-bounce text-4xl md:text-6xl font-bold mb-6">Üdvözöljük az álomnyaralás kezdetén</h1>
                    <p className="text-lg md:text-2xl max-w-3xl mx-auto mb-8 text-blue-100">
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

                <section className="py-16 bg-blue-100">
                    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                        <h2 className="text-3xl font-bold text-blue-900 mb-8 text-center">
                            Fedezze Fel Szobáinkat
                        </h2>
                        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
                            {rooms.slice(0, 3).map((room) => (
                                <div
                                    key={room.id}
                                    className="bg-white rounded-xl shadow-md overflow-hidden hover:shadow-xl transition-shadow duration-300 cursor-pointer"
                                    onClick={handleRoomClick}
                                >
                                    <img
                                        src={room.image}
                                        alt={room.name}
                                        className="w-full h-56 object-cover"
                                    />
                                    <div className="p-6">
                                        <h3 className="font-bold text-xl mb-2 text-blue-900">
                                            {room.name}
                                        </h3>
                                        <p className="text-blue-700 mb-4">
                                            {truncateDescription(room.description, 100)}
                                        </p>
                                        <p className="text-blue-600">
                                            Ár: {room.price} / éjszaka
                                        </p>
                                    </div>
                                </div>
                            ))}
                        </div>
                    </div>
                </section>

                <section className="py-16">
                    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                        <h2 className="text-3xl font-bold text-blue-900 mb-8 text-center">
                            Programok és Események
                        </h2>
                        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
                            {programs.map((program) => (
                                <div
                                    key={program.id}
                                    className="bg-white rounded-xl shadow-md overflow-hidden hover:shadow-xl transition-shadow duration-300"
                                >
                                    <img
                                        src={program.images}
                                        alt={program.name}
                                        className="w-full h-56 object-cover"
                                    />
                                    <div className="p-6">
                                        <h3 className="font-bold text-xl mb-2 text-blue-900">
                                            {program.name}
                                        </h3>
                                        <p className="text-blue-700 mb-4">
                                            {truncateDescription(program.description, 80)}
                                        </p>
                                        <p className="text-blue-600">
                                            Időpont: {new Date(program.schedule).toLocaleDateString()}
                                        </p>
                                        <p className="text-blue-600">
                                            Helyszín: {program.location}
                                        </p>
                                    </div>
                                </div>
                            ))}
                        </div>
                    </div>
                </section>

                {/* Testimonial Section */}
                {/* Testimonial Section */}
                <section className="py-16 bg-white">
                    <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                        <h2 className="text-3xl font-bold text-blue-900 mb-12 text-center">Vendégeink Véleménye</h2>
                        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
                            {testimonials.map((testimonial, index) => (
                                <div key={index} className="bg-blue-50 p-6 rounded-lg shadow-lg">
                                    <div className="flex items-center mb-4">
                                        <img
                                            src={testimonial.image}
                                            alt={testimonial.name}
                                            className="w-12 h-12 rounded-full object-cover mr-4"
                                        />
                                        <div>
                                            <h4 className="font-bold text-lg text-blue-900">{testimonial.name}</h4>
                                            <p className="text-blue-600 text-sm">{testimonial.role}</p>
                                        </div>
                                    </div>
                                    <p className="mb-4 text-blue-700">"{testimonial.content}"</p>
                                    <div className="flex">
                                        {[...Array(5)].map((_, i) => (
                                            <svg
                                                key={i}
                                                className={`w-5 h-5 ${i < testimonial.rating ? 'text-yellow-400' : 'text-gray-300'}`}
                                                fill="currentColor"
                                                viewBox="0 0 20 20"
                                            >
                                                <path d="M9.049 2.927c.3-.921 1.603-.921 1.902 0l1.07 3.292a1 1 0 00.95.69h3.462c.969 0 1.371 1.24.588 1.81l-2.8 2.034a1 1 0 00-.364 1.118l1.07 3.292c.3.921-.755 1.688-1.54 1.118l-2.8-2.034a1 1 0 00-1.175 0l-2.8 2.034c-.784.57-1.838-.197-1.539-1.118l1.07-3.292a1 1 0 00-.364-1.118L2.98 8.72c-.783-.57-.38-1.81.588-1.81h3.461a1 1 0 00.951-.69l1.07-3.292z" />
                                            </svg>
                                        ))}
                                    </div>
                                </div>
                            ))}
                        </div>
                    </div>
                </section>
            </main>

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
                        <p className="text-blue-700 mb-4">{selectedFeature.detailedDesc.description}</p>
                        {selectedFeature.detailedDesc.facilities && (
                            <>
                                <h4 className="font-bold text-blue-900 mb-2">Felszereltség:</h4>
                                <ul className="list-disc list-inside text-blue-700 mb-4">
                                    {selectedFeature.detailedDesc.facilities.map((facility, index) => (
                                        <li key={index}>{facility}</li>
                                    ))}
                                </ul>
                            </>
                        )}
                        {selectedFeature.detailedDesc.menuHighlights && (
                            <>
                                <h4 className="font-bold text-blue-900 mb-2">Kiemelt Fogások:</h4>
                                <ul className="list-disc list-inside text-blue-700 mb-4">
                                    {selectedFeature.detailedDesc.menuHighlights.map((highlight, index) => (
                                        <li key={index}>{highlight}</li>
                                    ))}
                                </ul>
                            </>
                        )}
                        <p className="text-blue-700 mb-2">
                            <span className="font-bold text-blue-900">Nyitvatartás:</span>{' '}
                            {selectedFeature.detailedDesc.openingHours}
                        </p>
                        <p className="text-blue-700 mb-2">
                            <span className="font-bold text-blue-900">Árak:</span>{' '}
                            {selectedFeature.detailedDesc.pricing}
                        </p>
                        <p className="text-blue-700">
                            <span className="font-bold text-blue-900">További Információ:</span>{' '}
                            {selectedFeature.detailedDesc.additionalInfo}
                        </p>
                    </div>
                </div>
            )}
        </div>
    );
}

export default Hero;