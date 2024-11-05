import React, { useState } from 'react';
import LoginModal from './LoginModal';
import RegisterModal from './RegisterModal'
import './css/Header.css';
import { Link } from 'react-router-dom';

function Header({ isLoggedIn, setIsLoggedIn, isAdmin, setIsAdmin, checkIfAdmin}) {
  const [showLogin, setShowLogin] = useState(false);
  const [showRegister, setShowRegister] = useState(false);

  const handleLoginClick = () => {
    setShowLogin(true);
  };

  const handleLogoutClick = () => {
    localStorage.removeItem('token');
    setIsLoggedIn(false);
    setIsAdmin(false);
  };

  const handleRegisterClick = () => {
    setShowRegister(true);
  };

  return (
    <header className="App-header">
      <div className="header-content">
        <Link to="/" className='app-name-link'>
          <h1 className="app-name">banbet</h1>
        </Link>
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
          {isAdmin && (
            <Link to="/admin">
              <button className="admin-button">Panel Admina</button>
            </Link>
          )}
          {!isAdmin && isLoggedIn && (
            <Link to="/user">
              <button className='admin-button'>Panel Użytkownika</button>
            </Link>
          )}
        </div>
      </div>
      {showLogin && (
        <LoginModal
          onClose={() => setShowLogin(false)}
          setIsLoggedIn={setIsLoggedIn}
          setIsAdmin={setIsAdmin}
          checkIfAdmin={checkIfAdmin}
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
