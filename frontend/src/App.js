import React, { useState } from 'react';
import './App.css';
import Header from './Components/Header';
import { jwtDecode } from 'jwt-decode';
import MainPageEvents from './Components/MainPageEvents';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import AdminPanel from './Components/AdminPanel';

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(!!localStorage.getItem('token'));
  const [isAdmin, setIsAdmin] = useState(checkIfAdmin());
  const [teamsChanged, setTeamsChanged] = useState(false);

  return (
    <Router>
      <div className="App">
        <Header isLoggedIn={isLoggedIn} setIsLoggedIn={setIsLoggedIn} isAdmin={isAdmin} setIsAdmin={setIsAdmin} checkIfAdmin={checkIfAdmin}/>
        <Routes>
          <Route path="/" element={<MainPageEvents />} />
          <Route path="/admin" element={<AdminPanel teamsChanged={teamsChanged} onTeamsChanged={() => setTeamsChanged(true)}/>} />
        </Routes>
      </div>
    </Router>
  );
}

function checkIfAdmin() {
  const token = localStorage.getItem('token');
  if (!token) {
    return null;
  }

  try {
    const decodedToken = jwtDecode(token);
    const role = decodedToken['http://schemas.microsoft.com/ws/2008/06/identity/claims/role'];
    if (role === "User") {
      return false;
    } else {
      return true;
    }
  } catch (error) {
    console.error('Błąd podczas dekodowania tokenu JWT:', error);
    return null;
  }
}

export default App;
