import React, { useState, useEffect } from 'react';
import './css/EventDetailsModal.css';

function EventDetailsModal({ eventID, onClose, setUserBalanceChanged}) {
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

      setMessage('Zakład został pomyślnie złożony.');
      setBetAmounts({ ...betAmounts, [odd.oddsID]: '' });
      setUserBalanceChanged(true);
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

  function getCategoryName(category) {
    switch (category) {
      case 0:
        return 'Piłka nożna';
      case 1:
        return 'Koszykówka';
      case 2:
        return 'Siatkówka';
      default:
        return 'Inna kategoria';
    }
  }

  if (!eventDetails) {
    return (
      <div className="modal-overlay">
        <div className="modal-content">
          <div className="modal-body">
            <p className="loading-text">Ładowanie...</p>
          </div>
          <div className="modal-footer">
            <button className="close-button" onClick={onClose}>Zamknij</button>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="modal-overlay">
      <div className="modal-content">
        <div className="modal-body">
          <div className="event-header">
            {eventDetails.teams.length !== 0 ?
              <h1 className="event-title">
                {eventDetails.teams[0].teamName} vs {eventDetails.teams[1].teamName}
              </h1>
              :
              <h1 className="no-teams-added-alert">
                Nie dodano żadnych drużyn do wydarzenia!!
              </h1>
            }
            <div className="event-category">
              {getCategoryName(eventDetails.category)}
            </div>
            {eventDetails.description && (
              <p className="event-description">{eventDetails.description}</p>
            )}
          </div>

          {message && <div className="successAlert">{message}</div>}

          <div className="betting-section">
            <h2 className="betting-title">Kursy</h2>
            <div className="odds-list">
              {eventDetails.odds.map((odd) => (
                <div key={odd.oddsID} className="odd-item">
                  <div className="odd-details">
                    <p className="bet-type">
                      <span className="label">Typ zakładu:</span>
                      <span className="value">{getBetTypeName(odd.betType)}</span>
                    </p>
                    <p className="odds-value">
                      <span className="label">Kurs:</span>
                      <span className="value">{odd.oddsValue}</span>
                    </p>
                    {odd.teamID && (
                      <p className="team-name">
                        <span className="label">Drużyna:</span>
                        <span className="value">{odd.teamName}</span>
                      </p>
                    )}
                  </div>
                  <div className="bet-input-section">
                    <label htmlFor={`bet-amount-${odd.oddsID}`}>Kwota zakładu:</label>
                    <input
                      id={`bet-amount-${odd.oddsID}`}
                      type="number"
                      min="1"
                      step="0.01"
                      value={betAmounts[odd.oddsID] || ''}
                      onChange={(e) => handleBetAmountChange(odd.oddsID, e.target.value)}
                      className="bet-amount-input"
                    />
                    <button 
                      className="place-bet-button"
                      onClick={() => handlePlaceBet(odd)}
                    >
                      Złóż zakład
                    </button>
                  </div>
                </div>
              ))}
            </div>
          </div>
        </div>
        <div className="modal-footer">
          <button className="close-button" onClick={onClose}>Zamknij</button>
        </div>
      </div>
    </div>
  );
}

export default EventDetailsModal;
