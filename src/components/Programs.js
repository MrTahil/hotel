import React, { useState } from 'react';
import '../styles/serprog.css';

function Programs() {
    const programs = [
        {
            id: 1,
            name: 'Jóga a hegyekben',
            description: 'Reggeli jógaórák a friss levegőn, gyönyörű kilátással.',
            image: '../img/jogahegyen.png',
            details: 'Részletes információk a programról: Egy csodálatos nap kezdete, amikor a friss hegyi levegőn végezheted el a gyakorlatokat, miközben csodálatos kilátásban gyönyörködhetsz.',
            schedule: 'Hétfő, Szerda, Péntek - 7:00-8:00',
        },
        {
            id: 2,
            name: 'Borkóstoló',
            description: 'Kóstold meg a helyi borászatok legjobb borait.',
            image: '../img/bor.png',
            details: 'A borkóstoló során megismerkedhetsz a helyi borászatok legfinomabb boraival, miközben egy helyi szakértő bemutatja a borok történelmét és készítési folyamatát.',
            schedule: 'Kedd, Csütörtök - 15:00-17:00',
        },
        {
            id: 3,
            name: 'Túrázás',
            description: 'Vezetett túrák a környező hegyekben és erdőkben.',
            image: '../img/tour.png',
            details: 'Fedezd fel a környező hegyek és erdők szépségét vezetett túráink segítségével. Tapasztalt túravezetők segítenek a természet felfedezésében.',
            schedule: 'Minden nap - 9:00-12:00',
        },
        {
            id: 4,
            name: 'Főzőtanfolyam',
            description: 'Tanulj meg tradicionális ételeket készíteni helyi szakácsokkal.',
            image: '../img/fozo.png',
            details: 'A főzőtanfolyam során a helyi szakácsok segítségével tradicionális éttermek ínycsiklandó fogásait tanulhatod meg elkészíteni.',
            schedule: 'Péntek - 18:00-21:00',
        },
        {
            id: 5,
            name: 'Szabadidős sportok',
            description: 'Futás, kerékpározás és egyéb szabadtéri aktivitások.',
            image: '../img/sport.png',
            details: 'A program keretében különböző szabadtéri sportokat próbálhatsz ki, mint futás, kerékpározás, vagy akár sziklamászás.',
            schedule: 'Szombat - 10:00-14:00',
        },
        // ... (további programok)
        {
            id: 6,
            name: 'Horgászás a tavon',
            description: 'Relaxálj és horgássz a festői környezetben.',
            image: '../img/horgaszat.png',
            details: 'A horgászat nemcsak egy szórakoztató program, hanem egy valódi kikapcsolódási élmény is. A festői tó partján horgászhatsz, miközben élvezheted a természet nyugalmát és a csodálatos tájat. A helyszín csendes és idilli, ideális azok számára, akik szeretnének kiszakadni a mindennapokból és kapcsolatba kerülni a természettel. A program során tapasztalt horgásztanárok segítenek a megfelelő technikák elsajátításában és abban, hogy a legjobb helyeket találjuk meg a tó körül.',
            schedule:'Akármikor'
        },
        {
            id: 7,
            name: 'Képzőművészeti workshop',
            description: 'Fedezd fel a kreativitásod festés és egyéb művészeti formák segítségével.',
            image: '../img/muveszet.png',
            details:'Ez a program lehetőséget ad arra, hogy kibontakoztasd kreativitásodat a képzőművészet világában. A workshop során különböző művészeti technikákat tanulhatsz, mint például festés, rajzolás, szobrászat vagy kézműves alkotás. A tapasztalt helyi művészek vezetésével egyéni vagy csoportos alkotásban vehetsz részt, miközben felfedezheted saját művészeti stílusodat. A program mindenki számára nyitott, függetlenül attól, hogy kezdő vagy haladó szintű művész vagy. Az oktatók segítenek abban, hogy a legjobb technikákat sajátítsd el, miközben szórakoztató és inspiráló környezetben alkothatsz.',
            schedule:'Kedd - 09:00-11:00 Csütörtök - 09:00-11:00'
        },
        {
            id: 8,
            name: 'Zenei fesztivál',
            description: 'Szórakozz a legjobb helyi zenekarokkal és DJ-kkel.',
            image: '../img/fesztival.png',
            details:'A Zenei fesztivál egy élvezetteljes rendezvény, ahol helyi és nemzetközi zenekarok, DJ-k és előadók szórakoztatják a közönséget különböző zenei stílusokkal. A fesztivál hangulatos szabadtéri helyszínen zajlik, ahol a zene mellett étkezési lehetőségek és egyéb szórakoztató programok is várnak.',
            schedule:'Minden vasárnap 20:00 - 03:00'
        },
        {
            id: 9,
            name: 'Lóháton túrázás',
            description: 'Fedezd fel a természetet lovon.',
            image: '../img/lo.png',
            details:'Fedezd fel a természetet lovon! A vezetett túrák során gyönyörű hegyi és erdei tájakon lovagolhatsz, miközben tapasztalt vezetők segítenek a lovaglásban és biztonságban tartanak. A program minden szintű lovas számára ideális.',
            schedule:'Hétfő, Szerda, Péntek - 10:00-12:00',
        },
        {
            id: 10,
            name: 'Fényképezési túra',
            description: 'Tarts velünk és tanulj meg gyönyörű tájképeket fényképezni.',
            image: '../img/foto.png',
            details:'Csatlakozz hozzánk egy fényképezési túrára, ahol szakértő vezetők segítenek abban, hogy a legszebb tájképeket örökíthesd meg. A túra során a legjobb fényviszonyokban és helyszíneken fényképezhetsz, miközben értékes tippeket és trükköket tanulhatsz.',
            schedule:'Kedd, Csütörtök - 8:00-11:00',
        },
        {
            id: 11,
            name: 'Csillagászat',
            description: 'Egy éjszakai csillagászati program, ahol felfedezheted a csillagokat és bolygókat.',
            image: '../img/csillag.png',
            details:' Éjszakai csillagászati programunk során a csillagok, bolygók és más égi jelenségek megfigyelésére nyílik lehetőség. Tapasztalt csillagászok vezetésével fedezheted fel az égbolt titkait távcsövekkel.',
            schedule:'Péntek, Szombat - 21:00-23:00',
        },
        {
            id: 12,
            name: 'Kézműves piac',
            description: 'Helyi kézművesek termékeinek bemutatója és vására.',
            image: '../img/kezmuves.png',
            details:'Fedezd fel a helyi kézművesek egyedi termékeit a piacunkon! Különböző kézműves alkotások, mint ékszerek, kerámia, textíliák és egyéb egyedi készítésű tárgyak várják a vásárlókat.',
            schedule:'Szombat, Vasárnap - 10:00-16:00',
        },
        {
            id: 13,
            name: 'Múzeumi túra',
            description: 'Fedezd fel a helyi múzeumok történelmét és kulturális örökségét.',
            image: '../img/muzeum.png',
            details:'Fedezd fel a helyi múzeumok gazdag történelmét és kulturális örökségét! A túrán végigvezetünk a legfontosabb kiállításokon, ahol szakértők mesélnek az egyes műtárgyak történetéről és jelentőségéről.',
            schedule:'Hétfő, Szerda, Péntek - 10:00-12:00',
        },
        {
            id: 14,
            name: 'Wellness hétvége',
            description: 'Pihenj és relaxálj egy wellness központban, masszázsokkal és szaunákkal.',
            image: '../img/wellness.png',
            details:'Pihenj és relaxálj egy exkluzív wellness központban! Élvezd a masszázsokat, szaunákat és a különböző relaxáló kezeléseket, miközben feltöltődsz és ellazulsz.',
            schedule:'24/7',
        },
        {
            id: 15,
            name: 'Hegyi kerékpározás',
            description: 'Fedezd fel a környéket hegyi kerékpárral, izgalmas terepen.',
            image: '../img/mtb.png',
            details:'Fedezd fel a környéket izgalmas hegyi kerékpártúrákkal! A túrák során változatos terepeken tekerhetsz, miközben gyönyörű hegyi tájakban gyönyörködhetsz. Tapasztalt túravezetők segítenek, hogy biztonságban élvezhesd a kalandot.',
            schedule:'Kedd, Csütörtök, Szombat - 9:00-12:00'
        },
        {
            id: 16,
            name: 'Koncertek',
            description: 'Amennyiben a részleteket tudni szeretné, kersesse fel kezelőorvosát vagy gyógyszerészét.',
            image: '../img/koncert.png',
            details:'Élvezd a legjobb élő zenét helyi és nemzetközi előadóktól! A koncertek különböző zenei stílusokban, hangulatos helyszíneken kerülnek megrendezésre, garantálva a felejthetetlen élményt.',
            schedule:'Péntek, Szombat - 19:00-22:00',
        },
        {
            id: 17,
            name: 'Gyerekeknek szóló kalandpark',
            description: 'Játékos, aktív élmények várnak a kicsikre a kalandparkban.',
            image: '../img/kalandpark.png',
            details:' A kalandparkban a kicsik különböző játékos és izgalmas akadálypályákon próbálhatják ki ügyességüket. A biztonságos környezetben mászófalak, hinta, és egyéb szórakoztató eszközök várják őket.',
            schedule:'Hétfő, Péntek, Vasárnap - 10:00-16:00',
        },
        {
            id: 18,
            name: 'Téli sportok',
            description: 'Síelés, snowboardozás és egyéb téli sportok a hegyekben.',
            image: '../img/sieles.png',
            details:'Élvezd a téli sportokat a hegyekben! Síelés, snowboardozás és egyéb téli aktivitások várják a kalandra vágyókat. Kezdőknek és haladóknak egyaránt kínálunk lehetőségeket a hóban való szórakozásra.',
            schedule:'December - Február: 24/7'
        }
    ];

    const [selectedProgram, setSelectedProgram] = useState(null);

    const openModal = (program) => {
        setSelectedProgram(program);
    };

    const closeModal = () => {
        setSelectedProgram(null);
    };

    return (
        <div className="programs-container">
            <h1 className="programs-title">Programok</h1>
            <div className="programs-grid">
                {programs.map((program) => (
                    <div key={program.id} className="program-card">
                        <div className="program-image">
                            <img src={program.image} alt={program.name} />
                        </div>
                        <div className="program-content">
                            <h2>{program.name}</h2>
                            <p>{program.description}</p>
                            <button className="program-button" onClick={() => openModal(program)}>Részletek</button>
                        </div>
                    </div>
                ))}
            </div>

            {selectedProgram && (
                <div className="modal-overlay">
                    <div className="modal">
                        <h2>{selectedProgram.name}</h2>
                        <p>{selectedProgram.details}</p>
                        <p><strong>Időpontok:</strong> {selectedProgram.schedule}</p>
                        <button className="close-button" onClick={closeModal}>Bezárás</button>
                    </div>
                </div>
            )}
        </div>
    );
}

export default Programs;
