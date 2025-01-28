import React from 'react';
import styled from 'styled-components';

export const Link = ({
  href,
  children,
  className,
  newPage,
  noStyle = true
}) => {

  const StyledLink = styled.a`
    &.no-style {
      text-decoration: none;
      color: inherit;
    }
  `;

  let completeClassName = '';

  if (className) {
    completeClassName += className;
  }

  if (noStyle) {
    completeClassName += ' no-style';
  }

  return (
    <StyledLink 
      href={href} 
      className={completeClassName}
      rel="noopener noreferrer"
      target={newPage ? "_blank" : "_self"}
    >
      {children}
    </StyledLink>
  );
};
