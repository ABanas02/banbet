import React from 'react';
import { useEffect, useState } from 'react';
import './css/UserBets.css';

function UserBets(){
    const [bets, setBets] = useState([]);



    useEffect(() => {
        const fetchBets = async () => {
            try {
                const token = localStorage.getItem('token');
                const response = await fetch(`${process.env.REACT_APP_BACKEND_URL}/api/Bets/MyBets`, {
                    headers: {Authorization: `Bearer ${token}`}
                });

                if (!response.ok) {
                    throw new Error("Błąd!")
                }

                const bets = await response.json();
                setBets(bets);
            } catch(err) {

            }
        }

        fetchBets();
    }, []);

    return (
      <div className="user-bets-container">
        <h3 className="bets-header">Twoje zakłady</h3>
        {bets.length === 0 ? (
          <p className="no-bets-message">Nie masz jeszcze żadnych zakładów.</p>
        ) : (
          <ul className="bets-list">
            {bets.map((bet) => (
              <li key={bet.betID} className="bet-item">
                <p className="bet-id">Zakład ID: {bet.betID}</p>
                <p className="bet-amount">Kwota zakładu: {bet.betAmount}</p>
                <p className={`bet-status ${getBetStatusName(bet.betStatus).toLowerCase()}`}>
                  Status zakładu: {getBetStatusName(bet.betStatus)}
                </p>
                <p className="bet-date">Data zakładu: {formatDate(bet.betDate)}</p>
                <p className="bet-type">Typ zakładu: {getBetTypeName(bet.odd.betType)}</p>
                <p className="bet-odds">Kurs: {bet.odd.oddsValue}</p>
                {bet.odd.team && <p className="bet-team">Drużyna: {bet.odd.team.teamName}</p>}
                <p className="bet-event">Wydarzenie: {bet.odd.event.eventName}</p>
              </li>
            ))}
          </ul>
        )}
      </div>
    );
    }
    
    function getBetStatusName(betStatus) {
      switch (betStatus) {
        case 0:
          return 'Otwarte';
        case 1:
          return 'Wygrane';
        case 2:
          return 'Przegrane';
        default:
          return 'Nieznany status';
      }
    }

    function getBetTypeName(betType) {
      switch (betType) {
        case 0:
          return 'Zwycięzca meczu';
        case 1:
          return 'Łączna liczba goli';
        case 2:
          return 'Obie drużyny strzelą';
        default:
          return 'Inny typ zakładu';
      }
    }
    
    function formatDate(dateString) {
      const options = { year: 'numeric', month: 'numeric', day: 'numeric', hour: 'numeric', minute: 'numeric' };
      const date = new Date(dateString);
      return date.toLocaleDateString('pl-PL', options);
    }


export default UserBets;