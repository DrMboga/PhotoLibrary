// @flow
import * as React from 'react';
import { useAppDispatch, useAppSelector } from '../../storeHooks';
import {
  getImporterLogs,
  getImporterStatus,
  selectImporterError,
  selectImporterLoading,
  selectImporterSteps,
  selectIsImporterInProgress,
} from './importerSlice';
import CircularProgress from '@mui/material/CircularProgress';
import { Alert, Box, Button } from '@mui/material';
import { useEffect } from 'react';
import { selectToken } from '../../keycloak-auth/authSlice';
import AddPhotoAlternateIcon from '@mui/icons-material/AddPhotoAlternate';

export function ImporterPage() {
  const dispatch = useAppDispatch();
  const authToken = useAppSelector(selectToken);
  const loading = useAppSelector(selectImporterLoading);
  const error = useAppSelector(selectImporterError);
  // TODO: if true, disable "Report" button
  const isImportInProgress = useAppSelector(selectIsImporterInProgress);
  const importerSteps = useAppSelector(selectImporterSteps);

  useEffect(() => {
    dispatch(getImporterStatus(authToken));
  }, [dispatch, authToken]);

  useEffect(() => {
    dispatch(getImporterLogs(authToken));
    if (isImportInProgress) {
      // TODO: Start to listen SignalR
    }
  }, [isImportInProgress, authToken]);

  const startImport = () => {
    // TODO: Call '/triggerMediaImport' post method, then getImporterStatus
  };

  return (
    <Box>
      {loading && <CircularProgress />}
      {error && <Alert severity="error">{error}</Alert>}
      {!loading && !error && (
        <Box sx={{ display: 'flex', flexDirection: 'column', alignItems: 'center' }}>
          <Box sx={{ display: 'flex', gap: '20px' }}>
            {isImportInProgress && <CircularProgress />}
            <Button
              variant="outlined"
              startIcon={<AddPhotoAlternateIcon />}
              disabled={isImportInProgress}
            >
              Start import
            </Button>
          </Box>
          <p>{JSON.stringify(importerSteps)}</p>
        </Box>
      )}
    </Box>
  );
}
