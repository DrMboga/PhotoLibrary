// @flow
import * as React from 'react';
import { Alert, Box } from '@mui/material';
import { GoogleMap, Marker, useLoadScript } from '@react-google-maps/api';
import CircularProgress from '@mui/material/CircularProgress';

type Props = {
  latitude: number;
  longitude: number;
  boxHeight: number;
  boxWidth: number;
};

const apiKey = process.env.REACT_APP_GOOGLE_MAPS_API_KEY;

export function MapView({ latitude, longitude, boxHeight, boxWidth }: Props) {
  const libraries: any[] = ['places'];
  const mapContainerStyle = {
    width: boxWidth,
    height: boxHeight,
  };
  const center = {
    lat: latitude,
    lng: longitude,
  };

  const { isLoaded, loadError } = useLoadScript({
    googleMapsApiKey: apiKey!,
    libraries,
  });

  return (
    <Box sx={{ width: boxWidth, height: boxHeight }}>
      {loadError && <Alert severity="error">Something wend wrong loading maps</Alert>}
      {!isLoaded && <CircularProgress />}
      {isLoaded && (
        <GoogleMap mapContainerStyle={mapContainerStyle} zoom={17} center={center}>
          <Marker position={center} />
        </GoogleMap>
      )}
    </Box>
  );
}
