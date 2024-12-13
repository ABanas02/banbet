import React, { useEffect, useState } from 'react';
import EventDetailsModal from './EventDetailsModal';
import './css/MainPageEvents.css';

function MainPageEvents({ setUserBalanceChanged, categories }) {
  const [events, setEvents] = useState([]);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState(null);
  const [selectedEventID, setSelectedEventID] = useState(null);
  const [showModal, setShowModal] = useState(false);
  const [isRecommended, setIsRecommended] = useState(false);
  const [isLoggedIn, setIsLoggedIn] = useState(false);
  const [recommendationType, setRecommendationType] = useState('basic');

  const categoryMap = {
    0: 'Football',
    1: 'Basketball',
    2: 'Volleyball',
  };

  const fetchNormalEvents = async () => {
    try {
      setLoading(true);
      const response = await fetch(`${process.env.REACT_APP_BACKEND_URL}/api/Events`);
      if (!response.ok) {
        throw new Error('Błąd podczas pobierania wydarzeń.');
      }
      const data = await response.json();
      setEvents(data);
      setIsRecommended(false);
      setLoading(false);
    } catch (err) {
      setError(err.message);
      setLoading(false);
    }
  };

  const fetchRecommendedEvents = async (strategy) => {
    try {
      setLoading(true);
      const token = localStorage.getItem('token');
      const recommendedResponse = await fetch(
        `${process.env.REACT_APP_BACKEND_URL}/api/Events/recommended?strategy=${strategy}`, 
        {
          headers: {
            'Authorization': `Bearer ${token}`
          }
        }
      );
      
      if (recommendedResponse.ok) {
        const data = await recommendedResponse.json();
        setEvents(data);
        setIsRecommended(true);
        setLoading(false);
      } else {
        throw new Error('Nie udało się pobrać rekomendowanych wydarzeń');
      }
    } catch (err) {
      setError(err.message);
      setLoading(false);
    }
  };

  useEffect(() => {
    const token = localStorage.getItem('token');
    setIsLoggedIn(!!token);
    fetchNormalEvents();
  }, []);

  const handleEventClick = (eventID) => {
    setSelectedEventID(eventID);
    setShowModal(true);
  };

  const handleCloseModal = () => {
    setShowModal(false);
    setSelectedEventID(null);
  };

  function getCategoryName(categoryValue) {
    return categoryMap[categoryValue];
  }

  const filteredEvents = events.filter((event) => {
    if (categories.length === 0) {
      return true;
    }
    const eventCategoryName = getCategoryName(event.category);
    return categories.includes(eventCategoryName);
  });

  const handleRecommendationTypeChange = (type) => {
    setRecommendationType(type);
    if (type === 'normal') {
      fetchNormalEvents();
    } else {
      fetchRecommendedEvents(type === 'ai' ? 'AI' : 'Basic');
    }
  };

  return (
    <div className="events-container">
      {isLoggedIn && (
        <div className="events-controls">
          <button 
            onClick={() => handleRecommendationTypeChange('normal')}
            className={!isRecommended ? 'active' : ''}
          >
            Wszystkie wydarzenia
          </button>
          <button 
            onClick={() => handleRecommendationTypeChange('basic')}
            className={isRecommended && recommendationType === 'basic' ? 'active' : ''}
          >
            Podstawowe rekomendacje
          </button>
          <button 
            onClick={() => handleRecommendationTypeChange('ai')}
            className={isRecommended && recommendationType === 'ai' ? 'active' : ''}
          >
            Rekomendacje AI
          </button>
        </div>
      )}
      {isRecommended && (
        <div className="recommendation-banner">
          {recommendationType === 'ai' 
            ? 'Wyświetlanie rekomendacji opartych na sztucznej inteligencji'
            : 'Wyświetlanie podstawowych rekomendacji na podstawie Twojej historii zakładów'
          }
        </div>
      )}
      {loading && <p>Ładowanie wydarzeń...</p>}
      {error && <p>Błąd: {error}</p>}
      {filteredEvents.map((event) => (
        <div key={event.eventID} className="event-card" onClick={() => handleEventClick(event.eventID)}>
          {event.teams && event.teams.length >= 2 ? (
            <h3>
              {event.teams[0].teamName} vs {event.teams[1].teamName}
            </h3>
          ) : (
            <h3>{event.eventName}</h3>
          )}
          <p>Kategoria: {getCategoryName(event.category)}</p>
          <p>Data: {new Date(event.startDateTime).toLocaleString()}</p>
        </div>
      ))}
      {showModal && (
        <EventDetailsModal
          eventID={selectedEventID}
          onClose={handleCloseModal}
          setUserBalanceChanged={setUserBalanceChanged}
        />
      )}
    </div>
  );
}

export default MainPageEvents;
