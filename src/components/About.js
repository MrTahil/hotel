import React from 'react';
import '../styles/About.css';

function About() {
    return (
        <section className="about">
            <h2>Rólunk</h2>
            <p>
                A szállodánk tökéletes választás azoknak, akik egy felejthetetlen élményre vágynak. 
                Egyedülálló kényelmet és páratlan panorámát kínálunk.
            </p>
            <div className="features">
                <div className="feature-item">
                    <h3>Luxus Szobák</h3>
                    <p>Pihenj meg gyönyörűen berendezett, kényelmes szobáinkban.</p>
                </div>
                <div className="feature-item">
                    <h3>Kiváló Ételek</h3>
                    <p>Élvezd séfjeink által készített ínycsiklandó fogásokat.</p>
                </div>
                <div className="feature-item">
                    <h3>Felejthetetlen Programok</h3>
                    <p>Fedezd fel a környéket vezetett túráinkkal vagy lazíts wellness részlegünkben.</p>
                </div>
            </div>
        </section>
    );
}

export default About;   