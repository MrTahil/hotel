import React, { useState, useEffect } from 'react';
import '../styles/serprog.css';

function Programs() {
    const [programs, setPrograms] = useState([]);
    const [selectedProgram, setSelectedProgram] = useState(null);

    useEffect(() => {
        // Az adatok lekérése a szerverről
        fetch(process.env.REACT_APP_API_BASE_URL+'/Events/Geteents')
            .then(response => response.json())
            .then(data => {
                // Az adatok átalakítása a komponens által várt formátumra
                const formattedPrograms = data.map(event => ({
                    id: event.eventId,
                    name: event.eventName,
                    description: event.description,
                    images: event.images || '../img/default_imgage.png', // Alapértelmezett kép, ha nincs megadva
                    schedule: event.eventDate,
                    organizerName: event.organizerName,
                    contactInfo: event.contactInfo,
                    location: event.location,
                    capacity: event.capacity,
                    price: event.price,
                }));
                console.log(data);

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

    // Funkció a leírás levágásához
    const truncateDescription = (description, maxLength) => {
        if (description.length > maxLength) {
            return description.substring(0, maxLength) + '...';
        }
        return description;
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
                            <p>{truncateDescription(program.description, 70)}</p> {/* Csak az első 100 karakter */}
                            <button 
                                className="w-full bg-blue-800 text-white px-4 py-2 rounded-lg font-semibold hover:bg-blue-700 transition-colors mt-4" 
                                onClick={() => openModal(program)}
                            >
                                Részletek
                            </button>
                        </div>
                    </div>
                ))}
            </div>

            {selectedProgram && (
                <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center p-4">
                    <div className="bg-white rounded-lg shadow-lg overflow-hidden max-w-2xl w-full mx-4">
                        <div className="p-6">
                            <h2 className="text-2xl font-bold mb-4">{selectedProgram.name}</h2>
                            <div className="flex flex-col md:flex-row gap-6">
                                <div className="md:w-1/2">
                                    <img 
                                        src={selectedProgram.images} 
                                        alt={selectedProgram.name} 
                                        className="w-full h-48 object-cover rounded-lg"
                                    />
                                </div>
                                <div className="md:w-1/2">
                                    <p className="text-gray-700 mb-2">
                                        <strong>Időpont:</strong> {selectedProgram.schedule}
                                    </p>
                                    <p className="text-gray-700 mb-2">
                                        <strong>Szervező:</strong> {selectedProgram.organizerName}
                                    </p>
                                    <p className="text-gray-700 mb-2">
                                        <strong>Elérhetőség:</strong> {selectedProgram.contactInfo}
                                    </p>
                                    <p className="text-gray-700 mb-2">
                                        <strong>Helyszín:</strong> {selectedProgram.location}
                                    </p>
                                    <p className="text-gray-700 mb-4">
                                        <strong>Férőhelyek:</strong> {selectedProgram.capacity}
                                    </p>
                                    <p className="text-gray-700 mb-4">
                                        <strong>Leírás:</strong> {selectedProgram.description} {/* Teljes leírás */}
                                    </p>

                                    <p className="text-gray-700 mb-4">
                                        <strong>Ár / Fő: {selectedProgram.price} Ft</strong>
                                    </p>
                                </div>
                            </div>
                        </div>
                        <div className="bg-gray-50 px-6 py-4 flex flex-col space-y-2 md:flex-row md:space-y-0 md:space-x-2">
                            <button 
                                className="w-full md:w-auto bg-blue-800 text-white px-4 py-2 rounded-lg font-semibold hover:bg-blue-700 transition-colors"
                            >
                                Foglalás
                            </button>
                            <button 
                                className="w-full md:w-auto bg-red-800 text-white px-4 py-2 rounded-lg font-semibold hover:bg-red-700 transition-colors"
                                onClick={closeModal}
                            >
                                Bezárás
                            </button>
                        </div>
                    </div>
                </div>
            )}
        </div>
    );
}

export default Programs;