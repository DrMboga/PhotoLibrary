import React, { useEffect } from 'react';
import { useAppDispatch, useAppSelector } from '../storeHooks';
import { getPhotos, selectPhotos } from './photosSlice';
import { Box } from '@mui/material';
import { MediaCard } from './MediaCard';
import { ScrollableBox } from '../components/ScrollableBox';

const topBarHeight = 56;

function HomePage() {
  const photos = useAppSelector(selectPhotos);
  const dispatch = useAppDispatch();

  useEffect(() => {
    dispatch(getPhotos());
  }, []);

  return (
    <>
      <Box>
        <p> Hello there, {topBarHeight} </p>
      </Box>
      <ScrollableBox indent={topBarHeight}>
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
          {photos.length > 0 &&
            photos.map((photo) => <MediaCard key={`media-card-id-${photo.id}`} media={photo} />)}
        </Box>
      </ScrollableBox>
    </>
  );
}

export default HomePage;
