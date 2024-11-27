import React from 'react';
import UserBets from './UserBets';
import './css/UserPanel.css';

function UserPanel() {
    return (
        <div className="user-panel-container">
            <div className="user-panel-content">
                <UserBets />
            </div>
        </div>
    );
}

export default UserPanel;
