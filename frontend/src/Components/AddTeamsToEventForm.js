import { useSignalEffect } from '@preact/signals-react';
import React, { useState, useEffect } from 'react';
import { teamsChangedSignal, eventsChangedSignal } from './Signals/teamsSignal';

function AddTeamsToEventForm() {
  const [events, setEvents] = useState([]);
  const [teams, setTeams] = useState([]);
  const [selectedEvent, setSelectedEvent] = useState('');
  const [selectedTeams, setSelectedTeams] = useState([]);
  const [message, setMessage] = useState('');

  const fetchEventsAndTeams = async () => {
    try {
      const token = localStorage.getItem('token');
      const [eventsResponse, teamsResponse] = await Promise.all([
        fetch(`${process.env.REACT_APP_BACKEND_URL}/api/Events`, {
          headers: { Authorization: `Bearer ${token}` },
        }),
        fetch(`${process.env.REACT_APP_BACKEND_URL}/api/Team`, {
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

  useSignalEffect(() => {
    console.log('teamsChangedSignal.value:', teamsChangedSignal.value);
    console.log('eventsChangedSignal.value:', eventsChangedSignal.value);
    fetchEventsAndTeams();
  });

  useEffect(() => {
    fetchEventsAndTeams();
  }, []);

  const handleAddTeamsToEvent = async (e) => {
    e.preventDefault();

    if (selectedTeams.length === 0) {
      setMessage('Proszę wybrać co najmniej jedną drużynę.');
      return;
    }

    try {
      const token = localStorage.getItem('token');
      const response = await fetch(
        `${process.env.REACT_APP_BACKEND_URL}/api/Team/AddTeamsToEvent`,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({
            eventID: selectedEvent,
            teamIDs: selectedTeams.map(id => parseInt(id)),
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
    const selectedOptions = Array.from(e.target.selectedOptions).map(option => option.value);
    setSelectedTeams(selectedOptions);
  };

  return (
    <div className="form-container">
      <h3>Dodaj drużyny do wydarzenia</h3>
      {message && <p className={message.includes('Błąd') ? 'error' : 'success'}>{message}</p>}
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
          <select
            multiple
            value={selectedTeams}
            onChange={handleTeamSelection}
            required
            size={5}
            className="multi-select"
          >
            {teams.map((team) => (
              <option key={team.teamID} value={team.teamID}>
                {team.teamName}
              </option>
            ))}
          </select>
          <small className="select-hint">Przytrzymaj Ctrl aby wybrać wiele drużyn</small>
        </div>
        <button type="submit">Dodaj drużyny do wydarzenia</button>
      </form>
    </div>
  );
}

export default AddTeamsToEventForm;
