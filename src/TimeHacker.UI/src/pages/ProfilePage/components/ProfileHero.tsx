import type { FC, ReactNode } from 'react';
import { useTranslation } from 'react-i18next';

interface ProfileHeroProps {
  name: string;
  email: string | undefined;
  /** Right-aligned action — "Edit profile" while viewing. */
  action?: ReactNode;
}

const initialsOf = (name: string) =>
  name
    .split(' ')
    .filter(Boolean)
    .map((word) => word[0])
    .join('')
    .toUpperCase()
    .slice(0, 2) || '?';

/** The profile's banner: initials avatar on the brand gradient, with the name and email beside it. */
export const ProfileHero: FC<ProfileHeroProps> = ({ name, email, action }) => {
  const { t } = useTranslation();

  return (
    <section className="th-profile__hero">
      <span className="th-profile__avatar" aria-hidden>
        {initialsOf(name)}
      </span>
      <div className="th-profile__identity">
        <h2 className="th-profile__name">{name || t('profile.noNameSet')}</h2>
        {email && <div className="th-profile__email">{email}</div>}
      </div>
      {action && <div className="th-profile__hero-action">{action}</div>}
    </section>
  );
};
