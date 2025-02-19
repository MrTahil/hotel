import React from 'react';
import '../styles/serprog.css';

function Programs() {
    const programs = [
        { id: 1, name: 'Jóga a hegyekben', description: 'Reggeli jógaórák a friss levegőn, gyönyörű kilátással.' },
        { id: 2, name: 'Borkóstoló', description: 'Kóstold meg a helyi borászatok legjobb borait.' },
        { id: 3, name: 'Túrázás', description: 'Vezetett túrák a környező hegyekben és erdőkben.' },
    ];

    return (
        <div className="container">
            <h1>Programok</h1>
            <div className="programs-list">
                {programs.map(program => (
                    <div key={program.id} className="program-card">
                        <h2>{program.name}</h2>
                        <p>{program.description}</p>
                    </div>
                ))}
            </div>
        </div>
    );
}

export default Programs;
