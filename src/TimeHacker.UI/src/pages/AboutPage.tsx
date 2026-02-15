// src/pages/AboutPage.tsx
import type { FC } from 'react';
import { useTranslation } from 'react-i18next';

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
    roleKey: string;
    bioKey: string;
    photoSrc: string;
}

const developers: Developer[] = [
    {
        name: 'Valery Shermanis',
        roleKey: 'about.devValeryRole',
        bioKey: 'about.devValeryBio',
        photoSrc: valeryPhoto
    },
    {
        name: 'Nikita Trofimov',
        roleKey: 'about.devNikitaRole',
        bioKey: 'about.devNikitaBio',
        photoSrc: nikitaPhoto
    },
    {
        name: 'Arina Fokina',
        roleKey: 'about.devArinaRole',
        bioKey: 'about.devArinaBio',
        photoSrc: arinaPhoto
    },
];

const AboutPage: FC = () => {
    const { t } = useTranslation();

    return (
        <div className={styles.aboutContainer}>
            <div className={styles.section}>
                <h1>{t('about.title')}</h1>
                <p>{t('about.intro1')}</p>
                <p>{t('about.intro2')}</p>
                <p>{t('about.version')}</p>

                <p>{t('about.descriptionParagraph1')}</p>
                <p>
                    <strong>{t('about.descriptionParagraph2Start')}</strong>
                    {t('about.descriptionParagraph2Middle')}
                    <strong>{t('about.descriptionParagraph2End')}</strong>
                </p>
                <p>{t('about.aiIntro')}</p>
                <ul>
                    <li><strong>{t('about.aiOptimize')}</strong>: {t('about.aiOptimizeDesc')}</li>
                    <li><strong>{t('about.aiInsights')}</strong>: {t('about.aiInsightsDesc')}</li>
                    <li><strong>{t('about.aiStreamline')}</strong>: {t('about.aiStreamlineDesc')}</li>
                    <li><strong>{t('about.aiAdapt')}</strong>: {t('about.aiAdaptDesc')}</li>
                </ul>
                <p>{t('about.closingParagraph')}</p>
            </div>

            <div className={styles.section}>
                <h2><MissionIcon /> {t('about.missionTitle')}</h2>
                <p>{t('about.missionText')}</p>
            </div>

            <div className={styles.section}>
                <h2><ValuesIcon /> {t('about.valuesTitle')}</h2>
                <ul>
                    <li><strong>{t('about.valueInnovation')}</strong>: {t('about.valueInnovationDesc')}</li>
                    <li><strong>{t('about.valueSimplicity')}</strong>: {t('about.valueSimplicityDesc')}</li>
                    <li><strong>{t('about.valueReliability')}</strong>: {t('about.valueReliabilityDesc')}</li>
                    <li><strong>{t('about.valueUX')}</strong>: {t('about.valueUXDesc')}</li>
                </ul>
            </div>

            <div className={styles.section}>
                <h2><TeamIcon /> {t('about.teamTitle')}</h2>
                <div className={styles.teamGrid}>
                    {developers.map((developer, index) => (
                        <div key={index} className={styles.developerCard}>
                            <div className={styles.developerPhoto}>
                                <img src={developer.photoSrc} alt={developer.name} />
                            </div>
                            <h3>{developer.name}</h3>
                            <p><strong>{t(developer.roleKey)}</strong></p>
                            <p>{t(developer.bioKey)}</p>
                        </div>
                    ))}
                </div>
            </div>
        </div>
    );
};

export default AboutPage;
