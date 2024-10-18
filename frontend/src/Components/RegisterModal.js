import React, { useState } from 'react';
import './LoginModal.css';

function RegisterModal({ onClose, setIsLoggedIn }) {
  const [username, setUsername] = useState('');
  const [password, setPassword] = useState('');
  const [email, setEmail] = useState('');
  const [successMessage, setSuccessMessage] = useState('');
  const [error, setError] = useState('');

  const handleLoginSubmit = async (e) => {
    e.preventDefault();
  
    try {
      const response = await fetch(`http://localhost:8080/api/Account/Register`, {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify({ username, password, email }),
      });
    
      if (response.ok) {
        const message = `Zarejestrowano! Witaj ${username}!`
        alert(message);
        setSuccessMessage(message);
        setError('')
      } else
      {
        const error = await response.text();
        throw new Error(error);
      }
      onClose();
    } catch (err) {
      setError(err.message);
    }
  };

  return (
    <div className="modal-overlay">
      <div className="modal">
        <h2>Zarejestruj się</h2>
        {error && <p className="error">{error}</p>}
        {successMessage && <p className='error'>{successMessage}</p>}
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
          <div>
            <label>Email:</label>
            <input
              type="email"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
            />
          </div>
          <button type="submit">Zarejestruj</button>
          <button type="button" onClick={onClose}>
            Anuluj
          </button>
        </form>
      </div>
    </div>
  );
}

export default RegisterModal;
