// @flow
import * as React from 'react';
import { useAppDispatch, useAppSelector } from '../../storeHooks';
import {
  getImporterStatus,
  selectImporterError,
  selectImporterLoading,
  selectIsImporterInProgress,
} from './importerSlice';
import CircularProgress from '@mui/material/CircularProgress';
import { Alert, Box } from '@mui/material';
import { useEffect } from 'react';
import { selectToken } from '../../keycloak-auth/authSlice';

export function ImporterPage() {
  const dispatch = useAppDispatch();
  const authToken = useAppSelector(selectToken);
  const loading = useAppSelector(selectImporterLoading);
  const error = useAppSelector(selectImporterError);
  const isImportInProgress = useAppSelector(selectIsImporterInProgress);

  useEffect(() => {
    dispatch(getImporterStatus(authToken));
  }, [dispatch, authToken]);

  return (
    <Box>
      {loading && <CircularProgress />}
      {error && <Alert severity="error">{error}</Alert>}
      {!loading && !loading && <p>{isImportInProgress ? 'true' : 'false'}</p>}
    </Box>
  );
}
