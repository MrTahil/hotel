import React, { useState } from 'react';
import { Link } from 'react-router-dom';

function Services() {
    const [hoveredCard, setHoveredCard] = useState(null);
    const [activeFilter, setActiveFilter] = useState('all');

    const services = [
        {
            id: 1,
            name: 'Wellness és Spa',
            description: 'Kényeztető masszázsok, szaunák és wellness szolgáltatások várják a vendégeket, hogy feltöltődjenek és ellazuljanak.',
            category: 'wellness',
            features: ['Masszázs', 'Szauna', 'Meditáció', 'Gyógyfürdő'],
            priceRange: '15,000 - 50,000 Ft',
            duration: '1-3 óra'
        },
        {
            id: 2,
            name: 'Éttermek',
            description: 'Prémium minőségű ételek és italok különleges atmoszférában. Séfjeink kreatív fogásai garantálják a felejthetetlen élményt.',
            category: 'dining',
            features: ['Helyi specialitások', 'Nemzetközi konyha', 'Vegán opciók', 'Sommelier'],
            priceRange: '5,000 - 30,000 Ft',
            duration: '1-2 óra'
        },
        {
            id: 3,
            name: 'Szobaszerviz',
            description: 'Non-stop szobaszerviz a kényelmed érdekében. Bármikor rendelhetsz friss ételeket, italokat vagy egyéb szolgáltatásokat.',
            category: 'room',
            features: ['24/7 elérhetőség', 'Gyors kiszolgálás', 'Speciális kérések', 'Prémium minőség'],
            priceRange: 'Ingyenes',
            duration: '15-30 perc'
        },
        {
            id: 4,
            name: 'Konferencia termek',
            description: 'Modern felszereltségű konferencia termek, amelyek ideálisak üzleti találkozókra, tréningekre és eseményekre.',
            category: 'business',
            features: ['HD projektor', 'Hangosítás', 'Kávészünet', 'Flipchart'],
            priceRange: '50,000 - 200,000 Ft',
            duration: 'Napijellegű'
        },
        {
            id: 5,
            name: 'Gyermekprogramok',
            description: 'Szórakoztató és oktató programok a legkisebb vendégeinknek. Képzett animátoraink felügyelete mellett.',
            category: 'family',
            features: ['Kreatív műhely', 'Túrák', 'Játékterem', 'Filmvetítés'],
            priceRange: 'Ingyenes - 10,000 Ft',
            duration: '1-4 óra',
            link: '/programok' // ide kerül a link
        },
        {
            id: 6,
            name: 'Privát strand',
            description: 'Exkluzív privát strand medencével és napozóágyakkal, ahol teljes magánéletet élvezhet.',
            category: 'wellness',
            features: ['Privát tér', 'Lombik', 'Bárpult', 'Masszázs'],
            priceRange: '5,000 - 20,000 Ft',
            duration: '2-6 óra'
        }
    ];

    const filteredServices = activeFilter === 'all'
        ? services
        : services.filter(service => service.category === activeFilter);

    const categories = [
        { id: 'all', name: 'Összes' },
        { id: 'wellness', name: 'Wellness' },
        { id: 'dining', name: 'Étkezés' },
        { id: 'room', name: 'Szoba' },
        { id: 'business', name: 'Üzleti' },
        { id: 'family', name: 'Családos' }
    ];

    return (
        <div className="min-h-screen bg-gradient-to-b from-blue-50 to-white py-12 px-4 sm:px-6 lg:px-8">
            <div className="max-w-7xl mx-auto">
                {/* Fejléc rész */}
                <div className="text-center mb-16">
                    <h1 className="text-4xl font-extrabold text-gray-900 sm:text-5xl sm:tracking-tight lg:text-6xl">
                        Szolgáltatásaink
                    </h1>
                    <p className="mt-5 max-w-xl mx-auto text-xl text-gray-500">
                        Fedezze fel exkluzív kínálatunkat, amely a kényelmet és a luxust helyezi előtérbe
                    </p>
                </div>

                {/* Szűrők */}
                <div className="flex flex-wrap justify-center gap-3 mb-12">
                    {categories.map((category) => (
                        <button
                            key={category.id}
                            onClick={() => setActiveFilter(category.id)}
                            className={`px-6 py-2 rounded-full text-sm font-medium transition-all duration-300 ${activeFilter === category.id ? 'bg-blue-600 text-white shadow-lg' : 'bg-white text-gray-700 hover:bg-gray-100 shadow-md'}`}
                        >
                            {category.name}
                        </button>
                    ))}
                </div>

                {/* Szolgáltatás kártyák */}
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
                    {filteredServices.map((service) => (
                        <div
                            key={service.id}
                            className={`relative overflow-hidden rounded-2xl shadow-xl transition-all duration-500 transform ${hoveredCard === service.id ? 'scale-105 shadow-2xl' : 'scale-100'}`}
                            onMouseEnter={() => setHoveredCard(service.id)}
                            onMouseLeave={() => setHoveredCard(null)}
                        >
                            {/* Kép rész (kép nincs, de egy színes háttér) */}
                            <div className="h-48 bg-gradient-to-br from-blue-600 via-blue-400 to-blue-300 text-white flex justify-center items-center">
    <span className="font-bold text-2xl">{service.name}</span>
</div>









                            {/* Tartalom */}
                            <div className="bg-white p-6">
                                <div className="flex justify-between items-start">
                                    <h3 className="text-xl font-bold text-gray-900">{service.name}</h3>
                                    <span className="inline-block bg-blue-100 text-blue-800 text-xs px-2 py-1 rounded-full">
                                        {service.priceRange}
                                    </span>
                                </div>

                                <p className="mt-3 text-gray-600">{service.description}</p>

                                {/* Jellemzők */}
                                <div className="mt-4">
                                    <h4 className="text-sm font-semibold text-gray-900 mb-2">Jellemzők:</h4>
                                    <div className="flex flex-wrap gap-2">
                                        {service.features.map((feature, index) => (
                                            <span
                                                key={index}
                                                className="inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium bg-blue-100 text-blue-800"
                                            >
                                                {feature}
                                            </span>
                                        ))}
                                    </div>
                                </div>

                                {/* Időtartam és gomb */}
                                <div className="mt-6 flex items-center justify-between">
                                    <span className="text-sm text-gray-500">
                                        <svg
                                            className="w-4 h-4 inline mr-1"
                                            fill="none"
                                            stroke="currentColor"
                                            viewBox="0 0 24 24"
                                            xmlns="http://www.w3.org/2000/svg"
                                        >
                                            <path
                                                strokeLinecap="round"
                                                strokeLinejoin="round"
                                                strokeWidth={2}
                                                d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"
                                            />
                                        </svg>
                                        {service.duration}
                                    </span>

                                    <Link
                                        to={`/programok`}
                                        className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-colors duration-300"
                                    >
                                        Részletek
                                        <svg
                                            className="ml-2 -mr-1 w-4 h-4"
                                            fill="currentColor"
                                            viewBox="0 0 20 20"
                                            xmlns="http://www.w3.org/2000/svg"
                                        >
                                            <path
                                                fillRule="evenodd"
                                                d="M10.293 5.293a1 1 0 011.414 0l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414-1.414L12.586 11H5a1 1 0 110-2h7.586l-2.293-2.293a1 1 0 010-1.414z"
                                                clipRule="evenodd"
                                            />
                                        </svg>
                                    </Link>

                                </div>
                            </div>

                            {/* Hover effekt */}
                            {hoveredCard === service.id && (
                                <div className="absolute inset-0 border-2 border-blue-400 rounded-2xl pointer-events-none" />
                            )}
                        </div>
                    ))}
                </div>

                {/* Extra információ */}
                <div className="mt-20 bg-blue-600 rounded-2xl p-8 text-white">
                    <div className="max-w-3xl mx-auto text-center">
                        <h2 className="text-3xl font-bold mb-4">Egyedi igények, kivételes szolgáltatások</h2>
                        <p className="text-blue-100 mb-6">
                            Ha speciális igénye van, vagy csoportos foglalást tervez, kérjük keressen minket bizalommal.
                            Mindent megteszünk, hogy maradandó élményt nyújtsunk Önnek és családjának.
                        </p>
                        <button className="inline-flex items-center px-6 py-3 border border-transparent text-base font-medium rounded-md text-blue-700 bg-white hover:bg-blue-50 transition-colors duration-300">
                            Kapcsolatfelvétel
                            <svg
                                className="ml-3 -mr-1 w-5 h-5"
                                fill="currentColor"
                                viewBox="0 0 20 20"
                                xmlns="http://www.w3.org/2000/svg"
                            >
                                <path
                                    fillRule="evenodd"
                                    d="M10.293 5.293a1 1 0 011.414 0l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414-1.414L12.586 11H5a1 1 0 110-2h7.586l-2.293-2.293a1 1 0 010-1.414z"
                                    clipRule="evenodd"
                                />
                            </svg>
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Services;
