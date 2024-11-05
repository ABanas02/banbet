import React from 'react';
import UserBets from './UserBets';

function UserPanel() {
    return (
        <div className='admin-panel'>
            <h2>Panel użytkownika</h2>
            <div className='admin-forms'>
                <UserBets />
            </div>
        </div>
    );
}

export default UserPanel;