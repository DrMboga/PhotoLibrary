// @flow
import * as React from 'react';
import { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '../../storeHooks';
import {
  getImporterLogs,
  getImporterStatus,
  importerStarted,
  selectImporterError,
  selectImporterLoading,
  selectImporterSteps,
  selectIsImporterInProgress,
  triggerMediaImport,
} from './importerSlice';
import CircularProgress from '@mui/material/CircularProgress';
import { Alert, Box, Button } from '@mui/material';
import { selectToken } from '../../authentication/authSlice';
import AddPhotoAlternateIcon from '@mui/icons-material/AddPhotoAlternate';
import { useReporterSignalRHub } from './useReporterSignalRHub';
import { ReportLogsTableComponent } from '../ReportLogsTableComponent';

export function ImporterPage() {
  const dispatch = useAppDispatch();
  const authToken = useAppSelector(selectToken);
  const loading = useAppSelector(selectImporterLoading);
  const error = useAppSelector(selectImporterError);
  const isImportInProgress = useAppSelector(selectIsImporterInProgress);
  const importerSteps = useAppSelector(selectImporterSteps);

  useReporterSignalRHub();

  useEffect(() => {
    dispatch(getImporterStatus(authToken));
  }, [dispatch, authToken]);

  useEffect(() => {
    dispatch(getImporterLogs(authToken));
  }, [isImportInProgress, authToken]);

  const startImport = () => {
    dispatch(triggerMediaImport(authToken));
    dispatch(importerStarted());
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
              onClick={startImport}
            >
              Start import
            </Button>
          </Box>
          <ReportLogsTableComponent importerSteps={importerSteps}></ReportLogsTableComponent>
        </Box>
      )}
    </Box>
  );
}
