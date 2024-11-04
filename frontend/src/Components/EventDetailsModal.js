import React, { useState, useEffect } from 'react';
import './css/EventDetailsModal.css';

function EventDetailsModal({ eventID, onClose }) {
  const [eventDetails, setEventDetails] = useState(null);

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
      }
    };

    fetchEventDetails();
  }, [eventID]);

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
        <h3>Kursy</h3>
        <div className="odds-list">
          {eventDetails.odds.map((odd, index) => (
            <div key={index} className="odd-item">
              <p>Typ zakładu: {getBetTypeName(odd.betType)}</p>
              <p>Kurs: {odd.oddsValue}</p>
              {odd.teamID && <p>Drużyna ID: {odd.teamID}</p>}
              {/* Pole do wpisania kwoty zakładu */}
              <div>
                <label>Kwota zakładu:</label>
                <input type="number" min="1" step="0.01" />
                {/* Przycisk do złożenia zakładu */}
                <button>Złóż zakład</button>
              </div>
            </div>
          ))}
        </div>
        <button onClick={onClose}>Zamknij</button>
      </div>
    </div>
  );
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

export default EventDetailsModal;
