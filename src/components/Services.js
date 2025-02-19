import React from 'react';
import '../styles/serprog.css';

function Services() {
    const services = [
        { id: 1, name: 'Wellness és Spa', description: 'Kényeztető masszázsok és szaunák várják a vendégeket.' },
        { id: 2, name: 'Éttermek', description: 'Prémium minőségű ételek és italok különleges atmoszférában.' },
        { id: 3, name: 'Szobaszerviz', description: 'Non-stop szobaszerviz a kényelmed érdekében.' },
    ];

    return (
        <div className="container">
            <h1>Szolgáltatások</h1>
            <div className="services-list">
                {services.map(service => (
                    <div key={service.id} className="service-card">
                        <h2>{service.name}</h2>
                        <p>{service.description}</p>
                    </div>
                ))}
            </div>
        </div>
    );
}

export default Services;
