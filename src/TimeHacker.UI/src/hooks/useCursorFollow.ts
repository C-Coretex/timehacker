import { useEffect, useRef } from 'react';

/**
 * Keeps `--th-cursor-x/y` on the returned element in step with the pointer anywhere on the page (relative to
 * that element), one write per frame and no React state. The variables go on this element alone, so a move
 * restyles only its own few children rather than the whole app. Leaving the window clears them, so CSS falls
 * back to its resting position. Does nothing for reduced motion or on devices without hover.
 */
export function useCursorFollow<T extends HTMLElement>() {
  const ref = useRef<T>(null);

  useEffect(() => {
    const element = ref.current;
    if (!element || !window.matchMedia('(hover: hover) and (prefers-reduced-motion: no-preference)').matches) return;

    const page = document.documentElement;
    let frame = 0;
    const follow = (event: PointerEvent) => {
      cancelAnimationFrame(frame);
      frame = requestAnimationFrame(() => {
        const bounds = element.getBoundingClientRect();
        element.style.setProperty('--th-cursor-x', `${event.clientX - bounds.left}px`);
        element.style.setProperty('--th-cursor-y', `${event.clientY - bounds.top}px`);
      });
    };
    const rest = () => {
      cancelAnimationFrame(frame);
      element.style.removeProperty('--th-cursor-x');
      element.style.removeProperty('--th-cursor-y');
    };

    page.addEventListener('pointermove', follow, { passive: true });
    page.addEventListener('pointerleave', rest);
    return () => {
      page.removeEventListener('pointermove', follow);
      page.removeEventListener('pointerleave', rest);
      cancelAnimationFrame(frame);
    };
  }, []);

  return ref;
}
