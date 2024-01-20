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
import { Alert, Box, Button, Divider, Typography } from '@mui/material';
import { selectToken } from '../../authentication/authSlice';
import AddPhotoAlternateIcon from '@mui/icons-material/AddPhotoAlternate';
import { dateFromUnixTime } from '../../helpers/date-helper';
import InfoIcon from '@mui/icons-material/Info';
import WarningIcon from '@mui/icons-material/Warning';
import ReportIcon from '@mui/icons-material/Report';
import { ImportStepReportSeverity } from '../../model/media-info';
import { useReporterSignalRHub } from './useReporterSignalRHub';

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

  const getIcon = (status: ImportStepReportSeverity, id: string) => {
    switch (status) {
      case ImportStepReportSeverity.ERROR:
        return <ReportIcon fontSize="small" sx={{ color: 'red' }} id={`icon-status-${id}`} />;
      case ImportStepReportSeverity.WARNING:
        return <WarningIcon fontSize="small" sx={{ color: 'yellow' }} id={`icon-status-${id}`} />;
      case ImportStepReportSeverity.INFORMATION:
        return <InfoIcon fontSize="small" sx={{ color: 'green' }} id={`icon-status-${id}`} />;
    }
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
          <Box sx={{ display: 'flex', flexDirection: 'column', gap: '5px', marginTop: '15px' }}>
            {importerSteps?.map((step) => (
              <Box key={`section-${step.id}`}>
                <Box
                  id={`log-row-${step.id}`}
                  sx={{
                    display: 'flex',
                    gap: '10px',
                    alignItems: 'center',
                    justifyContent: 'center',
                  }}
                >
                  <Typography id={`typography-1-${step.id}`} variant="body2">
                    [ {dateFromUnixTime(step.timestamp).toLocaleString('ru-RU')} ]
                  </Typography>
                  {getIcon(step.severity, step.id)}
                  <Typography
                    id={`typography-2-${step.id}`}
                    variant="body1"
                    align="left"
                    sx={{ width: '80%' }}
                  >
                    {step.stepMessage}
                  </Typography>
                </Box>
                <Divider flexItem variant="middle" id={`divider-${step.id}`} />
              </Box>
            ))}
          </Box>
        </Box>
      )}
    </Box>
  );
}
