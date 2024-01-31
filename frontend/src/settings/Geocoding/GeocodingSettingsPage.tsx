// @flow
import * as React from 'react';
import { useAppDispatch, useAppSelector } from '../../storeHooks';
import { selectToken } from '../../authentication/authSlice';
import {
  getImporterLogs,
  getImporterStatus,
  selectImporterError,
  selectImporterLoading,
  selectImporterSteps,
  selectIsImporterInProgress,
} from '../Importer/importerSlice';
import { useEffect } from 'react';
import { Box } from '@mui/material';

export function GeocodingSettingsPage() {
  const dispatch = useAppDispatch();
  const authToken = useAppSelector(selectToken);
  const loading = useAppSelector(selectImporterLoading);
  const error = useAppSelector(selectImporterError);
  const isImportInProgress = useAppSelector(selectIsImporterInProgress);
  const importerSteps = useAppSelector(selectImporterSteps);

  useEffect(() => {
    dispatch(getImporterStatus(authToken));
  }, [dispatch, authToken]);

  useEffect(() => {
    dispatch(getImporterLogs(authToken));
  }, [isImportInProgress, authToken]);

  return <Box></Box>;
}
