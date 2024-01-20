// @flow
import * as React from 'react';
import { useEffect, useState } from 'react';
import { favoriteAlbumName, importantAlbumName, printAlbumName } from '../helpers/album-helper';
import { Alert, Box, Button, ButtonGroup } from '@mui/material';
import FavoriteIcon from '@mui/icons-material/Favorite';
import FavoriteBorderIcon from '@mui/icons-material/FavoriteBorder';
import StarIcon from '@mui/icons-material/Star';
import StarOutlineIcon from '@mui/icons-material/StarOutline';
import LocalPrintshopIcon from '@mui/icons-material/LocalPrintshop';
import LocalPrintshopOutlinedIcon from '@mui/icons-material/LocalPrintshopOutlined';
import { useAppSelector } from '../storeHooks';
import { selectToken } from '../authentication/authSlice';
import { MediaInfo } from '../model/media-info';
import CircularProgress from '@mui/material/CircularProgress';
import { backendAPI } from '../api/BackendApi';
import { MediaCard } from './MediaCard';
import { MediaPreview } from './MediaPreview';

export function AlbumsPage() {
  const [selectedAlbum, setSelectedAlbum] = useState(favoriteAlbumName);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | undefined>(undefined);
  const [photos, setPhotos] = useState<MediaInfo[]>([]);
  const [selectedMedia, setSelectedMedia] = useState<MediaInfo>();
  const [dialogOpen, setDialogOpen] = useState(false);

  const authToken = useAppSelector(selectToken);
  useEffect(() => {
    setIsLoading(true);
    backendAPI
      .getMediasByAlbum(
        selectedAlbum === favoriteAlbumName,
        selectedAlbum === importantAlbumName,
        selectedAlbum === printAlbumName,
        authToken,
      )
      .then((media) => {
        setIsLoading(false);
        setError(undefined);
        setPhotos(media);
      })
      .catch((err) => {
        setIsLoading(false);
        setPhotos([]);
        setError(err);
      });
  }, [selectedAlbum]);

  const albumChanged = (album: string) => {
    setSelectedAlbum(album);
  };

  const handleCardClick = (media: MediaInfo) => {
    setSelectedMedia(media);
    setDialogOpen(true);
  };

  const handleDialogClose = () => {
    setDialogOpen(false);
    setSelectedMedia(undefined);
  };

  const handleDeleteCard = (mediaIdToDelete: string) => {
    setPhotos(photos.filter((m) => m.id !== mediaIdToDelete));
    handleDialogClose();
  };

  const handleAlbumMarkChanged = (mediaId: string, album: string) => {
    // Remove card from list if media does not belong to current album
    if (!album.includes(selectedAlbum)) {
      setPhotos(photos.filter((m) => m.id !== mediaId));
      handleDialogClose();
    }
  };

  return (
    <>
      <Box sx={{ display: 'flex', flexDirection: 'column' }}>
        <Box sx={{ marginTop: '10px', marginBottom: '10px' }}>
          <ButtonGroup aria-label="Album label">
            <Button
              key={favoriteAlbumName}
              aria-label="Favorites"
              size="small"
              onClick={() => albumChanged(favoriteAlbumName)}
              variant={selectedAlbum === favoriteAlbumName ? 'contained' : 'outlined'}
              startIcon={
                selectedAlbum === favoriteAlbumName ? <FavoriteBorderIcon /> : <FavoriteIcon />
              }
            >
              Favorites
            </Button>
            <Button
              key={importantAlbumName}
              aria-label="Important"
              size="small"
              onClick={() => albumChanged(importantAlbumName)}
              variant={selectedAlbum === importantAlbumName ? 'contained' : 'outlined'}
              startIcon={selectedAlbum === importantAlbumName ? <StarOutlineIcon /> : <StarIcon />}
            >
              Important
            </Button>
            <Button
              key={printAlbumName}
              aria-label="Important"
              size="small"
              onClick={() => albumChanged(printAlbumName)}
              variant={selectedAlbum === printAlbumName ? 'contained' : 'outlined'}
              startIcon={
                selectedAlbum === printAlbumName ? (
                  <LocalPrintshopOutlinedIcon />
                ) : (
                  <LocalPrintshopIcon />
                )
              }
            >
              For print
            </Button>
          </ButtonGroup>
        </Box>
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
              <MediaCard
                key={`media-card-id-${photo.id}`}
                media={photo}
                onClick={handleCardClick}
              />
            ))}
        </Box>
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
