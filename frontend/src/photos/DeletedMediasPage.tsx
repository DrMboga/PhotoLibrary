// @flow
import * as React from 'react';
import { useEffect, useState } from 'react';
import { MediaInfo } from '../model/media-info';
import { useAppSelector } from '../storeHooks';
import { selectToken } from '../authentication/authSlice';
import { backendAPI } from '../api/BackendApi';
import {
  Alert,
  Box,
  Card,
  CardContent,
  CardHeader,
  CardMedia,
  IconButton,
  Tooltip,
  Typography,
} from '@mui/material';
import CircularProgress from '@mui/material/CircularProgress';
import RestoreFromTrashIcon from '@mui/icons-material/RestoreFromTrash';
import { blobToImage } from '../helpers/blob-image.helper';
import { dateFromUnixTime } from '../helpers/date-helper';

export function DeletedMediasPage() {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | undefined>(undefined);
  const [photos, setPhotos] = useState<MediaInfo[]>([]);
  const [mediaToRestore, setMediaToRestore] = useState<string | undefined>(undefined);

  const authToken = useAppSelector(selectToken);

  useEffect(() => {
    setIsLoading(true);
    backendAPI
      .getDeletedMedias(authToken)
      .then((media) => {
        setIsLoading(false);
        setError(undefined);
        setPhotos(media.sort((a, b) => a.dateTimeOriginal - b.dateTimeOriginal));
      })
      .catch((err) => {
        setIsLoading(false);
        setPhotos([]);
        setError(err.message);
      });
  }, [authToken]);

  useEffect(() => {
    if (!mediaToRestore) {
      return;
    }
    setIsLoading(true);
    backendAPI
      .restoreDeletedMedia(mediaToRestore, authToken)
      .then(() => {
        setIsLoading(false);
        setPhotos(photos.filter((m) => m.id !== mediaToRestore));
        setMediaToRestore(undefined);
        setError(undefined);
      })
      .catch((err) => {
        setIsLoading(false);
        setError(err.message);
      });
  }, [mediaToRestore, authToken]);

  return (
    <Box
      sx={{
        flexGrow: 1,
        display: 'flex',
        flexWrap: 'wrap',
        gap: '10px',
        rowGap: '10px',
        justifyContent: 'space-evenly',
        alignItems: 'center',
      }}
    >
      {isLoading && <CircularProgress />}
      {error && <Alert severity="error">{error}</Alert>}
      {photos.length > 0 &&
        photos.map((photo) => (
          <Card key={`card-${photo.id}`}>
            <CardHeader
              subheader={photo.fileName}
              action={
                <Tooltip title="Restore media">
                  <IconButton aria-label="settings" onClick={() => setMediaToRestore(photo.id)}>
                    <RestoreFromTrashIcon />{' '}
                  </IconButton>
                </Tooltip>
              }
            />
            <Box sx={{ position: 'relative' }}>
              <CardMedia
                key={`card-media-${photo.id}`}
                component="img"
                width={photo.thumbnailWidth}
                height={photo.thumbnailHeight}
                image={blobToImage(photo.thumbnail)}
              ></CardMedia>
            </Box>
            <CardContent
              id={`card-content-1-${photo.id}`}
              sx={{
                padding: '2px',
                '&:last-child': {
                  paddingBottom: 0,
                },
              }}
            >
              <Typography id={`typography-1-${photo.id}`} variant="body2" align="right">
                {dateFromUnixTime(photo.dateTimeOriginal).toLocaleString('ru-RU')}
              </Typography>
            </CardContent>
          </Card>
        ))}
    </Box>
  );
}
