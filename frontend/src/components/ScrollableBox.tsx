import * as React from 'react';
import { ReactElement, useEffect, useRef } from 'react';
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

    const handleScroll = () => {
      if (!divRef.current) {
        return;
      }
      if (
        divRef.current?.scrollTop + divRef.current.offsetHeight === divRef.current?.scrollHeight &&
        scrollToBottom
      ) {
        scrollToBottom();
      }

      if (divRef.current?.scrollTop === 0 && scrollToTop) {
        scrollToTop();
      }
    };

    window.addEventListener('resize', handleResize);
    divRef.current?.addEventListener('scroll', handleScroll);

    return () => {
      window.removeEventListener('resize', handleResize);
      divRef.current?.removeEventListener('scroll', handleScroll);
    };
  }, []);

  return (
    <Box ref={divRef} sx={{ overflow: 'auto' }}>
      {children}
    </Box>
  );
};
