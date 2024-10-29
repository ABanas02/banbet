import React, { useState, useEffect } from 'react';
//import './AddTeamsToEventForm.css';

function AddTeamsToEventForm() {
  const [events, setEvents] = useState([]);
  const [teams, setTeams] = useState([]);
  const [selectedEvent, setSelectedEvent] = useState('');
  const [selectedTeams, setSelectedTeams] = useState([]);
  const [message, setMessage] = useState('');

  useEffect(() => {
    const fetchEventsAndTeams = async () => {
      try {
        const token = localStorage.getItem('token');
        const [eventsResponse, teamsResponse] = await Promise.all([
          fetch(`http://localhost:8080/api/Events`, {
            headers: { Authorization: `Bearer ${token}` },
          }),
          fetch(`http://localhost:8080/api/Team`, {
            headers: { Authorization: `Bearer ${token}` },
          }),
        ]);

        if (!eventsResponse.ok || !teamsResponse.ok) {
          throw new Error('Błąd podczas pobierania danych.');
        }

        const eventsData = await eventsResponse.json();
        const teamsData = await teamsResponse.json();

        setEvents(eventsData);
        setTeams(teamsData);
      } catch (error) {
        setMessage(error.message);
      }
    };

    fetchEventsAndTeams();
  }, []);

  const handleAddTeamsToEvent = async (e) => {
    e.preventDefault();

    try {
      const token = localStorage.getItem('token');
      const response = await fetch(
        `http://localhost:8080/api/Team/AddTeamsToEvent`,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({
            eventID: selectedEvent,
            teamIDs: selectedTeams,
          }),
        }
      );

      if (!response.ok) {
        throw new Error('Błąd podczas dodawania drużyn do wydarzenia.');
      }

      const data = await response.text();
      setMessage(data);
      setSelectedEvent('');
      setSelectedTeams([]);
    } catch (error) {
      setMessage(error.message);
    }
  };

  const handleTeamSelection = (e) => {
    const value = parseInt(e.target.value);
    if (e.target.checked) {
      setSelectedTeams([...selectedTeams, value]);
    } else {
      setSelectedTeams(selectedTeams.filter((id) => id !== value));
    }
  };

  return (
    <div className="form-container">
      <h3>Dodaj drużyny do wydarzenia</h3>
      {message && <p>{message}</p>}
      <form onSubmit={handleAddTeamsToEvent}>
        <div>
          <label>Wybierz wydarzenie:</label>
          <select
            value={selectedEvent}
            onChange={(e) => setSelectedEvent(e.target.value)}
            required
          >
            <option value="">-- Wybierz wydarzenie --</option>
            {events.map((event) => (
              <option key={event.eventID} value={event.eventID}>
                {event.eventName}
              </option>
            ))}
          </select>
        </div>
        <div>
          <label>Wybierz drużyny:</label>
          {teams.map((team) => (
            <div key={team.teamID}>
              <input
                type="checkbox"
                value={team.teamID}
                onChange={handleTeamSelection}
                checked={selectedTeams.includes(team.teamID)}
              />
              <label>{team.teamName}</label>
            </div>
          ))}
        </div>
        <button type="submit">Dodaj drużyny do wydarzenia</button>
      </form>
    </div>
  );
}

export default AddTeamsToEventForm;
