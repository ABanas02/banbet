import React, { useEffect, useState } from "react";

function ResolveMatchWinner() {
  const [message, setMessage] = useState('');
  const [eventID, setEventID] = useState('');
  const [teamID, setTeamID] = useState('');
  const [events, setEvents] = useState([]);

  useEffect(() => {
    const fetchEventsAndTeams = async () => {
      try {
        const token = localStorage.getItem("token");
        const eventsResponse = await fetch(`${process.env.REACT_APP_BACKEND_URL}/api/Events`,
          {
            headers: { Authorization: `Bearer ${token}` }
          });
        const eventsData = await eventsResponse.json();

        const eventsWithTeams = await Promise.all(eventsData.map(async (event) => {
          const teamsResponse = await fetch(`${process.env.REACT_APP_BACKEND_URL}/api/Team/GetTeamsFromEvent/${event.eventID}`,
            {
              headers: { Authorization: `Bearer ${token}` }
            });
          const teamsData = await teamsResponse.json();
          return { ...event, teams: teamsData };
        }));

        setEvents(eventsWithTeams);
      } catch (error) {
        console.error(error);
        setMessage('Wystąpił błąd podczas pobierania wydarzeń i drużyn.');
      }
    };

    fetchEventsAndTeams();
  }, []);

  const resolveBets = async (e) => {
    e.preventDefault();
    try {
      const token = localStorage.getItem('token');
      const response = await fetch(`${process.env.REACT_APP_BACKEND_URL}/api/Bets/ResolveMatchWinner`,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({
            eventID: parseInt(eventID),
            teamID: parseInt(teamID)
          })
        });

      if (!response.ok) {
        throw new Error('Błąd podczas rozliczania zakładów.');
      }

      setMessage('Zakłady zostały pomyślnie rozliczone.');
    } catch (error) {
      console.error(error);
      setMessage('Wystąpił błąd podczas rozliczania zakładów.');
    }
  };

  return (
    <div className="form-container">
      <h2>Rozliczenie zakładów</h2>
      {message && <p>{message}</p>}
      <form onSubmit={resolveBets}>
        <div>
          <label>Wybierz wydarzenie:</label>
          <select
            value={eventID}
            onChange={(e) => setEventID(e.target.value)}
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
        {eventID && (
          <div>
            <label>Wybierz zwycięską drużynę:</label>
            <select
              value={teamID}
              onChange={(e) => setTeamID(e.target.value)}
              required
            >
              <option value="">-- Wybierz drużynę --</option>
              {events
                .find((event) => event.eventID === parseInt(eventID))
                ?.teams.map((team) => (
                  <option key={team.teamID} value={team.teamID}>
                    {team.teamName}
                  </option>
                ))}
            </select>
          </div>
        )}
        <button type="submit">Rozlicz zakłady</button>
      </form>
    </div>
  );
}

export default ResolveMatchWinner;
