import React, { useState } from 'react';

function Services() {
    const [hoveredCard, setHoveredCard] = useState(null);
    const [activeFilter, setActiveFilter] = useState('all');
    const [selectedService, setSelectedService] = useState(null);

    const services = [
        {
            id: 1,
            name: 'Wellness és Spa',
            description: 'Kényeztető masszázsok, szaunák és wellness szolgáltatások várják a vendégeket.',
            detailedDescription: 'Tapasztalja meg a teljes testi-lelki megújulást professzionális terapeutáink kezei alatt. Választható szolgáltatásaink között szerepel svéd masszázs, aromaterápiás kezelés, finn szauna és infraszauna használata, valamint relaxációs medence.',
            category: 'wellness',
            features: ['Masszázs', 'Szauna', 'Meditáció', 'Gyógyfürdő'],
            priceRange: '15,000 - 50,000 Ft',
            duration: '1-3 óra'
        },
        {
            id: 2,
            name: 'Éttermek',
            description: 'Prémium minőségű ételek és italok különleges atmoszférában.',
            detailedDescription: 'Három különböző éttermünk várja Önt: a Panorama étterem lélegzetelállító kilátással, a Bistro modern magyar fogásokkal, és a Fine Dining exkluzív gourmet élményekkel. Minden étteremben elérhető gyermekmenü és borlapunk 50+ válogatott tételt kínál.',
            category: 'dining',
            features: ['Helyi specialitások', 'Nemzetközi konyha', 'Vegán opciók', 'Sommelier'],
            priceRange: '5,000 - 30,000 Ft',
            duration: '1-2 óra'
        },
        {
            id: 3,
            name: 'Szobaszerviz',
            description: 'Non-stop szobaszerviz a kényelmed érdekében.',
            detailedDescription: 'Éjjel-nappal elérhető szolgáltatásunk biztosítja, hogy szobájában is élvezhesse éttermeink kínálatát. Speciális kéréseket is teljesítünk, mint például gluténmentes vagy vegetáriánus ételek, valamint extra párnák vagy takarók igényelhetők.',
            category: 'room',
            features: ['24/7 elérhetőség', 'Gyors kiszolgálás', 'Speciális kérések', 'Prémium minőség'],
            priceRange: 'Ingyenes',
            duration: '15-30 perc'
        },
        {
            id: 4,
            name: 'Konferencia termek',
            description: 'Modern felszereltségű konferencia termek üzleti eseményekre.',
            detailedDescription: 'Három különböző méretű terem áll rendelkezésre (20-150 fő), mindegyik felszerelve 4K projektorral, professzionális hangrendszerrel és gyors Wi-Fi-vel. Catering szolgáltatás és technikai support is kérhető.',
            category: 'business',
            features: ['HD projektor', 'Hangosítás', 'Kávészünet', 'Flipchart'],
            priceRange: '50,000 - 200,000 Ft',
            duration: 'Napijellegű'
        },
        {
            id: 5,
            name: 'Gyermekprogramok',
            description: 'Szórakoztató és oktató programok gyerekeknek.',
            detailedDescription: 'Napi programjaink között szerepel kézműves foglalkozás, kincskereső túra a kertben, interaktív mesedélután és esti mozi vetítés. 3-12 éves korú gyermekek számára, szakképzett animátorokkal.',
            category: 'family',
            features: ['Kreatív műhely', 'Túrák', 'Játékterem', 'Filmvetítés'],
            priceRange: 'Ingyenes - 10,000 Ft',
            duration: '1-4 óra'
        },
        {
            id: 6,
            name: 'Privát strand',
            description: 'Exkluzív privát strand medencével és napozóágyakkal.',
            detailedDescription: 'Saját partszakaszunk kristálytiszta vízzel, fűtött infinity medencével és prémium napozóágyakkal várja vendégeinket. Koktélbár és privát masszázs szolgáltatás is elérhető a teljes kikapcsolódásért.',
            category: 'wellness',
            features: ['Privát tér', 'Lombik', 'Bárpult', 'Masszázs'],
            priceRange: '5,000 - 20,000 Ft',
            duration: '2-6 óra'
        },
        {
            id: 7,
            name: 'Fitnesz és Sport',
            description: 'Modern edzőterem és szabadtéri sportlehetőségek.',
            detailedDescription: 'Teljesen felszerelt konditerem kardió és erőgépekkel, valamint szabadtéri teniszpálya és futóösvény. Személyi edző és csoportos jóga órák is foglalhatók.',
            category: 'wellness',
            features: ['Edzőterem', 'Tenisz', 'Jóga', 'Személyi edző'],
            priceRange: 'Ingyenes - 15,000 Ft',
            duration: '1-2 óra'
        },
        {
            id: 8,
            name: 'Borkóstoló',
            description: 'Vezetett borkóstoló helyi borászatok termékeivel.',
            detailedDescription: 'Fedezze fel a régió legjobb borait sommelier vezetésével. A 2 órás program során 6 különböző bort kóstolhat meg, párosítva helyi sajtokkal és falatkákkal.',
            category: 'dining',
            features: ['Helyi borok', 'Sommelier', 'Sajttál', 'Vezetett túra'],
            priceRange: '12,000 - 25,000 Ft',
            duration: '2 óra'
        },
        {
            id: 9,
            name: 'Csillagvizsgáló Est',
            description: 'Csillagnézés és ismeretterjesztés szakértő vezetésével.',
            detailedDescription: 'Professzionális teleszkópokkal felszerelt csillagvizsgálónkban megismerheti az éjszakai égbolt csodáit. A program tartalmaz egy rövid előadást a csillagászatról, majd vezetett megfigyelést a szabad ég alatt.',
            category: 'family',
            features: ['Teleszkóp', 'Előadás', 'Csillagképek', 'Gyerekbarát'],
            priceRange: '8,000 - 15,000 Ft',
            duration: '2-3 óra'
        },
        {
            id: 10,
            name: 'Kerékpártúra',
            description: 'Vezetett túrák a környék legszebb útvonalain.',
            detailedDescription: 'Különböző nehézségi szintű túrák közül választhat, kezdőtől a haladóig. Kerékpárokat és védőfelszerelést biztosítunk, a túrák során pedig helyi idegenvezető mutatja be a környék nevezetességeit.',
            category: 'wellness',
            features: ['Kerékpár bérlés', 'Idegenvezetés', 'Panoráma', 'Csoportos'],
            priceRange: '10,000 - 20,000 Ft',
            duration: '3-5 óra'
        },
        {
            id: 11,
            name: 'Privát Mozi',
            description: 'Személyre szabott filmvetítés exkluzív környezetben.',
            detailedDescription: 'Válassza ki kedvenc filmjét, és nézze meg egy privát, 10 fős moziteremben. Pattogatott kukorica, üdítők és kényelmes fotelek biztosítják a tökéletes moziélményt.',
            category: 'room',
            features: ['Privát terem', 'Filmválasztás', 'Snack', 'Hangrendszer'],
            priceRange: '20,000 - 40,000 Ft',
            duration: '2-3 óra'
        },
        {
            id: 12,
            name: 'Workshopok',
            description: 'Kreatív és üzleti workshopok szakértők vezetésével.',
            detailedDescription: 'Választható témák: fotózás, főzés, prezentációs készségek vagy csapatépítés. Minden workshop gyakorlati oktatást kínál, kis létszámú csoportokban, hogy mindenki aktívan részt vehessen.',
            category: 'business',
            features: ['Szakértő oktató', 'Gyakorlat', 'Eszközök', 'Csoportos'],
            priceRange: '15,000 - 35,000 Ft',
            duration: '2-4 óra'
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
                {/* Header */}
                <div className="text-center mb-16">
                    <h1 className="text-4xl font-extrabold text-gray-900 sm:text-5xl lg:text-6xl">
                        Szolgáltatásaink
                    </h1>
                    <p className="mt-5 max-w-xl mx-auto text-xl text-gray-500">
                        Fedezze fel exkluzív kínálatunkat, amely a kényelmet és a luxust helyezi előtérbe
                    </p>
                </div>

                {/* Filters */}
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

                {/* Service Cards */}
                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-8">
                    {filteredServices.map((service) => (
                        <div
                            key={service.id}
                            className={`relative overflow-hidden rounded-2xl shadow-xl transition-all duration-500 transform ${hoveredCard === service.id ? 'scale-105 shadow-2xl' : 'scale-100'}`}
                            onMouseEnter={() => setHoveredCard(service.id)}
                            onMouseLeave={() => setHoveredCard(null)}
                        >
                            <div className="h-48 bg-gradient-to-br from-blue-600 via-blue-400 to-blue-300 text-white flex justify-center items-center">
                                <span className="font-bold text-2xl">{service.name}</span>
                            </div>

                            <div className="bg-white p-6">
                                <div className="flex justify-between items-start">
                                    <h3 className="text-xl font-bold text-gray-900">{service.name}</h3>
                                    <span className="inline-block bg-blue-100 text-blue-800 text-xs px-2 py-1 rounded-full">
                                        {service.priceRange}
                                    </span>
                                </div>

                                <p className="mt-3 text-gray-600">{service.description}</p>

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

                                <div className="mt-6 flex items-center justify-between">
                                    <span className="text-sm text-gray-500">
                                        <svg className="w-4 h-4 inline mr-1" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                                            <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z" />
                                        </svg>
                                        {service.duration}
                                    </span>

                                    <button
                                        onClick={() => setSelectedService(selectedService === service.id ? null : service.id)}
                                        className="inline-flex items-center px-4 py-2 border border-transparent text-sm font-medium rounded-md shadow-sm text-white bg-blue-600 hover:bg-blue-700 focus:outline-none focus:ring-2 focus:ring-offset-2 focus:ring-blue-500 transition-colors duration-300"
                                    >
                                        {selectedService === service.id ? 'Kevesebb' : 'Részletek'}
                                        <svg className="ml-2 -mr-1 w-4 h-4" fill="currentColor" viewBox="0 0 20 20">
                                            <path fillRule="evenodd" d="M10.293 5.293a1 1 0 011.414 0l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414-1.414L12.586 11H5a1 1 0 110-2h7.586l-2.293-2.293a1 1 0 010-1.414z" clipRule="evenodd" />
                                        </svg>
                                    </button>
                                </div>

                                {/* Detailed Description */}
                                {selectedService === service.id && (
                                    <div className="mt-4 p-4 bg-blue-50 rounded-lg transition-all duration-300">
                                        <p className="text-gray-700">{service.detailedDescription}</p>
                                    </div>
                                )}
                            </div>

                            {hoveredCard === service.id && (
                                <div className="absolute inset-0 border-2 border-blue-400 rounded-2xl pointer-events-none" />
                            )}
                        </div>
                    ))}
                </div>

                {/* Extra Info */}
                <div className="mt-20 bg-blue-600 rounded-2xl p-8 text-white">
                    <div className="max-w-3xl mx-auto text-center">
                        <h2 className="text-3xl font-bold mb-4">Egyedi igények, kivételes szolgáltatások</h2>
                        <p className="text-blue-100 mb-6">
                            Ha speciális igénye van, vagy csoportos foglalást tervez, kérjük keressen minket bizalommal.
                        </p>
                        <button className="inline-flex items-center px-6 py-3 border border-transparent text-base font-medium rounded-md text-blue-700 bg-white hover:bg-blue-50 transition-colors duration-300">
                            Kapcsolatfelvétel
                            <svg className="ml-3 -mr-1 w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                                <path fillRule="evenodd" d="M10.293 5.293a1 1 0 011.414 0l4 4a1 1 0 010 1.414l-4 4a1 1 0 01-1.414-1.414L12.586 11H5a1 1 0 110-2h7.586l-2.293-2.293a1 1 0 010-1.414z" clipRule="evenodd" />
                            </svg>
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
}

export default Services;