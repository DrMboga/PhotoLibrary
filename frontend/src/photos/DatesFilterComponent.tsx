// @flow
import * as React from 'react';
import { Chip, Stack } from '@mui/material';

type Props = {
  currentDate?: number;
  newDateSelected: (newDate: number) => void;
};

export function DatesFilterComponent({ currentDate, newDateSelected }: Props) {
  const handleChipClick = (year: number) => {
    console.log('Chip selected', year);
  };
  return (
    <Stack direction="row" spacing={1} sx={{ marginTop: '10px', marginBottom: '10px' }}>
      <Chip label="2003" size="small" onClick={() => handleChipClick(2003)} />
      <Chip label="2004" size="small" variant="outlined" onClick={() => handleChipClick(2004)} />
    </Stack>
  );
}
