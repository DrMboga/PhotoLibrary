import React, { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '../storeHooks';
import { getPhotos, selectPhotos } from './photosSlice';
import { Box } from '@mui/material';
import { MediaCard } from './MediaCard';

function HomePage() {
  const photos = useAppSelector(selectPhotos);
  const dispatch = useAppDispatch();

  useEffect(() => {
    dispatch(getPhotos());
  }, []);

  // const year = photos.length > 0 ? new Date(photos[0].dateTimeOriginal * 1000).getFullYear() : 1980;

  return (
    <>
      <p>Hello there</p>
      <Box
        sx={{
          display: 'flex',
          flexWrap: 'wrap',
          gap: '10px',
          rowGap: '10px',
          justifyContent: 'space-evenly',
          alignItems: 'center',
        }}
      >
        {photos.length > 0 && photos.map((photo) => <MediaCard media={photo} />)}
      </Box>
    </>
  );
}

export default HomePage;
