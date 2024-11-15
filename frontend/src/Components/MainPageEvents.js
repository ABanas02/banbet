import React, { useEffect, useState } from 'react';
import EventDetailsModal from './EventDetailsModal';
import './css/MainPageEvents.css';

function MainPageEvents({setUserBalanceChanged}) {
  const [events, setEvents] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedEventID, setSelectedEventID] = useState(null);
  const [showModal, setShowModal] = useState(false);

  useEffect(() => {
    const fetchEvents = async () => {
      try {
        const response = await fetch(`${process.env.REACT_APP_BACKEND_URL}/api/Events`);
        if (!response.ok) {
          throw new Error('Błąd podczas pobierania wydarzeń.');
        }
        const data = await response.json();
        setEvents(data);
        setLoading(false);
      } catch (err) {
        setError(err.message);
        setLoading(false);
      }
    };

    fetchEvents();
  }, []);

  const handleEventClick = (eventID) => {
    setSelectedEventID(eventID);
    setShowModal(true);
  };

  const handleCloseModal = () => {
    setShowModal(false);
    setSelectedEventID(null);
  };

  return (
    <div className="events-container">
      { loading && (
        <p>Ładowanie wydarzeń</p>
      )}
      { error && (
        <p>Błąd: {error}</p>
      )}
      {events.map((event) => (
        <div key={event.eventID} className="event-card" onClick={() => handleEventClick(event.eventID)}>
          {event.teams && event.teams.length >= 2 ? (
            <h3>
              {event.teams[0].teamName} vs {event.teams[1].teamName}
            </h3>
          ) : (
            <h3>{event.eventName}</h3>
          )}
          <p>Data: {new Date(event.startDateTime).toLocaleString()}</p>
        </div>
      ))}
      {showModal && (
        <EventDetailsModal eventID={selectedEventID} onClose={handleCloseModal} setUserBalanceChanged={setUserBalanceChanged} />
      )}
    </div>
  );
}

export default MainPageEvents;
