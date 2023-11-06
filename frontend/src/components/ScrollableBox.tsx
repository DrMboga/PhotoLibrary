import * as React from 'react';
import { ReactElement, useEffect, useRef, useState } from 'react';
import { Box } from '@mui/material';

type Props = {
  indent: number;
  children: ReactElement;
};

const appBarHeight = 80;

export const ScrollableBox = ({ children, indent }: Props) => {
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

    handleResize();
    window.addEventListener('resize', handleResize);

    return () => {
      window.removeEventListener('resize', handleResize);
    };
  }, []);

  return (
    <Box ref={divRef} sx={{ overflow: 'auto' }}>
      {children}
    </Box>
  );
};
