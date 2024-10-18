import React, { useState } from 'react';
import LoginModal from './LoginModal';
import RegisterModal from './RegisterModal'
import './Header.css';

function Header({ isLoggedIn, setIsLoggedIn }) {
  const [showLogin, setShowLogin] = useState(false);
  const [showRegister, setShowRegister] = useState(false);

  const handleLoginClick = () => {
    setShowLogin(true);
  };

  const handleLogoutClick = () => {
    localStorage.removeItem('token');
    setIsLoggedIn(false);
  };

  const handleRegisterClick = () => {
    setShowRegister(true);
  };

  return (
    <header className="App-header">
      <div className="header-content">
        <h1 className="app-name">banbet</h1>
        <div className='header-buttons'>
          {!isLoggedIn ? (
            <button className="login-button" onClick={handleLoginClick}>
              Zaloguj
            </button>
          ) : (
            <button className="logout-button" onClick={handleLogoutClick}>
              Wyloguj
            </button>
          )}
          {!isLoggedIn && 
            <button className='register-button' onClick={handleRegisterClick}>
              Zarejestruj
            </button>
          }
        </div>
      </div>
      {showLogin && (
        <LoginModal
          onClose={() => setShowLogin(false)}
          setIsLoggedIn={setIsLoggedIn}
        />
      )}
      {showRegister && (
        <RegisterModal
        onClose={() => setShowRegister(false)}
        setIsLoggedIn={setIsLoggedIn}
        />
      )}
    </header>
  );
}

export default Header;
