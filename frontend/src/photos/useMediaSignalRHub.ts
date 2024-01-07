import { useEffect, useRef, useState } from 'react';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { MediaInfo } from '../model/media-info';
import { useAppDispatch, useAppSelector } from '../storeHooks';
import { changeDateOfFirstPhoto, changeDateOfLastPhoto, errorOccurred } from './photosSlice';
import { selectToken } from '../keycloak-auth/authSlice';

const backendUrl = process.env.REACT_APP_BACKEND_URL;
const mediaHubPath = `${backendUrl}/Media`;
const defaultDateFrom = 9999999999; //Sat Nov 20 2286 17:46:39
const maxSizeOfPhotosOnAPage = 50;

export const useMediaSignalRHub = (dateOfLastPhoto: number | undefined) => {
  const connectCalledOnce = useRef(false);
  const [connection, setConnection] = useState<HubConnection>();
  // https://react.dev/learn/updating-arrays-in-state
  const [photos, setPhotos] = useState<MediaInfo[]>([]);

  const [newBottomMedias, setNewBottomMedias] = useState<MediaInfo[]>();
  const [newUpperMedias, setNewUpperMedias] = useState<MediaInfo[]>();

  const authToken = useAppSelector(selectToken);

  const dispatch = useAppDispatch();

  const setError = (err: Error) => {
    console.error(err);
    dispatch(errorOccurred(err.message));
  };

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
    middleDate: number,
  ): MediaInfo[] => {
    let updatedPhotos = [
      ...first.filter((m) => !second.some((sm) => sm.fileName === m.fileName)),
      ...second,
    ];
    if (updatedPhotos.length > maxSizeOfPhotosOnAPage) {
      const mediaWithMiddleDate = updatedPhotos.find((m) => m.dateTimeOriginal === middleDate);
      const middleDateIndex = updatedPhotos.indexOf(mediaWithMiddleDate!);
      // Cut top of the medias (on scroll to bottom)
      if (middleDateIndex - maxSizeOfPhotosOnAPage > 0) {
        updatedPhotos = updatedPhotos.slice(
          middleDateIndex - maxSizeOfPhotosOnAPage,
          updatedPhotos.length - 1,
        );
      }
      // Cut bottom of the medias (on scroll to top)
      if (middleDateIndex + maxSizeOfPhotosOnAPage > updatedPhotos.length) {
        updatedPhotos = updatedPhotos.slice(0, middleDateIndex + maxSizeOfPhotosOnAPage);
      }
    }
    if (updatedPhotos.length > 0) {
      dispatch(changeDateOfFirstPhoto(updatedPhotos[0].dateTimeOriginal));
      dispatch(changeDateOfLastPhoto(updatedPhotos[updatedPhotos.length - 1].dateTimeOriginal));
    }
    return updatedPhotos;
  };

  const cleanPhotos = () => {
    setPhotos([]);
  };

  useEffect(() => {
    if (!connectCalledOnce.current) {
      connectCalledOnce.current = true;
      console.log('hub connection is about to connect');
      const hubConnection = new HubConnectionBuilder()
        .withUrl(mediaHubPath, { accessTokenFactory: () => authToken ?? '' })
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
            (err) => setError(err),
          );
        })
        .catch((err) => setError(err));
    }

    // clean up:
    return () => {
      if (connection?.state === HubConnectionState.Connected) {
        connectCalledOnce.current = false;
        console.log('hub connection is about to disconnect');
        connection
          .stop()
          .then(() => {
            console.log('Disconnected from the hub');
            setConnection(connection);
          })
          .catch((err) => setError(err));
      }
    };
  }, [authToken]);

  // Media files added on scroll to bottom event
  useEffect(() => {
    if (!newBottomMedias || newBottomMedias.length === 0) {
      return;
    }

    setPhotos(mergeTwoPhotosArrays(photos, newBottomMedias, newBottomMedias[0].dateTimeOriginal));
  }, [newBottomMedias]);

  // Media files added on scroll to top event
  useEffect(() => {
    if (!newUpperMedias || newUpperMedias.length === 0) {
      return;
    }
    setPhotos(
      mergeTwoPhotosArrays(
        newUpperMedias,
        photos,
        newUpperMedias[newUpperMedias.length - 1].dateTimeOriginal,
      ),
    );
  }, [newUpperMedias]);

  return {
    connection,
    getNextPhotosChunkFromBackend,
    getPreviousPhotosChunkFromBackend,
    photos,
    cleanPhotos,
  };
};
