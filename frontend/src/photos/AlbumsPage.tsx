// @flow
import * as React from 'react';
import { useState } from 'react';
import { favoriteAlbumName, importantAlbumName, printAlbumName } from '../helpers/album-helper';
import { Box, Button, ButtonGroup, IconButton } from '@mui/material';
import FavoriteIcon from '@mui/icons-material/Favorite';
import FavoriteBorderIcon from '@mui/icons-material/FavoriteBorder';
import StarIcon from '@mui/icons-material/Star';
import StarOutlineIcon from '@mui/icons-material/StarOutline';
import LocalPrintshopIcon from '@mui/icons-material/LocalPrintshop';
import LocalPrintshopOutlinedIcon from '@mui/icons-material/LocalPrintshopOutlined';

export function AlbumsPage() {
  const [selectedAlbum, setSelectedAlbum] = useState(favoriteAlbumName);

  const albumChanged = (album: string) => {
    setSelectedAlbum(album);
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
      <Box sx={{ flexGrow: 1 }}>Albums here!</Box>
    </Box>
  );
}
