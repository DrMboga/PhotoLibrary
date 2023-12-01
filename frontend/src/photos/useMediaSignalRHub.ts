import { useEffect, useRef, useState } from 'react';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { MediaInfo } from '../model/media-info';
import { useAppDispatch } from '../storeHooks';
import { changeDateOfFirstPhoto, changeDateOfLastPhoto } from './photosSlice';

const backendUrl = process.env.REACT_APP_BACKEND_URL;
const mediaHubPath = `${backendUrl}/Media`;
const defaultDateFrom = 9999999999; //Sat Nov 20 2286 17:46:39
const maxSizeOfPhotosOnAPage = 30;

export const useMediaSignalRHub = (dateOfLastPhoto: number | undefined) => {
  const connectCalledOnce = useRef(false);
  const [connection, setConnection] = useState<HubConnection>();
  // https://react.dev/learn/updating-arrays-in-state
  const [photos, setPhotos] = useState<MediaInfo[]>([]);

  const [newBottomMedias, setNewBottomMedias] = useState<MediaInfo[]>();
  const [newUpperMedias, setNewUpperMedias] = useState<MediaInfo[]>();

  const dispatch = useAppDispatch();

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

  const handleNextPhotoPushed = (medias: MediaInfo[]) => {
    console.log('GetNextPhotosChunk -> media received', medias.length);
    if (medias) {
      setNewBottomMedias(medias);
    }
  };

  const handlePreviousPhotoPushed = (medias: MediaInfo[]) => {
    console.log('GetPreviousPhotosChunk -> media received', medias.length);
    if (medias) {
      setNewUpperMedias(medias);
    }
  };

  const mergeTwoPhotosArrays = (
    first: MediaInfo[],
    second: MediaInfo[],
    removeFromTop: boolean,
  ): MediaInfo[] => {
    let updatedPhotos = [
      ...first.filter((m) => !second.some((sm) => sm.fileName === m.fileName)),
      ...second,
    ];
    if (updatedPhotos.length > maxSizeOfPhotosOnAPage) {
      updatedPhotos = updatedPhotos.slice(
        removeFromTop ? 0 : maxSizeOfPhotosOnAPage,
        removeFromTop
          ? updatedPhotos.length - maxSizeOfPhotosOnAPage - 1
          : updatedPhotos.length - 1,
      );
    }
    if (updatedPhotos.length > 0) {
      dispatch(changeDateOfFirstPhoto(updatedPhotos[0].dateTimeOriginal));
      dispatch(changeDateOfLastPhoto(updatedPhotos[updatedPhotos.length - 1].dateTimeOriginal));
    }
    return updatedPhotos;
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

  // Media files added on scroll to bottom event
  useEffect(() => {
    if (!newBottomMedias) {
      return;
    }

    setPhotos(mergeTwoPhotosArrays(photos, newBottomMedias, false));
  }, [newBottomMedias]);

  // Media files added on scroll to top event
  useEffect(() => {
    if (!newUpperMedias) {
      return;
    }
    setPhotos(mergeTwoPhotosArrays(newUpperMedias, photos, true));
  }, [newUpperMedias]);

  return { connection, getNextPhotosChunkFromBackend, getPreviousPhotosChunkFromBackend, photos };
};
