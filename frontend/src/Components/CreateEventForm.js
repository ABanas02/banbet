import React, { useState } from 'react';

function CreateEventForm() {
  const [eventName, setEventName] = useState('');
  const [startDateTime, setStartDateTime] = useState('');
  const [description, setDescription] = useState('');
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
