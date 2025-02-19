import React, { useState } from 'react';
import { eventsChangedSignal } from './Signals/teamsSignal';

function CreateEventForm() {
  const [eventName, setEventName] = useState('');
  const [startDateTime, setStartDateTime] = useState('');
  const [description, setDescription] = useState('');
  const [category, setCategory] = useState('');
  const [message, setMessage] = useState('');

  const handleCreateEvent = async (e) => {
    e.preventDefault();

    try {
      const token = localStorage.getItem('token');
      const response = await fetch(
        `${process.env.REACT_APP_BACKEND_URL}/api/Events`,
        {
          method: 'POST',
          headers: {
            'Content-Type': 'application/json',
            Authorization: `Bearer ${token}`,
          },
          body: JSON.stringify({
            eventName,
            startDateTime,
            description,
            category: parseInt(category)
          }),
        }
      );

      if (!response.ok) {
        throw new Error('Błąd podczas tworzenia wydarzenia.');
      }

      const data = await response.json();
      setMessage(`Wydarzenie ${data.eventName} zostało utworzone.`);
      setEventName('');
      setStartDateTime('');
      setDescription('');
      eventsChangedSignal.value = !eventsChangedSignal.value;
    } catch (error) {
      setMessage(error.message);
    }
  };

  return (
    <div className="form-container">
      <h3>Utwórz nowe wydarzenie</h3>
      {message && <p>{message}</p>}
      <form onSubmit={handleCreateEvent}>
        <div>
          <label>Nazwa wydarzenia:</label>
          <input
            type="text"
            value={eventName}
            onChange={(e) => setEventName(e.target.value)}
            required
          />
        </div>
        <div>
        <label>Kategoria:</label>
          <select
            value={category}
            onChange={(e) => setCategory(e.target.value)}
            required
          >
            <option value="">-- Wybierz kategorię --</option>
            <option value="0">Piłka nożna</option>
            <option value="1">Koszykówka</option>
            <option value="2">Siatkówka</option>
          </select>
        </div>
        <div>
          <label>Data i godzina rozpoczęcia:</label>
          <input
            type="datetime-local"
            value={startDateTime}
            onChange={(e) => setStartDateTime(e.target.value)}
            required
          />
        </div>
        <div>
          <label>Opis:</label>
          <textarea
            value={description}
            onChange={(e) => setDescription(e.target.value)}
          ></textarea>
        </div>
        <button type="submit">Utwórz wydarzenie</button>
      </form>
    </div>
  );
}

export default CreateEventForm;
