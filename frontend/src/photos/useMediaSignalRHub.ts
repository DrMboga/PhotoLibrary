import { useEffect, useRef, useState } from 'react';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';

const backendUrl = process.env.REACT_APP_BACKEND_URL;
const mediaHubPath = `${backendUrl}/Media`;

export const useMediaSignalRHub = (dateOfLastPhoto: number | undefined) => {
  const connectCalledOnce = useRef(false);
  const [connection, setConnection] = useState<HubConnection>();

  const getNextPhotosChunkFromBackend = async (dateFrom: number, connection: HubConnection) => {
    if (connection.state === HubConnectionState.Connected) {
      await connection.send('GetNextPhotosChunk', dateFrom);
      console.log('getNextPhotosChunkFromBackend', dateFrom);
    }
  };

  useEffect(() => {
    if (!connectCalledOnce.current) {
      connectCalledOnce.current = true;
      const hubConnection = new HubConnectionBuilder()
        .withUrl(mediaHubPath)
        .withAutomaticReconnect()
        .build();
      hubConnection
        .start()
        .then(() => {
          console.log('Connected to media hub');
          setConnection(hubConnection);
          getNextPhotosChunkFromBackend(dateOfLastPhoto ?? 0, hubConnection).catch((err) =>
            console.error(err),
          );
        })
        .catch((err) => console.error(err));
    }

    // clean up:
    return () => {
      if (connection?.state === HubConnectionState.Connected) {
        connection
          .stop()
          .then(() => console.log('Disconnected from the hub'))
          .catch((err) => console.error(err));
      }
    };
  }, []);

  return { connection, getNextPhotosChunkFromBackend };
};
