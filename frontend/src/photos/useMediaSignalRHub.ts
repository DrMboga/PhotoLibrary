import { useEffect, useRef, useState } from 'react';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { MediaInfo } from '../model/media-info';

const backendUrl = process.env.REACT_APP_BACKEND_URL;
const mediaHubPath = `${backendUrl}/Media`;
const defaultDateFrom = 9999999999; //Sat Nov 20 2286 17:46:39
const maxSizeOfPhotosOnAPage = 30;

export const useMediaSignalRHub = (dateOfLastPhoto: number | undefined) => {
  const connectCalledOnce = useRef(false);
  const [connection, setConnection] = useState<HubConnection>();
  // https://react.dev/learn/updating-arrays-in-state
  const [photos, setPhotos] = useState<MediaInfo[]>([]);

  const getNextPhotosChunkFromBackend = async (dateFrom: number, connection: HubConnection) => {
    if (connection.state === HubConnectionState.Connected) {
      await connection.send('GetNextPhotosChunk', dateFrom);
      console.log('getNextPhotosChunkFromBackend', dateFrom);
    }
  };

  const getPreviousPhotosChunkFromBackend = async (dateTo: number, connection: HubConnection) => {
    if (connection.state === HubConnectionState.Connected) {
      await connection.send('GetPreviousPhotosChunk', dateTo);
      console.log('getPreviousPhotosChunkFromBackend', dateTo);
    }
  };

  const handleNextPhotoPushed = (media: MediaInfo) => {
    console.log('GetNextPhotosChunk -> media received', media.fileName, media.dateTimeOriginal);

    let updatedPhotos = [...photos, media];
    if (updatedPhotos.length > maxSizeOfPhotosOnAPage) {
      // Remove first ChunkSize elements
      updatedPhotos = updatedPhotos.slice(maxSizeOfPhotosOnAPage);
    }
    if (updatedPhotos.length > 0) {
      // TODO: Dispatch
      // state.dateOfFirstPhoto = photosArray[0].dateTimeOriginal;
      // state.dateOfLastPhoto = photosArray[photosArray.length - 1].dateTimeOriginal;
    }
    setPhotos(updatedPhotos);
  };

  const handlePreviousPhotoPushed = (media: MediaInfo) => {
    console.log('GetPreviousPhotosChunk -> media received', media.fileName, media.dateTimeOriginal);
    // TODO: Find the place of the media in the photos array and insert it, then pull the last element if needed
    let updatedPhotos = [...photos];
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
          // Subscribe to receive messages from SignalR
          hubConnection.on('GetNextPhotosChunk', handleNextPhotoPushed);
          hubConnection.on('GetPreviousPhotosChunk', handlePreviousPhotoPushed);

          // Initial call to SignalR to get first chunk of photos
          getNextPhotosChunkFromBackend(dateOfLastPhoto ?? defaultDateFrom, hubConnection).catch(
            (err) => console.error(err),
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

  return { connection, getNextPhotosChunkFromBackend, getPreviousPhotosChunkFromBackend, photos };
};
