import React from 'react';
import '../styles/Contact.css';

function Contact() {
    return (
        <section className="contact">
            <h2>Kapcsolat</h2>
            <p>Vedd fel velünk a kapcsolatot bármikor!</p>
            <p>Email: hmzrtkando@gmail.com</p>
            <p>Telefon: +36 70 323 7436</p>
            
            <a 
  href="https://mail.google.com/mail/u/0/?view=cm&fs=1&to=hmzrtkando@gmail.com" 
  target="_blank" 
  rel="noopener noreferrer"
  className="w-full bg-blue-800 text-white px-4 py-2 rounded-lg font-semibold hover:bg-blue-700 transition-colors mt-4"
>
  Írj nekünk
</a>
        </section>
    );
}

export default Contact;
