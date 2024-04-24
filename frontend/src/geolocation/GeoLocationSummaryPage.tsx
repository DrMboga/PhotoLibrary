// @flow
import * as React from 'react';
import { useEffect, useState } from 'react';
import { useAppSelector } from '../storeHooks';
import { selectToken } from '../authentication/authSlice';
import { MediaGeoLocationRegionsInfo } from '../model/media-geo-location-regions-info';
import { backendAPI } from '../api/BackendApi';
import { Alert, Box } from '@mui/material';
import CircularProgress from '@mui/material/CircularProgress';
import { GeoLocationSummaryCard } from './GeoLocationSummaryCard';
import { useNavigate } from 'react-router-dom';

export function GeoLocationSummaryPage() {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | undefined>(undefined);
  const [places, setPlaces] = useState<MediaGeoLocationRegionsInfo[]>([]);

  const authToken = useAppSelector(selectToken);

  const navigate = useNavigate();

  useEffect(() => {
    setIsLoading(true);
    backendAPI
      .getGeoLocationRegionsInfo(authToken)
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
    navigate(`/region/${region}`);
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
