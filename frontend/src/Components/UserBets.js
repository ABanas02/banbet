import React from 'react';
import { useEffect, useState } from 'react';
import './css/UserBets.css';

function UserBets() {
    useEffect(() => {
        const link = document.createElement('link');
        link.href = 'https://fonts.googleapis.com/css2?family=Poppins:wght@400;500;600;700&display=swap';
        link.rel = 'stylesheet';
        document.head.appendChild(link);

        return () => {
            document.head.removeChild(link);
        };
    }, []);

    const [bets, setBets] = useState([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState(null);

    useEffect(() => {
        const fetchBets = async () => {
            try {
                setLoading(true);
                setError(null);
                const token = localStorage.getItem('token');
                const response = await fetch(`${process.env.REACT_APP_BACKEND_URL}/api/Bets/MyBets`, {
                    headers: { Authorization: `Bearer ${token}` }
                });

                if (!response.ok) {
                    throw new Error("Nie udało się pobrać zakładów. Spróbuj ponownie później.");
                }

                const bets = await response.json();
                setBets(bets);
            } catch (err) {
                setError(err.message || "Wystąpił błąd podczas pobierania zakładów.");
            } finally {
                setLoading(false);
            }
        };

        fetchBets();
    }, []);

    if (loading) {
        return (
            <div className="user-bets-container">
                <div className="loading-spinner">
                    Ładowanie zakładów...
                </div>
            </div>
        );
    }

    if (error) {
        return (
            <div className="user-bets-container">
                <div className="error-message">
                    {error}
                </div>
            </div>
        );
    }

    return (
        <div className="user-bets-container">
            <h2 className="bets-header">Historia zakładów</h2>
            {bets.length === 0 ? (
                <p className="no-bets-message">
                    Nie masz jeszcze żadnych zakładów w historii
                </p>
            ) : (
                <ul className="bets-list">
                    {bets.map((bet) => (
                        <li key={bet.betID} className="bet-item">
                            <div className="bet-header">
                                <div className="bet-id">
                                    Zakład #{bet.betID}
                                </div>
                                <div className="bet-amount">
                                    {bet.betAmount} PLN
                                </div>
                                <div className={`bet-status ${getBetStatusName(bet.betStatus).toLowerCase()}`}>
                                    {getBetStatusName(bet.betStatus)}
                                </div>
                            </div>
                            <div className="bet-info">
                                <div className="bet-row">
                                    <div className="bet-type">
                                        {getBetTypeName(bet.odd.betType)}
                                    </div>
                                    <div className="bet-odds">
                                        Kurs {bet.odd.oddsValue}
                                    </div>
                                </div>
                                {bet.odd.team && (
                                    <div className="bet-team">
                                        {bet.odd.team.teamName}
                                    </div>
                                )}
                                <div className="bet-event">
                                    {bet.odd.event.eventName}
                                </div>
                                <div className="bet-date">
                                    {formatDate(bet.betDate)}
                                </div>
                            </div>
                        </li>
                    ))}
                </ul>
            )}
        </div>
    );
}

function getBetStatusName(betStatus) {
    switch (betStatus) {
        case 0:
            return 'Otwarte';
        case 1:
            return 'Wygrane';
        case 2:
            return 'Przegrane';
        default:
            return 'Nieznany status';
    }
}

function getBetTypeName(betType) {
    switch (betType) {
        case 0:
            return 'Zwycięzca meczu';
        case 1:
            return 'Łączna liczba goli';
        case 2:
            return 'Obie drużyny strzelą';
        default:
            return 'Inny typ zakładu';
    }
}

function formatDate(dateString) {
    const options = {
        day: 'numeric',
        month: 'long',
        year: 'numeric',
        hour: '2-digit',
        minute: '2-digit'
    };
    const date = new Date(dateString);
    return date.toLocaleDateString('pl-PL', options);
}

export default UserBets;
