import React, { useState } from 'react';
import { teamsChangedSignal } from './Signals/teamsSignal';

function AddTeamForm() {
  const [teamName, setTeamName] = useState('');
  const [message, setMessage] = useState('');

  const handleAddTeam = async (e) => {
    e.preventDefault();

    try {
      const token = localStorage.getItem('token');
      const response = await fetch(
        `${process.env.REACT_APP_BACKEND_URL}/api/Team/AddTeam`,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({ teamName }),
        }
      );

      if (!response.ok) {
        throw new Error('Błąd podczas dodawania drużyny.');
      }

      const data = await response.json();
      setMessage(`Drużyna ${data.teamName} została dodana.`);
      setTeamName('');
      teamsChangedSignal.value = !teamsChangedSignal.value;
    } catch (error) {
      setMessage(error.message);
    }
  };

  return (
    <div className="form-container">
      <h3>Dodaj nową drużynę</h3>
      {message && <p>{message}</p>}
      <form onSubmit={handleAddTeam}>
        <div>
          <label>Nazwa drużyny:</label>
          <input
            type="text"
            value={teamName}
            onChange={(e) => setTeamName(e.target.value)}
            required
          />
        </div>
        <button type="submit">Dodaj drużynę</button>
      </form>
    </div>
  );
}

export default AddTeamForm;
