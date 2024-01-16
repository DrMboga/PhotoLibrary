// @flow
import * as React from 'react';
import { useEffect, useState } from 'react';
import { favoriteAlbumName, importantAlbumName, printAlbumName } from '../helpers/album-helper';
import { Alert, Box, Button, ButtonGroup, IconButton } from '@mui/material';
import FavoriteIcon from '@mui/icons-material/Favorite';
import FavoriteBorderIcon from '@mui/icons-material/FavoriteBorder';
import StarIcon from '@mui/icons-material/Star';
import StarOutlineIcon from '@mui/icons-material/StarOutline';
import LocalPrintshopIcon from '@mui/icons-material/LocalPrintshop';
import LocalPrintshopOutlinedIcon from '@mui/icons-material/LocalPrintshopOutlined';
import { useAppSelector } from '../storeHooks';
import { selectToken } from '../keycloak-auth/authSlice';
import { MediaInfo } from '../model/media-info';
import CircularProgress from '@mui/material/CircularProgress';
import { backendAPI } from '../api/BackendApi';
import { MediaCard } from './MediaCard';

export function AlbumsPage() {
  const [selectedAlbum, setSelectedAlbum] = useState(favoriteAlbumName);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | undefined>(undefined);
  const [photos, setPhotos] = useState<MediaInfo[]>([]);

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
    // TODO: Dialog
    // setSelectedMedia(media);
    // setDialogOpen(true);
  };

  return (
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
            <MediaCard key={`media-card-id-${photo.id}`} media={photo} onClick={handleCardClick} />
          ))}
      </Box>
    </Box>
  );
}
