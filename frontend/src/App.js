import React, { useState } from 'react';
import './App.css';
import Header from './Components/Header';
import MainPageEvents from './Components/MainPageEvents';

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(!!localStorage.getItem('token'));
  const [isAdmin, setIsAdmin] = useState(false);

  return (
    <div className="App">
      <Header isLoggedIn={isLoggedIn} setIsLoggedIn={setIsLoggedIn} isAdmin={isAdmin} setIsAdmin={setIsAdmin}/>
      <MainPageEvents />
    </div>
  );
}

export default App;
