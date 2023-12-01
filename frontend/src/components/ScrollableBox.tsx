import * as React from 'react';
import { ReactElement, SyntheticEvent, useEffect, useRef } from 'react';
import { Box } from '@mui/material';

type Props = {
  indent: number;
  scrollToTop?: () => void;
  scrollToBottom?: () => void;
  children: ReactElement;
};

const appBarHeight = 80;

export const ScrollableBox = ({ children, indent, scrollToTop, scrollToBottom }: Props) => {
  const fullIndent = indent + appBarHeight;
  const divRef = useRef<HTMLDivElement>(null);

  const handleScroll = (event: SyntheticEvent) => {
    const targetDiv = event.target as HTMLDivElement;
    if (!targetDiv) {
      return;
    }
    if (
      targetDiv?.scrollTop + targetDiv.offsetHeight >= targetDiv?.scrollHeight &&
      targetDiv?.scrollTop !== 0 &&
      scrollToBottom
    ) {
      scrollToBottom();
    }

    if (
      targetDiv?.scrollTop === 0 &&
      targetDiv.offsetHeight !== targetDiv?.scrollHeight &&
      scrollToTop
    ) {
      scrollToTop();
    }
  };

  useEffect(() => {
    const handleResize = () => {
      const currentHeight = window.innerHeight;
      if (currentHeight) {
        if (divRef.current) {
          divRef.current.style.height = `${currentHeight - fullIndent}px`;
        }
      }
    };

    handleResize();

    window.addEventListener('resize', handleResize);

    return () => {
      window.removeEventListener('resize', handleResize);
    };
  }, []);

  return (
    <Box ref={divRef} onScroll={handleScroll} sx={{ overflow: 'auto' }}>
      {children}
    </Box>
  );
};
