import { useEffect, useRef, useState } from 'react';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { useAppDispatch, useAppSelector } from '../../storeHooks';
import { selectToken } from '../../keycloak-auth/authSlice';
import { ImportStepReport } from '../../model/media-info';
import {
  importerFinished,
  importerNewStepAdded,
  importerSignalRErrorOccurred,
} from './importerSlice';

const backendUrl = process.env.REACT_APP_BACKEND_URL;
const reporterHubPath = `${backendUrl}/ImporterLogger`;

export const useReporterSignalRHub = () => {
  const connectCalledOnce = useRef(false);
  const [connection, setConnection] = useState<HubConnection>();

  const dispatch = useAppDispatch();

  const setError = (err: Error) => {
    console.error(err);
    dispatch(importerSignalRErrorOccurred(err.message));
  };

  const handleImporterStep = (newStep: ImportStepReport) => {
    dispatch(importerNewStepAdded(newStep));
  };

  const handleMediaImportFinished = () => {
    dispatch(importerFinished());
  };

  useEffect(() => {
    if (!connectCalledOnce.current) {
      connectCalledOnce.current = true;
      const hubConnection = new HubConnectionBuilder()
        .withUrl(reporterHubPath)
        .withAutomaticReconnect()
        .build();
      hubConnection
        .start()
        .then(() => {
          console.log('Connected to reporter hub');
          setConnection(hubConnection);
          // Subscribe to receive messages from SignalR
          hubConnection.on('ImportStep', handleImporterStep);
          hubConnection.on('MediaImportFinished', handleMediaImportFinished);
        })
        .catch((err) => setError(err));
    }

    // clean up:
    return () => {
      if (connection?.state === HubConnectionState.Connected) {
        connection
          .stop()
          .then(() => console.log('Disconnected from the reporter hub'))
          .catch((err) => setError(err));
      }
    };
  }, []);
};