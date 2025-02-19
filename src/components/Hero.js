import React from 'react';
import '../styles/Hero.css';
import { Link } from 'react-router-dom';

function Hero() {
    return (
        <section className="hero">
            <h1>Üdvözlünk a Mesés Szállodánkban</h1>
            <p>Fedezd fel a luxus és a kényelem világát, lenyűgöző kilátással.</p>
            <Link to="/szobak" className="cta-button">Foglalj most</Link>
        </section>
    );
}

export default Hero;
