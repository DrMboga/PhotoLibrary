// @flow
import * as React from 'react';
import { useAppDispatch, useAppSelector } from '../../storeHooks';
import { selectToken } from '../../authentication/authSlice';
import {
  getGeocodingStatus,
  getImporterLogs,
  getImporterStatus,
  importerStarted,
  selectEmptyAddressesCount,
  selectFilledAddressesCount,
  selectImporterError,
  selectImporterLoading,
  selectImporterSteps,
  selectIsImporterInProgress,
  selectReporterProgress,
  triggerGeocodingCollect,
} from '../Importer/importerSlice';
import { useEffect, useState } from 'react';
import {
  Alert,
  Box,
  Button,
  FormControl,
  InputLabel,
  LinearProgress,
  OutlinedInput,
  Typography,
} from '@mui/material';
import CircularProgress from '@mui/material/CircularProgress';
import AddPhotoAlternateIcon from '@mui/icons-material/AddPhotoAlternate';
import { ReportLogsTableComponent } from '../ReportLogsTableComponent';
import { useGeocodingSignalRHub } from './useGeocodingSignalRHub';

const requestLimitInputName = 'request-limit-input';

export function GeocodingSettingsPage() {
  const dispatch = useAppDispatch();
  const authToken = useAppSelector(selectToken);
  const loading = useAppSelector(selectImporterLoading);
  const error = useAppSelector(selectImporterError);
  const isImportInProgress = useAppSelector(selectIsImporterInProgress);
  const importerSteps = useAppSelector(selectImporterSteps);
  const importerProgress = useAppSelector(selectReporterProgress);
  const [requestLimit, setRequestLimit] = useState<number>(15);
  const emptyAddressesCount = useAppSelector(selectEmptyAddressesCount);
  const filledAddressesCount = useAppSelector(selectFilledAddressesCount);

  useGeocodingSignalRHub();

  useEffect(() => {
    dispatch(getImporterStatus(authToken));
    dispatch(getGeocodingStatus(authToken));
  }, [dispatch, authToken]);

  useEffect(() => {
    dispatch(getImporterLogs(authToken));
  }, [isImportInProgress, authToken]);

  const startCollectGeocodingData = () => {
    dispatch(triggerGeocodingCollect({ requestLimit: requestLimit, authToken }));
    dispatch(importerStarted());
  };

  const handleInputChange = (event: any) => {
    const { name, value } = event.target;
    if (name === requestLimitInputName && !isNaN(value)) {
      setRequestLimit(+value);
    }
  };

  return (
    <Box>
      {loading && <CircularProgress />}
      {error && <Alert severity="error">{error}</Alert>}
      {!loading && !error && (
        <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
          <Box sx={{ display: 'flex', gap: '20px' }}>
            <Typography variant="body2">
              Empty addresses count: {emptyAddressesCount} | Filled addresses count:{' '}
              {filledAddressesCount}
            </Typography>
            <FormControl
              sx={{ m: 1, width: '25ch' }}
              variant="outlined"
              disabled={isImportInProgress}
            >
              <InputLabel htmlFor="outlined-adornment-email">Requests limit</InputLabel>
              <OutlinedInput
                id="outlined-adornment-email"
                type="text"
                name={requestLimitInputName}
                label="Email"
                onChange={handleInputChange}
                value={requestLimit}
              />
            </FormControl>
            <Button
              variant="outlined"
              startIcon={<AddPhotoAlternateIcon />}
              disabled={isImportInProgress}
              onClick={startCollectGeocodingData}
            >
              Start collect geocoding data
            </Button>
          </Box>
          <LinearProgress
            variant="determinate"
            value={importerProgress}
            sx={{ margin: '1em', width: '100%' }}
          />
          <ReportLogsTableComponent importerSteps={importerSteps}></ReportLogsTableComponent>
        </Box>
      )}
    </Box>
  );
}
