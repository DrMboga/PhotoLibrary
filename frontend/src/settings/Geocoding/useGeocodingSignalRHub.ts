import { useEffect, useRef, useState } from 'react';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { useAppDispatch, useAppSelector } from '../../storeHooks';
import { selectToken } from '../../authentication/authSlice';
import {
  importerFinished,
  importerNewStepAdded,
  importerProgressChanged,
  importerSignalRErrorOccurred,
} from '../Importer/importerSlice';
import { ImportStepReport } from '../../model/media-info';

const backendUrl = process.env.REACT_APP_BACKEND_URL;
const reporterHubPath = `${backendUrl}/GeocodingLogger`;

export const useGeocodingSignalRHub = () => {
  const connectCalledOnce = useRef(false);
  const [connection, setConnection] = useState<HubConnection>();

  const dispatch = useAppDispatch();
  const authToken = useAppSelector(selectToken);

  const setError = (err: Error) => {
    console.error(err);
    dispatch(importerSignalRErrorOccurred(err.message));
  };

  const handleCollectorStep = (newStep: { report: ImportStepReport; progress: number }) => {
    dispatch(importerProgressChanged(newStep.progress));
    dispatch(importerNewStepAdded(newStep.report));
  };

  const handleMediaImportFinished = () => {
    dispatch(importerFinished());
  };

  useEffect(() => {
    if (!connectCalledOnce.current) {
      connectCalledOnce.current = true;
      const hubConnection = new HubConnectionBuilder()
        .withUrl(reporterHubPath, { accessTokenFactory: () => authToken ?? '' })
        .withAutomaticReconnect()
        .build();
      hubConnection
        .start()
        .then(() => {
          console.log('Connected to reporter hub');
          setConnection(hubConnection);
          // Subscribe to receive messages from SignalR
          hubConnection.on('GeocodingCollectorStep', handleCollectorStep);
          hubConnection.on('GeocodingCollectorFinished', handleMediaImportFinished);
        })
        .catch((err) => setError(err));
    }

    // clean up:
    return () => {
      if (connection?.state === HubConnectionState.Connected) {
        connectCalledOnce.current = false;
        console.log('Disconnecting from hub..');
        connection
          .stop()
          .then(() => console.log('Disconnected from the geocoding reporter hub'))
          .catch((err) => setError(err));
      }
    };
  }, [authToken, connection]);
};
