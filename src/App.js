import React from 'react';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import Navbar from './components/Navbar';
import Hero from './components/Hero';
import About from './components/About';
import Rooms from './components/Rooms';
import Programs from './components/Programs';
import './App.css';
import './index.css';
import Footer from './components/Footer';
import Services from './components/Services';
import Foglalas from './components/Foglalas';
import ProfilePage from './components/ProfileModal';
import LoginModal from './components/LoginModal';
import ManageBookingEmail from './components/ManageBookingEmail';

function App() {
  return (
    <Router>
      <div className="App">
        <Navbar />
        <div className="main-content">
          <Routes>
            <Route path="/" element={<Hero />} />
            <Route path="/szobak" element={<Rooms />} />
            <Route path="/szobak/:roomId" element={<Foglalas />} /> {/* Changed from /foglalas/:id to match room navigation */}
            <Route path="/rolunk" element={<About />} />
            <Route path="/programok" element={<Programs />} />
            <Route path="/szolgaltatasok" element={<Services />} />
            <Route path="/login" element={<LoginModal />} /> {/* Added login route */}
            <Route path="/profile" element={<ProfilePage />} />
            <Route path="/Foglalas/:id" element={<Foglalas />} />
            <Route path="*" element={<h1>404 - Az oldal nem található</h1>} />
            <Route path="/booking/:bookingId" element={<ManageBookingEmail />} />


          </Routes>
        </div>
        <Footer />
      </div>
    </Router>
  );
}

export default App;