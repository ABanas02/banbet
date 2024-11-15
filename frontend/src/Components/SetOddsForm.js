import React, { useState, useEffect } from 'react';

function SetOddsForm() {
  const [events, setEvents] = useState([]);
  const [teams, setTeams] = useState([]);
  const [selectedEventID, setSelectedEventID] = useState('');
  const [selectedBetType, setSelectedBetType] = useState('');
  const [oddsValue, setOddsValue] = useState('');
  const [selectedTeamID, setSelectedTeamID] = useState('');
  const [message, setMessage] = useState('');
  const [selectedTeamName, setSelectedTeamName] = useState('');

  useEffect(() => {
    const fetchData = async () => {
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
          throw new Error('Błąd podczas pobierania wydarzeń lub drużyn.');
        }

        const eventsData = await eventsResponse.json();
        const teamsData = await teamsResponse.json();

        setEvents(eventsData);
        setTeams(teamsData);
      } catch (error) {
        setMessage(error.message);
      }
    };

    fetchData();
  }, []);

  const handleSetOdds = async (e) => {
    e.preventDefault();

    try {
      const token = localStorage.getItem('token');
      const response = await fetch(
        `http://localhost:8080/api/Odds/SetOdds`,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({
            eventID: parseInt(selectedEventID),
            betType: parseInt(selectedBetType),
            oddsValue: parseFloat(oddsValue),
            teamID: selectedTeamID ? parseInt(selectedTeamID) : null,
            teamName: selectedTeamName || null,
          }),
        }
      );

      if (!response.ok) {
        const errorText = await response.text();
        throw new Error(errorText || 'Błąd podczas ustawiania kursu.');
      }

      const data = await response.json();
      setMessage(`Kurs został pomyślnie ustawiony.`);
      setSelectedEventID('');
      setSelectedBetType('');
      setOddsValue('');
      setSelectedTeamID('');
      setSelectedTeamName('');
    } catch (error) {
      setMessage(error.message);
    }
  };

  function requiresTeam(betType) {
    return betType === '0' || betType === 'Handicap';
  }

  return (
    <div className="form-container">
      <h3>Ustaw kursy dla wydarzenia</h3>
      {message && <p>{message}</p>}
      <form onSubmit={handleSetOdds}>
        <div>
          <label>Wybierz wydarzenie:</label>
          <select
            value={selectedEventID}
            onChange={(e) => setSelectedEventID(e.target.value)}
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
          <label>Wybierz typ zakładu:</label>
          <select
            value={selectedBetType}
            onChange={(e) => setSelectedBetType(e.target.value)}
            required
          >
            <option value="">-- Wybierz typ zakładu --</option>
            <option value="0">Zwycięzca</option>
            <option value="Draw">Remis</option>
            <option value="TotalGoals">Łączna liczba goli</option>
            <option value="Handicap">Handicap</option>
          </select>
        </div>
        <div>
          <label>Wartość kursu:</label>
          <input
            type="number"
            step="0.01"
            value={oddsValue}
            onChange={(e) => setOddsValue(e.target.value)}
            required
          />
        </div>
        {requiresTeam(selectedBetType) && (
          <div>
            <label>Wybierz drużynę:</label>
            <select
              value={selectedTeamID}
              onChange={(e) => {
                const selectedID = e.target.value;
                setSelectedTeamID(selectedID);
                const selectedTeam = teams.find(
                  (team) => team.teamID === parseInt(selectedID)
                );
                setSelectedTeamName(selectedTeam ? selectedTeam.teamName : '');
              }}
              required
            >
              <option value="">-- Wybierz drużynę --</option>
              {teams.map((team) => (
                <option key={team.teamID} value={team.teamID}>
                  {team.teamName}
                </option>
              ))}
            </select>
          </div>
        )}
        <button type="submit">Ustaw kurs</button>
      </form>
    </div>
  );
}

export default SetOddsForm;
