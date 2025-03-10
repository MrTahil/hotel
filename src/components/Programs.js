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
                    images: event.images || '../img/lo.png', // Alapértelmezett kép, ha nincs megadva
                    details: event.description,
                    schedule: event.eventDate,
                    organizerName: event.organizerName,
                    contactInfo: event.contactInfo,
                    location: event.location,
                    capacity: event.capacity,

                }));
                console.log(data)

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
                            <img src={program.images} alt={program.name} />
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
                        <div className="modal-image">
                            <img src={selectedProgram.images} alt={selectedProgram.name} />
                        </div>
                        <p><strong>Időpont:</strong> {selectedProgram.schedule}</p>
                        <p><strong>Szervező:</strong> {selectedProgram.organizerName}</p>
                        <p><strong>Elérhetőség:</strong> {selectedProgram.contactInfo}</p>
                        <p><strong>Helyszín:</strong> {selectedProgram.location}</p>
                        <p><strong>Férőhelyek:</strong> {selectedProgram.capacity}</p>
                        <button className="close-button" onClick={closeModal}>Bezárás</button>
                        <button className="">Foglalás</button>
                    </div>
                </div>
            )}
        </div>
    );
}

export default Programs;