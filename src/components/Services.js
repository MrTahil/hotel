import React from 'react';
import '../styles/Services.css';
import roomservice from '../img/roomservice.png'; // Kép importálása
import restaurant from '../img/restaurant.png'; // Kép importálása
import wellness from '../img/wellness.png'; // Kép importálása

function Services() {
    const services = [
        {
            id: 1,
            name: 'Wellness és Spa',
            description: 'Kényeztető masszázsok, szaunák és wellness szolgáltatások várják a vendégeket, hogy feltöltődjenek és ellazuljanak. Professionális terapeutáink segítenek a teljes relaxációban.',
            image: wellness,
        },
        {
            id: 2,
            name: 'Éttermek',
            description: 'Prémium minőségű ételek és italok különleges atmoszférában. Séfjeink kreatív fogásai és a helyi specialitások garantálják, hogy minden étkezés felejthetetlen élmény legyen.',
            image: restaurant,
        },
        {
            id: 3,
            name: 'Szobaszerviz',
            description: 'Non-stop szobaszerviz a kényelmed érdekében. Bármikor rendelhetsz friss ételeket, italokat vagy igénybe vehetsz egyéb szolgáltatásokat, hogy a tartózkodásod még kényelmesebb legyen.',
            image: roomservice,
        },
    ];

    return (
        <div className="services-container">
            <h1 className="services-title">Szolgáltatások</h1>
            <div className="services-grid">
                {services.map((service) => (
                    <div key={service.id} className="service-card">
                        <div className="service-image">
                            <img src={service.image} alt={service.name} />
                        </div>
                        <div className="service-content">
                            <h2>{service.name}</h2>
                            <p>{service.description}</p>
                            <button className="service-button">További információ</button>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}

export default Services;