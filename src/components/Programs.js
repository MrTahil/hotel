import React, { useState, useEffect } from 'react';
import '../styles/serprog.css';

function Programs() {
    const [programs, setPrograms] = useState([]);
    const [selectedProgram, setSelectedProgram] = useState(null);

    useEffect(() => {
        // Az adatok lekérése a szerverről
        fetch('https://localhost:7047/Events/Geteents')
            .then(response => response.json())
            .then(data => {
                // Az adatok átalakítása a komponens által várt formátumra
                const formattedPrograms = data.map(event => ({
                    id: event.eventId,
                    name: event.eventName,
                    description: event.description,
                    image: '../img/default.png', // Alapértelmezett kép, ha nincs megadva
                    details: event.description,
                    schedule: event.eventDate,
                }));
                setPrograms(formattedPrograms);
            })
            .catch(error => console.error('Hiba történt az adatok lekérésekor:', error));
    }, []);

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