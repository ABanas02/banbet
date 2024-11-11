import React from "react";
import { useEffect, useState } from "react";

function ResolveMatchWinner() {
    const [message, setMessage] = useState('');
    const [eventID, setEventID] = useState(null);
    const [teamID, setTeamID] = useState(null);


    const resolveBets = async (e) => {
        e.preventDefault();
        try {
            const token = localStorage.getItem('token');
            await fetch(`${process.env.REACT_APP_BACKEND_URL}/api/Bets/ResolveMatchWinner`,
                {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json',
                        Authorization: `Bearer ${token}`,
                        },
                    body: JSON.stringify({
                        eventId: '7', // TESTOWE DANE
                        teamId: '4' //TESTOWE DANE
                    })
                })
        } catch (error) {
            setMessage(error);
        }
    }

    return (
        <div className="form-container">
            <h2>Rozliczenie zakładów</h2>
            <form onSubmit={resolveBets}>
                
            </form>
        </div>
    )

}




export default ResolveMatchWinner;