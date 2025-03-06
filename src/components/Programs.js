import React from 'react';
import '../styles/serprog.css';

function Programs() {
    const programs = [
        {
            id: 1,
            name: 'Jóga a hegyekben',
            description: 'Reggeli jógaórák a friss levegőn, gyönyörű kilátással.',
            image: '../img/jogahegyen.png', // Helyi kép használata
        },
        {
            id: 2,
            name: 'Borkóstoló',
            description: 'Kóstold meg a helyi borászatok legjobb borait.',
            image: '../img/bor.png', // Példa kép URL
        },
        {
            id: 3,
            name: 'Túrázás',
            description: 'Vezetett túrák a környező hegyekben és erdőkben.',
            image: '../img/tour.png', // Példa kép URL
        },
    ];

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
                            <button className="program-button">Részletek</button>
                        </div>
                    </div>
                ))}
            </div>
        </div>
    );
}

export default Programs;