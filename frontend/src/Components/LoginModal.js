import React, { useState } from 'react';
import { jwtDecode } from 'jwt-decode';
import './LoginModal.css';

function LoginModal({ onClose, setIsLoggedIn, setIsAdmin }) {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');

  const handleLoginSubmit = async (e) => {
    e.preventDefault();
  
    try {
      const response = await fetch(`http://localhost:8080/api/Account/Login`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ username, password }),
      });
  
      if (!response.ok) {
        throw new Error('Nieprawidłowa nazwa użytkownika lub hasło.');
      }
      
      const data = await response.json();
      localStorage.setItem('token', data.token);
      setIsLoggedIn(true);
      if (getUserRole() === "Admin")
      {
        setIsAdmin(true);
      }
      onClose();
    } catch (err) {
      setError(err.message);
    }
  };


  function getUserRole() {
    const token = localStorage.getItem('token');
    if (!token) {
      return null;
    }
  
    try {
      const decodedToken = jwtDecode(token);
      const role = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
      return role;
    } catch (error) {
      console.error('Błąd podczas dekodowania tokenu JWT:', error);
      return null;
    }
  }

  return (
    <div className="modal-overlay">
      <div className="modal">
        <h2>Zaloguj się</h2>
        {error && <p className="error">{error}</p>}
        <form onSubmit={handleLoginSubmit}>
          <div>
            <label>Nazwa użytkownika:</label>
            <input
              type="text"
              value={username}
              onChange={(e) => setUsername(e.target.value)}
              required
            />
          </div>
          <div>
            <label>Hasło:</label>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
            />
          </div>
          <button type="submit">Zaloguj</button>
          <button type="button" onClick={onClose}>
            Anuluj
          </button>
        </form>
      </div>
    </div>
  );
}

export default LoginModal;
