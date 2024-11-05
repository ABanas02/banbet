import React from 'react';
import { useEffect, useState } from 'react';


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
        <div>
          <h3>Twoje zakłady</h3>
          {bets.length === 0 ? (
            <p>Nie masz jeszcze żadnych zakładów.</p>
          ) : (
            <ul>
              {bets.map((bet) => (
                <li key={bet.betID}>
                  <p>Zakład ID: {bet.betID}</p>
                  <p>Kwota zakładu: {bet.betAmount}</p>
                  <p>Status zakładu: {getBetStatusName(bet.betStatus)}</p>
                  <p>Data zakładu: {formatDate(bet.betDate)}</p>
                  <p>Typ zakładu: {getBetTypeName(bet.odd.betType)}</p>
                  <p>Kurs: {bet.odd.oddsValue}</p>
                  {bet.odd.team && <p>Drużyna: {bet.odd.team.teamName}</p>}
                  <p>Wydarzenie: {bet.odd.event.eventName}</p>
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