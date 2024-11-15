import React, { useState, useEffect } from 'react';
import LoginModal from './LoginModal';
import RegisterModal from './RegisterModal'
import './css/Header.css';
import { Link, useNavigate } from 'react-router-dom';

function Header({ isLoggedIn, setIsLoggedIn, isAdmin, setIsAdmin, checkIfAdmin, decodedJWT, userBalanceChanged}) {
  const [showLogin, setShowLogin] = useState(false);
  const [showRegister, setShowRegister] = useState(false);
  const [userBalance, setUserBalance] = useState(null);
  const navigate = useNavigate();

  const handleLoginClick = () => {
    setShowLogin(true);
  };

  const handleLogoutClick = () => {
    localStorage.removeItem('token');
    setIsLoggedIn(false);
    setIsAdmin(false);
    navigate("/");
  };

  const handleRegisterClick = () => {
    setShowRegister(true);
  };

  useEffect(() => {
    const fetchUserBalance = async () => {
      if (!decodedJWT || !decodedJWT.userId) {
        return;
      }

      const id = decodedJWT.userId;

      try {
        const response = await fetch(`http://localhost:8080/api/Account/${id}`);
        const data = await response.json();
        setUserBalance(data.virtualBalance);
      } catch (error) {
        console.error('Błąd podczas pobierania salda użytkownika:', error);
      }
    };

    fetchUserBalance();
  }, [decodedJWT, userBalanceChanged]);
  

  return (
    <header className="App-header">
      <div className="header-content">
        <Link to="/" className='app-name-link'>
          <h1 className="app-name">banbet</h1>
        </Link>
        { isLoggedIn && (
          <p className='user-balance'>Saldo użytkownika: {userBalance}</p>
        )}
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
