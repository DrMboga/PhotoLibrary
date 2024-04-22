// @flow
import * as React from 'react';
import { useEffect, useState } from 'react';
import { useAppSelector } from '../storeHooks';
import { selectToken } from '../authentication/authSlice';
import { MediaGeoLocationSummary } from '../model/media-geo-location-summary';
import { backendAPI } from '../api/BackendApi';
import { Alert, Box } from '@mui/material';
import CircularProgress from '@mui/material/CircularProgress';
import { GeoLocationSummaryCard } from './GeoLocationSummaryCard';

export function GeoLocationSummaryPage() {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | undefined>(undefined);
  const [places, setPlaces] = useState<MediaGeoLocationSummary[]>([]);

  const authToken = useAppSelector(selectToken);

  useEffect(() => {
    setIsLoading(true);
    backendAPI
      .getGeoLocationSummary(authToken)
      .then((placesInfo) => {
        setIsLoading(false);
        setError(undefined);
        setPlaces(placesInfo);
      })
      .catch((err) => {
        setIsLoading(false);
        setPlaces([]);
        setError(err.message);
      });
  }, [authToken]);

  const handlePlaceClick = (region: string) => {
    console.log(`Region ${region} clicked`);
  };

  return (
    <Box
      sx={{
        marginTop: '10px',
        marginBottom: '10px',
        flexGrow: 1,
        display: 'flex',
        flexWrap: 'wrap',
        gap: '10px',
        rowGap: '10px',
        justifyContent: 'space-evenly',
        alignItems: 'center',
      }}
    >
      {isLoading && <CircularProgress />}
      {error && <Alert severity="error">{error}</Alert>}
      {!isLoading &&
        !error &&
        places.length > 0 &&
        places.map((place) => (
          <GeoLocationSummaryCard
            key={`geo-summary-card-${place.region}`}
            geolocationSummary={place}
            onClick={handlePlaceClick}
          />
        ))}
    </Box>
  );
}
