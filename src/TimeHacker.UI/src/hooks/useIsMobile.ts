import { Grid } from 'antd';

const { useBreakpoint } = Grid;

export function useIsMobile() {
  const screens = useBreakpoint();
  const isMobile = !screens.md;
  const isSmallMobile = !!screens.xs && !screens.sm;
  return { isMobile, isSmallMobile, screens };
}
