import React, { useState } from "react";

function Services() {
  const [hoveredCard, setHoveredCard] = useState(null);
  const [activeFilter, setActiveFilter] = useState("all");
  const [selectedService, setSelectedService] = useState(null);

  const services = [
    {
      id: 1,
      name: "Wellness és Spa",
      description: "Kényeztető masszázsok, szaunák és wellness szolgáltatások várják a vendégeket.",
      detailedDescription:
        "Tapasztalja meg a teljes testi-lelki megújulást professzionális terapeutáink kezei alatt. Választható szolgáltatásaink között szerepel svéd masszázs, aromaterápiás kezelés, finn szauna és infraszauna használata, valamint relaxációs medence.",
      category: "wellness",
      features: ["Masszázs", "Szauna", "Meditáció", "Gyógyfürdő"],
      priceRange: "15,000 - 50,000 Ft",
      duration: "1-3 óra",
    },
    {
      id: 2,
      name: "Éttermek",
      description: "Prémium minőségű ételek és italok különleges atmoszférában.",
      detailedDescription:
        "Három különböző éttermünk várja Önt: a Panorama étterem lélegzetelállító kilátással, a Bistro modern magyar fogásokkal, és a Fine Dining exkluzív gourmet élményekkel. Minden étteremben elérhető gyermekmenü és borlapunk 50+ válogatott tételt kínál.",
      category: "dining",
      features: ["Helyi specialitások", "Nemzetközi konyha", "Vegán opciók", "Sommelier"],
      priceRange: "5,000 - 30,000 Ft",
      duration: "1-2 óra",
    },
    {
      id: 3,
      name: "Szobaszerviz",
      description: "Non-stop szobaszerviz a kényelmed érdekében.",
      detailedDescription:
        "Éjjel-nappal elérhető szolgáltatásunk biztosítja, hogy szobájában is élvezhesse éttermeink kínálatát. Speciális kéréseket is teljesítünk, mint például gluténmentes vagy vegetáriánus ételek, valamint extra párnák vagy takarók igényelhetők.",
      category: "room",
      features: ["24/7 elérhetőség", "Gyors kiszolgálás", "Speciális kérések", "Prémium minőség"],
      priceRange: "Ingyenes",
      duration: "15-30 perc",
    },
    {
      id: 4,
      name: "Konferencia termek",
      description: "Modern felszereltségű konferencia termek üzleti eseményekre.",
      detailedDescription:
        "Három különböző méretű terem áll rendelkezésre (20-150 fő), mindegyik felszerelve 4K projektorral, professzionális hangrendszerrel és gyors Wi-Fi-vel. Catering szolgáltatás és technikai support is kérhető.",
      category: "business",
      features: ["HD projektor", "Hangosítás", "Kávészünet", "Flipchart"],
      priceRange: "50,000 - 200,000 Ft",
      duration: "Napijellegű",
    },
    {
      id: 5,
      name: "Gyermekprogramok",
      description: "Szórakoztató és oktató programok gyerekeknek.",
      detailedDescription:
        "Napi programjaink között szerepel kézműves foglalkozás, kincskereső túra a kertben, interaktív mesedélután és esti mozi vetítés. 3-12 éves korú gyermekek számára, szakképzett animátorokkal.",
      category: "family",
      features: ["Kreatív műhely", "Túrák", "Játékterem", "Filmvetítés"],
      priceRange: "Ingyenes - 10,000 Ft",
      duration: "1-4 óra",
    },
    {
      id: 6,
      name: "Privát strand",
      description: "Exkluzív privát strand medencével és napozóágyakkal.",
      detailedDescription:
        "Saját partszakaszunk kristálytiszta vízzel, fűtött infinity medencével és prémium napozóágyakkal várja vendégeinket. Koktélbár és privát masszázs szolgáltatás is elérhető a teljes kikapcsolódásért.",
      category: "wellness",
      features: ["Privát tér", "Lombik", "Bárpult", "Masszázs"],
      priceRange: "5,000 - 20,000 Ft",
      duration: "2-6 óra",
    },
    {
      id: 7,
      name: "Fitnesz és Sport",
      description: "Modern edzőterem és szabadtéri sportlehetőségek.",
      detailedDescription:
        "Teljesen felszerelt konditerem kardió és erőgépekkel, valamint szabadtéri teniszpálya és futóösvény. Személyi edző és csoportos jóga órák is foglalhatók.",
      category: "wellness",
      features: ["Edzőterem", "Tenisz", "Jóga", "Személyi edző"],
      priceRange: "Ingyenes - 15,000 Ft",
      duration: "1-2 óra",
    },
    {
      id: 8,
      name: "Borkóstoló",
      description: "Vezetett borkóstoló helyi borászatok termékeivel.",
      detailedDescription:
        "Fedezze fel a régió legjobb borait sommelier vezetésével. A 2 órás program során 6 különböző bort kóstolhat meg, párosítva helyi sajtokkal és falatkákkal.",
      category: "dining",
      features: ["Helyi borok", "Sommelier", "Sajttál", "Vezetett túra"],
      priceRange: "12,000 - 25,000 Ft",
      duration: "2 óra",
    },
    {
      id: 9,
      name: "Csillagvizsgáló Est",
      description: "Csillagnézés és ismeretterjesztés szakértő vezetésével.",
      detailedDescription:
        "Professzionális teleszkópokkal felszerelt csillagvizsgálónkban megismerheti az éjszakai égbolt csodáit. A program tartalmaz egy rövid előadást a csillagászatról, majd vezetett megfigyelést a szabad ég alatt.",
      category: "family",
      features: ["Teleszkóp", "Előadás", "Csillagképek", "Gyerekbarát"],
      priceRange: "8,000 - 15,000 Ft",
      duration: "2-3 óra",
    },
    {
      id: 10,
      name: "Kerékpártúra",
      description: "Vezetett túrák a környék legszebb útvonalain.",
      detailedDescription:
        "Különböző nehézségi szintű túrák közül választhat, kezdőtől a haladóig. Kerékpárokat és védőfelszerelést biztosítunk, a túrák során pedig helyi idegenvezető mutatja be a környék nevezetességeit.",
      category: "wellness",
      features: ["Kerékpár bérlés", "Idegenvezetés", "Panoráma", "Csoportos"],
      priceRange: "10,000 - 20,000 Ft",
      duration: "3-5 óra",
    },
    {
      id: 11,
      name: "Privát Mozi",
      description: "Személyre szabott filmvetítés exkluzív környezetben.",
      detailedDescription:
        "Válassza ki kedvenc filmjét, és nézze meg egy privát, 10 fős moziteremben. Pattogatott kukorica, üdítők és kényelmes fotelek biztosítják a tökéletes moziélményt.",
      category: "room",
      features: ["Privát terem", "Filmválasztás", "Snack", "Hangrendszer"],
      priceRange: "20,000 - 40,000 Ft",
      duration: "2-3 óra",
    },
    {
      id: 12,
      name: "Workshopok",
      description: "Kreatív és üzleti workshopok szakértők vezetésével.",
      detailedDescription:
        "Választható témák: fotózás, főzés, prezentációs készségek vagy csapatépítés. Minden workshop gyakorlati oktatást kínál, kis létszámú csoportokban, hogy mindenki aktívan részt vehessen.",
      category: "business",
      features: ["Szakértő oktató", "Gyakorlat", "Eszközök", "Csoportos"],
      priceRange: "15,000 - 35,000 Ft",
      duration: "2-4 óra",
    },
  ];

  const filteredServices =
    activeFilter === "all" ? services : services.filter((service) => service.category === activeFilter);

  const categories = [
    { id: "all", name: "Összes" },
    { id: "wellness", name: "Wellness" },
    { id: "dining", name: "Étkezés" },
    { id: "room", name: "Szoba" },
    { id: "business", name: "Üzleti" },
    { id: "family", name: "Családos" },
  ];

  return (
    <div className="min-h-screen bg-blue-50 py-12 px-4 sm:px-6 lg:px-8">
      <div className="max-w-7xl mx-auto">
        <div className="text-center mb-12">
          <h1 className="text-4xl font-bold text-blue-800 animate-bounce">Szolgáltatásaink</h1>
          <p className="mt-4 text-lg text-gray-600 max-w-2xl mx-auto">
            Fedezze fel prémium szolgáltatásainkat, amelyek a tökéletes kikapcsolódást és kényelmet biztosítják.
          </p>
        </div>

        <div className="flex flex-wrap justify-center gap-3 mb-12">
          {categories.map((category) => (
            <button
              key={category.id}
              onClick={() => setActiveFilter(category.id)}
              className={`px-5 py-2 rounded-full text-sm font-semibold ${
                activeFilter === category.id
                  ? "bg-blue-600 text-white shadow-lg"
                  : "bg-blue-100 text-blue-800 hover:bg-blue-200 shadow-md"
              }`}
            >
              {category.name}
            </button>
          ))}
        </div>

        <div className="grid grid-cols-1 sm:grid-cols-2 lg:grid-cols-3 gap-8">
          {filteredServices.map((service) => (
            <div
              key={service.id}
              className={`bg-white rounded-xl shadow-lg overflow-hidden hover:shadow-2xl hover:scale-105 transform animate-fade-in ${
                hoveredCard === service.id ? "border-2 border-blue-500" : ""
              }`}
              onMouseEnter={() => setHoveredCard(service.id)}
              onMouseLeave={() => setHoveredCard(null)}
            >
              <div className="h-48 bg-gradient-to-r from-blue-600 to-blue-400 flex items-center justify-center">
                <h3 className="text-2xl font-bold text-white text-center px-4">{service.name}</h3>
              </div>
              <div className="p-6">
                <div className="flex justify-between items-center mb-4">
                  <span className="text-xl font-semibold text-blue-800">{service.name}</span>
                  <span className="bg-blue-100 text-blue-800 px-2 py-1 rounded-full text-xs font-medium">
                    {service.priceRange}
                  </span>
                </div>
                <p className="text-gray-600 mb-4">{service.description}</p>
                <div className="mb-4">
                  <h4 className="text-sm font-semibold text-blue-800 mb-2">Jellemzők</h4>
                  <div className="flex flex-wrap gap-2">
                    {service.features.map((feature) => (
                      <span
                        key={feature}
                        className="bg-blue-100 text-blue-800 px-2 py-1 rounded-full text-xs font-medium"
                      >
                        {feature}
                      </span>
                    ))}
                  </div>
                </div>
                <div className="flex justify-between items-center">
                  <span className="text-sm text-gray-600 flex items-center gap-1">
                    <svg className="w-4 h-4" fill="none" stroke="currentColor" viewBox="0 0 24 24">
                      <path
                        strokeLinecap="round"
                        strokeLinejoin="round"
                        strokeWidth="2"
                        d="M12 8v4l3 3m6-3a9 9 0 11-18 0 9 9 0 0118 0z"
                      />
                    </svg>
                    {service.duration}
                  </span>
                  <button
                    onClick={() =>
                      setSelectedService(selectedService === service.id ? null : service.id)
                    }
                    className="bg-blue-600 text-white px-4 py-2 rounded-lg font-semibold hover:bg-blue-800 shadow-md"
                  >
                    {selectedService === service.id ? "Kevesebb" : "Részletek"}
                  </button>
                </div>
                {selectedService === service.id && (
                  <div className="mt-4 p-4 bg-blue-50 rounded-lg animate-fade-in">
                    <p className="text-gray-700">{service.detailedDescription}</p>
                  </div>
                )}
              </div>
            </div>
          ))}
        </div>

        <div className="mt-16 bg-blue-600 rounded-xl p-8 text-white text-center">
          <h2 className="text-3xl font-bold mb-4">Különleges igények?</h2>
          <p className="text-blue-100 mb-6 max-w-xl mx-auto">
            Csoportos foglalás vagy egyedi kérések esetén lépjen velünk kapcsolatba!
          </p>
          <a
            href="https://mail.google.com/mail/u/0/?view=cm&fs=1&to=hmzrtkando@gmail.com"
            target="_blank"
            rel="noopener noreferrer"
            className="inline-flex items-center bg-white text-blue-600 px-6 py-3 rounded-lg font-semibold hover:bg-blue-50 shadow-md"
          >
            <svg className="w-5 h-5 mr-2" fill="none" stroke="currentColor" viewBox="0 0 24 24">
              <path
                strokeLinecap="round"
                strokeLinejoin="round"
                strokeWidth="2"
                d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z"
              />
            </svg>
            Kapcsolat
          </a>
        </div>
      </div>
    </div>
  );
}

export default Services;