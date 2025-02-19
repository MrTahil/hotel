import React from 'react';
import '../styles/Contact.css';

function Contact() {
    return (
        <section className="contact">
            <h2>Kapcsolat</h2>
            <p>Vedd fel velünk a kapcsolatot bármikor!</p>
            <p>Email: hmzrtkando@gmail.com</p>
            <p>Telefon: +36 70 927 0458</p>
            <a 
  href="https://mail.google.com/mail/u/0/?view=cm&fs=1&to=hmzrtkando@gmail.com" 
  target="_blank" 
  rel="noopener noreferrer"
  className="cta-button"
>
  Írj nekünk
</a>
        </section>
    );
}

export default Contact;
