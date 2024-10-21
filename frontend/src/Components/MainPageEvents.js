import React, { useEffect, useState } from 'react';
import './MainPageEvents.css'; // Dodamy style później

function MainPageEvents() {
    const [events, setEvents] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);
    

    useEffect(() => {
        const fetchEvents = async () => {
          try {
            const token = localStorage.getItem('token');
            const response = await fetch(`http://localhost:8080/api/Events`, {
                headers: {
                  'Authorization': `Bearer ${token}`,
                },
            });
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

      if (loading) {
        return <p>Ładowanie wydarzeń...</p>;
      }
    
      if (error) {
        return <p>Błąd: {error}</p>;
      }

      return (
        <div className="events-container">
          {events.map((event) => (
            <div key={event.eventID} className="event-card">
                {/*tutaj do dania drużyny według teamsId*/}
              <h3>team 1 vs team 2</h3>
            </div>
          ))}
        </div>
      );
}  

export default MainPageEvents;
