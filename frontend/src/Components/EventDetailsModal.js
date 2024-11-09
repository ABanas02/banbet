import React, { useState, useEffect } from 'react';
import './css/EventDetailsModal.css';

function EventDetailsModal({ eventID, onClose }) {
  const [eventDetails, setEventDetails] = useState(null);
  const [betAmounts, setBetAmounts] = useState({});
  const [message, setMessage] = useState('');

  useEffect(() => {
    const fetchEventDetails = async () => {
      try {
        const response = await fetch(`${process.env.REACT_APP_BACKEND_URL}/api/Events/${eventID}`);
        if (!response.ok) {
          throw new Error('Błąd podczas pobierania szczegółów wydarzenia.');
        }
        const data = await response.json();
        setEventDetails(data);
      } catch (error) {
        console.error(error);
        setMessage('Wystąpił błąd podczas pobierania danych.');
      }
    };

    fetchEventDetails();
  }, [eventID]);

  const handleBetAmountChange = (oddsID, value) => {
    setBetAmounts({ ...betAmounts, [oddsID]: value });
  };

  const handlePlaceBet = async (odd) => {
    const amount = betAmounts[odd.oddsID];

    if (!amount || amount <= 0) {
      alert('Wprowadź poprawną kwotę zakładu.');
      return;
    }

    try {
      const token = localStorage.getItem('token');
      if (!token) {
        alert('Musisz być zalogowany, aby złożyć zakład.');
        return;
      }

      const response = await fetch(
        `${process.env.REACT_APP_BACKEND_URL}/api/Bets/PlaceBet`,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({
            oddsID: odd.oddsID,
            betAmount: parseFloat(amount),
          }),
        }
      );

      if (!response.ok) {
        const errorData = await response.json();
        throw new Error(errorData || 'Błąd podczas składania zakładu.');
      }

      const data = await response.json();
      setMessage('Zakład został pomyślnie złożony.');
      setBetAmounts({ ...betAmounts, [odd.oddsID]: '' });
    } catch (error) {
      console.error(error);
      alert(error.message || 'Wystąpił błąd podczas składania zakładu.');
    }
  };

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

  if (!eventDetails) {
    return (
      <div className="modal-overlay">
        <div className="modal-content">
          <p>Ładowanie...</p>
          <button onClick={onClose}>Zamknij</button>
        </div>
      </div>
    );
  }

  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <h2>{eventDetails.eventName}</h2>
        <p>{eventDetails.description}</p>

        {message && <p>{message}</p>}

        <h3>Kursy</h3>
        <div className="odds-list">
          {eventDetails.odds.map((odd) => (
            <div key={odd.oddsID} className="odd-item">
              <p>Typ zakładu: {getBetTypeName(odd.betType)}</p>
              <p>Kurs: {odd.oddsValue}</p>
              {odd.teamID && <p>Drużyna: {odd.teamID}</p>}
              <div>
                <label>Kwota zakładu:</label>
                <input
                  type="number"
                  min="1"
                  step="0.01"
                  value={betAmounts[odd.oddsID] || ''}
                  onChange={(e) => handleBetAmountChange(odd.oddsID, e.target.value)}
                />
                <button onClick={() => handlePlaceBet(odd)}>Złóż zakład</button>
              </div>
            </div>
          ))}
        </div>
        <button onClick={onClose}>Zamknij</button>
      </div>
    </div>
  );
}

export default EventDetailsModal;
