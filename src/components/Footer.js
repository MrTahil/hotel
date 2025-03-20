import React from 'react';
import "../styles/Footer.css";

export default function Footer() {
  return (
    <footer className="bg-blue-900 text-white py-8 md:py-12">
      <div className="max-w-7xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="grid grid-cols-1 sm:grid-cols-2 md:grid-cols-3 gap-8">
          <div>
            <h4 className="text-lg md:text-xl font-bold mb-4">Kapcsolat</h4>
            <p className="mb-2">Email: hmzrtkando@gmail.com</p>
            <p>Tel: +36 70 323 7436</p>
          </div>
          <div>
            <h4 className="text-lg md:text-xl font-bold mb-4">Címünk</h4>
            <p>Miskolc, Palóczy László utca 3, 3525</p>
          </div>
          <div>
            <h4 className="text-lg md:text-xl font-bold mb-4">Kövessen minket</h4>
            <div className="flex flex-col items-start space-y-4">
              <div className="flex space-x-4 items-center">
                <a href="#" className="hover:text-blue-300 transition-colors">
                  <i className="fa-brands fa-facebook text-xl md:text-2xl"></i>
                </a>
                <a href="#" className="hover:text-blue-300 transition-colors">
                  <i className="fa-brands fa-instagram text-xl md:text-2xl"></i>
                </a>
                <a href="#" className="hover:text-blue-300 transition-colors">
                  <i className="fa-brands fa-twitter text-xl md:text-2xl"></i>
                </a>
              </div>
              <a
                href="https://www.instagram.com/tahil_the_g"
                target="_blank"
                rel="noopener noreferrer"
                className="button-style flex items-center space-x-2 bg-green-600 text-white px-4 py-2 rounded-lg font-semibold hover:bg-green-500 transition-colors"
              >
                <p>Follow us!</p>
                <svg
                  xmlns="http://www.w3.org/2000/svg"
                  width="16"
                  height="16"
                  fill="currentColor"
                  className="bi bi-whatsapp"
                  viewBox="0 0 16 16"
                >
                  <path d="M8 0C5.829 0 5.556.01 4.703.048 3.85.088 3.269.222 2.76.42a3.917 3.917 0 0 0-1.417.923A3.927 3.927 0 0 0 .42 2.76C.222 3.268.087 3.85.048 4.7.01 5.555 0 5.827 0 8.001c0 2.172.01 2.444.048 3.297.04.852.174 1.433.372 1.942.205.526.478.972.923 1.417.444.445.89.719 1.416.923.51.198 1.09.333 1.942.372C5.555 15.99 5.827 16 8 16s2.444-.01 3.298-.048c.851-.04 1.434-.174 1.943-.372a3.916 3.916 0 0 0 1.416-.923c.445-.445.718-.891.923-1.417.197-.509.332-1.09.372-1.942C15.99 10.445 16 10.173 16 8s-.01-2.445-.048-3.299c-.04-.851-.175-1.433-.372-1.941a3.926 3.926 0 0 0-.923-1.417A3.911 3.911 0 0 0 13.24.42c-.51-.198-1.092-.333-1.943-.372C10.443.01 10.172 0 7.998 0h.003z"></path>
                </svg>
              </a>
              <a 
                href="https://mail.google.com/mail/u/0/?view=cm&fs=1&to=hmzrtkando@gmail.com" 
                target="_blank" 
                rel="noopener noreferrer"
                className="inline-block bg-blue-800 text-white px-4 py-2 rounded-lg font-semibold hover:bg-blue-700 transition-colors text-center"
              >
                Írj nekünk
              </a>
            </div>
          </div>
        </div>
      </div>
    </footer>
  );
}
