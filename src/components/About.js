import React, { useEffect, useState } from 'react';
import axios from 'axios';

function About() {
    const [hotelLocation, setHotelLocation] = useState({
        lat: 48.10544809701992,
        lng: 20.77997524506268,
        address: "Miskolc, Széchenyi utca 35, 3525"
    });

    const openGoogleMaps = () => {
        if (!hotelLocation) return;
        window.open(`https://www.google.com/maps/dir/?api=1&destination=${hotelLocation.lat},${hotelLocation.lng}&travelmode=driving`, '_blank');
    };

    return (
        <section className="relative py-16 bg-gradient-to-b from-blue-50 to-white">
            <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
                <div className="text-center mb-16 pt-8">
                    <div className="inline-block relative">
                        <h2 className="animate-bounce text-4xl md:text-5xl font-extrabold text-gray-900 mb-4">
                            <span className="bg-clip-text text-transparent bg-gradient-to-r from-blue-600 to-indigo-600">
                                Fedezd fel a HMZ Hotel világát
                            </span>
                        </h2>
                        <div className="absolute bottom-0 left-1/2 transform -translate-x-1/2 w-3/4 h-2 bg-gradient-to-r from-blue-200 to-indigo-200 rounded-full"></div>
                    </div>
                    <p className="text-xl text-gray-600 max-w-3xl mx-auto mt-6 leading-relaxed">
                        A HMZ Hotel nem csupán egy szállás, hanem egy életre szóló élmény.
                        A város központjában fekvő luxusszállodánkban minden pillanat a kikapcsolódásról
                        és a fényűzésről szól.
                    </p>
                </div>

                <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-4 gap-6 mb-20">
                    <FeatureCard
                        emoji="🏨"
                        title="Prémium szállás"
                        description="Kényelmes, modern felszereltségű szobáinkban pihenhet meg a város zajától távol."
                        bgColor="bg-blue-50"
                        borderColor="border-blue-300"
                    />
                    <FeatureCard
                        emoji="🍽️"
                        title="Gourmet élmények"
                        description="Kiváló séfjeink napi 3 étkezésben kínálják remekműveiket."
                        bgColor="bg-indigo-50"
                        borderColor="border-indigo-300"
                    />
                    <FeatureCard
                        emoji="💆‍♂️"
                        title="Wellness kínálat"
                        description="Spa-központunkban teljes mértékben feltöltődhetsz."
                        bgColor="bg-purple-50"
                        borderColor="border-purple-300"
                    />
                    <FeatureCard
                        emoji="🛎️"
                        title="Prémium szolgáltatások"
                        description="Személyre szabott konciergszolgáltatás minden vendégünk számára."
                        bgColor="bg-cyan-50"
                        borderColor="border-cyan-300"
                    />
                </div>

                <div className="mt-12 mb-20 bg-white rounded-2xl shadow-xl overflow-hidden transition-all duration-300 hover:shadow-2xl">
                    <div className="p-6 bg-gradient-to-r from-blue-600 to-indigo-700 text-white">
                        <div className="flex flex-col md:flex-row md:items-center md:justify-between gap-4">
                            <div>
                                <h3 className="text-2xl font-bold mb-1">Lokáció</h3>
                                <p className="text-blue-100">{hotelLocation.address}</p>
                            </div>
                            <button
                                onClick={openGoogleMaps}
                                className="px-6 py-3 bg-white text-blue-600 rounded-lg font-semibold hover:bg-blue-50 transition-all duration-200 flex items-center gap-2 shadow-md hover:shadow-lg"
                            >
                                Útvonaltervezés
                                <svg xmlns="http://www.w3.org/2000/svg" className="h-5 w-5" viewBox="0 0 20 20" fill="currentColor">
                                    <path fillRule="evenodd" d="M5.05 4.05a7 7 0 119.9 9.9L10 18.9l-4.95-4.95a7 7 0 010-9.9zM10 11a2 2 0 100-4 2 2 0 000 4z" clipRule="evenodd" />
                                </svg>
                            </button>
                        </div>
                    </div>
                    <div className="h-96 w-full">
                        <iframe
                            title="HMZ Hotel elhelyezkedése"
                            width="100%"
                            height="100%"
                            frameBorder="0"
                            className="border-0"
                            src={`https://maps.google.com/maps?q=${hotelLocation.lat},${hotelLocation.lng}&z=17&output=embed`}
                            allowFullScreen
                        ></iframe>
                    </div>
                </div>

                <div className="mt-12 bg-white p-8 rounded-2xl shadow-lg max-w-4xl mx-auto border-l-4 border-blue-500 transform transition-all duration-300 hover:-translate-y-1">
                    <div className="flex items-start">
                        <div className="text-4xl mr-4 text-blue-400">"</div>
                        <div>
                            <blockquote className="text-2xl italic text-gray-700 mb-4">
                                A HMZ Hotel nem csupán egy hely - ez egy érzés. Amint belépsz, tudod,
                                hogy valami különleges vár rád.
                            </blockquote>
                            <p className="text-right text-blue-600 font-medium">- Jánosi Marcell, Vendégünk</p>
                        </div>
                    </div>
                </div>

                <div className="mt-12 bg-white p-8 rounded-2xl shadow-lg max-w-4xl mx-auto border-l-4 border-blue-500 transform transition-all duration-300 hover:-translate-y-1">
                    <div className="flex items-start">
                        <div className="text-4xl mr-4 text-blue-400">"</div>
                        <div>
                            <blockquote className="text-2xl italic text-gray-700 mb-4">
                                Álmaim kéklő egén,
                                Gyümölcsfáim tetején,
                                Pálinka, szerelmem, légy az enyém!
                            </blockquote>
                            <p className="text-right text-blue-600 font-medium">- Magma Cum Laude, Vendégünk</p>
                        </div>
                    </div>
                </div>

                <div className="mt-12 bg-white p-8 rounded-2xl shadow-lg max-w-4xl mx-auto border-l-4 border-blue-500 transform transition-all duration-300 hover:-translate-y-1">
                    <div className="flex items-start">
                        <div className="text-4xl mr-4 text-blue-400">"</div>
                        <div>
                            <blockquote className="text-2xl italic text-gray-700 mb-4">
                                Olyan, mintha hazajönnék – a HMZ Hotelben minden részlet tökéletes, a párnák puhaságától a reggeli friss péksüteményekig.
                            </blockquote>
                            <p className="text-right text-blue-600 font-medium">- Kovács Eszter, Törzsvendég</p>
                        </div>
                    </div>
                </div>

                <div className="mt-12 bg-white p-8 rounded-2xl shadow-lg max-w-4xl mx-auto border-l-4 border-blue-500 transform transition-all duration-300 hover:-translate-y-1">
                    <div className="flex items-start">
                        <div className="text-4xl mr-4 text-blue-400">"</div>
                        <div>
                            <blockquote className="text-2xl italic text-gray-700 mb-4">
                                A wellness részleg varázslatos! A szauna után a csillagos ég alatti jakuzzi olyan élmény volt, amit soha nem felejtek.
                            </blockquote>
                            <p className="text-right text-blue-600 font-medium">- Tóth Balázs, Honeymoon vendég</p>
                        </div>
                    </div>
                </div>

                <div className="mt-12 bg-white p-8 rounded-2xl shadow-lg max-w-4xl mx-auto border-l-4 border-blue-500 transform transition-all duration-300 hover:-translate-y-1">
                    <div className="flex items-start">
                        <div className="text-4xl mr-4 text-blue-400">"</div>
                        <div>
                            <blockquote className="text-2xl italic text-gray-700 mb-4">
                                A kertből behozott levendulaillat, a tó partjáról érkező madárcsicsergés – itt a zajok helyett a csend zenéje ébreszt.
                            </blockquote>
                            <p className="text-right text-blue-600 font-medium">- Szabó Réka, Wellness hétvége</p>
                        </div>
                    </div>
                </div>

                <div className="mt-12 bg-white p-8 rounded-2xl shadow-lg max-w-4xl mx-auto border-l-4 border-blue-500 transform transition-all duration-300 hover:-translate-y-1">
                    <div className="flex items-start">
                        <div className="text-4xl mr-4 text-blue-400">"</div>
                        <div>
                            <blockquote className="text-2xl italic text-gray-700 mb-4">
                                A séfek művészek, a borok költők – egy vacsora a HMZ-ban olyan, mint egy gasztronómiai szonett.
                            </blockquote>
                            <p className="text-right text-blue-600 font-medium">- Fördős Zé, Gasztroblogger</p>
                        </div>
                    </div>
                </div>





            </div>
        </section>
    );
}

function FeatureCard({ emoji, title, description, bgColor = "bg-blue-50", borderColor = "border-blue-300" }) {
    return (
        <div className={`bg-white p-6 rounded-xl shadow-md hover:shadow-xl transition-all duration-300 border-t-4 ${borderColor} group`}>
            <div className="mb-4">
                <div className={`inline-block p-3 ${bgColor} rounded-lg text-3xl transition-transform duration-300 group-hover:scale-110`}>
                    {emoji}
                </div>
            </div>
            <h3 className="text-xl font-bold text-gray-800 mb-3">{title}</h3>
            <p className="text-gray-600 leading-relaxed">{description}</p>
        </div>
    );
}

export default About;