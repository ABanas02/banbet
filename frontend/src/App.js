import React, { useState } from 'react';
import './App.css';
import Header from './Components/Header';
import MainPageEvents from './Components/MainPageEvents';

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(!!localStorage.getItem('token'));

  return (
    <div className="App">
      <Header isLoggedIn={isLoggedIn} setIsLoggedIn={setIsLoggedIn} />
      <MainPageEvents />
    </div>
  );
}

export default App;
