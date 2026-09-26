import type { FC } from 'react';
import { Link } from 'react-router-dom';
import './styles.css';

/** The "TIM⏱ HACKER" wordmark; always links home. */
export const Logo: FC<{ onClick?: () => void }> = ({ onClick }) => (
  <Link to="/" className="th-logo" aria-label="TimeHacker" onClick={onClick}>
    <span className="th-logo__tim">
      TIM
      <svg className="th-logo__clock" viewBox="0 0 16 18" aria-hidden>
        <rect x="6" y="0" width="4" height="2.4" rx="1" fill="currentColor" />
        <circle cx="8" cy="10.6" r="6.6" fill="currentColor" />
        <path d="M8 6.8v4l2.6 1.6" fill="none" stroke="var(--th-on-primary)" strokeWidth="1.6" strokeLinecap="round" />
      </svg>
    </span>
    <span className="th-logo__hacker">HACKER</span>
  </Link>
);
