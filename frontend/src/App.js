import React, { useState, useEffect } from 'react';
import './App.css';
import Header from './Components/Header';
import MainPageEvents from './Components/MainPageEvents';
import { BrowserRouter as Router, Route, Routes } from 'react-router-dom';
import AdminPanel from './Components/AdminPanel';
import UserPanel from './Components/UserPanel';
import WelcomeScreen from './Components/WelcomeScreen';
import CategoryForm from './Components/CategoryForm';
import Footer from './Components/Footer';
import MainPageWelcomeText from './Components/MainPageWelcomeText';
import { jwtDecode } from 'jwt-decode';

function App() {
  const [isLoggedIn, setIsLoggedIn] = useState(!!localStorage.getItem('token'));
  const [isAdmin, setIsAdmin] = useState(checkIfAdmin());
  const [decodedJWT, setDecodedJWT] = useState('');
  const [userBalanceChanged, setUserBalanceChanged] = useState(false);
  const [showWelcomeScreen, setShowWelcomeScreen] = useState(false);
  const [categories, setCategories] = useState([]);

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (token) {
      try {
        const decodedToken = jwtDecode(token);
        console.log('Zdekodowany token:', decodedToken);
        setDecodedJWT(decodedToken);
      } catch (error) {
        console.error('Błąd podczas dekodowania tokenu JWT:', error);
      }
    }
  }, [isLoggedIn]);

  useEffect(() => {
    const hasSeenWelcome = sessionStorage.getItem('hasSeenWelcome');
    if (!hasSeenWelcome) {
      setShowWelcomeScreen(true);
    }
  }, []);

  const handleCloseWelcome = () => {
    setShowWelcomeScreen(false);
    sessionStorage.setItem('hasSeenWelcome', 'true');
  };

  return (
    
    <Router>
      <div className="App">
        <Header 
          isLoggedIn={isLoggedIn} 
          setIsLoggedIn={setIsLoggedIn} 
          isAdmin={isAdmin} 
          setIsAdmin={setIsAdmin} 
          checkIfAdmin={checkIfAdmin}
          decodedJWT={decodedJWT} 
          userBalanceChanged={userBalanceChanged}
        />
        <MainPageWelcomeText />
        {showWelcomeScreen && (
          <WelcomeScreen onClose={handleCloseWelcome} />
        )}
        <Routes>
          <Route path="/" element=
            {
            <>
              <MainPageEvents setUserBalanceChanged={setUserBalanceChanged} categories={categories}/>
              <CategoryForm setCategories={setCategories}/>
            </>
            } 
          />
          <Route path="/admin" element={<AdminPanel />} />
          <Route path="/user" element={<UserPanel />}/>
        </Routes>
        <Footer />
      </div>
    </Router>
  );

  function checkIfAdmin() {
    const token = localStorage.getItem('token');
    if (!token) {
      return null;
    }
  
    try {
      const decodedToken = jwtDecode(token);
      console.log(decodedToken);
      const role = decodedToken.role;
      if (role === "User") {
        return false;
      }
      return true;
    } catch (error) {
      console.error('Błąd podczas dekodowania tokenu JWT:', error);
      return null;
    }
  }
}

export default App;
