// @flow
import * as React from 'react';
import { useEffect, useState } from 'react';
import { MediaInfo } from '../model/media-info';
import { useAppSelector } from '../storeHooks';
import { selectToken } from '../authentication/authSlice';
import { Alert, Box } from '@mui/material';
import CircularProgress from '@mui/material/CircularProgress';
import { MediaCard } from '../photos/MediaCard';
import { MediaPreview } from '../photos/MediaPreview';
import { backendAPI } from '../api/BackendApi';

type Props = {
  region?: string;
  monthly?: { year: number; month: number };
};

export function GeoLocationRegionPhotos({ region, monthly }: Readonly<Props>) {
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | undefined>(undefined);
  const [photos, setPhotos] = useState<MediaInfo[]>([]);
  const [selectedMedia, setSelectedMedia] = useState<MediaInfo>();
  const [dialogOpen, setDialogOpen] = useState(false);

  const authToken = useAppSelector(selectToken);

  useEffect(() => {
    if (!region || !monthly) {
      return;
    }
    setIsLoading(true);
    backendAPI
      .getGeoLocationRegionMedias(region, monthly.year, monthly.month, authToken)
      .then((media) => {
        setIsLoading(false);
        setError(undefined);
        setPhotos(media);
      })
      .catch((err) => {
        setIsLoading(false);
        setPhotos([]);
        setError(err.message);
      });
  }, [region, monthly, authToken]);

  const handleCardClick = (media: MediaInfo) => {
    setSelectedMedia(media);
    setDialogOpen(true);
  };

  const handleDialogClose = () => {
    setDialogOpen(false);
    setSelectedMedia(undefined);
  };

  const handleDeleteCard = (mediaIdToDelete: string) => {
    const photosCopy = [...photos.filter((m) => m.id !== mediaIdToDelete)];
    setPhotos(photosCopy);
    handleDialogClose();
  };

  const handleAlbumMarkChanged = (mediaId: string, album: string) => {
    const photosCopy = [...photos];
    const changedPhoto = photosCopy.find((p) => p.id === mediaId);
    if (changedPhoto) {
      changedPhoto.albumName = album;
      changedPhoto.isFavorite = album.includes('Fav');
    }
    setPhotos(photosCopy);
  };

  return (
    <>
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
        {!isLoading &&
          !error &&
          photos.length > 0 &&
          photos.map((photo) => (
            <MediaCard key={`media-card-id-${photo.id}`} media={photo} onClick={handleCardClick} />
          ))}
      </Box>

      <MediaPreview
        media={selectedMedia}
        open={dialogOpen}
        onClose={handleDialogClose}
        deleteCardFromList={handleDeleteCard}
        albumMarkChanged={handleAlbumMarkChanged}
      />
    </>
  );
}
