// src/pages/AboutPage.tsx
import React from 'react';
import type { FC } from 'react';

import styles from './AboutPage.module.css';

import valeryPhoto from '../assets/images/valery.png';
import nikitaPhoto from '../assets/images/nikita.png';
import arinaPhoto from '../assets/images/arina.png';

const MissionIcon = () => (
    <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
        <path d="M12 2L2 7l10 5 10-5-10-5zM2 17l10 5 10-5M2 12l10 5 10-5"></path>
    </svg>
);

const ValuesIcon = () => (
    <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
        <path d="M12 2L2 7l10 5 10-5-10-5zM2 17l10 5 10-5M2 12l10 5 10-5"></path>
    </svg>
);

const TeamIcon = () => (
    <svg width="24" height="24" viewBox="0 0 24 24" fill="none" stroke="currentColor" strokeWidth="2" strokeLinecap="round" strokeLinejoin="round">
        <path d="M17 21v-2a4 4 0 0 0-4-4H5a4 4 0 0 0-4 4v2"></path>
        <circle cx="9" cy="7" r="4"></circle>
        <path d="M23 21v-2a4 4 0 0 0-3-3.87"></path>
        <path d="M16 3.13a4 4 0 0 1 0 7.75"></path>
    </svg>
);

interface Developer {
    name: string;
    role: string;
    bio: string;
    photoSrc: string;
}

const developers: Developer[] = [
    {
        name: 'Valery Shermanis',
        role: 'Director, backend Developer',
        bio: 'Works on server-side logic, databases, and APIs, ensuring the app runs smoothly.Defines the product strategy, gathers requirements, and guides the team towards goals.',
        photoSrc: valeryPhoto
    },
    {
        name: 'Nikita Trofimov ',
        role: 'Frontend Developer',
        bio: 'Responsible for the user interface and interaction, turning designs into a live application.',
        photoSrc: nikitaPhoto
    },
    {
        name: 'Arina Fokina',
        role: 'Frontend Manager',
        bio: 'Responsible for the user interface and interaction, turning designs into a live application.',
        photoSrc: arinaPhoto
    },
];

const AboutPage: FC = () => {
    return (
        <div className={styles.aboutContainer}>
            <div className={styles.section}>
                <h1>About the TimeHacker</h1>
                <p>
                    TimeHacker is an innovative time and task management application
                    designed to help you effectively plan your day,
                    track your progress, and achieve your goals.
                </p>
                <p>
                    We strive to provide an intuitive and powerful tool
                    that makes time management not only simple but also enjoyable.
                </p>
                <p>
                    Version: 1.0.0
                </p>

                <p>
                    In today's fast-paced world, effective time management is the cornerstone of success. TimeHacker stands at the forefront of innovation, offering a sophisticated yet intuitive solution designed to transform the way you interact with your daily schedule and long-term aspirations.
                </p>
                <p>
                    <strong>TimeHacker is an innovative time and task management application designed to help you effectively plan your day, track your progress, and achieve your goals.</strong> We believe that managing your time should not be a chore, but an empowering experience. <strong>We strive to provide an intuitive and powerful tool that makes time management not only simple but also enjoyable.</strong>
                </p>
                <p>
                    At the core of TimeHacker's transformative power is the seamless integration of Artificial Intelligence. We are committed to empowering individuals to manage their time with unparalleled efficiency. Our intelligent features work tirelessly behind the scenes to:
                </p>
                <ul>
                    <li><strong>Optimize your schedule</strong>: AI algorithms analyze your habits and priorities to suggest the most productive arrangement of your tasks.</li>
                    <li><strong>Provide actionable insights</strong>: Gain a deeper understanding of how you spend your time with intelligent reports and personalized recommendations.</li>
                    <li><strong>Streamline your workflow</strong>: Automate routine tasks and receive smart notifications, freeing you to focus on what truly matters.</li>
                    <li><strong>Adapt to your needs</strong>: TimeHacker learns from your interactions, becoming more personalized and effective over time.</li>
                </ul>
                <p>
                    Experience the future of productivity. TimeHacker isn't just an app; it's your personal AI assistant, dedicated to helping you master your most valuable asset: time.
                </p>
            </div>

            <div className={styles.section}>
                <h2><MissionIcon /> Our Mission</h2>
                <p>
                    Our mission is to inspire users to be productive by providing
                    effective tools to organize their lives. We believe that with the right
                    approach, anyone can unlock their potential and manage their time
                    to the fullest.
                </p>
            </div>

            <div className={styles.section}>
                <h2><ValuesIcon /> Our Values</h2>
                <ul>
                    <li><strong>Innovation</strong>: Constantly seeking new ways to improve the user experience.</li>
                    <li><strong>Simplicity</strong>: Making complex things accessible and understandable.</li>
                    <li><strong>Reliability</strong>: Building a stable and secure application.</li>
                    <li><strong>User Experience</strong>: Putting user needs first.</li>
                </ul>
            </div>

            <div className={styles.section}>
                <h2><TeamIcon /> Development Team</h2>
                <div className={styles.teamGrid}>
                    {developers.map((developer, index) => (
                        <div key={index} className={styles.developerCard}>
                            <div className={styles.developerPhoto}>
                                <img src={developer.photoSrc} alt={developer.name} />
                            </div>
                            <h3>{developer.name}</h3>
                            <p><strong>{developer.role}</strong></p>
                            <p>{developer.bio}</p>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
};

export default AboutPage;
