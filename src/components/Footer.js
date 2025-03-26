import React, { useState } from 'react';
import "../styles/Footer.css";

export default function Footer() {
  const [isModalOpen, setIsModalOpen] = useState(false);

  const teamMembers = [
    { name: "Hilóczki Tamás", instagram: "https://www.instagram.com/tahil_the_g" },
    { name: "Monostori Róbert", instagram: "https://www.instagram.com/monostori_r" },
    { name: "Zelenák Zalán Noel", instagram: "https://www.instagram.com/ze.zuzu" },
  ];

  return (
    <footer className="bg-blue-900 text-white py-8">
      {/* Modal */}
      {isModalOpen && (
        <div className="fixed inset-0 bg-black bg-opacity-50 flex items-center justify-center z-50">
          <div className="bg-white text-blue-900 rounded-lg p-6 max-w-md w-full">
            <div className="flex justify-between items-center mb-4">
              <h3 className="text-xl font-bold">Kövesse csapatunkat</h3>
              <button 
                onClick={() => setIsModalOpen(false)}
                className="text-blue-900 hover:text-blue-700"
              >
                <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M6 18L18 6M6 6l12 12" />
                </svg>
              </button>
            </div>
            <div className="space-y-4">
              {teamMembers.map((member, index) => (
                <div key={index} className="flex items-center justify-between p-3 bg-blue-50 rounded-lg">
                  <span className="font-medium">{member.name}</span>
                  <a 
                    href={member.instagram} 
                    target="_blank" 
                    rel="noopener noreferrer"
                    className="flex items-center text-blue-600 hover:text-blue-800"
                  >
                    <i className="fa-brands fa-instagram mr-2 text-lg"></i>
                    <span>Instagram</span>
                  </a>
                </div>
              ))}
            </div>
            <div className="mt-6 flex justify-end">
              <button 
                onClick={() => setIsModalOpen(false)}
                className="px-4 py-2 bg-blue-900 text-white rounded hover:bg-blue-800 transition-colors"
              >
                Bezárás
              </button>
            </div>
          </div>
        </div>
      )}

      <div className="max-w-6xl mx-auto px-4 sm:px-6 lg:px-8">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
          {/* Kapcsolat rész */}
          <div className="space-y-3">
            <div className="flex items-center">
              <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 mr-2 text-blue-300" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
              </svg>
              <h4 className="text-lg font-semibold">Kapcsolat</h4>
            </div>
            <div className="space-y-1 text-sm">
              <p className="flex items-center">
                <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4 mr-2 text-blue-200" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 5a2 2 0 012-2h3.28a1 1 0 01.948.684l1.498 4.493a1 1 0 01-.502 1.21l-2.257 1.13a11.042 11.042 0 005.516 5.516l1.13-2.257a1 1 0 011.21-.502l4.493 1.498a1 1 0 01.684.949V19a2 2 0 01-2 2h-1C9.716 21 3 14.284 3 6V5z" />
                </svg>
                +36 70 123 4567
              </p>
              <p className="flex items-center">
                <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4 mr-2 text-blue-200" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
                </svg>
                hmzrtkando@gmail.com
              </p>
            </div>
          </div>

          {/* Cím rész */}
          <div className="space-y-3">
            <div className="flex items-center">
              <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 mr-2 text-blue-300" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M17.657 16.657L13.414 20.9a1.998 1.998 0 01-2.827 0l-4.244-4.243a8 8 0 1111.314 0z" />
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M15 11a3 3 0 11-6 0 3 3 0 016 0z" />
              </svg>
              <h4 className="text-lg font-semibold">Címünk</h4>
            </div>
            <p className="flex items-start text-sm">
              <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4 mr-2 mt-0.5 text-blue-200" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M19 21V5a2 2 0 00-2-2H7a2 2 0 00-2 2v16m14 0h2m-2 0h-5m-9 0H3m2 0h5M9 7h1m-1 4h1m4-4h1m-1 4h1m-5 10v-5a1 1 0 011-1h2a1 1 0 011 1v5m-4 0h4" />
              </svg>
              <span>3525 Miskolc,<br />Palóczy László utca 3</span>
            </p>
          </div>

          {/* Közösségi média rész */}
          <div className="space-y-4">
            <div className="flex items-center">
              <svg xmlns="http://www.w3.org/2000/svg" className="h-6 w-6 mr-2 text-blue-300" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M8.684 13.342C8.886 12.938 9 12.482 9 12c0-.482-.114-.938-.316-1.342m0 2.684a3 3 0 110-2.684m0 2.684l6.632 3.316m-6.632-6l6.632-3.316m0 0a3 3 0 105.367-2.684 3 3 0 00-5.367 2.684zm0 9.316a3 3 0 105.368 2.684 3 3 0 00-5.368-2.684z" />
              </svg>
              <h4 className="text-lg font-semibold">Kövessen minket</h4>
            </div>
            
            <div className="flex space-x-4 mb-3">
              <a href="#" className="text-xl hover:text-blue-300 transition-colors duration-200">
                <i className="fa-brands fa-facebook"></i>
              </a>
              <a href="#" className="text-xl hover:text-blue-300 transition-colors duration-200">
                <i className="fa-brands fa-instagram"></i>
              </a>
              <a href="#" className="text-xl hover:text-blue-300 transition-colors duration-200">
                <i className="fa-brands fa-twitter"></i>
              </a>
            </div>
            
            <div className="flex flex-col sm:flex-row sm:space-x-3 space-y-2 sm:space-y-0">
              <button
                onClick={() => setIsModalOpen(true)}
                className="flex-1 flex items-center justify-center px-3 py-2 bg-blue-800 text-white rounded text-sm font-medium hover:bg-blue-700 transition-colors"
              >
                <i className="fa-brands fa-instagram mr-2"></i>
                Kövessen
              </button>
              
              <a 
                href="https://mail.google.com/mail/u/0/?view=cm&fs=1&to=hmzrtkando@gmail.com" 
                target="_blank" 
                rel="noopener noreferrer"
                className="flex-1 flex items-center justify-center px-3 py-2 bg-blue-800 text-white rounded text-sm font-medium hover:bg-blue-700 transition-colors"
              >
                <svg xmlns="http://www.w3.org/2000/svg" className="h-4 w-4 mr-2" fill="none" viewBox="0 0 24 24" stroke="currentColor">
                  <path strokeLinecap="round" strokeLinejoin="round" strokeWidth={2} d="M3 8l7.89 5.26a2 2 0 002.22 0L21 8M5 19h14a2 2 0 002-2V7a2 2 0 00-2-2H5a2 2 0 00-2 2v10a2 2 0 002 2z" />
                </svg>
                Írjon nekünk
              </a>
            </div>
          </div>
        </div>
        
        <div className="mt-8 pt-6 border-t border-blue-800 text-center text-blue-300 text-sm">
          <p>© {new Date().getFullYear()} HMZ rt. Kando. Minden jog fenntartva.</p>
        </div>
      </div>
    </footer>
  );
}