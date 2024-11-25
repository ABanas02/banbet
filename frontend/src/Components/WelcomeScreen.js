import React from "react";
import './css/WelcomeScreen.css';

function WelcomeScreen({ onClose }) {
  return (
    <div className="welcome-screen">
      <div className="welcome-content">
        <h1>Witaj w aplikacji bukmacherskiej banbet!</h1>
        <p>Dla ułatwienia korzystania z aplikacji zostało automatycznie utworzone</p>
        <p>konto administora w którym będziesz miał dostęp do tworzenia wszystkich</p>
        <p>wydarzeń i zakładów w panelu admina!</p>
        <p>By zalogować się na konto zaloguj się następującymi danymi:</p>
        <p className="pass">Nazwa użytkownika: admin</p>
        <p className="pass">Hasło: admin</p>
        <p>Miłej zabawy!</p>
        <button className='button' onClick={onClose}>Rozumiem</button>
      </div>
    </div>
  );
}

export default WelcomeScreen;
