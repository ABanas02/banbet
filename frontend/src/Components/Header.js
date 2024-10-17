import React, { useState } from 'react';
import LoginModal from './LoginModal';
import './Header.css';

function Header({ isLoggedIn, setIsLoggedIn }) {
  const [showLogin, setShowLogin] = useState(false);

  const handleLoginClick = () => {
    setShowLogin(true);
  };

  const handleLogoutClick = () => {
    localStorage.removeItem('token');
    setIsLoggedIn(false);
  };

  return (
    <header className="App-header">
      <div className="header-content">
        <h1 className="app-name">banbet</h1>
        {!isLoggedIn ? (
          <button className="login-button" onClick={handleLoginClick}>
            Zaloguj
          </button>
        ) : (
          <button className="logout-button" onClick={handleLogoutClick}>
            Wyloguj
          </button>
        )}
      </div>
      {showLogin && (
        <LoginModal
          onClose={() => setShowLogin(false)}
          setIsLoggedIn={setIsLoggedIn}
        />
      )}
    </header>
  );
}

export default Header;
