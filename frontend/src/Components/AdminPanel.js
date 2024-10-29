import React from 'react';
import './css/AdminPanel.css';
import AddTeamForm from './AddTeamForm';
import CreateEventForm from './CreateEventForm';
import AddTeamsToEventForm from './AddTeamsToEventForm';
import SetOddsForm from './SetOddsForm';

function AdminPanel({onTeamsChanged, teamsChanged}) {
  return (
    <div className="admin-panel">
      <h2>Panel Administratora</h2>
      <div className="admin-forms">
        <AddTeamForm onTeamsChanged={onTeamsChanged} teamsChanged={teamsChanged}/>
        <CreateEventForm onTeamsChanged={onTeamsChanged} teamsChanged={teamsChanged}/>
        <AddTeamsToEventForm onTeamsChanged={onTeamsChanged} teamsChanged={teamsChanged}/>
        <SetOddsForm onTeamsChanged={onTeamsChanged} teamsChanged={teamsChanged}/>
      </div>
    </div>
  );
}

export default AdminPanel;