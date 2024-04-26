// @flow
import * as React from 'react';
import { Box } from '@mui/material';

type Props = {
  latitude: number;
  longitude: number;
  boxHeight: number;
  boxWidth: number;
};

export function MapView({ latitude, longitude, boxHeight, boxWidth }: Props) {
  return (
    <Box sx={{ width: boxWidth, height: boxHeight }}>
      {latitude}, {longitude}
    </Box>
  );
}
