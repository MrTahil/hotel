import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Navbar from './components/Navbar';
import Hero from './components/Hero';
import About from './components/About';
import Gallery from './components/Gallery';
import Contact from './components/Contact';
import Rooms from './components/Rooms';
import Programs from './components/Programs'
import './App.css';
import './index.css';
import Footer from './components/Footer';
import Services from './components/Services';


function App() {
    return (
        <Router>
            <div className="App">
                <Navbar />
                <div className="main-content">
                    <Routes>
                        <Route path="/" element={<Hero/>} />
                        <Route path="/szobak" element={<Rooms/>} />
                        <Route path="/rolunk" element={<About/>} />
                        <Route path="/kapcsolat" element={<Contact/>} />
                        <Route path="/programok" element={<Programs/>} />
                        <Route path="/szolgaltatasok" element={<Services />} />

                    </Routes>
                </div>
                <Footer/>
            </div>
        </Router>
    );
}

export default App;