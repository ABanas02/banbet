import React, { useState } from 'react';
import LoginModal from './LoginModal';

function Header() {
  const [showLogin, setShowLogin] = useState(false);

  const handleLoginClick = () => {
    setShowLogin(true);
  };

  return (
    <header className="App-header">
      <h1>banbet</h1>
      <button onClick={handleLoginClick}>Zaloguj</button>
      {showLogin && <LoginModal onClose={() => setShowLogin(false)} />}
    </header>
  );
}

export default Header;