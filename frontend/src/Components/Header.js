import React, { useState, useEffect } from 'react';
import LoginModal from './LoginModal';
import RegisterModal from './RegisterModal';
import './css/Header.css';
import { Link, useNavigate } from 'react-router-dom';

function Header({ isLoggedIn, setIsLoggedIn, isAdmin, setIsAdmin, checkIfAdmin, decodedJWT,
  userBalanceChanged }) {
  const [showLogin, setShowLogin] = useState(false);
  const [showRegister, setShowRegister] = useState(false);
  const [userBalance, setUserBalance] = useState(null);

  const navigate = useNavigate();

  useEffect(() => {
    const link = document.createElement('link');
    link.href = 'https://fonts.googleapis.com/css2?family=Poppins:wght@400;500;600;700&display=swap';
    link.rel = 'stylesheet';
    document.head.appendChild(link);

    return () => {
      document.head.removeChild(link);
    };
  }, []);

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
      if (!decodedJWT || !decodedJWT.sub) {
        return;
      }

      const id = decodedJWT.sub;

      try {
        const response = await fetch(`${process.env.REACT_APP_BACKEND_URL}/api/Account/${id}`);
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
        { isLoggedIn && !isAdmin && (
          <p className='user-balance'>Saldo użytkownika: {userBalance}</p>
        )}
        <div className='header-buttons'>
          {!isLoggedIn ? (
            <>
              <button className="login-button" onClick={handleLoginClick}>
              Zaloguj
              </button>
            </>
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
