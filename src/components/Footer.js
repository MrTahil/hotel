import React from 'react';

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
            <div className="flex space-x-4">
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
          </div>
        </div>
      </div>
    </footer>
  );
}