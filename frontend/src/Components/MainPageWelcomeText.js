import React, { useEffect, useRef } from 'react';
import './css/MainPageWelcomeText.css';

function MainPageWelcomeText() {
    const textRef = useRef(null);

    useEffect(() => {
        const text = textRef.current;
        if (!text) return;

        const handleMouseMove = (e) => {
            const { left, top, width, height } = text.getBoundingClientRect();
            const centerX = left + width / 2;
            const centerY = top + height / 2;

            const deltaX = e.clientX - centerX;
            const deltaY = e.clientY - centerY;

            const rotateX = (deltaY / height) * 10;
            const rotateY = -(deltaX / width) * 10;

            text.style.transform = `perspective(1000px) rotateX(${rotateX}deg) rotateY(${rotateY}deg)`;
        };

        const handleMouseLeave = () => {
            text.style.transform = 'perspective(1000px) rotateX(0deg) rotateY(0deg)';
        };

        document.addEventListener('mousemove', handleMouseMove);
        document.addEventListener('mouseleave', handleMouseLeave);

        return () => {
            document.removeEventListener('mousemove', handleMouseMove);
            document.removeEventListener('mouseleave', handleMouseLeave);
        };
    }, []);

    return (
        <div className='welcomeTextContainer'>
            <h1 className='welcomeText' ref={textRef}>
                Witaj w aplikacji bukmacherskiej Banbet
            </h1>
        </div>
    );
}

export default MainPageWelcomeText;
