import React from 'react';
//import './AdminPanel.css';
import AddTeamForm from './AddTeamForm';
import CreateEventForm from './CreateEventForm';
import AddTeamsToEventForm from './AddTeamsToEventForm';
import SetOddsForm from './SetOddsForm';

function AdminPanel() {
  return (
    <div className="admin-panel">
      <h2>Panel Administratora</h2>
      <div className="admin-forms">
        <AddTeamForm />
        <CreateEventForm />
        <AddTeamsToEventForm />
        <SetOddsForm />
      </div>
    </div>
  );
}

export default AdminPanel;